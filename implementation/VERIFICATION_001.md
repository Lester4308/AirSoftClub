# Verification — Implementation 001

Date: 2026-09-06. Branch: codex/implementation-001-battle-core. Parent design commit: b12e317eb60ed43b1443252999ffa5f0908fd7de.

## Green gate

Airsoft_Club_Game root and exact HEAD verified before writes. Initial working tree/staging clean. No remotes or upstream configured; ahead/behind is not applicable. Created an independent implementation branch from the verified design commit.

Project Airsoft workspace at C:/Users/Ihor/Documents/ChatGPT/Розробка Страйкбол had clean Git status before work. No write command targeted it. Final status was checked again before this milestone commit. No imported code, architecture, history or assets.

## Exact verification commands

Run from C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game/:

```powershell
dotnet build AirsoftClubGame.sln -c Release
dotnet run --project tests/Airsoft.Battle.Tests -c Release --no-build
dotnet run --project tests/Airsoft.Battle.Tests -c Release --no-build -- --simulate 10000
dotnet format whitespace AirsoftClubGame.sln --verify-no-changes --no-restore
git -c core.whitespace=blank-at-eol,blank-at-eof,space-before-tab,cr-at-eol diff --check
```

Complete automation:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
```

The script also scans core source for float/double, Unity object dependencies, Time.deltaTime and System.Random. Tests are a dependency-free console runner with nonzero exit on failure, not dotnet test discovery. No existing repo-specific static tooling was present; compiler warnings-as-errors, format verification and forbidden-dependency scan are enabled.

## Results

- Release build: PASS, 0 errors, 0 compiler warnings.
- Named unit/scenario tests: **45 passed, 0 failed**.
- Same configuration/seed: **100 repeat results byte-identical** to the baseline full 16v16 authoritative result.
- All **256** roster-size pairs 1..16 × 1..16 checked for invariants/termination, plus required size fixtures across 20 seeds each.
- Independent event replay checks HP, alive actors/targets at batch start, no resurrection, weaponless non-attack and projectile/ammo conservation.
- Technical event guard test returns explicit TechnicalFailure with no gameplay outcome and no partially committed batch.
- Format and forbidden-core-dependency checks: PASS.
- Git emitted informational LF→CRLF normalization warnings before staging; no compiler/analyzer warnings. Source changes were staged with core.autocrlf=false; no functional warning was suppressed.

## Mass simulation — 10000 battles

Deterministic seeds 0–9999, rotating 1v1/1v2/2v1/3v3/8v8/16v16 and temporary weapon/armor/BB fixtures.

| Metric | Observed |
|---|---|
| Completed / invariant failures | 10000 / 0 |
| Termination rate | 100.00% |
| Attacker wins | 4913 (49.13%) |
| Defender wins | 5001 (50.01%) |
| Draws | 86 (0.86%) |
| Minimum simulated duration | 1552 ms |
| Mean simulated duration | 6089.70 ms |
| Median / p95 simulated duration | 6208 / 10864 ms |
| Maximum simulated duration | 16506 ms |
| Mean BB consumed attacker / defender | 81.05 / 81.30 |
| Elimination / simultaneous-elimination endings | 9914 / 86 |
| Duration cap / technical failures in mass run | 0 / 0 |

No-progress, duration-cap and technical-guard cases were forced in named tests, not expected in this stocked normal-fixture mass run. A 1000 or larger run is available by changing --simulate count (up to 1000000). Statistics use decimal formatting only in the non-authoritative harness; the combat library uses no float/double.

No obvious nontermination, zero-damage armor wall or negative-ammo defect appeared. Small aggregate side imbalance is not evidence of a formula defect in this mixed scenario catalog. Match durations are short and all observed draws were simultaneous elimination; both observations concern prototype tuning, not validated production balance. No final premium advantage claim is made from this run.

## Changed files and boundaries

- Root: .gitignore, AirsoftClubGame.sln, Directory.Build.props, global.json; minimal authorization notes in AGENTS.md and README.md.
- Core: Airsoft.Battle.csproj, Fixed.cs, Random.cs, Contracts.cs, Rules.cs, Result.cs, BattleEngine.cs.
- Tests/harness: Airsoft.Battle.Tests.csproj, Fixtures.cs, Program.cs.
- Verification tooling: tools/verify.ps1.
- Implementation records: IMPLEMENTATION_001.md, VERIFICATION_001.md, exact USER_REQUEST_001.txt.
- Design: only IMPLEMENTATION_QUESTIONS_v3.md to close Q01–Q04 for the core. Q05–Q15 remain open; inventory details beyond Q04 combat budget remain deferred.

Build outputs are ignored, not committed. Full formulas, event ordering, prototype constants, RNG algorithm and risks: [Implementation specification](IMPLEMENTATION_001.md).

Remaining risks: not yet executed under Unity/IL2CPP or a different platform; prototype fixture balance; fixed ordinal order allocates scarce same-time BB; complete event logging has memory cost near the technical guard. Production inventory, healing, economy, Steam, live PvP, UI/art and backend remain outside authorization.

**Implementation 001 complete after successful verification and commit. STOP; no subsequent systems started.**
