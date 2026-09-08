# Airsoft Club — modular low-poly character and weapon production contract

Status: **superseded for production art** by `REALISTIC_STYLIZED_3D_ART_DIRECTION_016.md`. Retained as historical migration guidance for the current sprite prototype; gameplay rules and useful naming conventions remain reference-only.

## 1. Core decision

Use a **2D skeletal modular character pipeline** rendered from authored low-poly source models or equivalent crisp 2D layers. Male and female characters share:

- the same skeleton hierarchy;
- animation clips and timing;
- named equipment sockets;
- layer categories and export rectangles;
- camera, light rig and orthographic render framing;
- gameplay presentation API.

They may use different body meshes/silhouettes, but equipment-compatible variants must preserve socket names and practical overlap envelopes. Unity Sprite Library / Sprite Resolver is suited to switching labeled parts at runtime; when sprite swapping is combined with skeletal animation, compatible sprites must use an identical skeleton.[1][2][3]

## 2. Canonical character hierarchy

```text
CharacterRoot
├─ Shadow
├─ Skeleton
│  ├─ Pelvis
│  ├─ Spine / Chest / Neck / Head
│  ├─ UpperArm_L / Forearm_L / Hand_L
│  ├─ UpperArm_R / Forearm_R / Hand_R
│  └─ Thigh / Calf / Foot L/R
├─ BodyBase
├─ Uniform
├─ CamouflageOverlay
├─ HeadBase
├─ HairOrCap
├─ EyesFace
├─ FaceProtection
├─ Helmet
├─ ArmorVest
├─ PouchesVisual
├─ WeaponRear
├─ HandsArms
├─ WeaponFront
├─ ClubPatch
└─ FXSockets
```

Gameplay still has exactly four mechanical slots: Weapon, Camouflage, Head Protection and Load-bearing/Armor. Visual sublayers such as hair, face, gloves, patch and pouches are presentation parts, not new mechanical slots.

## 3. Universal male and female bases

### Shared contract

- Canvas: 512×768 at detail LOD; feet at `(256,710)`.
- Facing: authored facing right; mirror `CharacterRoot` for facing left.
- Named sockets: `GripPrimary`, `GripSupport`, `Muzzle`, `StockShoulder`, `HeadTop`, `FaceCenter`, `ChestFront`, `PatchChest`, `PatchArm`, `FeetGround`.
- Bounds: all swappable parts remain within the canonical canvas or declare an extended weapon bound.
- No equipment is baked into BodyBase.

### Male base

- Broad shoulder/chest plane, straighter waist, heavier forearm/boot silhouette.
- Head approximately 15–20% larger than realistic military simulation proportions for readability.
- Two neutral faces and three hair/short-cap shapes at first production delivery.

### Female base

- Same height/feet/sockets and hand targets; narrower shoulder plane, shaped torso/waist, distinct jaw and hair silhouette.
- Avoid sexualized armor or unrealistic stance. Vest and weapon handling remain functionally equivalent.
- Equipment may have body-specific raster/mesh labels under the same category, e.g. `Armor/Carrier01/Male` and `Armor/Carrier01/Female`.

## 4. Customization taxonomy

| Category | Initial production target | Mechanical? |
|---|---:|---|
| Body type | Male / Female | No |
| Skin tone | 6 controlled low-poly palettes | No |
| Face | 4 per body family | No |
| Hair/cap | 6 shared-style silhouettes | No |
| Club patch | emblem + two club colors | No |
| Uniform base | 3 neutral material sets | No |
| Camouflage | catalog-driven variants | Yes: Camouflage slot |
| Face/head | goggles, mask, helmet combinations | Yes: Head Protection slot |
| Vest/armor | chest rig / light / medium / heavy | Yes: Armor slot |
| Weapon | six families × MK1/2/3 | Yes: Weapon slot |

Customization is deterministic data, not prefabs generated ad hoc:

```text
AppearanceId
BodyFamily
SkinPalette
FaceLabel
HairLabel
UniformLabel
ClubPrimaryColor
ClubSecondaryColor
MechanicalEquipmentInstanceIds
```

Server persists allowed cosmetic IDs and mechanical item ownership; client composes visuals but never grants items or combat stats.

## 5. Animation set

Shared clips:

- IdleReady (subtle breathing/weight shift)
- IdleRecovering
- Aim
- FireSingle
- FireBurst
- FireShotgun
- BoltOrPump
- HitFront / HitRear
- Out
- VictoryShort / DefeatShort

Animation events are presentation-only. Authoritative replay events select visual clips; animation never generates hits, BB use or settlement.

## 6. Weapon contract

Six families must have different silhouettes at 64 px:

| Family | Silhouette cue |
|---|---|
| Pistol | short slide, no stock, one-hand dominant profile |
| SMG | compact receiver, short barrel, compact stock |
| Assault Rifle | balanced full stock, standard barrel/magazine |
| Shotgun | long tube/fore-end, pump silhouette |
| DMR | long barrel, full stock, medium optic |
| Sniper Rifle | longest barrel/stock, large optic/bipod visual |

Every weapon asset exports the same named points:

```text
GripPrimary (required)
GripSupport (required except pistol)
StockShoulder
Muzzle (required)
MagazineCenter
OpticRailStart / OpticRailEnd
EjectionPort
```

### MK visual language

- **MK1:** plain factory finish, minimal rail silhouette.
- **MK2:** muted two-material finish, compact optic treatment, refined stock/handguard.
- **MK3:** premium two-tone/club accent, stronger edge/material contrast and visual-only detail.

MK visuals may show attachments only as a baked presentation treatment corresponding to existing stats. They must not imply selectable scopes, suppressors or magazines as new gameplay systems.

### Export LODs

- `Icon`: 128×64, pure family silhouette.
- `Card`: 512×192, shop/loadout view.
- `Combat`: 512×128 or canonical rig source, transparent.
- `Inspect`: 1024×384, optional high-detail store view.

Naming:

```text
wpn_{family}_{model}_mk{1|2|3}_{lod}_{variant}.png
chr_{body}_{part}_{style}_{palette}_{lod}.png
```

## 7. Low-poly material and texture rules

- Prefer authored geometry/color planes over painted detail.
- Use 3–6 tonal planes per material: base, light, shadow, edge, optional accent/AO.
- No noisy photo textures. Fabric weave and scratches are visible only at Inspect LOD and must disappear cleanly at Combat LOD.
- Separate materials visually: polymer = broad matte plane; metal = narrow cool highlight; fabric = softer value transition; glass = restrained cyan/grey reflection.
- Bake consistent studio lighting: warm key, cool fill, neutral rim. All catalog assets use the same rig.
- Team identity uses patches/armbands and UI side markers, not full-body recoloring.

## 8. Unity implementation

Recommended package path after production art begins:

- `com.unity.2d.animation` compatible with Unity 6000.3 LTS;
- one Character Prefab per body family;
- Sprite Library categories for Body, Head, Hair, FaceProtection, Helmet, Uniform, Camo, Armor and Weapon;
- Sprite Resolver per swappable renderer;
- one shared Animator Controller;
- data-driven AppearanceDefinition ScriptableObjects;
- editor validator for labels, skeleton IDs, sockets, canvas bounds, pivots and missing LODs.

The current `Male017UiView` remains a prototype bridge only. Production integration should retain one composed character per visible fighter rather than rebuilding all images every frame.

## 9. Required automated validation

- All appearance combinations instantiate without missing sprites.
- Male/female hand and shoulder sockets remain within tolerance for every weapon.
- Muzzle lies outside the weapon body and points toward facing direction.
- No layer exceeds the declared bound at roster, card and 16v16 scales.
- Every mechanical catalog item maps to a truthful visual label.
- Every weapon family passes a 64 px silhouette identification sheet.
- Club/team identity remains distinguishable in grayscale and simulated color-vision modes.
- Sprite swap compatibility checks a shared skeleton identifier.[1][2]

## 10. Approval gates before mass production

1. Approve one male and one female base in body-only, equipped and battle scales.
2. Approve one full weapon family sheet across six families.
3. Approve MK1/MK2/MK3 material language.
4. Approve club-color patch/armband behavior.
5. Verify 1v1, 8v8 and 16v16 composition.
6. Only then generate/commission the full catalog.

This order prevents repainting an entire inventory after discovering incompatible hands, anchors, silhouettes or MK language.

## Sources

[1] Unity, “Sprite Swap overview”: https://docs.unity3d.com/Packages/com.unity.2d.animation@12.0/manual/SpriteSwapIntro.html

[2] Unity, “Sprite Swap”: https://docs.unity.cn/Packages/com.unity.2d.animation@9.0/manual/SpriteSwapLanding.html

[3] Unity, “Sprite Library Asset”: https://docs.unity3d.com/Packages/com.unity.2d.animation@4.1/manual/SLAsset.html
