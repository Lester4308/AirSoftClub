# UI interaction changelog — 2026-09-09

## Root causes

- `replayTime` continued increasing after an authoritative battle result completed. Because the Battle render signature included the unbounded replay frame, Modern UGUI destroyed and recreated the whole hierarchy roughly every 33 ms. A physical pointer could press one Result button instance and release after that instance was destroyed.
- The four Result actions occupied 1290 px plus offsets inside a 1310 px content area; the HISTORY button extended past the reference canvas and failed a real `EventSystem.RaycastAll` check in the batch render target.
- One global action gate was previously applied to both page/background actions and controls inside inventory/Credits modals.
- Modal-local mutation controls could bypass `busy`, leaving a duplicate dispatch window.
- Several legacy functions were implemented by the state/backend but absent from Modern navigation or Settings.

## Changes

- Clamp replay time to `SimulatedDurationMs`; include replay frame in `ModernRenderSignature()` only while animation is active.
- Fit NEW BATTLE, BACK TO CLUB, REPLAY and HISTORY inside the content region.
- Keep modal CLOSE/CANCEL usable while blocking background actions; require `!busy` for modal mutation/confirmation requests.
- Add coroutine-entry duplicate guards to Login, SteamLogin, Send, Retry and leaderboard requests; preserve the original command key for valid retry.
- Add Return-to-login and Escape-to-close-modal keyboard paths.
- Preserve input text/focus across idle frames by avoiding unnecessary rebuilds.
- Restore Equipment and Recovery sidebar routes.
- Restore functional Modern Settings controls: name, emblems, leaderboard and local development fixtures.
- Disable catalog BUY when the server-provided `Access` flag is false.
- Fall back to the first actual recruitment offer instead of assuming an ID ending in `-0`.
- Widen equipment slot actions and verify training row geometry.

## Verification

- Result regression: same button instance survives pointer-down, three frames and pointer-up; page changes afterward.
- Result matrix: all four actions pass `EventSystem.RaycastAll` and pointer dispatch for attacker win, defender win and draw.
- Credits modal: background navigation disabled; CONFIRM/CANCEL active; confirm receives raycast.
- Login input: instance, text and focus survive idle updates.
- Training: buttons remain inside rows at 1280×720, 1600×900 and 1920×1080 reference scaling.
- Latest PlayMode at changelog creation: **15 passed, 0 failed, 0 skipped**.
- Full final .NET/EditMode/PlayMode/Mono build results are recorded in `BUTTON_AUDIT.md` and the final task report after packaging.

## Limitations

Native Windows automation did not expose individual Unity Canvas controls, so physical native clicks are not claimed where only EventSystem tests were available. Steam payments/login and the separate baked-gear visual limitation remain outside this interaction fix.
