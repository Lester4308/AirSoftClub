# IMPLEMENTATION 001 — Deterministic Headless Battle Core

User authorization: [exact request](USER_REQUEST_001.txt). Design base: b12e317eb60ed43b1443252999ffa5f0908fd7de. Branch: codex/implementation-001-battle-core. This milestone authorizes only pure C# combat. Earlier design-gate statements that implementation was not authorized describe the pre-milestone state; all unrelated systems remain unauthorized.

## Architecture and execution

[Airsoft.Battle](../src/Airsoft.Battle/Airsoft.Battle.csproj) targets netstandard2.1, with no external package or Unity dependency. SDK 10.0.302 builds it; the [console tests/harness](../tests/Airsoft.Battle.Tests/Airsoft.Battle.Tests.csproj) target net10.0. A future Unity host must validate its own API compatibility; no Unity scene or engine installation was needed. ASP.NET Core can later reference the domain without making client execution authoritative.

Contracts are sealed/get-only, and collections are copied, ordinal-ID sorted and exposed read-only. Fighter IDs need be unique within each side; side + ID identifies a battle actor. TeamSnapshot accepts 1–16 fighters. MatchConfig validates prepared health against the selected rules. No recruitment, roster selection service, recovery clocks, live inventory, accounts, rewards or ratings exist.

Public entry point: BattleEngine.Run(MatchConfig). Default RNG is created fresh from Seed per Run. An injected RNG factory is supported for tests; reproducibility requires the same RNG algorithm/behavior, not merely a matching seed. Injected invalid range results throw a diagnostic exception. Invalid contracts are rejected before simulation. The standard engine has no shared mutable battle state.

MatchResult includes status, nullable outcome, reason, simulated milliseconds, per-fighter MaxHP/final HP/elimination, per-side consumed/remaining BB, immutable projectile events, seed and ruleset version. Completed outcomes: AttackerWin, DefenderWin, Draw. TechnicalFailure has no gameplay outcome. ToCanonicalBytes uses invariant length-prefixed UTF-8 fields with a serialization version. Retain full config/snapshots with the result for replay; a version label alone does not serialize all coefficients.

## Numeric model

[Fixed](../src/Airsoft.Battle/Runtime/Fixed.cs): signed Int64 raw units, **10000 = 1.0000**. Add/subtract/multiply/divide use checked arithmetic. Multiplication and division truncate toward zero after the indicated operation, including negative boundary tests. There are no authoritative float/double calculations. Intermediate overflow throws rather than wrapping; domain validation bounds operational inputs so standard simulation cannot overflow.

Stats <=10000; weapon damage/protection/penetration <=10000; BB multiplier >0 and <=2; HP <=1000100; weapon interval 1–60000 ms; 1–64 projectiles; BB budget 0–1000000; duration 1–3600000 ms. These are representational/validation bounds, not balance targets. Revisit them with a ruleset/contract version change if future content needs more.

Readiness uses exact comparison currentHP * 10 >= MaxHP with 0 < currentHP <= MaxHP. Thus below 10% NotReady, 10–100% Ready; no latch. No recovery timer. Increasing Endurance recomputes MaxHP while caller-supplied CurrentHP stays unchanged. Example: 80/100 → 80/110.

## Rules and formulas

All coefficients and caps live in immutable [BattleRules](../src/Airsoft.Battle/Runtime/Rules.cs). Defaults are **BALANCE HYPOTHESIS / PROTOTYPE CONFIG**, version prototype-001.

| Formula | Prototype defaults |
|---|---|
| MaxHP = BaseHP + Endurance × HpPerEndurance | 50 + Endurance × 5 |
| EffectiveAgility = max(0, Agility − equipment AgilityPenalty) | Penalty supplied by loadout |
| Evasion = EffectiveAgility × EvasionCoefficient + equipment EvasionBonus | Coefficient 0.005 |
| HitChance = clamp(BaseHit + Accuracy × AccuracyCoefficient + weapon contribution − target Evasion, MinHit, MaxHit) | Base 0.50; coefficient 0.01; cap 0.05–0.95 |
| Speed = 1 + EffectiveAgility × TempoCoefficient | Coefficient 0.02 |
| IntervalMs = max(MinIntervalMs, ceil(weapon IntervalMs / Speed)) | Minimum 10 ms; integer ceiling |
| RawDamage = weapon Damage × BB DamageMultiplier | Fixed multiplication |
| EffectiveProtection = max(0, armor Protection − weapon Penetration − BB Penetration) | Prepared loadout protection |
| Mitigation = min(MaxMitigation, EffectiveProtection / (ArmorScale + EffectiveProtection)) | Scale 50; maximum 0.80, validated <1 |
| HitDamage = max(one raw quantum, RawDamage × (1 − Mitigation)) | Quantum 0.0001 HP |
| Duration deadline | 120000 ms temporary, exclusive |
| Technical projectile-event guard | 1000000 events, separate failure path |

Exactly three base fighter stats: Accuracy, Endurance, Agility. No critical hits. Equipment combat values represent camouflage/head/body contributions without implementing inventory. Six weapon families exist as identifiers, with configurable damage, accuracy, penetration, interval and projectiles.

## RNG and target selection

[SeededRandom](../src/Airsoft.Battle/Runtime/Random.cs) implements SplitMix64 with explicit unsigned wraparound, increment 0x9E3779B97F4A7C15, mixing multipliers 0xBF58476D1CE4E5B9 and 0x94D049BB133111EB. NextUInt32 returns the high 32 bits. Seed 0 is valid; golden-vector tests freeze the sequence.

NextInt(bound) uses multiply-high: (UInt64(nextUInt32) × bound) >> 32. It consumes exactly one RNG draw and always terminates, avoiding a rejection loop. There is at most one source-value difference between range bucket populations; this tiny quantization bias is accepted for this prototype. This is not cryptographic randomness.

Target policy: uniform seeded choice from ordinal-ID sorted alive enemy fighters at batch start, once per volley; every projectile in that volley uses that target and gets an independent hit roll in [0,10000). This supplies simple variance without tactical AI. Reordering input collections does not affect output.

## Event scheduling and simultaneous damage

No round-robin or wall clock. Initial next-action time equals each fighter's interval; weaponless fighters have no ranged schedule. Each iteration jumps to the minimum eligible next-action time. Within a timestamp:

1. Freeze alive actors and valid targets conceptually by deferring all damage.
2. Reserve/check the total projectile count for the whole timestamp against the technical guard.
3. In Attacker-then-Defender, ordinal fighter-ID order, allocate shared BB and consume target/hit RNG draws.
4. Emit one BattleEvent per projectile, accumulate per-target damage, and schedule acting fighters at time + positive interval.
5. Apply aggregate damage simultaneously, clamping HP to zero. Evaluate elimination before selecting a later timestamp.

A fighter alive at batch start may fire even if another shot in the same batch eliminates it. It never fires at a later time after elimination. Targets eliminated by aggregate damage remain valid for shots already in that simultaneous batch; overkill is allowed, resurrection is not. Event Damage is pre-overkill-clamp damage; replay aggregates by timestamp before clamping. Same-time final elimination of both sides yields Draw, independent of input iteration order.

Stable side/ID ordering determines RNG consumption and which same-team fighter receives scarce last BB. That ordering is explicit and deterministic, but is a prototype scheduling policy, not a claim of competitive fairness under all swaps of team identity.

## BB, weaponless and termination

One tier and one finite shared BB budget per side. DefenseAtCapacity uses **100% captured maximum club capacity** as virtual defense budget. Caller supplies full-HP defense fighters; the generic engine does not read or infer live profile state.

One projectile consumes one BB, including every burst bullet and shotgun pellet. If stock is below planned projectiles, fire a partial volley of min(stock, planned); no hidden free projectile or negative stock. No auto-switch tier. A ready weaponless fighter is present and targetable but cannot attack: no fallback gun/melee/fists.

One side out of ammo does not stop an armed enemy. If neither side has an alive armed fighter with BB, NoProgress Draw. Valid damage is positive and mitigation <100%, so an armed fighter with BB can potentially damage every alive target.

Elimination is checked before NoProgress. Next action at or beyond MaxDuration produces DurationLimit Draw at exactly the deadline without firing that action. No Team Power tie-break. Positive intervals guarantee monotonic time. Finite duration bounds execution; the separate projectile guard also bounds event storage. If a batch would exceed the guard, return TechnicalFailure with diagnostic TECHNICAL_EVENT_GUARD and no Outcome before changing that batch's HP/BB/RNG. The result contains only previously committed batches. Never treat this as a rewarded draw.

## Test catalog and verification

[Fixtures](../tests/Airsoft.Battle.Tests/Fixtures.cs) contains 6 weapons, None/Light/Medium/Heavy armor and Basic/Improved/Advanced/High-End/Premium BB (1/1.03/1.05/1.10/1.15 damage multipliers). **PROTOTYPE TEST VALUES — NOT FINAL BALANCE.** They are test-only, not a production shop.

The dependency-free console runner executes named unit/scenario tests, exits nonzero on any failure, and offers --simulate N up to 1000000. It is intentionally not a dotnet test-discovered framework; use the exact commands in [verification report](VERIFICATION_001.md) or [verify.ps1](../tools/verify.ps1).

An independent event replay validator checks live actors/targets at each timestamp, HP conservation/clamps, no resurrection, no weaponless attacks, per-projectile ammo conservation and termination. Coverage includes all 256 size pairs, 100 repeated byte-identical 16v16 results, alternative seeds, input permutations, fixed/RNG boundaries and technical failure.

## Remaining limits / stop

Q01–Q04 core decisions are closed for this prototype; Q05+ remain open. Full inventory capacity/overflow, healing clocks, production balance, cross-runtime Unity/IL2CPP verification, hosting/persistence and social/economy systems are outside this milestone. A complete 1000000-event log may consume substantial memory; it is diagnostic guard capacity, not a normal gameplay target. Future batch simulation may need an explicit summary-only output mode after profiling.

No UI, Unity scenes, Steam, ASP.NET backend, PostgreSQL, live PvP, Credits, monetization, recruitment, healing service, matchmaking, leaderboards or production art were implemented. Stop after verification/commit; another explicit prompt is required for further systems.
