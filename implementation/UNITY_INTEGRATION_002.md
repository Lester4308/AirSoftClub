# UNITY INTEGRATION 002 — Unity host & combat integration gate

Current status (2026-09-06): **Windows x64 native IL2CPP gate PASSED; blocker CLOSED.** See [final native verification](VERIFICATION_002_IL2CPP_FINAL.md). Earlier blocker statements below describe the historical run.


Scope: [exact authorization](USER_REQUEST_002.txt). Base: 943d987147b82e63e8f26fe46f2f1cf002c19d09; new branch codex/implementation-002-unity-host. Implementation 001 history remains intact. No battle formula or balance change.

## Confirmed version and environment

Unity **6000.3.21f1 LTS (c02631ffc030)**, installed production release, Windows x64. [Official release notes](https://unity.com/releases/editor/whats-new/6000.3.21f1) and [Unity 6 LTS support](https://unity.com/releases/unity-6/support). Chosen from installed supported LTS editors; not beta/experimental and not a claim to be the newest patch.

API compatibility: .NET Standard 2.1 (ApiCompatibilityLevel.NET_Standard); core project remains netstandard2.1. Standalone SDK 10.0.302 / test host net10.0. Editor/PlayMode and built Windows managed player use Mono. Intended production backend is Windows x64 IL2CPP; **native build/run blocked by missing Windows IL2CPP backend module**. Direct conversion using this editor's IL2CPP converter and unityaot-win32 BCL succeeded.

## Single-source assembly structure

| Assembly | Source / references |
|---|---|
| Airsoft.Battle | src/Airsoft.Battle/Runtime; pure code, noEngineReferences=true |
| AirsoftClub.Unity | UnityHost/Assets/Runtime; references Airsoft.Battle |
| AirsoftClub.Tests.EditMode | Editor-only tests → core + Unity adapter |
| AirsoftClub.Tests.PlayMode | Runtime smoke → core + Unity adapter |
| AirsoftClub.Editor | Editor-only build utility |

[Package manifest](../UnityHost/Packages/manifest.json) references com.airsoftclub.core through file:../../src/Airsoft.Battle. The standalone csproj compiles the same Runtime directory; there is no copied simulator or DLL deployment step. Existing six combat files moved unchanged into Runtime. New BattleWire is pure C# serialization, not a second battle engine.

Build outputs relocated to ignored root Artifacts so Unity's local package never imports generated obj C# files. Package/Assets .meta GUIDs are versioned. ProjectSettings and package lock are committed; Library/Temp/Logs/UserSettings/builds/IDE caches are ignored. Minimal dependencies: Test Framework 1.6.0, resolved NUnit 2.0.5, JSONSerialize and transitive IMGUI required by tests; no Steam/Netcode/DOTS/Addressables packages.

No circular references. Core uses no UnityEngine/objects/time/RNG. Its csc.rsp enables nullable and checked arithmetic in Unity. No records or dynamic generated serializers.

## Host / scene

BattleHost.Execute takes encoded MatchConfig, decodes validated immutable snapshots, invokes the same BattleEngine and returns its result. Validation exceptions propagate explicitly. Technical guard stays TechnicalFailure with null gameplay outcome; it is not converted to Draw.

EditMode and PlayMode instantiate the plain adapter without scene presentation. BuildGate creates a completely empty TechnicalBootstrap scene (zero fighter/map/camera/HUD objects). A RuntimeInitializeOnLoad callback runs only with --airsoft-smoke, verifies golden/wire/numeric contracts, runs a batch and exits with code 0/1. This is a technical executable, not gameplay UI.

## Serialization v1

[BattleWire](../src/Airsoft.Battle/Runtime/BattleWire.cs) uses explicit BinaryWriter/BinaryReader primitives, no reflection-based object serializer. All integers little-endian. Header: Int32 magic 0x41434257, Int32 schema=1, Int32 kind (1=config, 2=team, 3=result). Strings: Int32 UTF-8 byte length, strict UTF-8 bytes (max4096). Booleans: exactly byte 0/1. Fixed numbers: Int64 raw (scale10000). Seed: UInt64, preserving values above Int64 range.

Field order:
- Config: seed; rules version, duration/event/min-interval limits, ten coefficient raw values in constructor order; attacker team; defender team.
- Team: BB ID/multiplier/penetration, budget, fighter count; fighters ordinal ID order with ID, Accuracy/Endurance/Agility/StartingHP, armor protection/evasion/penalty, weapon-present byte and optional complete weapon values.
- Result: status, outcome (-1 means null), end reason, diagnostic, duration, seed, rules version; attacker/defender HP and BB summaries; ordered projectile events (time, actor side/ID, target ID, hit, raw damage).
- Eliminations are derived from final HP=0, matching existing result semantics.

Config, team and result each have public encode/decode paths. Nested fighter/weapon/armor/BB/event/version fields are explicit, not serializer defaults. Missing/truncated fields, unsupported schema/kind, invalid enum/boolean/count/HP/order, event/ammo mismatch and trailing bytes are rejected. Limits: packet64MiB, fighters1..16, events<=1000000. Existing domain constructors validate config. No nullable implicit defaults; absent weapon and failure outcome are explicitly represented.

This is an integration/debug boundary, not authorization: deserializing a result does not prove a remote simulation or authorize rewards. A future backend must resolve/verify independently. No migration engine or backward compatibility guess: schema1 is exact, unknown versions rejected. RulesetVersion is preserved alongside all input coefficients; archive complete inputs and matching algorithm versions for replay. New schema requires explicit support rather than silently accepting missing fields.

## Golden fixture and digest

Versioned Resources in the shared package:
- [golden-input.bytes](../src/Airsoft.Battle/Runtime/Resources/golden-input.bytes)
- [golden-result.bytes](../src/Airsoft.Battle/Runtime/Resources/golden-result.bytes)
- [golden-digest.txt](../src/Airsoft.Battle/Runtime/Resources/golden-digest.txt)

Input: rules prototype-001 with explicit full coefficients, seed **0xFEDCBA9876543210**, 3 attackers / 2 defenders, weapons/armor/BB and starting HP included. Canonical result created in standalone .NET and committed as the expected artifact. Normal verification never rewrites it. Explicit regeneration command --generate-golden is only for intentional reviewed fixture changes.

SHA-256 digest of existing versioned MatchResult.ToCanonicalBytes:
**22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f**

Canonicalization includes ordered outcome/status/reason, duration, HP, BB, event sequence, seed and rules version with invariant formatting, not runtime object hash codes. Both full wire result bytes and digest are compared across .NET, Unity EditMode, PlayMode and Windows Mono player. All match.

## Deterministic / AOT audit

Ordinal-ID snapshots and explicitly ordered arrays drive scheduling, targets, damage application and event emission. Dictionary/HashSet usage in standalone tests is not authoritative battle ordering; serialization writes no dictionaries. Existing input permutation tests remain green. No authoritative wall clock, Time or Unity random.

SplitMix64 seed-zero vector: E220A839, 6E789E6A, 06C45D18. Additional fixed seeds are covered by repeated golden and different-seed runs. Signed fixed arithmetic, division/multiplication/truncation, clamp, overflow and readiness were checked in Editor/runtime; .NET suite covers numeric boundaries in depth.

The AOT path uses sealed classes, read-only collections, explicit constructors and static generic instantiations. Enum.IsDefined/Enum.ToObject use known enums; there is no Reflection.Emit/dynamic assembly generation, runtime-created generic serializer or ScriptableObject authority. SHA256/BinaryReader/LINQ paths execute in Mono and convert to C++ successfully. Direct conversion is **not** proof of native linking, stripping or IL2CPP execution.

## How to run

Open **UnityHost/** in Unity Hub with 6000.3.21f1. Test Runner offers AirsoftClub.Tests.EditMode and PlayMode.

From repository root:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-aot.ps1
```

After the matching Windows IL2CPP support module is available, rerun IL2CPPBuild then -Stage IL2CPPRun. The missing-module error is currently expected; do not substitute a successful Mono run. Scripts accept explicit editor path overrides. Logs/XML/generated code/binaries go only into ignored Artifacts.

## Limitations and stop

[Verification report](VERIFICATION_002.md) distinguishes every pass from the native IL2CPP blocker. Mono baseline includes execution plus invariant checks, not a rendering benchmark. Heap deltas are GC-sensitive, not allocated-byte measurements. Tiny timings are approximate warm samples, not optimization targets.

Q05–Q15 remain open; only verified Unity/API/shared-library/serialization information is added to Q12. Hosting/PostgreSQL/backend deployment are still OPEN. No UI/art/Steam/backend/economy/Recruitment/healing/PvP systems were started. This gate stops after verification and commit; another explicit prompt is required.
