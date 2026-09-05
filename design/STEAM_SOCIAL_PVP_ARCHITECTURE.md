# Steam social PvP architecture

PD-16–18/20/22/23. Весь Steam design — NEW AIRSOFT DESIGN. Старий friend UI E009/E014 CONFIRMED; historical async STRONGLY SUPPORTED; snapshots/revenge/rating internals не встановлені. Офіційні сторінки перевірено 2026-09-05; реальний SDK integration test ще не виконаний.

## Identity та authentication

Документований backend flow: client GetAuthTicketForWebApi → дочекатися GetTicketForWebApiResponse_t → передати ticket захищеному backend → backend AuthenticateUserTicket → отримати verified SteamID64. Ownership можна перевірити окремим CheckAppOwnership. [Steam authentication](https://partner.steamgames.com/doc/features/auth).

Для partner endpoint використовувати publisher key тільки на сервері, configured appid, hex ticket і фіксований identity string однаковий при створенні та валідації ticket. Документація AuthenticateUserTicket явно описує ці параметри; це інший метод, ніж AuthenticateUser. [ISteamUserAuth](https://partner.steamgames.com/doc/webapi/ISteamUserAuth#AuthenticateUserTicket).

Наша session policy: TLS, auth attempt nonce, short-lived access session30min, fresh ticket для повторної auth, revoke-on-logout, issuer/audience validation, rate limiting. Exact TTL — технічний baseline. Ticket та publisher key не потрапляють у client config, logs, traces чи URL telemetry; server-to-Steam GET logging redacted. Не вважати наш nonce доказом криптографічної прив'язки Steam ticket: replay cache ticket hash і success validation потрібні окремо. Ticket retry одного auth attempt повертає existing session response, не створює інший account. SDK ticket handle закривати після завершення відповідного lifecycle; перевірити це на M2.

Player UUID — primary key; ExternalIdentity(provider=steam,subject=SteamID64) unique. SteamID64 у JSON decimal string. Steam nickname ніколи не key. Entitlement decision враховує фактичний Steam ownership result; не підміняти verified playerID ownerID сімейного доступу без окремої політики. Paid access/family-sharing test cases — release gate. Stub identity живе в dev environment із separate DB/secrets; production route stub відсутній.

## Friends discovery та privacy boundary

ISteamFriends на клієнті дозволяє GetFriendCount/GetFriendByIndex; persona names можуть бути ще невідомі та приходити асинхронно. [ISteamFriends](https://partner.steamgames.com/doc/api/ISteamFriends). Public Web API GetFriendList повертає 401 для private friend list; це не доказ того, що локальний SDK завжди має ті самі обмеження. Для game-client profile display документація радить client APIs. [ISteamUser](https://partner.steamgames.com/doc/webapi/ISteamUser#GetFriendList).

Наша схема: SDK IDs → bounded batch backend discovery → тільки discoverable ClubPublicCard → local join із Steam avatar/name. Presence/«грає зараз» не використовується для визначення наявності profile. Backend є джерелом існування club і valid snapshot. Нуль friends, SDK not logged on, Steam timeout, profile absent і discovery disabled — різні стани.

IDs, прислані клієнтом, недовірені. У v1 усі discoverable cards містять лише поля, доступні будь-якому authenticated player; немає friends-only приватного inventory. Непідтверджений список не відкриває секрети, bonus rewards або rating access. Якщо later додаємо friends-only profile — вимагати server-verifiable relationship або окрему game mutual consent; за недоступної перевірки fail closed для цих даних.

Discovery запити обмежені 100IDs/batch з server rate limit; не зберігати повний graph non-player friends. Store only opted-in discoverable profiles, тимчасовий запит не стає постійною дружбою. Last-active field відсутнє у public card v1. Avatar cache локальний із refresh; unavailable → placeholder. Name text escaped і може локально маскуватися через moderation preferences.

## Challenge modes

| Mode | Opponent | Rating | Economy | Revenge generated |
|---|---|---|---|---|
| Friend/general challenge | selected discoverable club |0 | directed pair policy | так, якщо ticket для цієї пари ще не створено за 24h |
| Ranked | server issued eligible offer | symmetric Elo-style | та сама policy | так, за тією самою pair dedupe |
| Revenge | ticket target current valid snapshot |0 | та сама policy, без bonus | ні |
| Practice | NPC/local replay |0 | online NPC rule або 0offline | ні |

General challenge з Club/leaderboard має ті самі правила, що Friend challenge. Blocked account або opt-out не обходиться через іншу tab. Ticket лише право challenge у межах public/allowed states, не entitlement bypass. Один incoming pair може мати не більше одного нового revenge ticket за rolling24h; інші reports агрегуються. Ticket origin match зберігається.

## Ranked lifecycle

1. Сервер бере rating, досягнутий training budget, progression band і valid3v3 snapshot. Progression bands за unlocked maximum budget одного fighter:0–3,4–7,8–12; band клубу = максимум серед owned fighters і незменшуваний historical maximum. Це захищає від dismiss/de-equip sandbagging, але може погіршити matching для нерівномірно розвиненого roster — beta metric.
2. Filter: той самий format/band, power ratio0.8–1.25, not self/blocked, active≤7days, compatible snapshot, pair not rated24h, defender incoming quota<5accepted/settled за 24h. Power estimate = sum(A+M+E) deployed, gear окремо перевіряється на allowed catalog; це грубий filter, не combat forecast.
3. Rating window±100→200→300 без weakening hard constraints. Stable server-selected offer10min. Reopen не reroll. Якщо нічого — no ranked offer + unranked NPC option.
4. Acceptance перевіряє offer freshness, обидва snapshots і rating eligibility повторно. Preview snapshot mismatch повертає conflict для нового preview. Атомарно pin inputs, reserve pair/quota, create match+job. Snapshot, seed і reward policy після цього незмінні.
5. Після simulation settlement бере поточні рейтинги обох clubs під locks у стабільному UUID order. EA=1/(1+10^((RB−RA)/400)); delta=roundHalfAwayFromZero(24×(SA−EA)). Зміна B=−delta. При rating floor0 transfer обмежується доступними points loser; draw теж використовує формулу й може мати ненульову delta при різних ratings. Formula/rating math version зберігається окремо від combat build.
6. Rating records before/after immutable per match. Incoming defenses отримують нуль wallet/XP; reports пояснюють rating change. Quotas рахують accepted outstanding та settled matches; failed internal resolution звільняє reservation. Немає client cancel після acceptance.

Повторна асинхронна робота не означає exactly-once delivery; гарантується один DB settlement. Один active battle на attacker. За 5incoming defenses клуб тимчасово не eligible як ranked defender, але може грати інші режими. Не приховувати цей pool constraint. Abuse rings залишаються ризиком навіть із pair limit; потрібні review tools, а не обіцянка, що SteamID вирішує collusion.

## Leaderboards

Backend canonical leaderboard: Global/Friends/Around Me, season/ruleset/ratingVersion scope. Friends filter бере local discovery context лише для вже public entries. Columns: rank,Steam avatar/name,Club,rating,ranked W/L/D. Friends rank — позиція всередині friends view з явним label; global rank окремий. Around Me —±10canonical entries. Ties: rating desc, ranked wins desc,clubUUID asc. Немає client score upload до canonical DB.

Steam має global/friend leaderboards і Trusted writes, які дозволяють score updates через серверний Web API. [Steam Leaderboards](https://partner.steamgames.com/doc/features/leaderboards). Optional mirror — outbox із monotonic ratingRevision: затримка mirror не змінює matchmaking. MVP використовує backend view, Steam mirror не release dependency.

## Notifications

In-game inbox: attacked / defense won/lost/draw / revenge available; friend passed rating — local comparison попереднього friends snapshot з поточним, dedupe і opt-out. Не обіцяти доставку, коли social context unavailable. Offline events видимі при вході через backend inbox.

Steam Game Notifications документовано для async sessions, де потрібна дія для продовження гри. [Game Notifications](https://partner.steamgames.com/doc/features/game_notifications). Наша revenge не обов'язкова для завершення session, тому підтримка довільного revenge push з цього не випливає; v1 не залежить від Steam push. Окремий capability/product review перед можливою інтеграцією.

## Integration tests M2/M3

Wrong appid/identity, invalid ticket, logged-out Steam, expired session, missing entitlement; two accounts with same nickname; IDs beyond JS safe integer; friends empty/private/unavailable; delayed avatar/name callbacks; offline defender; concurrent equip/accept; accept retry; block/unpublish before accept; profile delete після settlement; failed Steam mirror; old ruleset. Реальні Steam accounts/permissions потрібні для частини тестів; зараз це план, не test report.
