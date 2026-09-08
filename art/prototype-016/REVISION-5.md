# Revision 5 — grip layering and close-up

2026-09-07. Art prototype only, no gameplay or source PNG changes.

The weapon previously covered the entire supporting hand, making the hand appear to sit beneath the handguard. The renderer now redraws the visible fingers over the handguard using a source-space visibility polygon transformed by the same forearm matrix. The sleeve remains behind the weapon. Hiding either the support arm or weapon disables this extra pass.

Added a magnified grip view driven by the same combat renderer and controls. Paused recoil now uses a fixed animation time so changing controls does not move the inspected pose through idle sway.

Verification: JavaScript syntax check passed. Browser inspected Rifle at 180 ms on graphite and SMG at 90 ms on light background. The support fingers visibly wrap the handguard in both views. Browser error log was empty. Bone math unchanged from revision 4. This verifies the sampled compositions, not final anatomical artwork or all future weapons.

Combat clothing/head/armor separation remains outstanding. Source art still needs refinement for additional poses and weapon families.
