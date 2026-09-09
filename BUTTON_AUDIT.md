# Modern UGUI button audit

Date: 2026-09-09  
Scope: `Airsoft_Club_Game` only. Modern UGUI is the default runtime UI. Legacy IMGUI remains as a fallback and was not removed.

## Evidence levels

- **EVENT PASS** — exercised by Unity `EventSystem` pointer/submit/raycast tests; no direct `onClick.Invoke()`.
- **FLOW PASS** — authoritative server/domain flow exercised by repository verification or runtime walkthrough.
- **STRUCTURAL PASS** — source, visibility, local gate, raycast target and geometry inspected; not individually clicked in the native player.
- **NOT PHYSICALLY VERIFIED** — Windows native-input automation could not expose Unity canvas controls through UI Automation; no claim of a physical mouse click.
- **BLOCKED BY RULE** — visible disabled state is intentional and the governing rule is listed.

## System-wide findings and fixes

| Finding | Result | Evidence |
|---|---|---|
| Result screen rebuilt after replay completion | **FIXED / EVENT PASS** | Replay time is clamped to duration and `ModernRenderSignature()` excludes completed replay time. Long pointer hold preserves the button instance across frames. |
| Result row exceeded 1600-reference content width | **FIXED / RAYCAST PASS** | Four buttons now occupy 1210 px inside the 1310 px content region. All four raycast and navigate for victory, defeat and draw. |
| Inventory modal actions inherited the background gate | **FIXED / EVENT PASS** | CLOSE/EQUIP/UNEQUIP use modal-local override; mutation buttons still require `!busy`. |
|| Credits modal leaked background keyboard/navigation actions | **FIXED / EVENT PASS** | Background Selectables fail `IsInteractable`; CONFIRM/CANCEL remain usable. **Toppest raycast at a background nav position hits the modal shade, not the nav button (real `EventSystem.RaycastAll` suppression proof).** |
| Duplicate transaction dispatch | **FIXED / STRUCTURAL PASS** | Login, Steam login, Send, Retry and leaderboard entry points reject a second coroutine while `busy`. Existing command key is retained for retry. |
| Input/selection lost on idle updates | **FIXED / EVENT PASS** | No signature change means no hierarchy replacement; login `InputField` instance, text and focus survive idle frames. |
| Decorative raycast interception | **PASS** | Labels and decorative panel images explicitly use `raycastTarget=false`; modal shades intentionally receive raycasts. |
| Event plumbing | **PASS** | One EventSystem, StandaloneInputModule, ScreenSpaceOverlay Canvas, CanvasScaler and GraphicRaycaster are created. |
| Training overlap | **PASS** | At 1280×720, 1600×900 and 1920×1080 reference scales every `+1` button remains within its stat row, interactable below cap, and has no blocking CanvasGroup. |
| Hidden Modern parity routes | **FIXED** | Equipment and Recovery are restored to sidebar navigation. Settings now contains profile controls rather than a Club fallback/read-only placeholder. |

## Login and global navigation

| Screen | Control | Expected action | Gate | Result / evidence |
|---|---|---|---|---|
| Login | Account field | focus, type/paste, Backspace; value stored locally | available while login screen exists | **EVENT PASS for focus/text retention; STRUCTURAL PASS for InputField editing.** Native key injection not completed. |
| Login | Enter/Return | start development login | `!busy` | **STRUCTURAL PASS**; `Update` routes Return to `Login`; double dispatch rejected. |
| Login | ENTER CLUB | authenticate, refresh club/opponents; retry after failure | standard action gate | **FLOW PASS** in local runtime/server walkthrough; button geometry test passes. |
| Failure bar | RETRY / REFRESH | retry same idempotency payload, then refresh authoritative state | `failed && !busy`, no modal | **STRUCTURAL PASS / FLOW PASS for server retry architecture.** HTTP 400/409 intentionally clear rejected payload. |
| Sidebar | Club | Club dashboard | standard navigation gate | **STRUCTURAL PASS** |
| Sidebar | Roster | fighter list/detail | standard navigation gate | **STRUCTURAL PASS** |
| Sidebar | Equipment | fighter equipment view | standard navigation gate | **RESTORED / STRUCTURAL PASS** |
| Sidebar | Recruitment | candidate pool | standard navigation gate | **STRUCTURAL PASS** |
| Sidebar | Training | fighter training view | standard navigation gate | **STRUCTURAL + GEOMETRY PASS** |
| Sidebar | Recovery | recovery/heal view | standard navigation gate | **RESTORED / STRUCTURAL PASS** |
| Sidebar | Shop | item catalog | standard navigation gate | **STRUCTURAL PASS** |
| Sidebar | BB | shared BB reserve | standard navigation gate | **STRUCTURAL PASS** |
| Sidebar | Opponents | mode/target selection | standard navigation gate | **STRUCTURAL PASS** |
| Sidebar | History | saved battles | standard navigation gate | **EVENT PASS from Result; STRUCTURAL PASS from sidebar** |
| Sidebar | Settings | real Settings screen | standard navigation gate | **EVENT PASS** route regression; controls listed below. |

## Club

| Control | Expected action | Gate / blocked reason | Result |
|---|---|---|---|
| CHOOSE FIRST RECRUIT / FIND A BATTLE | navigate based on roster state | standard gate | **STRUCTURAL PASS** |
| CLAIM DAILY | claim once per UTC day | only visible when not claimed; server authoritative | **STRUCTURAL PASS** |
| CLAIM CREDITS | progression reward claim | standard gate; server eligibility | **STRUCTURAL PASS** |
| CONVERT 1 CREDIT | stage irreversible premium-currency confirmation | standard gate, then modal | **EVENT PASS** staging and modal isolation |
| Shield offer buttons | stage Credits confirmation | standard gate | **STRUCTURAL PASS**; confirmation component is EVENT PASS |
| EMERGENCY BASIC BB | server emergency refill | standard gate; cooldown enforced server-side | **STRUCTURAL PASS** |
| AUTO-BUY ON/OFF | toggle automatic Basic BB purchase | visible from club level 3 | **STRUCTURAL PASS** |
| CONFIRM / CANCEL | execute or abandon Credits action | modal-local; confirm also `!busy` | **EVENT PASS** |

## Recruitment

| Control | Expected action | Gate | Result |
|---|---|---|---|
| INSPECT each candidate | select candidate and show matching detail | standard gate | **STRUCTURAL PASS** |
| CHOOSE FREE RECRUIT / HIRE | authoritative hire | standard gate; roster/currency server rules remain | **FLOW PASS** for hire pipeline; UI structurally audited |
| REFRESH POOL | free refresh after timer/10 battles | eligibility-controlled visibility | **STRUCTURAL PASS** |
| paid REFRESH | authoritative Money refresh | shown before free eligibility | **STRUCTURAL PASS** |
| invalid prior selection | fall back to first actual offer | local | **FIXED / STRUCTURAL PASS**; no `-0` ID assumption remains |

## Roster, Training, Recovery and Equipment

| Control | Expected action | Gate | Result |
|---|---|---|---|
| Fighter SELECT (up to 16) | choose fighter | standard gate | **STRUCTURAL PASS**; list is clamped vertical ScrollRect and stores position |
| roster scroll | access all 16 fighters | ScrollRect | **STRUCTURAL PASS**; native wheel/drag not physically verified |
| Accuracy/Endurance/Agility +1 | train selected stat | standard gate + below TrainingCap | **GEOMETRY/INTERACTABILITY PASS** at three target sizes; server training FLOW PASS |
| HEAL | authoritative full heal | standard gate; affordability/server rules unchanged | **FLOW + STRUCTURAL PASS** |
| DISMISS | dismiss selected fighter | visible only when roster has >1 fighter | **STRUCTURAL PASS** |
| each of four slot buttons | UNEQUIP or open inventory | standard gate | **STRUCTURAL PASS**; width enlarged to keep label within slot |
| inventory CLOSE | close modal | modal-local, allowed while inventory open | **EVENT PASS** with pointer event |
| inventory EQUIP / UNEQUIP | authoritative mutation | modal-local and `!busy` | **IsInteractable PASS / FLOW PASS**; duplicate operation rejected |
| Escape | close inventory, Credits, then Revenge modal | modal state | **STRUCTURAL PASS** |
| empty inventory | explain that compatible unequipped items are absent | display only | **STRUCTURAL PASS** |
| VISIT SHOP | navigate to Weapons catalog | standard gate | **STRUCTURAL PASS** |

## Shop and BB

| Control | Expected action | Gate | Result |
|---|---|---|---|
| category buttons | filter Weapons/Head/Armor/Camouflage | standard gate | **STRUCTURAL PASS** |
| catalog scroll | access all catalog entries | ScrollRect | **STRUCTURAL PASS**; native wheel/drag not physically verified |
| BUY | authoritative Money/Credits purchase | now locally disabled when `CatalogView.Access=false`; server validates | **FIXED / STRUCTURAL PASS** |
| UNLOCK EARLY | stage Credits access purchase | only when `EarlyAllowed` | **EVENT PASS** confirmation staging |
| BB SELECT TIER | set active shared BB tier | standard gate | **STRUCTURAL PASS / server rules unchanged** |
| BB REFILL Money | refill immediately | standard gate | **STRUCTURAL PASS** |
| BB REFILL Credits | stage confirmation | standard gate, then modal | **EVENT PASS** confirmation architecture |
| BB scroll | access all tiers | ScrollRect | **STRUCTURAL PASS** |

## Opponents, battle, result and history

| Screen | Control | Expected action | Gate | Result |
|---|---|---|---|---|
| Opponents | PRACTICE/RANKED/FRIEND | set mode | standard gate | **STRUCTURAL PASS** |
| Opponents | SELECT | choose rival | standard gate | **STRUCTURAL PASS** |
| Opponents | CONFIRM … BATTLE | submit one attack | hidden for protected rival; standard gate and duplicate guard | **FLOW PASS** attack→settlement; UI structurally audited |
| Opponents | REVENGE | submit ticket-bound Revenge | only with matching ticket; standard gate | **STRUCTURAL PASS** |
| Battle | SKIP | advance replay presentation only | standard gate | **STRUCTURAL PASS** |
| Battle | playback | advances visual frames until exact duration | local | **FIXED / EVENT PASS**; timer clamps and completed result is static |
| Result | NEW BATTLE | Opponents | standard gate | **RAYCAST + POINTER EVENT PASS** for victory/defeat/draw |
| Result | BACK TO CLUB | Club | standard gate | **RAYCAST + LONG-PRESS EVENT PASS** for victory/defeat/draw |
| Result | REPLAY | reset playback and stay Battle | standard gate | **RAYCAST + POINTER EVENT PASS** for victory/defeat/draw |
| Result | HISTORY | History | standard gate | **RAYCAST + POINTER EVENT PASS** for victory/defeat/draw |
| Result | sidebar navigation | navigate after settlement | standard gate | **STRUCTURAL PASS**; static result no longer destroys controls |
| History | REVIEW | load stored authoritative replay | standard gate | **STRUCTURAL PASS / LoadMatch flow audited** |
| History | return/sidebar | any permitted page | standard gate | **STRUCTURAL PASS** |

## Settings

| Control | Expected action | Result |
|---|---|---|
| club-name field / SAVE CLUB NAME | edit and save name | **RESTORED / STRUCTURAL PASS** |
| emblem 0–7 | authoritative emblem selection | **RESTORED / STRUCTURAL PASS** |
| LOAD LEADERBOARD | fetch and render leaders | **RESTORED / STRUCTURAL PASS** |
| local development fixtures | Grant, Recovery, recruit seed, Revenge ticket | **RESTORED**, only for `dev-*` non-Steam profile |
| NEW DEVELOPMENT PROFILE | return to login with a new local ID; preserve old server profile | **RESTORED / STRUCTURAL PASS** |
| VISUAL / MK PREVIEW | ArtSheet | **RESTORED / STRUCTURAL PASS** |

## Error and repeated-cycle behavior

| Case | Expected behavior | Status |
|---|---|---|
| no network / timeout | visible recoverable failure, busy clears after request returns | **STRUCTURAL PASS** (`UnityWebRequest.timeout=15`, all public operation coroutines release busy) |
| HTTP 401 | session-expired message; no silent operation retry | **FLOW PASS** for auth guard; reconnect remains via login/new dev profile rather than auto retry |
| HTTP 400/409 | rejected payload is cleared; refresh-only retry avoids replaying invalid/spent operation | **STRUCTURAL PASS** |
| HTTP 429 | visible wait/retry message; original key retained | **STRUCTURAL PASS** |
| insufficient funds/full roster/unavailable item | server rejection remains authoritative; UI stays on current screen with retry/refresh | **SERVER TEST PASS / UI STRUCTURAL PASS** |
| repeated modal open/close | background blocked, local buttons active, Escape closes | **EVENT + STRUCTURAL PASS** |
| click during refresh | all ordinary actions disabled; modal mutation confirm/equip also require `!busy` | **STRUCTURAL PASS** |
| double purchase/battle click | second `Send` coroutine exits while busy; retry reuses original key | **STRUCTURAL PASS** |
| multiple battles / result→replay→history→new battle | code paths and authoritative walkthrough verified; every Result transition pointer-tested | **EVENT + FLOW PASS**, full native physical sequence not completed |
| long mouse hold on Result | same button instance survives multiple frames before pointer-up | **EVENT PASS** |

## Remaining honest limitations

1. The Unity player canvas is not exposed as individual Windows UI Automation controls. Native background automation could capture the window but could not identify every Unity button; therefore a full physical mouse/keyboard matrix in the built executable is **not claimed**.
2. Headless Unity PlayMode fixed its render target at 640×480. Target-resolution coverage therefore uses the Canvas reference geometry/scaling contract for 1280×720, 1600×900 and 1920×1080; result buttons have a real `EventSystem.RaycastAll` proof in the available render target.
3. No real Steam login/payment was executed. Development-only and Credits confirmation UI was tested without spending real currency.
4. Network loss, timeout and each HTTP code are covered by request logic/server tests and static UI state tests, not by a packet-level fault injector in the native build.
5. Modern Settings restores the functional legacy profile actions, but Steam sandbox sign-in remains legacy-only and is not part of the local development login flow.
6. Baked fighter sprites still cannot visually swap Head/Rig/Camo layers; this existing art limitation is unrelated to interaction and was not disguised or changed.
