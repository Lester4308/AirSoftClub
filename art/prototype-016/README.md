# Current deliverable: cohesive static character

User rejected the modular assembly. The current index.html shows the rebuilt character. See FINISHED-CHARACTER.md for files, provenance and scope. Everything below is historical.

# Male modular art feasibility test 016 — revision 7

User review rejected revision 2 overlap quality: visible base-clothing contours, hair outside helmet and sleeve pocket on the visible side of the left support arm. Anchor math PASS never meant art approval. Revision 3 removes the support-arm pocket (assets/arm-rear-v3.png), renders only exposed body regions when camouflage is active, hides hair under helmet, and re-renders foreground sleeves over carrier side panels. Carrier placement is adjusted. These are runtime visibility masks in the composition renderer; source raster assets are not edited by code. See REVISION-3.md. Revision 6 closes the hollow combat shoulder socket with an image_gen edit, aligns the replacement body to the existing rig and adds boot contact shadows. See REVISION-6.md for prompts and verification.

2026-09-07. Independent Airsoft_Club_Game only. Source checkpoint verified locally: codex/implementation-003-development-pass, 946526be7b3ec7f746ba36b75c03aa73f6cbb488. Runtime/gameplay files were not changed.

## Status

USER APPROVED: appearance in assets/approved-reference.png, soft low-poly direction, proportions, male/female design, no thigh holsters. This approval does not automatically approve the newly generated derivatives.

PROTOTYPE IMPLEMENTED: transparent male base, camouflage-v2, armor, face protection and open helmet-shell-v2; three roster combinations. Combat now uses an armless body, independent front/rear arm PNGs and interchangeable Rifle/SMG PNGs. Two-bone inverse kinematics keeps segment lengths fixed and grips attached during recoil and idle sway. A continuous textured mesh blends the elbow; the recoil slider allows paused inspection. See REVISION-4.md. Revision 5 adds foreground support fingers around the handguard, a magnified grip view and deterministic paused recoil; see REVISION-5.md. Both the close-up and 1v1/4v4/16v16 views use this modular assembly. Controls hide individual layers and show anchors. Open index.html in a browser or run node serve.cjs for http://127.0.0.1:8769/. No account/backend required.

NOT PRODUCTION READY: combat body still bakes torso armor, head protection and camouflage; these slots are independently demonstrated only in roster. Arms now use an articulated mesh derived from the existing bent cutouts. Fingers and extreme elbow poses still need corrective artwork; outfit fit and helmet hair edge need refinement. Generated gear is more textured than the approved reference. No Unity integration, final-art approval, gameplay verification or native build is claimed. The old opaque camouflage and baked fire sprite are retained as historical artifacts and excluded from the current renderer.

## Proposed production contract

- Fixed 1024x1536 source canvas for body and clothing; feet anchor (512,1484), top-left source coordinates. Gear source sizes may differ; explicit placement is mandatory, never inferred from transparent bounds at runtime.
- Body variants: bare-headed base and helmet-compatible hair silhouette. Clothing must have exact matching cuffs, neck opening and ankles. Boots stay baseline visual only.
- Current combat draw order: rear arm / body / weapon / support fingers / front arm. Roster order: body / camouflage / armor / helmet shell / face protection. Source anchor definitions are in rig.js. Source PNGs are never changed by renderer.
- Separate art layers are not additional gameplay slots. Keep the four existing slots; face protection remains baseline presentation.
- Start with roster idle and a separate combat-ready pose. Prove one weapon grip before adding six weapon families. Animation candidates: authored skeletal cutout with corrective sprites for elbow/shoulder, or per-pose layers with identical anchors. Do not generate unrelated full frames and assume temporal consistency.
- Source PNG straight RGBA, genuine alpha, anti-aliased edges, no baked checkerboard. Reject if nontransparent outside silhouette. Review on light, graphite and olive backgrounds. Alpha audit is a coarse diagnostic, not complete matte validation.
- Battle view tests use repeated pose sprites to expose scale/overlap, not representative combat decisions. No reload. All future event feedback remains presentation-only and must consume authoritative events.

## Next required work

1. Separate combat clothing, head and armor against the locked combat pose; match all textures to approved reference.
2. Refine hand overlap and elbow textures on the articulated rig; test a wider motion range before authoring additional poses.
3. Approve derivative pose/style and test remaining motions, then replicate to Female and the gear catalog.
4. Integrate through TacticalArt texture providers only after the art pipeline gate.

## Verification commands

node verify-rig.cjs checks 1530 pose/time/aim samples for fixed shoulders, attached grips, fixed upper/lower lengths, reachable targets, finite transforms, forward muzzle, reduced motion and settled recoil. This proves anchor math, not anatomical art quality.

./verify-assets.ps1 checks sampled alpha occupancy for seven new RGBA assets and writes dimensions/hashes to alpha-audit-v2.json. Browser QA checks real composition on dark/light backgrounds. See VERIFICATION-V2.md; original VERIFICATION.md is historical.

## Provenance

Images generated with built-in image_gen from the user-approved reference in this conversation; no Project Airsoft assets used. Original outputs retained under C:/Users/Ihor/.codex/generated_images/01a07b64-1b96-7472-92b6-61342fdf6f7c/. Selected copies are local in assets/. Rejected matte retained for diagnosis, excluded from viewer.

Generation prompts requested: male base preserving approved head/body proportions and olive clothing; matching camouflage-only layer; separate tan carrier, helmet and goggles/mesh mask in the same three-quarter direction; a two-handed firing-ready male with no thigh holster; background extraction preserving silhouettes. Full prompt transcript is in prompts.md.




Revision 7 adds three aiming positions with coupled grips, recoil and BB direction; see REVISION-7.md.


