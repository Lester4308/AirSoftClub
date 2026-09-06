> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Backend architecture — v3

**Стек затверджено:** C# / ASP.NET Core, PostgreSQL, Steam-first. Unity/C# client використовує Steamworks через відповідний C# layer. Версії, hosting і конкретні libraries OPEN. Жодного backend implementation у цьому pass.

## Authority

Backend відповідає за identity, Credits, economy-sensitive actions, PvP, rating, matchmaking, battle verification/resolution, progression rewards, snapshots, leaderboards. Client надсилає intent і відтворює presentation; local battle result, clock, balance чи inventory не є truth.

Офлайн допускається launch/read-only cache; authoritative progression, покупки та PvP потребують server validation. Free recovery враховує минулий offline time на сервері після reconnect.

## Межі майбутніх модулів

| Модуль | Відповідальність |
|---|---|
| Identity / policy | Steam verification, session, sanction та request eligibility |
| Club / fighters | Roster 16, recruitment, XP/training, health/recovery, 4-slot gear |
| Inventory / ledger | Shared BB classes/capacity, one active class, Money/Credits, grant/debit/reversal |
| Battle service | Accepted immutable MatchConfig, deterministic simulation, one settlement |
| PvP policy | 3–5 Ranked candidates, directed friend windows, exposure 4/24h, shields, Revenge tickets |
| Snapshots | Publish immutable defense versions after committed changes |
| Projection / retention | Results, rating leaderboard, daily/streak, reports/moderation foundation |

Це логічні boundaries окремої гри, не імпорт архітектури Project Airsoft. Початковий deployment shape OPEN; не обирати microservices без потреби.

## Transaction lifecycle

1. Authenticate та validate request ID, profile version, mode eligibility, shield, roster/readiness і BB.
2. Зафіксувати immutable attacker input і defender snapshot; reserve потрібні ресурси/quotas. Конкретна locking/reservation scheme — implementation decision.
3. Resolve battle за seed + ruleset + balance version; client не визначає outcome.
4. Atomic idempotent settlement: offensive HP, actual shot BB debit, rewards, rating, pair counters, exposure, ticket consumption та ledger references.
5. Publish нову snapshot/projections через надійний післятранзакційний механізм; його реалізація OPEN.

Defense settlement не пише live HP/BB. Concurrent battles можуть читати одну snapshot; settlement перевіряє live quota ledger, щоб не перевищити 4 incoming impacts. Пропозиція: serialize власні offensive state-changing commands, поки матч не settled; retries повертають існуючий match/result. Часові межі, recovery і failure recovery — [Q06/Q14](IMPLEMENTATION_QUESTIONS_v3.md).

Для purchase grant потрібні verified platform state та unique order/ledger keys. Fault між оплатою й grant має відновлюватися звіркою, а не повторною покупкою. [Steam flow](STEAM_SOCIAL_PVP_ARCHITECTURE.md).

## Shared C#

Безпечно ділити DTO, identifiers, enums, validated domain primitives та за потреби deterministic primitives. Shared code не створює довіри до client execution. Secret keys, authorization, authoritative ledger/settlement та server-only policies залишаються backend-side.

Target shared library має бути сумісним з обраною Unity API compatibility level; не припускати, що backend target framework автоматично працює всередині Unity. Остаточні versions OPEN. Офіційне джерело: [Unity .NET profile support](https://docs.unity3d.com/Manual/dotnet-profile-support.html).

Moderation foundation: club names, reports, sanctions; emblem/UGC controls якщо UGC додано. Точний workflow пізніше. Перший дозволений у майбутньому milestone матиме лише локальний pure C# battle core; live backend/PostgreSQL/Steam/commerce до нього не входять.
