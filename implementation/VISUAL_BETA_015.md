# Visual Beta015 — UI / presentation pass

2026-09-07. **LOCAL VISUAL BETA PASS**; all platform/UI gates below completed. Scope: C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game only. Started clean on codex/implementation-003-development-pass at e193bf06b70a974114c6bad9a052ac402a76e1f4.

## Direction implemented

A unified dark graphite / blue-black tactical UI, warm gold headings, orange primary actions, restrained blue selection/HP highlights and muted olive/tan art. The014 flat diagnostic presentation has been replaced with an atmospheric club hub, left navigation, reusable cards, selected-detail panels, compact lists and consistent resource/progression header.

The source-of-truth master and execution pack exist at repository root, not the prompt's outputs/ paths. Latest015 visual instructions take precedence;014 monetization rules are preserved. Premium appears both in ordinary weapon browsing and in a dedicated Premium category. Level1 early unlock remains unavailable. No combat, reward, balance, RNG, progression or persistence rule changed.

## Screens and interactions

| Surface | Beta implementation |
|---|---|
| Home / Club | Industrial HQ background, structured navigation, readiness/progression/ammo summary, daily/earned rewards, emergency Basic, conversion and shields. No permanent central hero display. |
| Roster | Scrollable visual grid on the left, selected fighter on the right. Full-body sprites, name, level, ready state and HP. Hover information. |
| Fighter / Equipment | Selected full-body preview, stats/caps, HP/XP, training, full/partial recovery, four clickable equipment slots and inventory overlay. Baseline boots remain visual-only. |
| Training / Recovery | Same cohesive fighter surface; direct navigation and status-specific heading rather than a separate incompatible flow. |
| Recruitment | Compact seven-candidate grid, subtle idle animation, hover-selected details, stat comparison with selected fighter and clear free/Money hire. Refresh behavior unchanged. |
| Shop | Categories left, visual catalog center, larger preview/stat summary/actions right. Locked, available, owned, equipped and early states. MK1/2/3 coexist with Premium entry. |
| BB | Shared reserve, tier selection/refill, five color identities and existing opt-in auto Basic. |
| Opponents | Candidate list and selected club panel, approximate strength, mode/rating context, shield warning, confirm action. Read-only server full-strength win reward preview; current HP/anti-farm can alter settlement. |
| Revenge | Dedicated overlay lists all valid tickets, including targets outside the five current candidates; attempts/expiry/origin loss and explicit confirmation remain visible. |
| Battle | Shallow side-view warehouse, compact lane placement, restrained team markers/HP, captured tier names, remaining accepted BB budget, shooting/hit/elimination feedback, skip. |
| Result | Perspective-correct Victory/Defeat/Draw, four reward cards, duration/ammo summary, New Battle / Club / Replay / History. Historical unknown rating remains UNKNOWN. |
| History / Settings | Review saved attack/defense records; profile/session/dev tools retained. Art-layer/MK sheet is a development review surface, not an economy fixture. |

Short panel fades, button/card hover states, idle breathing, recoil and reward-card arrival motion are presentation-only. Modal overlays prevent underlying gameplay actions. Input/layout uses a centered1600×900 logical canvas scaled to desktop aspect ratios; long catalogs/rosters scroll. Status/retry/reconnect remains available.

## Modular fighter pipeline

TacticalArt creates cached transparent2D raster layers from independent polygon definitions. No real-time3D character rig or heavy model pipeline:
- Two reusable male/female body variants; serious compact tactical stance and slightly oversized faceted head.
- Camouflage changes jacket and pants. Boots remain the same baseline.
- Face mask/goggles, helmet and torso rig are separate layers. Face protection is baseline presentation, not a fifth mechanical slot.
- Complete weapon profile is layered over the held pose. Families have different lengths, stocks, magazines, pump/scope details.
- MK1 baseline; MK2 muted color/optic treatment; MK3 two-tone tactical skin, stronger accents and visual-only attachments. No new attachment gameplay.
- A review sheet displays both bases/layer combinations and all six families in all three MK tiers.

Layer replacement is localized to TacticalArt's cached texture providers; future hand-authored sprite assets can replace them while preserving composition/callers. Procedural faceted art is a beta asset language, not final production character approval. Current male/female differences are intentionally modest, with gear providing the main visual identity.

## Arena and battle feedback

One implemented CQB/industrial warehouse theme. Reusable warehouse planes, cover/crate components, lane scaling and palette hook form the base for later yard/outdoor themes; multiple finished maps are **not** claimed. Decorative cover does not change targeting, mitigation or collision.

One to four lanes; up to four columns per side per lane. Fighters face inward with shallow depth, remain individually visible at16v16 and use small HP bars. Team colors affect HP/markers, not entire uniforms.

Authoritative events drive:
- Brief muzzle flashes aligned to weapon muzzle geometry.
- Short traveling BB clusters/trails (not full-field laser beams).
- Hit-tinted target body and grouped floating damage.
- Muted OUT state at zero HP.
- Event-derived fired/remaining BB counts and result review.

Same-time damage grouping from014 remains; no damage/hit/RNG decision comes from animation. Displayed reserve uses accepted budget minus already presented events; top club wallet/stock is current settled server state. Replay speed/skip never changes rewards. Movement between gameplay positions/reloads are not invented.

## BB visual mapping

| Captured tier | Catalog name | Flight color | RGB |
|---|---|---|---|
| bb-0 | Basic | white |239,244,234 |
| bb-1 | Improved | green |115,215,128 |
| bb-2 | Advanced | blue |110,179,255 |
| bb-3 | High-End | red |237,117,106 |
| bb-4 | Premium | purple |201,141,250 |

The palette is presentation data in BetaTheme. It maps the captured tier ID; multipliers and consumption remain untouched. The BB walkthrough prepares both clubs through actual server commands, accepts five real matches, checks both captured tiers, then captures shots. A pixel check searches only the battlefield crop (excluding HUD/legend) for each assigned color. No altered input/config or fake cosmetic match is substituted.

## Screenshot evidence

Fresh Unity ScreenCapture PNGs from running Windows players, using actual local backend battles. DEV squads/resource grants are explicit visual fixtures, not naturally earned progression or a live purchase demonstration.

| Screen |1920×1080 Mono |
|---|---|
| Home |[PNG](evidence/015/screens-1920/club-visual-club.png) |
| Roster grid |[PNG](evidence/015/screens-1920/club-visual-roster-squad.png) |
| Recruitment |[PNG](evidence/015/screens-1920/club-visual-recruitment.png) |
| Shop |[PNG](evidence/015/screens-1920/club-visual-shop.png) |
| Equipment |[PNG](evidence/015/screens-1920/club-visual-equipment.png) |
| Opponents |[PNG](evidence/015/screens-1920/club-visual-ranked.png) |
| Battle1v1 |[PNG](evidence/015/screens-1920/club-battle-1v1.png) |
| Battle4v4 |[PNG](evidence/015/screens-1920/club-battle-4v4.png) |
| Battle8v8 |[PNG](evidence/015/screens-1920/club-battle-8v8.png) |
| Battle16v16 |[PNG](evidence/015/screens-1920/club-battle-16v16.png) |
| Result |[PNG](evidence/015/screens-1920/club-visual-result-16.png) |
| Recovery |[PNG](evidence/015/screens-1920/club-visual-recovery-16.png) |
| Layer/MK sheet |[PNG](evidence/015/screens-1920/club-visual-art-sheet.png) |
| Ammo management |[PNG](evidence/015/screens-1920/club-visual-ammunition.png) |

1280×800 native counterparts are under screens-1280, including [16v16](evidence/015/screens-1280/club-battle-16v16.png). Five BB captures per backend are under bb-mono and bb-native:
[white](evidence/015/bb-native/club-bb-tier-0.png), [green](evidence/015/bb-native/club-bb-tier-1.png), [blue](evidence/015/bb-native/club-bb-tier-2.png), [red](evidence/015/bb-native/club-bb-tier-3.png), [purple](evidence/015/bb-native/club-bb-tier-4.png).

Layouts inspected at1920×1080 and1280×800. No major panel overlap; intentional scroll clipping is retained. Small secondary labels and tiny16v16 gear details are a remaining accessibility/polish consideration. Screenshots and controller walkthroughs do not claim a comprehensive manual mouse/keyboard usability study.

## Verification

| Gate | Fresh result |
|---|---|
| Combat suite |46 passed /0 failed |
| Domain suite |36 passed /0 failed |
| Actual PostgreSQL |26 passed /0 failed |
| Unity EditMode |9 passed /0 failed |
| Unity PlayMode |1 passed /0 failed |
| Mass simulation |10,000 completed /0 invariant failures |
| Windows Mono |Windows player executable build/run, golden/numeric/wire +300 smoke battles PASS |
| Windows IL2CPP |Full native compile/link/build/run, golden/numeric/wire +300 smoke battles PASS |
| Visual sizes |1/4/8/16 real server battles in both backends PASS |
| BB tiers |Five real server battles + battlefield pixel proof per backend PASS;76 matching pixels per tier/backend |
| UI loop/reconnect |Mono/native PASS, including separate-process reconnect |
| HTTP/security |Source HTTP loop and alpha lifecycle PASS; production auth/DEV isolation/rate limit PASS |
| Packaged client/backend |Published backend HTTP lifecycle PASS; copied native executable golden +300 battles PASS; copied native client real UI loop PASS |

Golden unchanged:
22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f

Native smoke verifies SplitMix64 vectors, fixed-point truncation/arithmetic/overflow, config/team/result roundtrips and bounded HP/BB/termination including16v16. It is not a claim that all46 standalone cases execute inside the player. The4,000 monetization matrix from014 is historical; no balance changed and it is not presented as a fresh015 simulation.

Unity6000.3.21f1 LTS; Windows SDK10.0.26100.0; MSVC14.51.36231. Windows x64 Development, .NET Standard2.1 API compatibility. C# → managed assemblies → IL2CPP C++ → native compile/link → final exe/GameAssembly completed.

Evidence logs are retained under evidence/015 (build/player markers, suites, HTTP/security, UI/reconnect, visual matrices, BB pixel JSON and screenshot hash/dimension manifest). Full Unity logs remain in ignored Artifacts. All52 screenshot files are included.

The final historical-rating-label correction was followed by both platform builds/runs and EditMode/PlayMode; native UI/reconnect and packaged UI also ran after that correction. Mono UI/reconnect and BB captures predate only this label-only correction. No combat/domain source differs from the starting checkpoint.

## Reproduction / delivery

From repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-server.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-alpha-http.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-alpha-security.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-visual.ps1 -Backend Mono
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-visual.ps1 -Backend IL2CPP -Width 1280 -Height 800
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-bb-visual.ps1 -Backend Mono
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-bb-visual.ps1 -Backend IL2CPP
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-ui.ps1 -Backend Mono -Visible
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-ui.ps1 -Backend IL2CPP -Visible
```

Run each visual harness serially with a running local backend; allow the existing240/minute request window between large batches. Stop the owned backend before server rebuild/security migration/publishing to avoid Windows DLL locks.

Native client: Artifacts/IL2CPP/AirsoftClubIntegration.exe. Mono: Artifacts/Mono/AirsoftClubIntegration.exe. Distribution: Artifacts/AirsoftClub-Development-Windows-x64.zip, with published backend, launcher, current manifest and reports/evidence. [Launch guide](DEVELOPMENT_RUNBOOK.md). No external publishing.

## Limitations / warnings / boundary

- Final .NET build0 warnings/0 errors. Docker was initially stopped; starting Docker Desktop resolved the PostgreSQL prerequisite. No code workaround.
- Unity licensing logged `Error: Access token is unavailable; failed to update`; this was nonfatal and both builds completed with exit0. No final C# compile errors. Git also emitted LF-to-CRLF advisories during verification; whitespace checks passed.
- Initial development compile mismatched tier object versus ID; corrected before passing builds.
- One warehouse theme, procedural faceted sprites, restrained static-pose effects, English prototype copy. Final art/textures/animation/audio/localization, richer environment artwork and accessibility refinement remain later work.
- Reward preview is explicitly full-strength baseline; server settlement remains final. No gameplay/data-schema/economy change.
- Steam sandbox credentials/AppID remain external OPEN. Local identity/provider/commerce seams preserved; no production Steam/payment verification claimed.
- All work remained within this independent repository. Project Airsoft / Airsoft Manager was not modified or used as an asset/code source.

Commits and exact final HEAD are included in the delivery response and packaged BUILD-MANIFEST.json; final branch is codex/implementation-003-development-pass.
