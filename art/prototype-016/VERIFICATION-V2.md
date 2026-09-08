# Revision 2 verification — 2026-09-07

Completed changes: new alpha camouflage, open helmet shell, independent combat body/arms/rifle/SMG, shared grip/shoulder anchors, recoil on weapon with attached arm transforms. Removed baked fire-pose usage from current viewer. Original files retained.

Automated: verify-rig.cjs PASS, 510 pose samples, max anchor error 1.28e-13 source pixels. Both shoulders and both grips checked per pose; finite transforms, forward muzzle, reduced motion and recoil settling checked. verify-assets.ps1 PASS for seven RGBA sprites with 52–74% sampled fully transparent area. This is a coarse alpha diagnostic, not a full silhouette quality metric.

Browser: 10 PNGs loaded; roster camouflage composite and open helmet visually inspected, eyes no longer obscured by helmet lining. Rifle/SMG switching observed, anchor display enabled, recoil action triggered. Hiding both arms and weapon exposed the armless body layer, confirming no baked gun remains in current combat body. Light/dark background visual inspection confirmed no opaque matte rectangles. 16v16 renders 32 separate modular figures.

Additional browser checks: SMG 1v1 rendered on light background; 4v4 selected; idle entered its running state; browser error log empty. Light-background inspector text contrast corrected. Original v1 baked fire image is not loaded by revision 2.

Limits: clothing cuffs/collar and hair edge are approximate. Each arm is one cutout scaled between two anchors, so limb dimensions vary slightly with grip distance. Head/armor/camo remain baked in combat body. No final-art approval or Unity integration is implied by these checks.
