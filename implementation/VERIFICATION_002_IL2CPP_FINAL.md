# VERIFICATION 002 — Final Windows native IL2CPP gate

Date: 2026-09-06. Branch: codex/implementation-002-unity-host.
Verified starting commit: 1ae6fb8dc0857e171939b574aa175d3e2e46049c.

**IMPLEMENTATION 002 — WINDOWS x64 NATIVE IL2CPP GATE: PASSED. BLOCKER FULLY CLOSED.**

This successful native build/run supersedes the environment blockers recorded in [initial verification](VERIFICATION_002.md) and [SDK retry](VERIFICATION_002_IL2CPP_RETRY.md). No combat formulas, RNG, serialization, golden fixtures, tests or game code were changed.

## Precheck and environment

Repository root, branch and exact HEAD matched the user-provided point. Working tree and staging were clean before execution. All writes stayed within Airsoft_Club_Game; Project Airsoft was not modified.

- Unity **6000.3.21f1 LTS**, revision **c02631ffc030**.
- Windows Standalone x64 IL2CPP support installed.
- Windows SDK **10.0.26100.0**; registry product version10.0.26100, root C:/Program Files (x86)/Windows Kits/10/.
- SDK Include and Lib/10.0.26100.0 directories present, including um/x64/kernel32.lib.
- Existing toolchain: Visual Studio Community2026 /18.8.2, MSVC14.51.36231, compiler19.51.36252.0 (previous retry detection).
- Target: StandaloneWindows64, IL2CPP, Development build, .NET Standard2.1 API compatibility.

## Full pipeline and executable

Executed the actual Unity BuildPipeline path:

**C# → managed assemblies → IL2CPP-generated C++ → MSVC native compile/link → Windows x64 executable.**

Build command:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
```

The script invokes Unity6000.3.21f1 with -batchmode -nographics -projectPath UnityHost -quit -executeMethod BuildGate.Il2Cpp and an absolute Artifacts/IL2CPPBuild.log path.

Result: process exit0; log confirms:
```text
Build Finished, Result: Success.
AIRSOFT_BUILD_PASSED backend=IL2CPP
```

Executable: Artifacts/IL2CPP/AirsoftClubIntegration.exe.
Native code library: Artifacts/IL2CPP/GameAssembly.dll; PE machine **0x8664 (AMD64)** confirmed. This was not a conversion-only validation or a Mono substitute.

Native launch command:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPRun
```

This launches the generated executable with -batchmode -nographics --airsoft-smoke and Artifacts/IL2CPPRun.log. Result: **exit0**, expected success marker and exact digest.

## Native deterministic evidence

```text
AIRSOFT_PLAYER_PASSED digest=22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f
```

The existing PlayerSmoke path actually executed on IL2CPP:
- Golden result: full serialized bytes and SHA-256 canonical digest matched.
- MatchConfig, both TeamSnapshots and MatchResult serialization roundtrips passed.
- SplitMix64 seed-zero vectors E220A839 /6E789E6A /06C45D18 matched.
- Fixed-point signed truncation, multiplication, division, clamp and overflow checks passed.
- **300 batch battles** completed: 100×1v1, 100×8v8, **100×16v16**.
- Each batch checked completed status, duration bound, HP bounds, nonnegative BB and projectile/ammo accounting. No deterministic mismatch or invariant failure.

Native timings for 100 battles: 1v1 **5.363ms**, 8v8 **18.496ms**, 16v16 **28.960ms**. These warm technical observations include invariant validation, not presentation; no performance optimization or balance claim.

## Fresh regression matrix

| Runtime/check | This pass |
|---|---|
| Standalone Release build | PASS, 0 compiler warnings /0 errors |
| Standalone named tests | **46 passed /0 failed** |
| Deterministic repeat | **100/100 identical** |
| Standalone mass simulation | **10000/10000 completed; 0 invariant failures** |
| Unity EditMode | **9 passed /0 failed /0 skipped** |
| Unity PlayMode | **1 passed /0 failed /0 skipped** |
| Existing verified Windows Mono executable rerun | Exit0, golden/numeric/serialization and300-battle batch PASS |
| New Windows native IL2CPP executable | Exit0, golden/numeric/serialization and300-battle batch PASS |

The same expected digest was verified in standalone .NET, EditMode, PlayMode, Mono executable and native IL2CPP. Native smoke is one technical execution path with300 batch scenarios, not a claim that the entire46-test .NET suite ran natively.

Regression commands:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
```

Standalone mass statistics unchanged: attacker4913 /defender5001 /draw86 (0.86%); simulated duration min1552 /mean6089.70 /max16506ms; mean BB81.05/81.30. Format/purity checks also passed.

## Artifact identities / warnings

Generated binaries and logs remain ignored, not committed. SHA-256:

| Artifact | SHA-256 |
|---|---|
| AirsoftClubIntegration.exe | 9688AE089D590352DBE0CBA8722328926CD4D9B7722F68EC9942243925C1282A |
| GameAssembly.dll | FF664804B3221C51832E075711FA7CB93F916AFD8163E1E652DF6AD0B1F9912F |
| IL2CPPBuild.log | A02235E2A57BB87608371129ABD5CA69F669CF4BF6EDC62786A2EFF43579C0AA |
| IL2CPPRun.log | CCABDE321CBA22F9673258B18551398A34ABF84ADF7086DE9AB5FB6809B864BD |

No project C#/native compiler warning or error lines were found in the successful native build log. Unity logged a licensing refresh message: Access token is unavailable; failed to update. It did not block build/run. Git reported a potential LF→CRLF normalization notice for ProjectSettings.asset, but no tracked settings/content diff remained.

## Closure

Windows SDK and native Unity module prerequisites now work together. Native compile/link, launch, digest, numeric/RNG/serialization and16v16 checks passed; regressions remain green.

**No remaining blocker for Implementation002's Windows x64 native IL2CPP gate.** This is verification of the existing development smoke executable, not a production release certification. Only documentation changes are committed. No UI/economy/Steam/backend/art/PvP work began. STOP.
