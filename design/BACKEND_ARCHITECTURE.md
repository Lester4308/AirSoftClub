# Backend architecture

RECONSTRUCTION DECISION / NEW AIRSOFT DESIGN. Historical API E026/E027 не встановлює поточний stack. Ціль — малий modular backend, не MMO.

## Deployment boundary

PC client → HTTPS application API → transactional relational DB. Simulation worker використовує durable jobs у тій самій DB на початку; object storage для великих replay archives після вимірювання. Auth adapter звертається до Steam. Один deployable backend із модулями, worker може працювати окремим process із тієї ж codebase. Redis, Kafka, service mesh і multi-region writes не залежності MVP.

Мова/engine gate M1: перевага pure domain simulation library, яку можна headless запускати на сервері й offline practice. C# — кандидат для спільної library, остаточний engine/DB vendor фіксуються після короткого integration spike і перевірки актуальних умов; цей design не приписує конкретній бібліотеці неперевірену підтримку Steam.

## Modules і ownership

| Domain | Owns | Commands / reads |
|---|---|---|
| Authentication | identities,sessions,entitlements | exchange ticket,logout |
| Profiles/Clubs | public identity,privacy,settings | create/rename,public card |
| Fighters | XP,training budget,roster | hire/train/respec/dismiss |
| Inventory | owned instances,equipment | buy/equip/sell |
| Economy | wallet,ledger,eligibility | internal settlement; no arbitrary client credit |
| Snapshots | immutable battle inputs,current pointer | publish from owned state |
| Matchmaking | ranked offers,pair/quota reservations | offer,accept |
| Resolution | input versions,seed,jobs,events | simulate/retry |
| Rating | ratings,revision,season | internal atomic rating settlement |
| History/Notifications | records,inbox,revenge tickets | paginated reads,redeem ticket |
| Leaderboards | canonical projection | global/friends/around me |

Modules можуть мати separate logical tables, але їх критичні зміни виконуються в одній DB transaction. RPC між ними не вводиться тільки заради «масштабування».

## Command contracts

POST /auth/steam — ticket exchange; GET /me — authoritative profile. POST /clubs; POST /fighters/hire; POST /fighters/{id}/train; POST /fighters/{id}/respec; POST /loadouts; POST /shop/purchases; GET /opponents; POST /ranked/offers; POST /matches; GET /matches/{id}; GET /history; GET /leaderboards; POST /revenge/{ticketId}/redeem. Це **нові** endpoints, не historical reconstruction URLs.

Mutations authenticated, scoped to playerUUID із session, expectedRevision і idempotencyKey. Клієнт не передає owner identity як доказ права, ready stats, reward amount, authoritative seed, price або resulting rating. Requests містять обрані IDs і очікувані display versions; сервер визначає фактичні значення. Public DTO не містить wallet,Steam tickets,всього inventory чи приватної історії.

## Match transaction flow

**Acceptance transaction:** lock attacker,validate identity/entitlement/ruleset/offer/opponent privacy,ensure at most one active attacker match; validate and pin both current snapshots; reserve reward ordinal/pair/incoming quota; consume revenge ticket якщо є; generate/store seed та configs; insert Match(accepted),Job,outbox marker. Idempotency row у тій самій transaction. Невдала validation не витрачає ticket/counter.

**Worker:** lease job з expiry; load frozen input; simulate; persist immutable result/events із unique matchId і output hash; стан resolved. Duplicate worker може обчислити вдруге, але write conflict повертає existing result. Output hash mismatch для того самого input — fail/quarantine, не «обрати останнє».

**Settlement transaction:** lock match та обидва rating rows у deterministic UUID order; якщо settled — повернути існуюче. Apply wallet entries,XP,ratings/quota finalization,history та inbox/revenge rows; update Match(settled); insert projection outbox. Unique(matchId) settlement. Сервер, не client claim, виконує цю дію.

Client disconnect після acceptance не скасовує match. API timeout → retry same key або query command status. Internal permanent failure → failed зі structured reason, no reward/rating; повернути reserved ticket у valid state, якщо не expired, звільнити quota/reward reservation; original record зберегти. Нове acceptance отримує новий matchId; failed seed не показувати як безкоштовний scout.

## Snapshot mutations і patches

Equip/train/respec/hire/dismiss зберігають власність та новий currentDefense pointer транзакційно. Defense preset окремий від attack preset, за замовчуванням linked. Якщо linked=false, training поточного defense fighter усе одно породжує новий snapshot. Збереження invalid defense складу відхиляється; draft можна залишити локально. Existing pinned battles завершуються на старому immutable input.

Balance patch: immutable version tables; maintenance gate new acceptances за потреби; regenerate current snapshots from owned definitions нової версії, invalid old pointers прибрати з discovery. Old records залишаються. Відсутність compatibility snapshot → no challenge, не silent mixed-version sim.

## Offline, save і міграції

Online source of truth — DB; cache локальний для inspection/practice. Cache keyed playerUUID+schemaVersion із lastSyncedAt; на зміну Steam account не показувати private cache попереднього account автоматично. Offline practice save має separate namespace і ніколи не merge-иться з authoritative economy. Settings можна синхронізувати з allowlist, не generic upload profile JSON.

Internal UUID дозволяє later verified account linking. Migration не створює подвійну economy: explicit merge policy, audit і доказ контролю identities; v1 linking лише Steam і не UI feature. Schema migrations з backups/rollback plan; balance versioning окремо від DB schema version.

## Operations і moderation

Метрики: auth failures, acceptance latency, queue age, sim duration, settlement retries, duplicate-key conflicts, no-offer rate, snapshot invalidation, ledger reconciliation. Logs keyed matchId/player pseudonym, secrets redacted. DB backup і restore drill до external beta; alert при stuck accepted/resolved match і ledger inconsistency.

Club name length3–32Unicode graphemes після normalization; filter + report + moderator rename/appeal. Block зупиняє майбутні directed challenges і discovery. Для ranked block не використовується як безкоштовний нескінченний reroll offer: existing offer expires зазвичай, нова видається за timetable; fairness review відстежує abuse. Profile deletion приховує public identity та snapshots із discovery, anonymizes history attribution; combat numeric inputs зберігаються за оголошеною retention policy. Це product plan, не юридичний висновок.

## Capacity hypothesis

Для planning тільки:1000DAU×20matches=20k/day, середнє 0.23match/s; peak10×≈2.3/s. Якщо worker CPU10ms/match, це 23ms CPU/s; якщо 200ms —460ms/s. Потрібен benchmark, не гарантована пропускна здатність. При compressed replay50KB обсяг≈1GB/day без replication/metadata. Виміряти actual sizes і вартість retention до public release; whole-history audit input/version retention планувати окремо від доступності графічного replay.
