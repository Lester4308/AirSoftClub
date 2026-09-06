# Airsoft Club — local development runbook

This is the independent Airsoft_Club_Game. It is a functional development slice with placeholder UI, not a Steam release or approved production art.

## Run from source (Windows)

Prerequisites: .NET SDK10.0.302/runtime10 LTS, Docker Desktop running, Unity6000.3.21f1 with Windows x64 IL2CPP support, MSVC and Windows SDK10.0.26100.0. PostgreSQL18.6 is downloaded by the project Compose configuration; it does not use another project's database.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/run-development.ps1
```

This creates/uses only the airsoft-club-development Compose project, binds PostgreSQL to127.0.0.1:55432, generates the local password in ignored Artifacts/local-db-password.txt, applies forward EF migrations, and starts the backend at http://127.0.0.1:5080. Preserve the password file with the local DB volume; deleting it does not reset the DB password. No secrets belong in Git.

Open Artifacts/IL2CPP/AirsoftClubIntegration.exe (native), or build it first:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
```

Click Connect using the generated development account. Choose a free recruit, buy Pistol-MK1 in Supply, equip it in Roster. Train/heal/refill as needed. Select Practice, Ranked or local Friend policy, preview an opponent and confirm. All ready fighters join automatically. Result/history come from the server; skip changes only playback. Refresh/reconnect restore persisted state. Do not create a new operation key when retrying a timed-out purchase; the client retains the original payload. Account name is stored locally for development reconnect; tokens expire after1h and a backend restart requires reconnect.

Starter:1000 Money,10 Credits,60 Basic BB. No free weapon.60 is a measured prototype estimate for~6.3 initial equal-opponent battles; final pacing is not approved. Basic emergency and free HP recovery remain available without premium spending.

## Verification

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
dotnet run --project tests/Airsoft.Club.Tests -c Release -- --balance
dotnet run --project tests/Airsoft.Club.Tests -c Release -- --economy
```

HTTP verification requires a running backend. Stop that backend before rebuilding changed server sources (Windows locks the executable). DB tests use unique fixture accounts, preserve existing data, and use actual PostgreSQL. Do not run verification against production data. EF migrations are forward/versioned; migration down or volume removal is not part of normal verification.

Native UI smoke uses --club-ui-smoke against the running backend. It creates an isolated dev account and attempts roster/result screenshots. Hidden windows produced black/no frames on this environment and are not visual evidence. Visible launch was explicitly authorized on2026-09-06; use tools/verify-ui.ps1 -Visible for an automated visible pass. Manual input verification remains separate. Normal interactive launch is for the player to see/control the game.

## Architecture and operational limits

Pure Battle remains netstandard2.1, immutable schema1, fixed arithmetic and seeded RNG. Server Club domain is net10.0 and never shipped as client authority. PostgreSQL stores club-owned aggregate JSON plus relational ledger/operation/match/session/order records. A cross-process transaction advisory lock serializes mutations for this bounded slice; production-scale partitioning is deferred. Defense publication becomes visible at transaction commit. Pending battle worker uses captured inputs and a120s fence; expired work fails without rewards and releases resources/reservations. There are no distributed notifications requiring an outbox yet.

Health:/health; schema/DB readiness:/ready. Structured console logs and System.Diagnostics.Metrics meter Airsoft.Club.Server provide a foundation. Do not log tickets, bearer tokens, publisher keys or full external Steam URLs. Runtime Dockerfile is provided as a packaging option; local verified run uses Windows .NET and Compose PostgreSQL.

## Explicit external/product gates

- Steam app entitlement, STEAM_APP_ID and server-only STEAM_PUBLISHER_KEY are absent. Adapter fixtures are not sandbox proof. Steamworks.NET2025.164.1 is pinned; sample AppID480 is ignored. The explicit Steam sandbox sign-in button uses the adapter, but actual sign-in/discovery rollout remains unverified without credentials.
- Revenge is enabled: target loses no rating, rated recovery uses floor(120% origin actual loss), shared exposure cap4; cap fallback is non-rated without recovery. Tickets are shown under Opponents. See Verification012 for fresh evidence and manual visual-capture limitation.
- Live payment provider integration, real prices, spent-Credits refund/debt policy, authorized moderation provisioning and production deployment are not enabled. Durable payment reconciliation is tested using explicit provider fixtures.
- Premium DPS/effective-HP ratio is below20%, but measured wins are substantially higher than50%. Competitive balance is not final or proven by that metric alone.
- Final art, localization/accessibility polish, keyboard/scaling visual QA, production load/security review, hosting and release remain separate gates.

No Project Airsoft code, data, assets, architecture or Git history was imported or changed.
