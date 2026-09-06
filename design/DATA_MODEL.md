# Conceptual data model — v3

Це незалежна domain specification, **не SQL schema, migrations або production code**. [Backend](BACKEND_ARCHITECTURE.md), [combat](AIRSOFT_COMBAT_SYSTEM.md), [open decisions](IMPLEMENTATION_QUESTIONS_v3.md).

| Concept | Мінімальний зміст / invariant |
|---|---|
| Account | Internal ID, verified SteamID mapping, access/sanction state; client identity не authoritative |
| Club | Owner, Club Level/XP, rating, roster version, capacity progression; max 16 owned fighters |
| Fighter | Stable ID, ownership, Accuracy/Endurance/Agility, Fighter Level/XP, training state, Current HP/Max HP, recovery timestamp |
| RecruitmentOfferSet | Club, version, 3 initial free candidates або 6–7 normal offers, generation inputs, next free refresh/time/match counter |
| StarterEntitlement | Одноразова видача одного free fighter; dismissal не відновлює entitlement |
| ItemDefinition / ItemInstance | Versioned catalog, ownership, family/slot, MK, progression gate, price/access entitlements; MK1 OPEN |
| EquipmentAssignment | Рівно 4 типи slot: Weapon, Camouflage, Head Protection, Load-bearing/Armor; без duplicated equipped ownership |
| ClubBBInventory | Кількості за BB class, capacity policy, один active class; жодних per-fighter BB balances |
| Wallet / LedgerEntry | Money/Credits, server delta/reason, unique operation ID, external order/refund reference; повтор не дає duplicate grant |
| DefenseSnapshot | Immutable version, fighters/stats/gear, full defensive HP, BB class/budget rule, rating, ruleset/balance version |
| MatchConfig | Mode/origin, accepted participant inputs, snapshot IDs, starting HP, BB budgets/classes, seed, simulation versions |
| MatchResult | Outcome/reason, participants, final HP, shot counts/BB consumed, events/digest; без client-authored economic authority |
| MatchSettlement | Unique match ID, exact HP/BB/reward/rating effects, status, ledger links, applied once |
| FriendPairWindow | Directed attacker ID → friend ID, window bounds 8h, first-win eligibility, bounded loss usage, win ordinal, reward exhaustion |
| DefenseExposure | Profile/window, impact reservations and committed count <=4/24h; shared across modes |
| ProtectionShield | Owner, purchased duration 8h/1d/3d/7d, server start/end, purchase reference |
| RevengeOpportunity | Origin defense match, exact rating lost, created/expires +24h, max 3 attempt records, successful consumed marker |
| PurchaseOrder | Server SKU/amount, platform transaction state, grant/reversal ledger IDs, idempotency keys |
| RetentionClaim | Daily/streak/achievement/level grant key, 7-day streak tracking, server entitlement; faucet budget |
| ModerationRecord | Club-name/report/sanction references; emblem checks only if UGC present |
| LeaderboardProjection | Authoritative rating projection/version; not client-written truth |

## Critical invariants

- Немає manual ActiveSquad entity; participants виводяться з owned roster + readiness policy. Недостатня екіпіровка не причина виключення.
- Немає persistent Energy, training currency або fighter-owned BB stock.
- Endurance upgrade: 80/100 → 80/110; recovery amount обчислюється окремо і не маскується під training heal.
- Live fighter health та defense simulated health розділені. Defense не змінює live HP/BB. Snapshot не редагується in place; committed heal/equip породжує нову version.
- Friend pair key directed. Win ordinal 1: full; 2–3: reduced; >=4: zero Money/Club XP/Fighter XP до window end. Після first win rating disabled для всіх наступних pair matches у window.
- Ranked draw rating delta = 0. Exposure reservation має запобігати fifth incoming impact навіть при concurrent matches.
- Revenge зберігає **origin rating loss**, не current rating. Recovery 1.2 × origin loss, single successful consumption; rounding/counterparty policy OPEN.
- Shield перевіряється під час new attack acceptance; не анулює accepted match. Own Ranked interaction OPEN.
- Credits early-unlock entitlement та soft purchase item — дві окремі операції; bypass <=3 Club Levels.
- Versions, timestamps, quantities, bounds і uniqueness перевіряє backend. Offline cache не є authoritative save system.

У першому майбутньому code milestone лише fighter/weapon/armor/BB battle-domain subset, MatchConfig/Result і deterministic seed. Account, wallets, PvP records, database та commerce не реалізуються в ньому.
