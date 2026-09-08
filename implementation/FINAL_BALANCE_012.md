# Final Milestone012 — balance and pacing audit

Status: fresh local deterministic evidence; prototype balance, not production population/pricing approval. Monetization014's intentional premium combat advantage is unchanged.

## Quantified findings

- **Premium stack:** exhaustive available-build metric remains **1.216789× max at Club Level2** and **1.060828–1.060834× at Levels1 and3–10**. The 4,000-battle matrix remains reproducible: max premium versus optimized normal is **75% vs39% wins at L1**, **83% vs45% at L2**, and **61% vs44% at L10**. These sharp differences are expected threshold evidence, not a request to restore the superseded strict stack ceiling.
- **Starter BB:** 1,000 equal-build 1v1 seeds consumed **10.956 Basic BB per offense**. Starter60 therefore supports **5.476 battles**, inside the approved 5–7 target. A500 refill supports **45.637 equivalent starter battles** and costs50 Money.
- **Recovery/refill:** configured zero-to-full recovery is **100min** and zero-to-ready is **10min**. From the 1,000 starter battles, mean post-battle recovery is **89.141min to full** but **5.995min to the10% readiness threshold**. Thus free replay is not normally blocked for100min, while full-strength replay is deliberately paced. Emergency Basic remains500 after180min when below15% capacity and unable to pay; level3 opt-in auto-buy triggers below100 stock with a150-Money guard and never spends Credits.
- **Economy loop:** the existing100-account ×20-battle longitudinal fixture completed **2,000 battles** with no negative wallet/BB and no premium spend. Per battle averages were **53.12 Money reward**, **85.215 Money healing purchased**, **2.5 Money refill cost**, and **7.65min free-recovery wait**; minimum wallet reached0. This is solvent through its bounded20-battle horizon but confirms that always healing fully is not sustainable. Monetization matrix full-heal nets remain negative in all sampled builds (for example L1 normal **−359.80**, L2 max **−270.78**, L10 normal **−573.95** per battle), while waiting for free recovery remains positive after BB as documented in Monetization014.
- **Asymmetric rosters:** added2,500 battles across all25 pairs of sizes1/2/4/8/16,100 seeds each; all completed with nonnegative BB and only7 draws. Equal-stat numerical superiority is decisive: every sampled unequal pair had the larger roster win100/100 and the smaller roster0/100 (for example1v16=0/100,16v1=100/100). Equal sizes stayed near symmetric across the aggregate;16v16 was51 attacker wins/48 defender wins/1 draw. This is not a combat defect, but opponent selection must continue surfacing fighter count/relative category and avoid presenting large roster mismatches as balanced.

## Decision

No tuning config was changed. The starter target is met, the bounded economy has a zero-resource path, recovery reaches readiness quickly, all asymmetric cases terminate, and observed premium advantage matches the user-approved014 direction. Changing combat/economy constants from these fixtures alone would be less safe than retaining the measured prototype.

## Fresh verification

- Release build:0 warnings,0 errors.
- Combat tests:46/46.
- Club domain tests:37/37.
- Core mass simulation:10,000/10,000 completed,0 invariant failures;4,913 attacker wins,5,001 defender wins,86 draws.
- Balance evidence:5,400 legacy stack battles;1,000 starter/recovery battles;2,500 asymmetric roster battles;2,000 economy-loop battles;4,000 monetization battles.

Raw final evidence: `implementation/evidence/012/final-balance-012.json`.
