# Playable Visual Alpha014 — 2026-09-07

**Local playable visual prototype: PASS.** Functional systems preserved; progressive early-access controls implemented. Final art, final balance and live Steam sandbox remain OPEN. Product approval of this prototype's aesthetics is not assumed.

Starting clean point: codex/implementation-003-development-pass at7313ed07911ee5861b32c0a130397f5d66c2e4c2. Code delivery commits:91c7b5c (monetization/policy/simulations) andbc42d17 (visual shell, immutable appearance, walkthrough). Final evidence/package documentation commit is a descendant; exact final HEAD is returned in the delivery response and packaged BUILD-MANIFEST.json.

## Product / monetization

See [Monetization014](MONETIZATION_BALANCE_014.md): no early unlock at Club1; depth1 at2, depth2 at3–4, depth3 at5+. Permanent entitlement followed by Money item purchase; configurable relative Credits1/2/4. Individual early contribution capped at125% of best natural current-band normal gear; natural-level release requires no rebuy. Strict combined15–20% ceiling is superseded, premium combat advantage remains intentional.

Old Level1 stack2.445× becomes1.061×; Level2 measured1.217× in exhaustive sustained DPS × effective-HP audit. The40-cell/4,000-battle report includes power, wins, damage, HP, BBcost, XP-speed estimate and money efficiency. Instant full healing remains expensive; wait-for-recovery economics and late catalog progression need further tuning. No new payment wall or zero-resource dead end introduced.

## Playable loop and destinations

Launch local backend and client, Connect using stable development account, choose one free recruit, buy/equip a Money weapon, train, manage shared BB, select opponent/mode, confirm battle, watch/skip authoritative replay, inspect saved rewards/history, recover and repeat. Server owns currency/progression, snapshots, policy, simulation and settlement. Relaunch restores state.

| Area | Functional surface |
|---|---|
| Club hub | Team silhouettes, wallet, rating, Club XP/next level, ready count, navigation, daily/earned Credits, emergency BB, conversion, shields |
| Roster / details | Name, three stats, fighter level/XP/cap, HP/readiness, prototype appearance, dismiss |
| Recruitment | Offer stats/price, permanent starter choice, refresh timer/free/paid refresh |
| Training | Direct destination to fighter management; Money training and cap shown |
| Inventory / Equipment | Manage gear opens selected fighter; equipped/free inventory, equip/unequip; four slots |
| Shop | Money/MK/Credits catalog, availability, server early-access quote; locked purchase disabled; Level1 premium exploration collapsed |
| BB | Dedicated shared-stock/tier/refill surface and opt-in auto Basic |
| Recovery | Direct fighter recovery destination, full/partial Money heal, free recovery estimate |
| Opponents | Practice/Ranked/dev Friend selection, five candidates/categories, preview/confirm; shield warning |
| Revenge | Tickets/attempts/expiry and confirmed start within Opponents; target rating unchanged |
| Battle / Result | Teams, HP, event shooting, elimination marks, elapsed time, event-derived BBuse, skip, saved reward totals |
| History | Attack/defense, outcome/time/rewards/rating, open immutable replay |
| Settings | Profile name/emblem, leaderboard, session/state, development fixtures |

Training/Recovery reuse the fighter detail component. Inventory is grouped with selected-fighter Equipment; daily/progression/shield are in Club. These are grouped functional surfaces, not21 separate scene assets. Long lists scroll intentionally. English prototype copy and Unity immediate-mode controls are not final PC UI styling/localization.

## Visual foundation

One geometric arena: one to four engagement lanes with central cover silhouettes, four columns per team, size adapts1/4/8/16. Cover is decorative presentation only; no new cover/position combat rule. Universal male/female base variation from stable identity, enlarged head, goggles, helmet, rig pouches, camouflage blocks, family-dependent weapon length and MK accent. No imported art or production animation set.

Accepted server metadata captures head/rig/camouflage per participant inside already immutable match capture JSON. Weapon/MK comes from accepted combat snapshot. Appearance is unaffected by later equipment changes. Historical matches with no appearance metadata use a neutral base with known weapon; no false reconstruction from current equipment. Numeric combat/wire bytes remain untouched.

Replay applies authoritative damage in same-time groups. Traces show up to8 recent actors, HP bars remain compact at16v16, elimination is a muted silhouette/slash. No client combat or collision authority.

## Visual validation and screenshots

Actual Windows executable + actual local PostgreSQL backend. Automated Unity-authored walkthrough creates explicitly DEV resource/squad fixtures, then starts and loads real server Practice matches for each size. These screenshots do not represent naturally earned progression or a monetization purchase flow. OS capture was not needed; ScreenCapture produced nonblack PNGs.

1920×1080 (16:9, Mono) and1280×800 (16:10, native IL2CPP) inspected. Centered uniform logical-canvas scaling, scroll clipping, readable labels and stable32-silhouette layout. Initial rotated trace/elimination clipping bug was found in screenshots and corrected; retained screenshots are the corrected build. Long roster/shop/result surfaces may scroll, without major overlap. At1280×80016v16 small silhouettes remain distinguishable, but fine cosmetic detail is intentionally absent.

| Capture |1920×1080 |
|---|---|
| Club |[PNG](evidence/014/screens-1920/club-visual-club.png) |
| Recruitment |[PNG](evidence/014/screens-1920/club-visual-recruitment.png) |
| Roster |[PNG](evidence/014/screens-1920/club-visual-roster.png) |
| Shop |[PNG](evidence/014/screens-1920/club-visual-shop.png) |
| Equipment |[PNG](evidence/014/screens-1920/club-visual-equipment.png) |
| Battle1v1 |[PNG](evidence/014/screens-1920/club-battle-1v1.png) |
| Battle4v4 |[PNG](evidence/014/screens-1920/club-battle-4v4.png) |
| Battle8v8 |[PNG](evidence/014/screens-1920/club-battle-8v8.png) |
| Battle16v16 |[PNG](evidence/014/screens-1920/club-battle-16v16.png) |
| Result |[PNG](evidence/014/screens-1920/club-visual-result-16.png) |
| Recovery |[PNG](evidence/014/screens-1920/club-visual-recovery-16.png) |
| Ranked candidates |[PNG](evidence/014/screens-1920/club-visual-ranked.png) |

Native1280×800 counterparts: [16v16](evidence/014/screens-1280/club-battle-16v16.png), [result](evidence/014/screens-1280/club-visual-result-16.png); all other counterparts in screens-1280. Raw ignored copies: Artifacts/visual-1920 and Artifacts/visual-1280.

## Fresh tests / native gate

| Suite | Result |
|---|---|
| Standalone combat |46 passed /0 failed |
| Domain |36 passed /0 failed (four014 monetization tests) |
| Actual PostgreSQL |26 passed /0 failed |
| Unity EditMode |9 passed /0 failed /0 skipped |
| Unity PlayMode |1 passed /0 failed /0 skipped |
| Core mass simulation |10,000 completed /0 invariant failures;4913 attacker wins,5001 defender wins,86 draws |
| Premium progression |4,000 completed /40 rows /10 levels |
| Mono player |Golden/numeric/wire +300 size1/8/16 smoke battles PASS |
| Native IL2CPP player |Golden/numeric/wire +300 size1/8/16 smoke battles PASS |
| UI loop + separate process reconnect |Mono and IL2CPP PASS |
| Visual server matrix |4 real matches per backend, sizes1/4/8/16 PASS |
| HTTP functional alpha |PASS, including duplicate command/reconnect and Level1 early rejection |
| Copied native bundle client | Golden/numeric/wire + additional300 smoke battles PASS |
| Published bundle backend | Actual separate PostgreSQL port55433 migrate/start/HTTP loop PASS |
| Production security |PASS: dev login rejected even flag1; unauthenticated routes401; excess requests429 |

Golden across .NET/EditMode/PlayMode/Mono/native:
22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f

Native verifies SplitMix64 vectors, signed fixed-point truncation/multiply/divide/clamp/overflow, config/team/result roundtrip, golden bytes, HP/ammo/projectile invariants and bounded termination including16v16. Native smoke is not a claim that the entire46-case .NET suite runs inside the player.

Native16v16 batch100:32.653ms; Mono38.088ms (warm-up excluded; machine-specific, not UI FPS/production benchmark). Evidence logs/XML under [evidence014](evidence/014/verification-014.log).

## Build / environment / commands

Unity6000.3.21f1 LTS revisionc02631ffc030. Windows Standalone support includes win64 development/nondevelopment Mono and IL2CPP variations (also installed win32/ARM64 variants). MSVC14.51.36231, Visual Studio Community2026; Windows SDK10.0.26100.0. .NETSDK10.0.302/runtime10.0.10. Windows x64 Standalone, .NET Standard2.1 API compatibility, Development build.

From repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-server.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-visual.ps1 -Backend Mono
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-visual.ps1 -Backend IL2CPP -Width 1280 -Height 800
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-ui.ps1 -Backend Mono -Visible
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-ui.ps1 -Backend IL2CPP -Visible
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-alpha-http.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-alpha-security.ps1
```

Expanded native build command:

```powershell
& 'C:/Program Files/Unity/Hub/Editor/6000.3.21f1/Editor/Unity.exe' -batchmode -nographics -projectPath 'C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game/UnityHost' -logFile 'C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game/Artifacts/IL2CPPBuild.log' -quit -executeMethod BuildGate.Il2Cpp
```

C# → managed assemblies → IL2CPP C++ → native compile/link → executable and GameAssembly.dll: PASS. Both player smoke processes exited0. Native executable667136 bytes; GameAssembly11675136 bytes at verification. Distribution: Artifacts/AirsoftClub-Development-Windows-x64.zip with native client, published local backend, launcher, manifest and evidence. Mono executable remains at Artifacts/Mono/AirsoftClubIntegration.exe; native at Artifacts/IL2CPP/AirsoftClubIntegration.exe. Run instructions: [Development runbook](DEVELOPMENT_RUNBOOK.md). No live payments/network deployment.

## Warnings and residual work

- Final .NET build0 warnings/0 errors. Unity licensing logged access token unavailable but licensed build succeeded; no missing toolchain dependency.
- One security harness attempt hit Windows DLL file locks while source backend was running. Stopping the owned backend and rerunning passed. No game-code workaround.
- Rapid combined UI/HTTP harness traffic hit the intended240/minute limiter once; this is an observed harness scheduling issue, not a bypass. Retry after the window was required.
- Git emitted line-ending normalization notices; whitespace verification passed.
- Geometric forms, default UI controls, static poses and visual-only cover are prototype scope. Final art/animation/audio/localization/accessibility polish remain later.
- Live Steam AppID/credentials/sandbox purchase/refund verification is external OPEN. Local dev identity, provider ports and commerce reconciliation tests remain available. No production Steam claim.

Project Airsoft / Airsoft Manager untouched. All writes and verification artifacts are within this independent repository; no code/assets/history imported.
