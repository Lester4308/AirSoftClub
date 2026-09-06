# Monetization / balance014 — 2026-09-07

Status: implemented and measured prototype; NOT final pricing or population balance approval.

## Approved commercial direction
Progression + monetization driven. Paying and investing well may yield stronger builds and more wins. The old strict15–20% combined premium ceiling and blanket+3 early access are SUPERSEDED by the user's014 request. Individual early item contribution control is not a total premium stack ceiling. Time can substitute for spending; no new mandatory payment was introduced.

| Club level | Early depth |
|---|---|
| 1 | unavailable |
| 2 | +1 |
| 3–4 | +2 |
| 5+ | +3 |

Credits buy permanent access only; the item remains a separate Money purchase. Depth prices use configurable base ×1/2/4. Current prototype base1 Credit is a simulation input, not final commercial approval. Existing ownership/entitlements survive the policy update.

## Contribution control and release
Early weapons are bounded against the best naturally available soft weapon (including MK2) at the actual fighter's tempo: damage × projectiles / interval. This includes burst count and agility/armor tempo; all current weapons have equal zero accuracy/penetration contributions. The cap changes the generated domain snapshot, never combat formulas, RNG or wire format. It is a sustained contribution metric, not proof of bounded win-rate or identical burst lethality. Any future accuracy/penetration/weapon-special mechanics require extending this metric.

Early armor uses effective HP ratio (ArmorScale + protection), bounded per slot against best natural same-slot gear; the native agility penalty remains. Other armor reduces each slot's relative contribution. The configurable target is125%; integer flooring stays conservative. Total gear/MK/BB stacking is intentionally measured separately.

At natural Club level native item stats return automatically, with no charge/rebuy and permanent entitlement. Startup refresh republishes old defenses with the new catalog revision. Pending offense skips refresh; accepted immutable battle inputs settle under their captured data. Wallet ledger/history are not rewritten.

## Old pathology and new measurements
Historical013 exhaustive equal-stat DPS × effective-HP stack audit measured Level1 **2.4449025×** through early Assault Rifle access. New best available paid/optimized-normal ratios:
- Level1 **1.060828×** (natural Pistol MK3 + Premium BB versus soft MK2 + High-End).
- Level2 **1.216789×** (early Assault Rifle MK2, armor3, Premium BB).
- Levels3–10 **1.060834×** (natural Assault Rifle MK3 + Premium BB).
These are observations, not replacement global ceilings. See [exhaustive stack evidence](evidence/014/early-access-014.json).

The level-boundary test covers every current catalog entry at levels1–10. Weapon cap test covers all actual allowed early weapons with agility1/20/100. Level3 covers+1/+2; Level4 covers+1/+2; Level5 has only+1 real content because catalog ends at6. Depth+2/+3 for levels5+ has no current item; policy/price boundary is tested, combat content there is N/A rather than fabricated.

## 4,000 paired battles / 40 rows
Four builds at each Club level1–10, four equal-stat fighters (each stat10+level), seeds0–99, versus optimized-normal same-level defense. Normal: MK1/Basic; optimized normal: MK2/High-End; moderate: allowed soft early weapon/MK2 + Standard BB; max: best allowed native-throughput weapon including natural MK3/early soft + Premium BB. Current best early weapon remains AR; later families are not assumed superior by unlock level.

TeamPower is the existing reward/matchmaking heuristic, **not** the DPS × effective-HP metric. Win-rate can change sharply at hit-count thresholds despite a small heuristic power change. Damage includes overkill. Survival is summed remaining attacker HP. BB costs are prorated per500 refill. C means Credits, M means Money. Level-speed is1000 / mean Club XP within a fixed band; it excludes recovery waiting, training and changing opponents. It is not a longitudinal mid-game completion claim.

Money columns: free-recovery net subtracts BB Money only; instant-heal net also subtracts full missing-HP healing. Credit BB cost remains separate, with no invented exchange valuation. Initial gear, recruit, training and access costs are not amortized here; the raw evidence includes per-item access quote. See [all raw metrics](evidence/014/monetization-014.json).

| Level | Build | TeamPower | Wins/100 | Damage | Surviving HP | BB | BB cost | Battles/level | Net M free recovery | Net M instant heal |
|---|---|---:|---:|---:|---:|---:|---|---:|---:|---:|
| 1 | Normal | 6448 | 39% | 388.0 | 21.7 | 45.2 | 4.52 M / 0.000 C | 19.9 | 39.0 | -359.8 |
| 1 | Optimized normal | 7172 | 39% | 435.4 | 21.7 | 45.2 | 9.94 M / 0.000 C | 22.1 | 29.3 | -369.5 |
| 1 | Moderate premium | 6748 | 39% | 407.7 | 21.7 | 45.2 | 6.32 M / 0.000 C | 21.0 | 35.3 | -363.6 |
| 1 | Max premium | 7580 | 75% | 443.9 | 60.0 | 43.1 | 0.00 M / 0.086 C | 13.6 | 71.4 | -289.9 |
| 2 | Normal | 10892 | 14% | 386.4 | 5.6 | 108.3 | 10.83 M / 0.000 C | 36.1 | 6.6 | -428.1 |
| 2 | Optimized normal | 12152 | 45% | 435.8 | 25.3 | 108.5 | 23.88 M / 0.000 C | 17.7 | 26.7 | -388.9 |
| 2 | Moderate premium | 11560 | 64% | 488.8 | 50.2 | 85.6 | 11.98 M / 0.000 C | 12.9 | 61.6 | -329.0 |
| 2 | Max premium | 12836 | 83% | 530.6 | 84.1 | 83.4 | 0.00 M / 0.167 C | 11.4 | 86.3 | -270.8 |
| 3 | Normal | 11416 | 15% | 398.6 | 8.3 | 85.3 | 8.53 M / 0.000 C | 31.9 | 11.7 | -440.1 |
| 3 | Optimized normal | 12732 | 39% | 445.4 | 28.2 | 84.8 | 18.65 M / 0.000 C | 18.2 | 29.0 | -403.2 |
| 3 | Moderate premium | 11956 | 24% | 420.0 | 16.4 | 85.4 | 11.96 M / 0.000 C | 24.5 | 19.8 | -424.1 |
| 3 | Max premium | 13464 | 39% | 472.4 | 28.2 | 84.8 | 0.00 M / 0.170 C | 19.4 | 44.9 | -387.4 |
| 4 | Normal | 11804 | 15% | 427.1 | 5.2 | 90.7 | 9.07 M / 0.000 C | 29.2 | 13.3 | -461.7 |
| 4 | Optimized normal | 13160 | 38% | 478.0 | 26.1 | 90.3 | 19.86 M / 0.000 C | 16.8 | 31.5 | -422.9 |
| 4 | Moderate premium | 12360 | 26% | 451.0 | 11.4 | 91.1 | 12.75 M / 0.000 C | 21.8 | 23.5 | -445.4 |
| 4 | Max premium | 13916 | 54% | 500.7 | 42.8 | 89.1 | 0.00 M / 0.178 C | 13.8 | 66.8 | -371.2 |
| 5 | Normal | 12196 | 26% | 461.2 | 12.7 | 96.9 | 9.69 M / 0.000 C | 18.9 | 32.0 | -455.6 |
| 5 | Optimized normal | 13600 | 42% | 516.6 | 22.5 | 96.6 | 21.25 M / 0.000 C | 14.6 | 39.3 | -438.8 |
| 5 | Moderate premium | 12772 | 26% | 484.5 | 12.7 | 96.9 | 13.57 M / 0.000 C | 19.9 | 26.3 | -461.3 |
| 5 | Max premium | 14384 | 59% | 542.6 | 39.9 | 95.4 | 0.00 M / 0.191 C | 11.9 | 78.9 | -382.1 |
| 6 | Normal | 12600 | 14% | 460.8 | 6.0 | 96.2 | 9.62 M / 0.000 C | 26.6 | 14.2 | -500.1 |
| 6 | Optimized normal | 14048 | 43% | 518.4 | 27.3 | 96.2 | 21.15 M / 0.000 C | 13.6 | 44.1 | -449.2 |
| 6 | Moderate premium | 13192 | 27% | 486.9 | 15.5 | 96.6 | 13.52 M / 0.000 C | 17.9 | 31.4 | -473.5 |
| 6 | Max premium | 14856 | 61% | 540.3 | 48.3 | 94.4 | 0.00 M / 0.189 C | 10.8 | 86.7 | -385.8 |
| 7 | Normal | 12992 | 11% | 483.2 | 5.1 | 100.5 | 10.05 M / 0.000 C | 28.1 | 10.4 | -524.6 |
| 7 | Optimized normal | 14488 | 42% | 544.8 | 23.4 | 101.0 | 22.22 M / 0.000 C | 12.9 | 45.8 | -471.3 |
| 7 | Moderate premium | 13604 | 23% | 513.4 | 10.4 | 101.3 | 14.18 M / 0.000 C | 18.9 | 25.8 | -504.1 |
| 7 | Max premium | 15324 | 58% | 577.6 | 42.6 | 100.5 | 0.00 M / 0.201 C | 10.6 | 88.3 | -409.8 |
| 8 | Normal | 13404 | 10% | 482.6 | 6.1 | 99.7 | 9.97 M / 0.000 C | 27.4 | 10.3 | -543.7 |
| 8 | Optimized normal | 14948 | 42% | 545.1 | 26.1 | 100.3 | 22.06 M / 0.000 C | 12.2 | 50.2 | -484.2 |
| 8 | Moderate premium | 14040 | 22% | 511.7 | 13.1 | 100.3 | 14.04 M / 0.000 C | 18.3 | 26.5 | -520.6 |
| 8 | Max premium | 15808 | 57% | 575.6 | 47.7 | 99.6 | 0.00 M / 0.199 C | 10.2 | 91.6 | -421.4 |
| 9 | Normal | 13812 | 14% | 515.9 | 6.8 | 105.6 | 10.56 M / 0.000 C | 21.4 | 18.9 | -554.4 |
| 9 | Optimized normal | 15404 | 44% | 586.2 | 26.4 | 106.3 | 23.40 M / 0.000 C | 10.9 | 58.5 | -495.5 |
| 9 | Moderate premium | 14468 | 25% | 549.7 | 13.1 | 106.8 | 14.95 M / 0.000 C | 15.4 | 35.6 | -531.5 |
| 9 | Max premium | 16292 | 61% | 613.3 | 45.7 | 105.1 | 0.00 M / 0.210 C | 9.0 | 105.0 | -430.1 |
| 10 | Normal | 14236 | 13% | 516.1 | 7.7 | 105.0 | 10.50 M / 0.000 C | 21.1 | 18.5 | -574.0 |
| 10 | Optimized normal | 15876 | 44% | 586.4 | 29.5 | 106.0 | 23.31 M / 0.000 C | 10.3 | 63.1 | -507.8 |
| 10 | Moderate premium | 14908 | 13% | 542.2 | 7.7 | 105.0 | 14.70 M / 0.000 C | 22.1 | 13.1 | -579.4 |
| 10 | Max premium | 16788 | 61% | 614.6 | 52.4 | 104.6 | 0.00 M / 0.209 C | 8.5 | 110.5 | -437.8 |

## Observations / remaining hypotheses
- Paid builds can win more: Level2 max83/100 versus optimized45/100; Level10 max61 versus44. Level1 max75 versus39 is a discrete shot-count threshold risk despite only1.061× sustained stack contribution. Early unlock is disabled there and premium equipment discovery is collapsed by default; do not claim onboarding economy final.
- Moderate spend is not automatically optimal: lower BB tier can lose to an optimized soft build. Time/investment choices matter.
- No observed2×–3× single early-item spike. Current catalog plateaus around AR; higher unlock families need later identity/balance work. No production premium-only armor added.
- All four builds have positive mean Money after BB when waiting for free recovery in this fixture, but full instant healing is heavily negative. Recovery time (~100min from zero), recruitment/training affordability and opponent selection need longitudinal economy tuning. Daily/earned Credits, emergency Basic, soft recruitment/equipment and passive recovery remain available; existing zero-resource tests pass.
-100 seeds/cell is prototype signal, not statistical final balance. Progression-speed estimates do not establish lifetime F2P viability alone.
- No attempt to force premium equality; no exact Credit pricing approval inferred from these simulations.

## Configuration and verification
AlphaConfig: EarlyCredits1, EarlyItemCapPercent125, MK2 permille1020, MK3 permille1035, BB multipliers and prices remain explicit prototype constants. Policy version alpha-014-v1; catalog-014-v1. Combat fixed-point/RNG/serialization untouched.

Fresh domain36/0 includes four monetization tests; PostgreSQL26/0 includes catalog refresh/idempotency and existing transaction/settlement/ledger regressions.4,000 balance simulations completed;10,000 core mass simulations completed with zero invariant failures. Full platform evidence is consolidated in [Visual014](VISUAL_FOUNDATION_014.md).
