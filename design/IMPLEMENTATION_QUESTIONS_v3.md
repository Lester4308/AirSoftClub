# Implementation questions Q01–Q15 — CLOSED

Актуальний реєстр синхронізовано з [Master v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md), розділ 14. Попередня версія з OPEN збережена у Git на d553cd3d9fee998a9764be04b63f8881658d299f. Reference § у таблиці нижче вказують на master, а не на цей короткий індекс.

**Усі Q01–Q15 CLOSED як узгоджені питання.** Residual values/implementation details не оголошуються ані фінальними, ані вже implemented.

| ID | Актуальне закриття | Implementation status / residual |
|---|---|---|
| Q01 | Accuracy hit; Agility evasion+tempo; Endurance HP; 5–95% prototype; no crit; fixed integers, seeded RNG, action-time; duration Draw, guard failure | Core verified; exact tuning prototype (§8) |
| Q02 | Per-fighter 10% readiness, no latch, MaxHP change не scales CurrentHP | Helper verified; recovery service pending |
| Q03 | Ready weaponless participates/targetable, no attack/fallback; both no-progress Draw | Verified |
| Q04 | Defense BB=100% max capacity, finite/virtual; actual projectile accounting | Verified core + partial volley; inventory capacity/overflow pending |
| Q05 | Win100/100/100; Draw25/35/50; Loss0/10/25; friend100/50/25/0; power bounded target0.1–1.5 | Economy pending; curves/rounding config |
| Q06 | First rated directed-pair battle anchors8h; max1 loss; first win freezes later rating; rolling24h; floor0 | Pending; rating curve prototype ±5…20 |
| Q07 | Reserve at rated start, not discovery; settle once; cap4 shared with Revenge; TTL | Pending; exact lease/post-cap UX implementation |
| Q08 | Own Ranked removes shield; rated Revenge removes; Friend/non-rated does not; accepted incoming finishes | Pending |
| Q09 | Revenge24h/3; draw attempt; in-flight expiry valid; start eligibility; deterministic120%; unique origin/start; no chains; no Friend/non-ranked tickets | Pending; exact rounding/counterparty policy distinction (§16) |
| Q10 | Base=MK1; MK2 expensive soft; MK3 Credits | Catalog pending |
| Q11 | Starter10 Credits; no soft→Credits; fixed configurable Credits→soft; no hard daily spend cap; combined15–20%; starter resources | Pending; exact prices/faucets/metric not final |
| Q12 | Unity6000.3.21f1/C#/Standard2.1; ASP.NET Core current LTS; PostgreSQL/EF Core; wirev1; shared pure core; observability; Docker; secrets external | Unity/wire verified; backend version pin/hosting pending |
| Q13 | Pool6–7 full regeneration;1h OR10 completed; reset both counters; one-time free; original-price resale; gear returns | Pending; refund fraction/distribution config |
| Q14 | Server timestamp+remainder; no MaxHP scaling; concurrency control; post-commit snapshots; min defense1 independent live HP | Pending backend lifecycle |
| Q15 | UTC00:00 daily/account; missed day→Day1; minimal UGC/no open chat; sanctions; idempotent ledger/reconciliation/refunds | Pending |


Native Windows x64 IL2CPP blocker CLOSED: [final report](../implementation/VERIFICATION_002_IL2CPP_FINAL.md). Цей документаційний pass не повторює tests і не починає implementation003.
