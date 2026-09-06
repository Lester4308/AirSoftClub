# VERIFICATION 002 — Windows native IL2CPP retry

Current status (2026-09-06): **Windows x64 native IL2CPP gate PASSED; blocker CLOSED.** See [final native verification](VERIFICATION_002_IL2CPP_FINAL.md). Earlier blocker statements below describe the historical run.


Date: 2026-09-06. Repository: C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game/.
Branch: codex/implementation-002-unity-host.
Verified pre-retry HEAD: 7c50261a88ac752e70471032420cb4527f64a3aa.

**GATE: BLOCKED. The Unity module blocker is resolved; the Windows SDK dependency is missing. Native IL2CPP build/run is NOT PASSED.**

This report supersedes only the environment-blocker finding in [VERIFICATION_002.md](VERIFICATION_002.md). No game code, formulas, RNG, serialization, golden fixtures or test scripts changed.

## Precheck and detected environment

Correct repository/branch/exact HEAD verified; initial working tree and staging clean. Existing branch retained for a separate verification-only commit. All write operations stayed within this repository. Project Airsoft was not modified.

| Component | Detected |
|---|---|
| Unity Editor | 6000.3.21f1 LTS, revision c02631ffc030 |
| Unity Windows Standalone support | Installed |
| Windows IL2CPP variations | win64_player_development_il2cpp and win64_player_nondevelopment_il2cpp present; x86 and ARM64 IL2CPP variations also present |
| Visual Studio | Community 2026, 18.8.2; installationVersion 18.8.12023.21 |
| Visual Studio location | C:/Program Files/Microsoft Visual Studio/18/Community |
| C++ tool component | Microsoft.VisualStudio.Component.VC.Tools.x86.x64 detected by vswhere |
| MSVC toolset | 14.51.36231 |
| x64 cl.exe | VC/Tools/MSVC/14.51.36231/bin/Hostx64/x64/cl.exe, file version 19.51.36252.0 |
| Required Windows SDK | **Missing: Windows SDK version 10.0.19041.0 or newer** |
| SDK discovery | No Windows v10.0 installation registry result; no Windows Kits/10/Lib directory found at the standard location; only NETFXSDK under Windows Kits |

A .NET Framework SDK is not the Windows SDK required by this C++ toolchain. MSVC exists; the error identifies the missing Windows SDK specifically. No environment installation or registry workaround was attempted.

Detection commands included:
```powershell
git rev-parse --show-toplevel
git branch --show-current
git rev-parse HEAD
git status --porcelain=v1
& 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe' -all -products '*' -format json
& 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe' -all -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
Get-ChildItem 'C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations' -Name
Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0'
Get-ChildItem 'C:\Program Files (x86)\Windows Kits\10\Lib'
```

## Actual native build attempt

Exact repository command:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
```

Expanded editor invocation:
```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe' -batchmode -nographics -projectPath 'C:\Users\Ihor\Documents\ChatGPT\Airsoft_Club_Game\UnityHost' -logFile 'C:\Users\Ihor\Documents\ChatGPT\Airsoft_Club_Game\Artifacts\IL2CPPBuild.log' -quit -executeMethod BuildGate.Il2Cpp
```

Existing BuildGate selects StandaloneWindows64, ScriptingImplementation.IL2CPP, ApiCompatibilityLevel.NET_Standard (.NET Standard 2.1), Development build, empty TechnicalBootstrap scene, and output Artifacts/IL2CPP/AirsoftClubIntegration.exe. This invoked the real BuildPipeline native player build, not the conversion-only fallback.

Build failed during native toolchain setup; it did not complete compile/link or produce the requested executable. Unity log records return code 1. No native executable was launched and no stale executable was used as evidence.

Exact decisive diagnostics:
```text
Internal build system error. BuildProgram exited with code 1.
error: Could not set up a toolchain for Architecture x64.
Windows SDK (version 10.0.19041.0 or newer) is not installed.
Unity.IL2CPP.Bee.BuildLogic.ToolchainNotFoundException
```

The same diagnostic accepts Visual Studio 2022 or newer with C++ compilers and Windows SDK >=10.0.19041.0 (also lists VS2019). It states SDK discovery uses SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\v10.0\InstallationFolder.

Log: Artifacts/IL2CPPBuild.log (ignored build artifact).
SHA-256: E8403AC534BDA25F6C5501A99245F8A31B0FA5D5E457EC66272DAFD7BFA582E7.

After Unity had exited and its failure was recorded, the PowerShell Start-Process -Wait wrapper remained waiting; only that identified wrapper was closed. No Unity/game/editor source or environment setting was changed to bypass the failure.

## Regression results from this retry

| Check | Result |
|---|---|
| Standalone Release build | PASS, 0 compiler warnings / 0 errors |
| Standalone named tests | **46 passed / 0 failed** |
| Deterministic repeats | **100/100** |
| Standalone mass simulation | **10000/10000 completed; 0 invariant failures** |
| EditMode | **9 passed / 0 failed / 0 skipped** |
| PlayMode | **1 passed / 0 failed / 0 skipped** |
| Existing verified Windows Mono executable rerun | Exit0, golden/wire/numeric checks and 300 battles passed |
| Native IL2CPP executable run | **NOT RUN — no successful native build** |
| Native SplitMix64/fixed arithmetic/golden/roundtrip/16v16 | **NOT VERIFIED on native IL2CPP** |

Exact regression commands:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
```

Standalone mass results unchanged: attacker4913, defender5001, draw86 (0.86%); simulated duration min1552 / mean6089.70 / max16506 ms; BB mean81.05/81.30. Unity Editor batch300 and Mono player batch300 passed, including 100×16v16 each.

Verified digest again in .NET and Unity managed paths:
```text
22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f
```
This remains the expected native digest, **not an observed native IL2CPP result**.

Unity batch logs also contained licensing-refresh/asset-discovery messages; successful managed tests are recorded separately from the decisive native ToolchainNotFoundException. No deterministic mismatch was observed in completed managed runs. Native determinism remains untested.

## Required next environment change / stop

Install **Windows SDK 10.0.19041.0 or newer**, with Windows headers, libraries and tools, for the existing Visual Studio C++ installation. There is no evidence that MSVC or Unity's Windows IL2CPP module must be reinstalled.

After that environment prerequisite is available, rerun IL2CPPBuild, then:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPRun
```

Only successful compile/link, actual executable launch and matching native golden/numeric/serialization/16v16 checks can fully close this blocker. **Blocker not fully closed.** This retry changes only this verification document; no gameplay implementation or balance changes. STOP.
