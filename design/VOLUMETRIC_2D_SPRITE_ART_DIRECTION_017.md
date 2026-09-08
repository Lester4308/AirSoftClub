# AIRSOFT CLUB — VOLUMETRIC 2D SPRITE ART DIRECTION 017

**Status:** active production direction  
**Rendering model:** 2D sprites only at runtime  
**Supersedes:** any real-time 3D character/weapon proposal and the visibly primitive polygon/vector interpretation of “low-poly”  
**Reference policy:** user images define quality, believable construction, material response, and visual density. Do not copy identities, UI layouts, markings, costumes, or exact weapon designs.

## 1. Visual target

Airsoft Club uses **volumetric realistic-stylized 2D sprites**. Every fighter and weapon must read as a painted or pre-rendered dimensional object even though runtime presentation is 2D.

The final look combines:

- believable anatomy and equipment construction;
- controlled stylization and clean silhouettes;
- broad modeled light and shadow masses;
- localized ambient occlusion/contact shadows;
- material-specific highlights and edge behavior;
- restrained surface detail that survives downscaling;
- no visible 3D models in gameplay;
- no flat vector limbs, obvious polygon mosaics, or paper-doll seams.

“Low-poly” is retained only as inspiration for **clean shape hierarchy and restrained noise**. It must not appear as triangular skin, faceted cylinders, or three flat colors pretending to be volume.

## 2. Runtime decision

Unity displays authored transparent PNG sprites through UGUI/SpriteRenderer. It does not render real-time character meshes.

Use a **hybrid sprite pipeline**:

1. Author a cohesive, high-resolution finished fighter for each approved body family and pose.
2. Derive compatible equipment and weapon layers against locked pose templates.
3. Use limited skeletal transforms only for breathing, recoil, aim offsets, and small weight shifts.
4. Use corrective sprites at shoulders, elbows, wrists, helmet/hair, and carrier edges where bone deformation exposes seams.
5. Use authored pose swaps for large actions such as Aim, Hit, Out, Victory, and Defeat.
6. Render portraits, roster cards, shop previews, and combat LODs from the same mastered sprite set.

This avoids both extremes:

- one baked full-body image for every possible equipment combination;
- over-fragmented paper-doll anatomy whose arms and armor visibly detach.

## 3. Source and export sizes

### Character master

- source canvas: **2048×3072** RGBA;
- authored facing: three-quarter right;
- feet anchor: `(1024, 2920)` in top-left source coordinates;
- full silhouette must remain inside a 96 px safe margin, except declared long-weapon bounds;
- straight alpha, no baked checkerboard or colored matte;
- body and every compatible layer use identical canvas, camera, perspective, lighting, and anchor metadata.

### Character runtime LOD

| LOD | Export | Use |
|---|---:|---|
| Inspect | 1024×1536 | fighter detail / customization |
| Roster | 512×768 | roster / recruitment |
| Combat Near | 256×384 | 1v1 / selected fighter |
| Combat Mass | 128×192 | 8v8 / 16v16 |
| Portrait | 256×256 | cards / identity |
| Icon | 96×96 | compact lists |

Downscale from the master with alpha-aware Lanczos/production resampling, followed by a controlled one-pixel silhouette cleanup. Do not separately redraw LODs until measured downscaling proves insufficient.

### Weapon master and LODs

| LOD | Export | Use |
|---|---:|---|
| Inspect | 1536×512 | store hero view |
| Card | 768×256 | shop/loadout |
| Combat | 512×192 | hands / battle |
| Icon | 256×96 | inventory |
| Silhouette | 128×64 | recognition test |

## 4. Volumetric painting/render language

### Form hierarchy

Every asset must contain four readable scales:

1. **Silhouette:** body type, stance, helmet, carrier, and weapon family.
2. **Large volume:** rib cage, pelvis, limbs, carrier mass, receiver, stock, barrel.
3. **Construction:** seams, pouches, straps, buckles, hand placement, weapon controls.
4. **Micro-detail:** weave, edge wear, dust, engraving—only where visible in Inspect.

### Lighting baked into sprites

Use one canonical studio rig:

- warm key from upper viewer-left;
- cool weak fill from viewer-right;
- neutral narrow rim separating dark gear from the background;
- soft contact AO at collar, carrier edges, straps, pouches, gloves, stock, and helmet;
- no crushed black cavities;
- no white clipped highlights;
- no inconsistent light direction among swappable layers.

The sprite should retain volume on graphite, olive, warm-grey, and light QA backgrounds.

### Material response in 2D

Volume must come from material-specific value structure, not random texture:

- **skin:** soft warm half-tone, restrained pores, localized cheek/nose/ear warmth;
- **uniform fabric:** broad soft folds with compression under straps;
- **Cordura/webbing:** tighter coarse response, strong edge thickness, readable stitching;
- **polymer:** broad matte highlight and molded edge breakup;
- **painted metal:** narrower cool highlight with causal edge wear;
- **rubber:** dark diffuse response above absolute black;
- **optic glass:** small controlled reflection with visible lens volume, never a permanent cyan glow;
- **hair:** grouped masses with selective strand accents, not noisy individual hairs.

## 5. Male and female bases

Both bases share canvas, feet, weapon, head, chest, and UI anchors. They do not share the same painted anatomy.

### Male

- 7.25–7.75-head believable proportion;
- athletic enthusiast, not exaggerated military superhero;
- clavicle, shoulder, elbow, wrist, pelvis, knee, and ankle positions remain anatomically plausible;
- natural asymmetry and weight on one supporting leg;
- head enlargement limited to **5–8%** for gameplay readability.

### Female

- independently authored anatomy, not a narrowed male or color swap;
- practical shoulder girdle, rib cage, waist, pelvis, hands, neck, and facial structure;
- body-specific fitted uniform and carrier edges under the same category IDs;
- equal equipment mass, competence, and visual detail;
- no sexualized armor, heels, exposed skin, doll face, or pin-up stance;
- helmet-compatible hair variants designed as grouped silhouettes.

### Shared anchors

```text
FeetGround
Pelvis
ChestCenter
HeadCenter
HeadTop
ShoulderPrimary
ShoulderSupport
GripPrimary
GripSupport
StockShoulder
Muzzle
PatchChest
PatchArm
```

Anchor tolerance after LOD export: ≤ 1 source-equivalent pixel for rigid equipment; ≤ 2 pixels for corrective arm overlays.

## 6. Layer strategy

### Roster/customization composition

```text
Shadow
BodyAndBaseUniform
CamouflageOrUniformVariant
HairRear
HeadAndFace
FaceProtection
Helmet
CarrierRear
CarrierFront
PouchesAndPatches
WeaponRear
CorrectiveArmsHands
WeaponFrontDetails
TeamTape
FX
```

### Combat composition

Prefer **cohesive pose modules**, not dozens of anatomical fragments:

```text
Shadow
CombatBodyPose
HeadOption
CarrierOption
Weapon
RearArmCorrective
FrontArmAndFingersCorrective
TeamMarker
FX
```

Rules:

- torso and legs remain cohesive per pose to preserve anatomy and folds;
- separate only layers that truly change or animate;
- shoulders/elbows may use painted corrective caps hidden beneath clothing seams;
- fingers wrap the handguard/grip as foreground corrective art;
- helmet hides incompatible hair using explicit variant metadata;
- large carrier variants use body-specific raster masks, never runtime guessed bounds;
- no visible seam may rely on UI background color to disappear.

## 7. Animation strategy

### Authored pose families

- IdleReady: 6–8 frame subtle loop or limited bone breathing;
- IdleRecovering: 6–8 frames;
- AimLow / AimLevel / AimHigh: locked pose masters at −8°, 0°, +8°;
- FireSingle: 4–6 frames;
- FireBurst: 6–8 frames;
- ShotgunPump / Bolt: class-specific 6–10 frames;
- HitFront / HitRear: 5–7 frames;
- Out: 8–12 frames;
- Victory / Defeat: 8–12 frames.

Use animation transforms only within the pose envelope that passed seam and anatomy review. Gameplay remains authoritative; sprite animation consumes replay events and never decides hits or BB use.

### Mirroring

Author right-facing. Mirror the entire CharacterRoot for left-facing, including sockets, team markers, recoil, and muzzle FX. Do not mirror readable patches/markings; provide neutral or mirrored-safe decals.

## 8. Weapon quality bar

Six families must differ by construction and silhouette:

1. Pistol — slide/frame/grip, no stock.
2. SMG — compact receiver, short barrel, compact stock, small magazine language.
3. Assault Rifle — balanced stock/receiver/handguard/barrel.
4. Shotgun — long barrel and tube/fore-end pump language.
5. DMR — longer barrel, full stock, medium optic and heavier receiver rhythm.
6. Sniper Rifle — longest profile, large optic, bolt/stock/bipod cues.

Every weapon sprite must visibly resolve:

- receiver separation;
- handguard thickness;
- barrel and hollow muzzle;
- trigger and guard;
- grip;
- magazine and well;
- stock/brace mechanism;
- rail/slot rhythm;
- sling point;
- optic mount and lens;
- airsoft-specific cue where appropriate.

Avoid real manufacturer names and one-to-one real weapon copies. Build original families from plausible mechanical grammar.

### MK visual language

- MK1: utilitarian finish, basic sighting, clean factory geometry.
- MK2: refined furniture, clearer material separation, compact optic treatment.
- MK3: premium machining language, restrained club accent, richer but plausible construction detail.

MK tiers must remain the same family and must not advertise selectable attachments that gameplay does not support.

## 9. Color and team readability

- Team color appears on arm tape, chest patch, small gear tags, base shadow/UI marker, and muzzle/replay UI—not as whole-body tint.
- Friendly cyan and enemy orange-red must also differ by marker shape/direction.
- Clothing remains olive, ranger green, coyote, graphite, muted woodland, and controlled club variants.
- Primary UI orange remains separate from team identification where simultaneous actions appear.

## 10. Acceptance gates

### Gate A — hero close-up

- male and female look volumetric at Inspect LOD;
- anatomy remains believable without equipment;
- face, hands, helmet, carrier, and weapon survive 200% review;
- no flat vector gradients, polygon mosaic, plastic-everything material, or AI deformation;
- female is immediately distinct without sexualization.

### Gate B — modularity

- approved Male/Female × camo × helmet × carrier × representative weapon combinations align;
- no hair/helmet, neck/collar, carrier/arm, hand/grip, stock/shoulder, or pouch/belt seams;
- all anchors remain inside tolerance after every LOD export;
- missing or incompatible variants fail validation rather than silently substituting.

### Gate C — animation

- aim −8°/0°/+8° passes anatomy and seam review;
- recoil preserves both grips and stock contact;
- corrective art hides shoulder/elbow/wrist joins;
- reduced-motion mode removes sway/shake while retaining readable state.

### Gate D — weapon

- all six families are identifiable as black silhouettes at 128×64;
- hero AR and pistol remain mechanically coherent in Card and Inspect views;
- muzzle, grips, stock, magazine, and optic anchors are truthful;
- no thin barrel alpha loss or bright fringe on light/dark backgrounds.

### Gate E — game scale

- 1v1 shows detail and stance;
- 8v8 preserves body/weapon/team separation;
- 16v16 preserves silhouette and state without microdetail shimmer;
- HP/name/marker overlays do not hide heads, muzzles, or poses;
- sprite atlas, draw calls, memory, and frame time meet measured targets.

### Gate F — provenance

- every generated, painted, purchased, or commissioned source records license and origin;
- user references never enter shipped assets;
- final characters and weapons do not reproduce identifiable protected designs;
- Shop and Battle use the same approved `visualId` family.

## 11. Vertical slice scope

Before mass production, complete:

- one high-quality male body/base-uniform sprite;
- one equal-quality female body/base-uniform sprite;
- one helmet/eye/face option;
- one carrier/pouch option fitted for each body;
- one hero AR and one hero pistol;
- silhouette sheet for all six weapon families;
- IdleReady, AimLevel, FireSingle, Hit, and Out pose families;
- Inspect, Roster, Shop, 1v1, 8v8, and 16v16 presentation;
- deterministic alpha/anchor/LOD/atlas validation.

Do not generate the full catalog until all gates pass.

## 12. Immediate corrective actions

1. Keep current `Male017` only as a runtime integration bridge.
2. Use `art/prototype-016/assets/male-finished-cutout.png` as a quality-comparison candidate, not an automatically approved production master.
3. Reject current primitive body/weapon polygons as final assets.
4. Produce a paired male/female hero sheet first.
5. Separate equipment only after cohesive anatomy, lighting, and pose are approved.
6. Integrate the approved master through one sprite provider so Roster, Shop, and Battle cannot drift.
