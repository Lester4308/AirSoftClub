# Volumetric 2D Integration 017

## Scope

This checkpoint proves that a cohesive, volumetric transparent 2D fighter can replace the primitive modular `Male017` presentation without moving runtime rendering to 3D.

## Asset source

The temporary bridge master is `art/prototype-016/assets/male-finished-cutout.png`, previously generated from the user-approved style reference. It is not declared final art and does not approve a future female derivative. It is used only because it already has coherent anatomy, lighting, hands, equipment, weapon, and genuine alpha.

## Runtime assets

Alpha-aware exports were generated into `UnityHost/Assets/Resources/Art/Volumetric017/Sprites/`:

- `male-inspect.png` — 1024×1536
- `male-roster.png` — 512×768
- `male-combat-near.png` — 256×384
- `male-combat-mass.png` — 128×192

Downscaling used premultiplied RGB and separately resampled alpha to avoid bright fringes.

## Runtime implementation

- `Volumetric017UiView.cs` selects a sprite LOD from the actual display height.
- Roster fighter cards, selected fighter detail, recruitment cards, and selected recruitment detail now use the cohesive sprite.
- Shop equipment previews and Battle continue using `Male017UiView`, because replacing those before matching modular layers and combat poses exist would falsely show equipped state and remove existing recoil behavior.
- `Volumetric017SpriteImporter.cs` enforces single-sprite RGBA import, alpha transparency, no mipmaps, clamp, bilinear filtering, high-quality compression, and 2048 max size.

## Verification

- Unity EditMode: 15 passed, 0 failed, 0 skipped.
- Unity PlayMode: 7 passed, 0 failed, 0 skipped.
- Unity Mono build: `AIRSOFT_BUILD_PASSED backend=Mono2x`.
- Built player launched successfully in a visible window.
- Desktop capture succeeded for the running player.

## Known limits and next gate

- Current bridge is male only.
- Equipment in the cohesive sprite is baked and therefore not used for equipment-state previews.
- No new female sprite, hero pistol, or six-family weapon sheet is claimed in this checkpoint.
- Next gate is a paired Male/Female hero sheet with identical camera/light/canvas rules, followed by compatible helmet/carrier layers and authored Aim/Fire/Hit/Out combat poses.
