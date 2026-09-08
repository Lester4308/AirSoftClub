# Vertical Slice 016 — Blockout Checkpoint

## Purpose

This checkpoint replaces the primitive 2D character/weapon production direction with an executable 3D-first asset pipeline. It is deliberately a geometry and integration blockout, **not final character art**.

## Verified toolchain

- Blender: `5.2.1 LTS`
- MPFB extension: `2.0.17`, official Blender Extensions package
- MPFB package SHA-256: `4f0a879d64a39bf646fbf5f53601ac678855da329d650617dca5737548239a87`
- MPFB generated human base: 19,158 vertices before production retopology/LOD work
- Unity target: `6000.3.21f1`

## Generated artifacts

- `source/mpfb_base_created.blend` — procedural human base source.
- `source/airsoft_vertical_slice_016.blend` — editable male/female, equipment, AR, material and lighting blockout.
- `exports/airsoft_vertical_slice_016.obj` + `.mtl` — static Unity-readable geometry checkpoint.
- `renders/vertical-slice-blockout-016.png` — actual Blender EEVEE catalog render.
- `tools/art/build_realistic_vertical_slice_016.py` — deterministic rebuild script.

## Current content

- Separate Male and Female body shapes generated from the same topology.
- Volumetric torso uniform, carrier, front plate, three pouches, belt, knee protection, helmet, eye protection, ear protection and local team tape.
- Original AR blockout with receiver, handguard, barrel, muzzle, stock, grip, magazine, rail, optic and lens.
- Weapon sockets: `GripPrimary`, `GripSupport`, `StockShoulder`, `Muzzle`, `Magazine`, `OpticRail`.
- Six family silhouette blockouts: Pistol, SMG, Assault Rifle, Shotgun, DMR and Sniper Rifle.
- Initial fabric, Cordura, polymer, painted metal, rubber, optic-glass, skin and team-accent materials.
- Catalog camera and three-point lighting rig.

## Validation

- Blender source creation completed.
- Blender source save completed.
- EEVEE render completed at 1600×900.
- OBJ/MTL export completed.
- Unity EditMode verification after copying the static checkpoint into `Assets/Art/Realistic3D/Blockout016`: **15 passed, 0 failed, 0 skipped**.
- Local vision inspection correctly classified the render as blockout-only. It also misclassified the female as male, confirming that body-type differentiation and camera presentation are not yet strong enough for approval.

## Known limitations / next next

- This is unrigged static geometry; the shared game skeleton, skinning and IK are the next milestone.
- Uniform and carrier are parametric volume proxies, not fitted production clothing.
- Hands do not yet grip the rifle.
- Faces, hair, cloth folds, webbing construction, stitching, UVs, PBR texture maps and causal wear remain unfinished.
- Six weapon families are silhouette proxies only; the AR is a medium-detail construction proof, not hero quality.
- The OBJ checkpoint is for import validation. Final animated interchange will use Unity-supported FBX/glTF tooling after the shared rig is created.
- The female silhouette needs stronger, practical anatomical and fitted-equipment differentiation without sexualization.
- `reference-contact-sheet.jpg` is review evidence derived from user-provided images and must not ship with the game.

## Next acceptance gate

1. shared humanoid rig and Male/Female skinning;
2. fitted uniform/carrier meshes with body hide masks;
3. Aim pose with right-hand placement and left-hand IK;
4. stronger Male/Female silhouette test;
5. hero AR refinement and 64×64 readability render;
6. Unity inspect camera plus 1v1/8v8/16v16 stress scene.
