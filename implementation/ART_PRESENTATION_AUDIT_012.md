# Final Milestone 012 — art / presentation audit

**Status: CONDITIONAL PROTOTYPE PASS; FINAL ART REMAINS OPEN.**

Audit scope: current UGUI presentation, the integrated `Male017` sprite pack, repository art records, and battle composition from 1v1 through 16v16. This report does not approve generated derivatives or call the current client production-ready.

## Asset truth and provenance

- The integrated `Male017` pack contains ten independent RGBA PNG layers: two bodies, two support arms, two shooting arms, two heads, one vest, and one rifle. The Unity resource copies are byte-identical to `art/sprite-pack-male-017/sprites/`; dimensions and SHA-256 values match `asset-audit.json`.
- `PROVENANCE.md` identifies built-in image generation, the user-provided reference identifier, prompts, selected output identifiers, rejected variants, and the retained local generation directory. It explicitly states that no Project Airsoft content was used.
- Provenance is traceable for this repository, but distribution/licensing clearance of the user-supplied reference and generated derivatives is **not documented**. That remains a release gate.
- `atlas.png` and `atlas-rects.json` predate the corrected support arm and are explicitly historical. `source/` contains rejected arms. They must not be promoted or copied into runtime resources.
- `art/prototype-016` is not the runtime source. Its modular rig was rejected and remains historical. Its cohesive `male-finished-cutout.png` is a separate static derivative and is not integrated into Unity. Calling either set final would be inaccurate.

## Current Unity integration

- `Male017UiView` composes six visible layers in an honest fixed order: support arm, body, head, vest, rifle, shooting arm. Camouflage swaps body and both arm textures; head protection swaps the complete head; armor toggles the vest; weapon visibility toggles the rifle.
- These are presentation switches over gameplay state, not additional gameplay slots. Recoil is a small presentation-only displacement.
- Layer independence is limited. Head protection is baked into a complete replacement head; camouflage is baked into complete body/arm variants; gloves and baseline clothing are not independent; the rifle is the only weapon image. Therefore this is a useful prototype compositor, not a final modular wardrobe/weapon pipeline.
- The same male pose and face are repeated for every fighter, recruit, equipment preview, and combatant. IDs and equipment toggles provide state variation, but character identity does not.

## Missing production families

Open art gates:

1. **Female fighter family:** no female runtime raster assets or female UGUI compositor exist. The earlier procedural `TacticalArt` claim of male/female bases does not describe the current `Male017` integration.
2. **Weapon families and MK truth:** runtime `Male017` has one rifle sprite. Pistol, SMG, shotgun, assault rifle, sniper and machine-gun silhouettes, and their MK1/MK2/MK3 treatments, are not represented by this compositor. Current shop previews can therefore show the same male/rifle vocabulary for different definitions.
3. **Character diversity:** additional approved heads, skin/hair variants, silhouettes, and controlled uniform variants are absent.
4. **Layer production contract:** separate helmet, eye/face protection, clothing/camouflage, armor, hands and weapon grip corrections are not complete. Source anchors and overlap need final art QA at every supported scale.
5. **Animation/effects:** idle, aim, hit, OUT and recoil are presentation approximations over a static cutout. Authored animation, corrective poses, final VFX, audio, and environment art remain open.
6. **Rights and approval:** generated-image/reference licensing review and explicit user art approval remain open.
7. **Accessibility/localization:** small labels at dense scale, color-vision-safe team distinction, localized text fit, keyboard/focus review, and UI scale testing remain open.

## Battle readability audit (1v1–16v16)

- The arena uses one to four rows and four columns per side per row. In 1v1, each fighter is capped at 120x200 logical pixels, leaving substantial negative space; identity and HP are readable, though the static repeated pose limits spectacle.
- At 16v16, each row previously reserved 50 pixels inside a 125-pixel lane, reducing fighter art to roughly 75 pixels high. Fine equipment and facial differences cannot carry information at that size. Names and eight-pixel HP bars are near the practical lower bound.
- Team direction (inward-facing), left/right placement, blue/orange HP, team labels, and faint card tint provide redundant side cues. Dense battles still need stronger non-color cues and should not depend on uniform detail.
- BB dots indicate recent authoritative damage events but are not actor-to-target trajectories. The code comment now states that limitation rather than implying spatial simulation.
- Safe local polish increases dense fighter height, raises dense name size, increases HP thickness/contrast, keeps trails aligned with the adjusted lanes, and adds a solid side-colored edge marker to every fighter card. It does not alter replay timing, events, HP, BB accounting, settlement, or gameplay state.

## Acceptance boundary

The current presentation is suitable for continued local prototype/UI evaluation after Unity verification. It is **not** sufficient for final-art acceptance, store imagery, a claim of male/female coverage, a claim of six weapon families/MK visual coverage, or production release.
