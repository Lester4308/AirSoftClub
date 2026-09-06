# Verification 011 — development pass, 2026-09-06

**Functional local authority/build gates PASS. Full011 visual/product completion NOT CLAIMED.** Source and packaged backend/client complete the local management→battle→settlement→reconnect loop. Visible UI/scaling/keyboard evidence remains unverified, Steam/commerce sandbox credentials are absent, and Revenge live settlement/product balance remain explicit gates below.

## Boundary and Git

Only C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game was used for project work. Start branch codex/implementation-002-unity-host, code baseline d553cd3d9fee998a9764be04b63f8881658d299f;24 known documentation-only files inspected and preserved in4c79e182a9f5c5f81c1f9346c8b38fc9ecef7d15. New branch codex/implementation-003-development-pass. No remote configured/push performed. Project Airsoft code/data/assets/history/architecture untouched. `git diff d553cd3 -- src/Airsoft.Battle` is empty: combat formulas, RNG, fixed arithmetic, wire and golden fixtures unchanged.

## Delivered scope

| Milestone | Actual result |
|---|---|
|003|Reward table/repeat policy, wallet ledger, starter and one-way conversion implemented/tested|
|004|Recruitment, max16, original-price dismissal/gear return, recovery/readiness/training/heal, four-slot catalog, BB/snapshots implemented/tested|
|005|ASP.NET+actual PostgreSQL/EF migrations, ownership/uniqueness/nonnegative/ledger constraints, transaction/version/idempotency, health/readiness|
|006|Immutable accepted inputs, trusted resolve outside transaction, once-only settlement, pending/restart/fencing, committed defense publication|
|007|Ranked/Friend/exposure/shields/history/leaderboard foundation; Revenge ticket/policy tests, live settlement deliberately disabled|
|008|Unity development management client calls actual backend; Mono/native automated onboarding+battle loop passes; visual QA pending|
|009|Pinned Steamworks.NET and server ticket/ownership/friend/session adapters compiled; fixtures tested; real Steam sandbox NOT RUN|
|010|Daily/streak/progression/access, durable commerce reconciliation and minimal moderation foundation; no real payment or admin rollout|
|011|Regression/native/DB/restart/economy/premium evidence and local package; incomplete visual/external/product gates remain explicit|

## Actual environment

- Unity6000.3.21f1 LTS, revision c02631ffc030; Windows Standalone x64 IL2CPP module.
- Windows SDK10.0.26100.0; established MSVC14.51.36231/VS Community2026 toolchain successfully performs native compile/link.
- SDK10.0.302; installed .NET/ASP.NET/WindowsDesktop runtime10.0.10. Server target net10.0; shared battle and Unity API remain .NET Standard2.1.
- EF Core/Relational/Design and dotnet-ef10.0.8; Npgsql EF10.0.2, locked transitive packages.
- Docker29.7.2; actual PostgreSQL18.6 pinned digest4ef4dbc939d61acea57712655ddb4b4ab27419c913f94cca0cd57cb3ea3c2280.
- Steamworks.NET2025.164.1, commit c21a8f0e31c56ae8707130967faf491f7dd7c0d8. No app/publisher credentials configured.

Official compatibility/source links are in [005](IMPLEMENTATION_005.md) and [009](IMPLEMENTATION_009.md). No release/production deployment occurred.

## Fresh verification

| Check | Result |
|---|---|
|Release solution build|PASS;0 compiler warnings/errors|
|Standalone combat suite|46/46;100 identical repeated golden results|
|Club domain suite|25/25|
|Actual PostgreSQL suite|16/16|
|Mass combat simulations|10000/10000 complete;0 invariant failures|
|Unity EditMode|9/9|
|Unity PlayMode|1/1|
|Windows Mono build/run|PASS;exit0;300 smoke battles|
|Windows native IL2CPP build/run|PASS;exit0;300 smoke battles|
|Published backend HTTP walkthrough|PASS;fresh account, hire/buy/equip/train/heal/refill/attack/result/reconnect; every command duplicated safely|
|Published backend+PostgreSQL actual restart|PASS;same persisted account version8, wallet and match history restored|
|Native client HTTP/UI coroutine|PASS on actual server;capture files exist but hidden frames are black, so NOT visual QA|
|Anonymous HTTP access|401 rejection verified|
|Premium full-build matrix|5400 completed battles over6families×3armor weights×3roster sizes×50seeds×2side swaps|
|Economy loop|100clubs×20battles=2000;no negative Money/BB,zero premium spending|
|Whitespace/core purity|PASS after formatting;core source unchanged|

Commands are in [runbook](DEVELOPMENT_RUNBOOK.md), and tools/verify.ps1 now includes Club tests. DB tests use unique fixture accounts and real PostgreSQL, not an in-memory substitute. Fresh/forward migrations, concurrent last-resource commands, payload conflicts, lost-response retries, rollback after SaveChanges before commit, immutable ledger conservation, fifth incoming acceptance race, expired workers, UTC race and payment-confirmed→DB-failure→retry are exercised.

The native pipeline completed **C#→managed assemblies→IL2CPP C++→MSVC native compile/link→Windows executable**. Command: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild`; launch with `-Stage IL2CPPRun`. Native output:

```text
AIRSOFT_BUILD_PASSED backend=IL2CPP
AIRSOFT_PLAYER_PASSED digest=22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f
```

Golden exactly matches standalone, EditMode, PlayMode, Mono and native IL2CPP. Native smoke actually checks SplitMix64 vectors, fixed signed arithmetic/overflow, golden bytes/digest, config/team/result serialization, HP/ammo/duration invariants and100×each1v1/8v8/16v16. It does not claim all standalone/DB tests ran inside IL2CPP.

Latest native100-battle timings:1v1 5.554ms;8v8 18.609ms;16v16 29.683ms. These warm engine+validation observations are not rendering or production-load measurements. Mass results unchanged:4913 attacker wins,5001 defender wins,86 draws;mean simulated6089.70ms,mean BB81.05/81.30.

## Balance and final review decisions

- Starter Basic changed from provisional500 to60, based on1000 equal first-recruit/Pistol-MK1 simulations:mean9.526 BB/battle,~6.30 battle estimate. No weapon gifted; starter1000 Money/10Credits unchanged. Existing accounts are not reset.
- Full-build predeclared metric is matched expected DPS×effectiveHP ratio. Identical stats/armor/HP/hit/tempo/projectiles cancel; MK3 damage×Premium BB yields maximum1.1902496 (19.02496%). Paired simulation result4213/5400 premium wins=78.02%,347 draws. **This is not proof that win-rate advantage is≤20%, nor final commercial balance.** Competitive balance/product metric review remains required.
- Economy evidence:94900 total Money rewards,163415 healing,5000 refills;21400 total minutes of free recovery waiting;minimum Money0,Credits spent0. It shows a free continuation path, not validated retention pacing. Raw matrices:Artifacts/balance-011.json and economy-011.json.
- Fixed free-candidate eligibility to original offer IDs0–2; buying an earlier paid offer cannot shift candidate4 into the free set.
- Wallet projection must equal its append-only ledger and cannot rewrite prior entries. A zero actual defender rating impact releases its exposure reservation after floor.
- Auto-buy Basic is implemented as an explicit Level3 opt-in at battle acceptance: Basic active, stock<100, Money≥150; a50-Money refill leaves≥100. Never spends Credits or switches tier. Boundary/opt-in tests passed. Production-scale partitioned DB locks, richer matchmaking/social discovery, full localization/accessibility and final art remain future work.

## Package and remaining gates

Artifacts/AirsoftClub-Development-Windows-x64.zip,108591303bytes. SHA256:
`82021CF3699E191909EEF1FDDB21E34065A24E017E38ED3C3D36D666538D0EBA`.

Archive contains331 entries: native client, published server, Compose, launcher and runbook. Explicit archive allow-list excludes local passwords/.env/sample steam_appid; exclusion verified after bundle testing generated its local password. The bundle uses its own Compose project/volume and loopback55433; source DB uses55432. The published Windows backend and bundle DB were actually launched and restarted. Docker backend image build and container readiness against actual PostgreSQL also PASS; production-mode dev-login returns401. Image airsoft-club-development:011, manifest60d81783d8732937d4d993e8830afb40ffec24860c0fbac110399bcc52b46d2a; runtime ASP.NET10.0.11. The test container was stopped after verification.

Specific unfinished gates:
1. **Visual gate:** hidden D3D12 capture failed; D3D11 captured black frames. Not accepted as screenshots. Visible launch permission was requested and remains pending; keyboard/focus/scaling/readable16v16 visual checks are NOT RUN. Environment instructions require explicit permission to launch a visible window. The functional coroutine/HTTP success does not close this gate.
2. **Steam external gate:** requires actual AppID, publisher key and entitled test accounts. Fixtures are not sandbox verification; current placeholder client defaults to dev identity; an explicit Steam sandbox button is wired to the ticket adapter but cannot verify without configured credentials. No spoofed Steam identity or sample AppID used as authority.
3. **Revenge product gate:** confirm counterparty rating debit and cross-mode reward semantics before enabling live settlement. Tickets/pure test-only recovery exist; API rejects Revenge.
4. **Commerce/moderation production gate:** provider credentials/sandbox, final prices, spent-credit refund policy and moderator provisioning. No real charges/sanctions executed.
5. **Balance/release gate:** effective-advantage metric and pacing need approval/tuning; final art/hosting/publishing/production security/load review not part of an approved release.

Historical compile conflicts and a span/await test compilation error were fixed; a prematurely recorded006 test claim is explicitly corrected by93a32b0 and fresh passing runs. No hidden failure is represented as PASS. Nonblocking Unity licensing-token messages, D3D12 info-queue diagnostic and development shutdown allocator reports occurred; zero C# compiler warnings/errors in final builds. Hidden screenshot failure remains a real verification limitation.

## Commits before the final011 checkpoint

4c79e18 docs: add master development and agent implementation specs
4ec50c4 feat: implement reward and wallet policies
22c1bf9 feat: implement club recruitment recovery and inventory
3e292e0 feat: add postgres authority persistence and migration gates
d84e7f8 feat: implement durable authoritative battle settlement
93a32b0 test: fix awaited snapshot comparison and verify battle gates
a34359c feat: enforce friend rating exposure and shield policies
717f2e6 feat: connect unity management client to authoritative server
49515f7 feat: add verified-ticket steam identity adapter seams
f5f8f18 feat: add retention and durable commerce reconciliation foundation
