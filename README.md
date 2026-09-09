# Airsoft_Club_Game

> **Modern UGUI + Volumetric 2D Sprites — ACTIVE, 2026-09-09.** All screens use `Volumetric017UiView` (fighters) and `VolumetricWeapon017UiView` (6 weapon families). Login, roster, shop, training, equip/unequip, battle — all functional via Modern UI. Legacy BetaShell/TacticalArt retained for smoke tests only. Commit `66090b3`.

## Quick start

```bash
# 1. Start PostgreSQL
powershell -ExecutionPolicy Bypass -File tools/start-db.ps1 -Migrate

# 2. Start server (in a separate terminal)
export ASPNETCORE_ENVIRONMENT=Development
export AIRSOFT_DEV_AUTH=1
export AIRSOFT_CONNECTION='Host=127.0.0.1;Port=55470;Database=airsoft_club_dev;Username=airsoft_dev;Password=<from Artifacts/local-db-password.txt>'
export ASPNETCORE_URLS=http://127.0.0.1:5080
dotnet run --project src/Airsoft.Server

# 3. Build Unity client
powershell -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild

# 4. Launch game
Artifacts/Mono/AirsoftClubIntegration.exe
```

Game auto-creates a `dev-*` account on first launch. Login screen is Modern UGUI with GraphicRaycaster-enabled Canvas.

## Run all tests

```bash
# Server-side (battle 46/46, club 37/37, simulation 10K)
powershell -ExecutionPolicy Bypass -File tools/verify.ps1

# Unity EditMode (15/15)
powershell -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode

# Unity MonoBuild
powershell -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
```

## Project structure

| Directory | Purpose |
|---|---|
| `UnityHost/` | Unity 6000.3.21f1 LTS project |
| `UnityHost/Assets/Runtime/` | Game scripts (ModernScreens1-3, Volumetric017UiView, etc.) |
| `UnityHost/Assets/Resources/Art/Volumetric017/` | Runtime sprites (fighters + 6 weapon families) |
| `UnityHost/Assets/Tests/` | EditMode + PlayMode tests |
| `src/Airsoft.Server/` | ASP.NET Core backend |
| `src/Airsoft.Battle/` | Pure C# deterministic battle core |
| `src/Airsoft.Club/` | Club domain library |
| `tests/` | .NET test projects |
| `art/` | Source art assets |
| `art/volumetric-017/` | Current volumetric 2D sprite source |
| `design/` | Product design, decisions, art direction |
| `implementation/` | Milestone reports and evidence |
| `tools/` | Build, test, and DB scripts |
| `Artifacts/` | Build output (Mono build, test logs) — .gitignored |

## Tech stack

- **Client:** Unity 6000.3.21f1 LTS, C#, UGUI/Canvas
- **Server:** ASP.NET Core, .NET 10.0.302
- **Database:** PostgreSQL 15 (Docker)
- **Art:** Volumetric 2D sprites (programmatic + post-processed PNGs)
- **Branch:** `codex/implementation-003-development-pass`

## Key decisions

- `useModern = true` by default — Modern UGUI is the primary UI
- Stable fighter gender via `IsFemale(fighterId)` deterministic hash
- Explicit `CatalogToFamily` dictionary for 6 weapon families
- Battle weapon overlay uses `CreateSilhouette()` at hands level
- GraphicRaycaster required on Canvas for mouse input routing
- `account` excluded from `ModernRenderSignature()` to prevent per-keystroke UI rebuild

## Documentation

- [Master Development Spec v1](AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md)
- [Agent Implementation Pack v1](AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md)
- [Volumetric 2D Art Direction](design/VOLUMETRIC_2D_SPRITE_ART_DIRECTION_017.md)
- [Cleanup Report](CLEANUP_REPORT.md)
- [Development Runbook](implementation/DEVELOPMENT_RUNBOOK.md)
