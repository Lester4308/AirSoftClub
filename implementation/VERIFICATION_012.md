# Verification012 — visible launch, Revenge and reward integrity

2026-09-06. Independent Airsoft_Club_Game only. Precheck: branch codex/implementation-003-development-pass, HEAD cd578bd1d50cc628748e4f5ee69acf831b45cbf6, clean working tree.

## Visual gate: NOT PASSED — manual interaction blocked

User explicitly authorized visible launch. The native executable launched in a normal1280×800 D3D11 window against the published Windows backend and real PostgreSQL18.6 bundle database. Computer Use selected the actual game window, but capture failed twice, including fresh selection/retry:
`SetIsBorderRequired failed: No such interface supported (0x80004002)`.
No blind coordinates, custom input helper, hidden or black frames were accepted.

An independent **automated visible** walkthrough completed twice. Unity's rendered screenshot capture works in the visible window. This proves rendering and the coroutine/API loop, **not manual button clickability**.

Reviewed and retained:
- [Roster](evidence/012/roster.png): readable stats,115/115HP,ready state,Pistol-MK1 equipment and controls; no visible overlap at1280×800.
- [Result](evidence/012/result.png): readable two-team HP/result/reward display; no visible overlap for1v1.

Defect found in the first screenshot: header wallet/XP changed, but result showed Money/XP+0. Cause: match API omitted reward fields. Fixed using captured economy/policy; defender view gets zero offensive reward. HTTP regression now checks Money/Club XP/Fighter XP against committed deltas. Final screenshot shows+99 each, wallet825→924 andClub XP0→99.

Not manually verified: clicking,recruitment/resources screens,Revenge controls,reconnect/relaunch UI,keyboard/focus/scaling,16v16 layout. These two images do not establish full visual PASS. Functional16v16 tests are separate.

## Playable loop and changes

Native client→local development login→free recruitment→buy/equip Pistol→train/heal/refill→Practice battle→server settlement→visible result and updated state. HTTP test duplicates every mutation and reconnects with the same persisted account; passed. Existing source/bundle databases preserved.

Revenge enabled through domain, API ticket input, owner-scoped ticket projection and Opponents UI. User clarified this turn: **target loses no rating**. Master/pack/runbook updated. Existing idempotency/fencing transaction protects duplicate starts and settlements.

Implemented rules:
- 24h origin ticket,3 attempts,draw consumes attempt; any win closes ticket.
- Eligibility fixed at start; expiry during battle does not cancel accepted settlement.
- Rated success: floor(actual origin loss×120/100),checked Int64 intermediate; no stacked normal gain or target debit.
- Global4incoming impacts/rolling24h; rated Revenge consumes a slot even with target rating unchanged.
- At cap: accept non-rated,no recovery,retain own shield; rated start cancels own shield.
- Unique origin,no chains,no Friend/non-rated origin tickets.
- Standard rewards with existing directed friend-window anti-farm budget; no extra multiplier.

Fixed former test seam awarding120% despite non-rated capture. Removed disabled seam after counterparty approval. Added tests for cap fallback,frozen eligibility after slot release,in-flight expiry,non-rated success closure,rounding and shield.

Actual PostgreSQL test races identical Revenge starts,recovers with a fresh worker after ticket expiry,repeats settlement,checks one attempt/ledger grant,unchanged target rating,outcome-dependent recovery and no chain. This explicitly inserts a fixture origin ticket; it is not a live Steam defense claim.

## Fresh verification

| Gate | Result |
|---|---|
| Release solution |PASS,0 compiler warnings/errors |
| Combat |46/46 |
| Domain |27/27,previous25 preserved,+2 |
| Actual PostgreSQL |17/17,previous16 preserved,+1 |
| Mass simulations |10000 completed,0 invariant failures;4913 attacker wins,5001 defender wins,86 draws |
| Unity EditMode |9/9 |
| Unity PlayMode |1/1 |
| Windows Mono build/run |PASS,exit0;300 smoke battles,100 each1/8/16 |
| Windows x64 native IL2CPP build/run |PASS,exit0;300 smoke battles,100 each1/8/16 |
| HTTP loop/retry/reconnect/reward deltas |PASS |
| Visible automated native loop |PASS;2 screenshots inspected |
| Manual visual loop |BLOCKED by capture API error |
| Core diff vs d553cd3 |empty |

Golden for standalone/EditMode/PlayMode/Mono/native:
`22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f`.
Numeric vectors,fixed arithmetic,wire roundtrips and16v16 are included in established smoke gates. No golden regeneration.

Commands from repository root:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-server.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-http.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-ui.ps1 -Visible
```

Unity6000.3.21f1,Windows x64 IL2CPP,.NET Standard2.1;installed Windows SDK10.0.26100.0. Native compile/link and executable run completed. Native100-battle timings:1v1=6.286ms,8v8=19.313ms,16v16=31.216ms (measurements,not guarantees).
Nonblocking Unity licensing log:Access token unavailable/failed to update; builds succeeded. Git LF/CRLF notices occurred. No C# compiler warnings/errors. Windows capture failure remains a real verification blocker.

## External gates and scope

STEAM_APP_ID/STEAM_PUBLISHER_KEY absent in current process environment. No configured entitled app/test identity provided. Steam ticket/ownership/friends adapter and durable commerce fixtures are local evidence, **not sandbox verification**.
External prerequisites: owned Steamworks AppID with test-app access,entitled test accounts/configured client,server-only publisher Web API key,configured commerce sandbox/items/currency and provider authorization. No real purchase attempted. Commerce is reconciliation foundation; full provider transport/partner verification remains incomplete. Spent-Credits refund policy is a separate unresolved product detail.

Final art and balance remain OPEN and **do not block functional development**. Prior premium win-rate and recovery pacing pathologies remain recorded inVerification011. No final tuning/art imposed.
This pass stops on actual manual Windows capture limitation and missing partner environment. Revenge product ambiguity is closed.

Project Airsoft / Airsoft Manager untouched. No code/assets/data/history imported. Combat formulas,RNG andserialization unchanged.

## Checkpoint and package

Implementation commit: 0c87c1e (fix: enable approved revenge and show committed battle rewards). Documentation/evidence are committed separately; final documentation commit is returned in the task response. Branch remains codex/implementation-003-development-pass.

Updated native client and published backend packaged in Artifacts/AirsoftClub-Development-Windows-x64.zip.
SHA256: 2DE1F6387787E3C0DB8DE9F44CE024F744193A64002314EC306276ACF9540D35.
Initial archive attempt encountered a DLL lock from the running bundle backend; stopped only that identified local process and retried successfully. Archive uses the existing explicit allow-list excluding local password/.env files. Backend is stopped after packaging; restart using DEVELOPMENT_RUNBOOK.md. No game window remains running after automated smoke exit.
