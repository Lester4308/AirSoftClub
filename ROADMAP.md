# Airsoft_Club_Game roadmap — Master v1

Канон: [Master Development Spec](./AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md). Implementation001–002 complete; наступний pass ще не запущений.

Milestone sequence нижче — план наступного окремо запущеного development pass; номери003+ не означають уже наявні commits.

| Milestone | Результат / exit gate |
|---|---|
| 001 | DONE: deterministic pure battle core |
| 002 | DONE: Unity host + native IL2CPP parity |
| 003 | Rewards/economy domain: outcome table, wallets/ledger semantics, curves/config, anti-farm tests |
| 004 | Club lifecycle: recruitment, stats/training, recovery/heal, inventory/MK/BB, snapshots, starter |
| 005 | ASP.NET Core/PostgreSQL/EF bootstrap: migrations, persistence, concurrency/idempotency, diagnostics |
| 006 | Authoritative battle orchestration: accept/resolve/settle/recover, resource isolation, durable snapshots |
| 007 | PvP domain/application: Friends/Ranked/rating/exposure/shields/Revenge/history/leaderboards |
| 008 | Unity playable vertical slice: full management→battle→result loop, readable16v16 placeholders |
| 009 | Steam identity/Friends adapters, isolated test mode, official sandbox checks where available |
| 010 | Credits/retention/moderation: grants/conversion/shields/unlocks, purchase reconciliation, sandbox commerce |
| 011 | Integrated hardening: crash/race/retry, Windows native regressions, economy/paid-stack simulation, reproducible package |
| 012 | Separate final art/product balance/real commerce/hosting/release approvals and execution |

Не зупиняти великий pass після003 лише через стару пораду «next milestone economy». Детальні дозволи, sequencing, gates, git discipline і stop boundaries — у **AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md**.


## Historical roadmap v3 — superseded status

# Airsoft_Club_Game roadmap — v3

Незалежний проєкт; [boundary](PROJECT_BOUNDARY.md). Рішення 2026-09-06: [Product Design Gate v3](design/AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md).

| Stage | Статус / результат |
|---|---|
| Historical reconstruction basis | Завершена база попереднього pass; provenance збережено, не переносить assets |
| Product decisions v1/v2 | SUPERSEDED; попередні тексти в Git history |
| Product Design Gate v3 | FINALIZED; APPROVED FOR IMPLEMENTATION PREPARATION |
| First implementation authorization | **Не отримано** |
| Future foundation milestone | Лише recommended scope: Unity/C#, pure domain, deterministic headless core, models/config/result/seed/tests |
| Economy / health / recruitment prototype | Майбутній окремий scope після відповідних питань/дозволу |
| Authoritative backend / PostgreSQL | Майбутня система, не частина першого milestone |
| Steam identity / Friends / Ranked / Revenge | Майбутня integration; exposure/shields/anti-farm уточнення до launch |
| Credits / retention / moderation | Майбутня implementation та verified transaction flow |
| Final art pass | OPEN; повернутися наприкінці core design / foundation |
| Production UI / content / release | Не розпочато; залежить від перевірок попередніх stages |

Перший code milestone **виключає** production UI/art, Steamworks, live backend, PostgreSQL, monetization/Credits purchases та matchmaking. [Рекомендований scope](design/MVP_IMPLEMENTATION_PLAN.md). **IMPLEMENTATION NOT YET AUTHORIZED.**
