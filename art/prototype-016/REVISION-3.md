# User-directed overlap correction

2026-09-07. User supplied screenshots of poor roster layer fit and a left support arm whose pocket should be hidden on the back of the sleeve.

Changes:
- New arm-rear-v3.png without visible rectangular pocket, flap or stitching. Original v2 retained.
- Camouflage now replaces the base clothing silhouette. Only exposed head/neck/hands/boots are rendered from the base beneath the garment.
- Head visibility changes with helmet, eliminating the spiky hair sticking beyond shell.
- Sleeves render in front of carrier side pouches; carrier size and placement tightened.
- No gameplay or Unity runtime files changed.

Verification: edited arm is 1536x1024 with corner alpha 0, inspected without pocket. Browser assembly inspected on graphite/light backgrounds; camouflage and helmet toggled off/on. Recoil action exercised with new arm; captured browser error log empty. This checks the specific reported defects, not final art readiness. Garment shaping, hand grips, combat gear separation and articulated elbows remain prototype work.

Built-in image_gen prompt (source assets/arm-rear-v2.png): Precise local edit: REMOVE the large rectangular upper-sleeve pocket, its flap, all pocket stitching and its rectangular outline from this left supporting arm. Replace the pocket area with continuous plain camouflage fabric and natural matching sleeve folds. In this camera view the pocket is on the hidden rear side, so there must be NO visible pocket or patch on this sleeve. Keep the EXACT same 1536x1024 canvas, silhouette, pose, glove, hand, elbow, shoulder, color, scale and positioning. Preserve transparent background and original alpha silhouette. Do not recenter or resize. Only erase the pocket from the visible upper sleeve.

Follow-up on first generated edit: Remove the background. Transparent background. Keep the sleeve without any pocket.

Final generated source: C:/Users/Ihor/.codex/generated_images/01a07b64-1b96-7472-92b6-61342fdf6f7c/exec-482c284d-e2e1-4c98-b07f-d38d8241ef24.png. Selected project copy: assets/arm-rear-v3.png. Tool regeneration introduced small texture differences; do not describe as pixel-identical.
