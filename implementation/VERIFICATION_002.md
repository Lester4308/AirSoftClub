# VERIFICATION 002 — Unity integration gate

Current status (2026-09-06): **Windows x64 native IL2CPP gate PASSED; blocker CLOSED.** See [final native verification](VERIFICATION_002_IL2CPP_FINAL.md). Earlier blocker statements below describe the historical run.


Date: 2026-09-06. Branch: codex/implementation-002-unity-host. Verified parent: 943d987147b82e63e8f26fe46f2f1cf002c19d09.

**Managed integration / deterministic serialization gate: PASSED.**
**IL2CPP C++ conversion: PASSED. Native Windows IL2CPP build/run: BLOCKED (missing installed backend module), not claimed passed.**

## Green gate and preservation

Correct Airsoft_Club_Game root, Implementation 001 branch/HEAD verified before work; working tree and staging empty. No remote/upstream; ahead/behind not applicable. New branch created at the verified parent without rewriting history. Project Airsoft workspace Git status checked read-only before and after; no write targeted it.

Six existing battle files moved to Runtime; normalized text comparison against 943d987 confirms unchanged contents. Formulas, HP, hit/tempo/damage/armor/BB and 120-second default cap unchanged. Existing standalone tests/mass outcomes are preserved.

## Runtime matrix

| Environment | Actual result |
|---|---|
| .NET SDK 10.0.302 Release build | PASS, 0 warnings / 0 errors |
| Standalone named suite | **46 passed, 0 failed** (45 prior + wire/golden integration group) |
| Standalone identical repeats | 100/100 byte-identical |
| Unity 6000.3.21f1 LTS EditMode / Mono | **9 passed, 0 failed, 0 skipped** |
| Unity PlayMode | **1 passed, 0 failed, 0 skipped** |
| Windows x64 Mono build | Success |
| Built Windows x64 Mono executable | Exit0; golden, serialization, numeric vectors, 300-battle batch PASS |
| Windows x64 IL2CPP BuildPipeline | Failed: currently selected scripting backend IL2CPP is not installed |
| Direct IL2CPP Windows AOT conversion | Exit0; generated Airsoft.Battle.cpp, AirsoftClub.Unity.cpp and code registration |
| Native IL2CPP compilation/link/execution | Not performed successfully; missing matching player backend remains blocker |

Unity exact revision: **6000.3.21f1 (c02631ffc030)**. API: .NET Standard 2.1; shared core target netstandard2.1. Settings currently record intended IL2CPP Standalone backend; MonoBuild explicitly selects Mono. Packages: local com.airsoftclub.core, Test Framework1.6.0, resolved NUnit2.0.5, JSONSerialize and test-required IMGUI.

## Golden and serialization

Seed: 0xFEDCBA9876543210; RulesetVersion prototype-001. Input, expected wire result and digest committed under the core package Resources; no runtime regeneration.

SHA-256 authoritative digest:
```text
22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f
```

.NET, EditMode, PlayMode and built Mono executable all compare full result wire bytes and the digest. Covered outcome, duration, final HP, BB, eliminated state, ordered events, seed/version. Config/Team/Result roundtrips include nested FighterSnapshot/BattleEvent/RulesetVersion. Nullable weapon and TechnicalFailure/null outcome checked; bad schema, trailing and truncated input rejected. The negative-test helper was corrected to catch InvalidDataException explicitly; no formula change was required.

Fixed signed multiplication/division/truncation, clamp, overflow and SplitMix64 vectors match .NET/Unity managed paths. Input permutation remains deterministic. Explicit serializer and conversion do not provide server authority; future backend validation remains required.

## Simulation and technical performance

Standalone regression harness remains **10000 completed / 0 invariant failures**, attacker4913 / defender5001 / draw86 (0.86%). Duration min1552 / mean6089.70 / max16506 simulated ms; mean BB81.05/81.30. Same figures as Implementation001.

Unity EditMode ran 300 battles; built Mono player ran another 300, each split across 100×1v1, 100×8v8 and 100×16v16. All invariant checks passed. Approximate warm wall-clock measurements exclude fixture preparation but include result invariant checks:

| Runtime | 100×1v1 | 100×8v8 | 100×16v16 |
|---|---|---|---|
| Unity Editor | 6.384 ms | 19.027 ms | 38.093 ms |
| Windows Mono executable | 6.519 ms | 20.334 ms | 36.350 ms |

GC.GetTotalMemory deltas were measured, **not allocation totals**: Editor -151552/+40960/+503808 bytes; player +98304/+208896/+90112. Negative delta demonstrates GC noise. Do not infer per-battle allocation from these values. Visible likely allocation sites are per-timestamp arrays/target LINQ and event/result collections; no profiler-based hotspot claim or premature optimization.

## Exact commands

All run from repository root. Editor path default: C:/Program Files/Unity/Hub/Editor/6000.3.21f1/Editor/Unity.exe.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-aot.ps1
```

Editor tests expand to -batchmode -nographics -projectPath UnityHost -runTests -testPlatform EditMode|PlayMode -testResults absolute XML path -logFile absolute log path. No -quit on test invocations. Builds use -quit -executeMethod BuildGate.Managed|BuildGate.Il2Cpp. Built player uses -batchmode -nographics --airsoft-smoke -logFile absolute path. Scripts validate process exit and nonempty test pass counts/success markers.

AOT fallback copies generated Mono player assemblies into ignored Artifacts/AotManaged, overlays the installed **unityaot-win32 BCL**, then invokes:
```text
il2cpp.exe --convert-to-cpp --platform=WindowsDesktop --architecture=x64 --dotnetprofile=unityaot-win32 --directory=Artifacts/AotManaged --generatedcppdir=Artifacts/AotConversion --data-folder=Artifacts/AotData --enable-array-bounds-check --enable-divide-by-zero-check --jobs=4
```

The direct conversion used the same installed editor converter, not another Unity version. Initial attempts without the correct AOT profile/BCL failed; supplying its matching AOT libraries resolved conversion. This is generated validation input, not a duplicate production core. It does not verify Unity native player libraries, C++ linking, stripping behavior or IL2CPP execution.

Underlying standalone commands remain dotnet build, console test runner, --simulate10000, format verification and purity scan in tools/verify.ps1. Build outputs/logs/XML/C++ stay ignored in Artifacts; no cache or executable committed.

## Warnings / open risk

No project C# compiler errors/warnings in successful builds/tests. Unity batch logs included licensing refresh/access-token messages, keyboard GetVirtualKey notices and shutdown Curl42; these did not prevent licensed Editor tests or Mono build/run. The **native IL2CPP missing-module error is an unresolved environment blocker**. Install the exact matching Windows Build Support (IL2CPP) through an explicitly authorized environment change, then rerun IL2CPPBuild and IL2CPPRun. No installation outside the allowed repository was attempted.

## Changed scope

UnityHost Assets (adapter, editor builder, EditMode/PlayMode tests, empty technical scene + metas), minimal Packages and generated ProjectSettings; shared-core local package/asmdef/csc flags/metas; new BattleWire and golden resources; standalone integration tests; verification scripts; build-output isolation/.gitignore; implementation docs and minimal README/AGENTS updates. Q12 receives only verified technical facts; Q05–Q15 are not closed. Existing Implementation001 doc links adjusted only for moved files.

**STOP:** no economy, Recruitment, production UI/HUD, models/maps/art, Steam, backend/PostgreSQL, matchmaking or live PvP implementation.

## Committed file manifest

Unity-generated YAML trailing spaces are preserved using scoped .gitattributes rules; source-code whitespace checks remain enabled. Binary golden .bytes files are explicitly binary.

- .gitattributes
- .gitignore
- AGENTS.md
- Directory.Build.props
- README.md
- UnityHost/Assets/Editor.meta
- UnityHost/Assets/Editor/AirsoftClub.Editor.asmdef
- UnityHost/Assets/Editor/AirsoftClub.Editor.asmdef.meta
- UnityHost/Assets/Editor/BuildGate.cs
- UnityHost/Assets/Editor/BuildGate.cs.meta
- UnityHost/Assets/Runtime.meta
- UnityHost/Assets/Runtime/AirsoftClub.Unity.asmdef
- UnityHost/Assets/Runtime/AirsoftClub.Unity.asmdef.meta
- UnityHost/Assets/Runtime/BattleHost.cs
- UnityHost/Assets/Runtime/BattleHost.cs.meta
- UnityHost/Assets/Scenes.meta
- UnityHost/Assets/Scenes/TechnicalBootstrap.unity
- UnityHost/Assets/Scenes/TechnicalBootstrap.unity.meta
- UnityHost/Assets/Tests.meta
- UnityHost/Assets/Tests/EditMode.meta
- UnityHost/Assets/Tests/EditMode/AirsoftClub.Tests.EditMode.asmdef
- UnityHost/Assets/Tests/EditMode/AirsoftClub.Tests.EditMode.asmdef.meta
- UnityHost/Assets/Tests/EditMode/IntegrationTests.cs
- UnityHost/Assets/Tests/EditMode/IntegrationTests.cs.meta
- UnityHost/Assets/Tests/PlayMode.meta
- UnityHost/Assets/Tests/PlayMode/AirsoftClub.Tests.PlayMode.asmdef
- UnityHost/Assets/Tests/PlayMode/AirsoftClub.Tests.PlayMode.asmdef.meta
- UnityHost/Assets/Tests/PlayMode/RuntimeSmokeTests.cs
- UnityHost/Assets/Tests/PlayMode/RuntimeSmokeTests.cs.meta
- UnityHost/Packages/manifest.json
- UnityHost/Packages/packages-lock.json
- UnityHost/ProjectSettings/AudioManager.asset
- UnityHost/ProjectSettings/ClusterInputManager.asset
- UnityHost/ProjectSettings/DynamicsManager.asset
- UnityHost/ProjectSettings/EditorBuildSettings.asset
- UnityHost/ProjectSettings/EditorSettings.asset
- UnityHost/ProjectSettings/GraphicsSettings.asset
- UnityHost/ProjectSettings/InputManager.asset
- UnityHost/ProjectSettings/MemorySettings.asset
- UnityHost/ProjectSettings/MultiplayerManager.asset
- UnityHost/ProjectSettings/NavMeshAreas.asset
- UnityHost/ProjectSettings/Physics2DSettings.asset
- UnityHost/ProjectSettings/PresetManager.asset
- UnityHost/ProjectSettings/ProjectSettings.asset
- UnityHost/ProjectSettings/ProjectVersion.txt
- UnityHost/ProjectSettings/QualitySettings.asset
- UnityHost/ProjectSettings/SceneTemplateSettings.json
- UnityHost/ProjectSettings/TagManager.asset
- UnityHost/ProjectSettings/TimeManager.asset
- UnityHost/ProjectSettings/UnityConnectSettings.asset
- UnityHost/ProjectSettings/VFXManager.asset
- UnityHost/ProjectSettings/VersionControlSettings.asset
- design/IMPLEMENTATION_QUESTIONS_v3.md
- implementation/IMPLEMENTATION_001.md
- implementation/UNITY_INTEGRATION_002.md
- implementation/USER_REQUEST_002.txt
- implementation/VERIFICATION_002.md
- src/Airsoft.Battle/Airsoft.Battle.csproj.meta
- src/Airsoft.Battle/Runtime.meta
- src/Airsoft.Battle/Runtime/Airsoft.Battle.asmdef
- src/Airsoft.Battle/Runtime/Airsoft.Battle.asmdef.meta
- src/Airsoft.Battle/Runtime/BattleEngine.cs
- src/Airsoft.Battle/Runtime/BattleEngine.cs.meta
- src/Airsoft.Battle/Runtime/BattleWire.cs
- src/Airsoft.Battle/Runtime/BattleWire.cs.meta
- src/Airsoft.Battle/Runtime/Contracts.cs
- src/Airsoft.Battle/Runtime/Contracts.cs.meta
- src/Airsoft.Battle/Runtime/Fixed.cs
- src/Airsoft.Battle/Runtime/Fixed.cs.meta
- src/Airsoft.Battle/Runtime/Random.cs
- src/Airsoft.Battle/Runtime/Random.cs.meta
- src/Airsoft.Battle/Runtime/Resources.meta
- src/Airsoft.Battle/Runtime/Resources/golden-digest.txt
- src/Airsoft.Battle/Runtime/Resources/golden-digest.txt.meta
- src/Airsoft.Battle/Runtime/Resources/golden-input.bytes
- src/Airsoft.Battle/Runtime/Resources/golden-input.bytes.meta
- src/Airsoft.Battle/Runtime/Resources/golden-result.bytes
- src/Airsoft.Battle/Runtime/Resources/golden-result.bytes.meta
- src/Airsoft.Battle/Runtime/Result.cs
- src/Airsoft.Battle/Runtime/Result.cs.meta
- src/Airsoft.Battle/Runtime/Rules.cs
- src/Airsoft.Battle/Runtime/Rules.cs.meta
- src/Airsoft.Battle/Runtime/csc.rsp
- src/Airsoft.Battle/Runtime/csc.rsp.meta
- src/Airsoft.Battle/package.json
- src/Airsoft.Battle/package.json.meta
- tests/Airsoft.Battle.Tests/Integration002.cs
- tests/Airsoft.Battle.Tests/Program.cs
- tools/verify-aot.ps1
- tools/verify-unity.ps1
- tools/verify.ps1
