# Steam social PvP architecture — v2

[Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). USER APPROVED V2-24–27:Friends non-ranked, default Revenge non-ranked, Ranked окремий backend pool. Технічні рішення нижче PROPOSED до implementation gate.

## Identity і Steam platform

Steam authentication:client GetAuthTicketForWebApi, callback, backend validation через AuthenticateUserTicket, verified SteamID64; ownership перевіряється окремо відповідно до entitlement model. [Steam auth](https://partner.steamgames.com/doc/features/auth). AppID/identity параметри configured на сервері, publisher key не client secret. [ISteamUserAuth](https://partner.steamgames.com/doc/webapi/ISteamUserAuth#AuthenticateUserTicket).

Власний profile UUID, ExternalIdentity(steam, SteamID64) unique. IDs у JSON decimal strings, не floating-point numbers. Nickname/аватар mutable display metadata, Club name окрема identity. TLS, session expiry, replay protection, redacted logs, server ownership/authorization потрібні; exact TTL не затверджено. Dev stub не може входити у production routing.

## Friends discovery

Клієнтський ISteamFriends надає enumeration/persona/avatars; дані можуть приходити асинхронно. [ISteamFriends](https://partner.steamgames.com/doc/api/ISteamFriends). Web GetFriendList може бути недоступний через privacy; для game client persona data документація радить client API. [ISteamUser](https://partner.steamgames.com/doc/webapi/ISteamUser#GetFriendList).

Client friend IDs — discovery input, не доказ прав на private data. Backend віддає тільки discoverable public club cards; friends-only private fields потребували б server verification або game mutual consent. Не зберігати graph non-player friends без потреби. Empty list, error/private/unavailable і not-in-game-profile мають окремі UI states. Presence не визначає можливість challenge.

Card:Steam name/avatar, Club name, Club Level, rating, defense count1–16, power estimate, HP/readiness summary за privacy policy, View Club/Challenge. Точний power formula можна не показувати. Немає mandatory default3v3 або вимоги бути online.

## Asymmetric challenge

A обирає 1–16 власних available fighters, weapon/MK/armor/BB loadout. B представлений valid defense snapshot із 1–16. Backend перевіряє ownership/readiness/version і privacy,**не перевіряє sizeA=sizeB**. Friend1v5,3v10,16v2 допустимі. A не обирає довільно чужого одного weak fighter із повного B roster; target selection належить defense owner/policy, Q-06/Q-10.

Preview фіксує countA/countB, opponent level, current/reference power context, reward policy, starting HP/BB class і versions. Seed/result не видаються до acceptance. Якщо input змінився — refresh quote/preview, не silent swap. Після acceptance inputs immutable.

## Modes і reward boundary

| Mode | Discovery | Rating | Normal reward |
|---|---|---|---|
| Friends/general social | Обраний discoverable club | Нуль | Money/XP за level/power/repetition policy |
| Revenge | Valid ticket target | Нуль за default approved policy | Можлива Money/XP; без reset farming counters |
| Ranked | Окремий backend opponent pool | Тільки тут | Money/XP за versioned mode policy |
| Offline practice/replay | Isolated cache/history | Нуль | No authoritative rewards за proposed offline boundary |

Leaderboard Challenge відкриває social challenge, не обходить ranked pool. Якщо колись backend випадково підбере знайомих у Ranked, anti-collusion лишається; Friends tab не створює rated request.

## Ranked assistance без forced symmetry

Matchmaking inputs:Club Level, derived Team Power, actualHP/gear/MK/BB, ranking history, recent repetitions і opponent availability. Близькі power можуть бути у 2vs5; не замінювати power/count різницю equality restriction.

Backend може пріоритезувати схожий ризик; ширина windows, acceptance flow, rating formula, порядок ranked changes, exposure quotas і захист від alt rings Q-10. Старі K24, hard0.8–1.25ratio, fiveincoming/24h і max7inactive не активні constants.

Rating outcomes authoritative й idempotent. Thresholds не повинні мовчки перетворити дозволену asymmetry на format lock. При недостатньому pool повідомлення no suitable ranked opponent; social/PvE fallback якщо доступний, без маскування bots під Steam людей.

## Revenge

Approved:non-ranked, normal rewards possible. Proposed details:ticket від incoming battle, one-use, current valid target snapshot,72h candidate TTL, no recursive revenge ticket chain. TTL/snapshot/reward-dedupe limits OPEN Q-10. Усі pair counters keyed clubUUID, а не count/Steam display name/UI tab. Зміна team size або heal не скидає history.

UI report показує outcome з perspective defender, фактичні approved rating/resource changes, Revenge/replay. Resource changes offline defense не вигадувати до Q-06:див. [Combat](AIRSOFT_COMBAT_SYSTEM.md).

## Offline defense resource gate

Immutable snapshot потрібний для reproducibility, але не означає безкоштовних ресурсів. Live HP/ammo debit вимагає reservations/serialization з owner attacks/heals; isolated snapshot усуває drain ціною нового винятку; dedicated resource pool додає UX. Q-06 не обрана. Until adopted, design не обіцяє ні unlimited free defense, ні множинні пасивні витрати.

## Credits purchases через Steam

Steam описує in-game purchases через Steam Wallet/microtransaction API, зокрема купівлю in-game currency. [Microtransactions](https://partner.steamgames.com/doc/features/microtransactions).

Documented purchase flow:server створює order/InitTxn, user authorization, server FinalizeTxn; grant після успішної finalization, не лише client callback. [Implementation guide](https://partner.steamgames.com/doc/features/microtransactions/implementation). Наша схема додає unique order→Credits ledger grant і не змішує Steam Wallet currency з Credits або Money.

При ambiguous timeout перевірити QueryTxn/GetReport перед повторним grant; transaction reporting використовується для reconciliation/reversal. [ISteamMicroTxn](https://partner.steamgames.com/doc/webapi/ISteamMicroTxn). Order ownership, AppID, items/amount/currency зі server catalog, auth, refund/chargeback lineage обов'язкові. Credits уже могли стати Money, BBs або MK; policy debt/clawback OPEN Q-11, без double compensation.

Real-money SKU pricing, Steam permissions/sandbox tests і live activation — майбутній gate. У correction pass payment не виконується.

## Leaderboards, notifications, privacy

Global/Friends/Around Me показують canonical ranked rating, Club Level, records і public identities. Friends filter не дає private access. Steam trusted server writes можуть бути optional mirror, не authority. [Steam Leaderboards](https://partner.steamgames.com/doc/features/leaderboards).

In-game inbox дає defense/revenge events без залежності від push. Steam Game Notifications описано для async sessions, яким потрібна дія для продовження; це не автоматичний доказ придатності для optional revenge alerts. [Game Notifications](https://partner.steamgames.com/doc/features/game_notifications). UI redesign окремо; report/block, public field allowlist і moderation лишаються requirements, деталі Q-12.
