# Backend architecture — v2

[Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Technical proposals для Airsoft_Club_Game. Незалежна архітектура; implementation не дозволена цим correction pass.

## Foundation

PC/Steam client → authenticated HTTPS backend → transactional store/durable work → event replay. Modular monolith як proposed старт, окремі boundary interfaces для Steam Auth/Commerce/Storage/Simulation. Engine/runtime/vendor не chosen. Не успадковувати calendars, classes, save schema або repo layout Project Airsoft.

| Domain | Дані/відповідальність v2 |
|---|---|
| Auth/Profile/Club | verified identity, Club Level, settings/public card |
| Recruitment |6–7 offers, quality distribution by level, free-first entitlement, growing price |
| Fighters/Training | stats, progression, owned roster≤16 |
| Health/Recovery | CurrentHP/MaxHP, server timestamps, Money healing quote |
| Gear/MK | owned definitions, tier, visual+gameplay modifiers, early access |
| Ammo | persistent stock per class, loadouts, reservations, spent/released |
| Economy | Money/Credits ledger, exchange, reward level/power policy |
| Commerce | Steam orders, verified grants, reversals, entitlements |
| Snapshots/Power | immutable1–16 inputs, actual/reference power і versions |
| Battle/Settlement | one battle, result, HP/ammo/reward/rating effects once |
| Social/Ranked/Revenge | separate modes, opponent availability, abuse counters |
| History/Inbox/Leaderboards | projections із canonical results |

## Atomic commands

Hire validates free-first usage/roster capacity/offer ownership і quote; debit correct currency, create fighter once. Train/equip/MK upgrade validates costs та busy policy; new snapshot revision. Exchange дебетує Credits і кредитує Money в одній transaction. Heal матеріалізує recovery, перевіряє quote/missingHP, Money debit+HP update atomic; Credits не accepted healing currency.

Ammo purchase створює stock lot; loadout/reservation не подвоює stock. Ціну, XP, HP, damage, reward, seed або Steam grant клієнт не встановлює. Expected revision+idempotency key+body hash; повтор same command повертає existing result, інший body із same key conflict.

## Battle lifecycle

1. Preview:materialize health на server time, validate attacker1–16 і opponent1–16; power/reward quote with versions. Повна validity не залежить від рівності counts.
2. Acceptance transaction:перевірити preview freshness, ownership/privacy, availability, ammo; pin snapshots і server seed; reserve attacker ammo та deployed actors; record health timestamps/versions; create durable battle job й pair context.
3. Worker:deterministic pure sim на frozen inputs; persist one result/events. Duplicate worker не перезаписує іншим output.
4. Settlement transaction:unique BattleSettlement; spent BB consumption/unspent release, final attackerHP, recovery timestamp, reward Money/XP, ranked rating тільки if applicable, history/inbox. Defense writes додаються тільки після Q-06 policy selection.
5. Disconnect/Skip — presentation changes, не cancel/reward rollback. Internal failure до settlement звільняє reservations і залишає audit record, без free payouts.

Proposed busy lock на deployed actors від acceptance до settlement; heal/train/equip serialize/reject, benched actors не повинні блокуватись без потреби. Atomic HP merge між offline defense і owner action не вирішується простим last-write-wins. Q-03/Q-06 gate до resource implementation.

## Recovery authority

Server clock/lastMaterializedAt; free recovery нараховується за elapsed time, коли policy дозволяє. Не потрібний tick-writing кожного fighter щосекунди:lazy materialization при read/command як proposal. Client countdown display не змінює authority. Exact speed/cost,0HP і upgrade MaxHP semantics OPEN.

Не нараховувати health двічі після retry і не overwrite paid heal старим battle result. Accepted input immutability окрема від live state write policy. Немає Energy recharge domain.

## Commerce / ledger

CommerceOrder власний unique ID, player identity, provider order/transaction IDs, catalog quote, real currency/amount, Credits entitlement, status. Server verifies Steam finalized state перед grant. Idempotent provider callback/polling, reconciliation job, outbox та source-linked ledger. Secrets тільки server; payment failures не стають currency grant.

Credits conversion або purchase premium BB/MK має provenance до ledger transaction/lot. Refund/chargeback може відбутись після consumption; Q-11 визначить policy, не silent negative spendable balance чи rewrite history. Потрібні support/audit records і компенсаційні entries замість видалення старих. [Steam details](STEAM_SOCIAL_PVP_ARCHITECTURE.md).

## Snapshots і patching

Snapshots містять stats/HP/gear/MK/BB і версії. Current defense pointer не mutable battle input. Patch створює new definitions і compatible snapshot, старі records зберігають old build/tables. Seed alone недостатній.

Replay може зберігати events, для audit потрібні inputs/build/PRNG/ruleset. Storage/retention/vendor і часові limits OPEN Q-12; не переносити v1capacity estimate як benchmark16v16.

## Offline і операції

Proposed:cached club inspection+isolated practice, без wallet/HP/BB upload. Free recovery при reconnect обчислюється сервером від timestamp; це не offline economy merge. Settings allowlist окремо від progression.

Monitoring:stuck jobs, reservation leaks, HP conflicts, ammo reconciliation, wallet mismatch, payment unknown state, repeat payout anomalies, power/reward outliers. Backup/restore, redacted logs, public DTO allowlist, moderation/report/block потрібні до external release; exact policy окремо. Жодних production services зараз не створено.
