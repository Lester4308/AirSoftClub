# Revision 4 — articulated arms

2026-09-07. Independent Airsoft_Club_Game art prototype only.

## Change

Both arms now use two-bone inverse kinematics, with fixed shoulder positions and fixed upper-arm and forearm lengths. Rifle and SMG grip points drive elbow placement. A continuous textured triangle mesh blends around each elbow using existing PNGs; source images are unchanged. Poses are cached per arm and shared by the battle-view actors.

The support arm remains arm-rear-v3.png with no visible sleeve pocket. Revision 3 roster visibility corrections remain active. Added a 0–180 ms recoil slider for inspecting frozen poses.

## Verification

- node verify-rig.cjs: PASS, 510 pose/time samples; maximum anchor error 1.2710574864626038e-13. Checks fixed bone lengths, segment scale, shoulder/elbow/grip attachment, reachable targets, finite clamping, reduced motion and settled recoil.
- node --check skin-renderer.js and node --check preview.js: PASS.
- Browser: inspected Rifle articulation and SMG at 90 ms on graphite; inspected Rifle at 180 ms on light background. No obvious open elbow seam at the displayed scale. Sleeve pocket remains hidden on the support arm. This is visual prototype inspection, not final art approval.

## Remaining limitations

Combat body still includes armor, camouflage and head protection in one image. Finger contact and shoulder/elbow texture distortion need further artwork review at larger scale and through a broader range of poses. The Canvas renderer has not been profiled for production or integrated into Unity. No gameplay changes or raster generation in this revision.
