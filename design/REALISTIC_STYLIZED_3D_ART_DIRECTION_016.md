# AIRSOFT CLUB — REALISTIC-STYLIZED 3D ART DIRECTION 016

**Status:** production target  
**Supersedes:** final 2D low-poly character/weapon direction in `INDUSTRIAL_LOW_POLY_DESIGN.md` and `MODULAR_CHARACTER_WEAPON_ART_CONTRACT_013.md`  
**Reference policy:** user images define quality, material response, density, and presentation only. Do not copy identities, layouts, proprietary marks, costumes, or weapon designs.

## 1. Target

Airsoft Club uses **modern sporting-tactical realistic stylization**:

- believable anatomy, posture, grip, equipment construction, and weapon mechanics;
- selective simplification and controlled detail, not visible low-poly triangles;
- stylized PBR response with separate fabric, Cordura/webbing, polymer, painted metal, rubber, glass, skin, and hair;
- club/sport identity through protective eyewear, face protection, colored team tape, patches, casual layers, and clean used condition;
- no blood, military-fantasy exaggeration, or clone-army presentation.

The word “low-poly” now describes **performance topology and clean faceted macro-shape**, not the visible finish. Faces, skin, hands, fabric, round barrels, optics, and cylinders must not look polygonized.

## 2. Production decision: 3D-first hybrid

The source of truth is modular real-time 3D. Unity uses the models directly for inspect, roster, shop, and battle. Portraits, cards, icons, and marketing stills are rendered automatically from the same approved assets.

Do not mass-produce final 2D cutout parts. Sprite Resolver remains a temporary fallback until the 3D vertical slice passes all gates.

### Benefits

- one item fits all approved poses and cameras;
- realistic overlap among body, carrier, pouches, arms, sling, stock, and weapon;
- male/female fitted meshes share animation while retaining distinct anatomy;
- one weapon visual is identical in Shop and Battle;
- LODs remove detail without redrawing combinations;
- catalog imagery cannot drift from gameplay assets.

## 3. Character art bar

### Anatomy and proportion

- baseline height: **7.25–7.75 heads**;
- presentation LOD may emphasize head/hands/boots by no more than **5–8%**;
- realistic clavicle, shoulder, elbow, wrist, pelvis, knee, and ankle placement;
- natural asymmetry and an actual center of gravity;
- neutral stance is relaxed-ready, never a lowered T-pose;
- Aim stance must allow stock-to-shoulder contact and an achievable sight line;
- fingers wrap grips and handguards rather than intersecting or floating.

### Male base

- athletic but ordinary enthusiast; no universal superhero V-shape;
- face retains skull structure under masks and straps;
- equipment mass changes the silhouette but does not replace anatomy.

### Female base

- equal quality, not a scaled male mesh and not sexualized;
- separately fitted torso, pelvis, shoulders, neck, hands, carrier, belt, and straps;
- practical hair/helmet compatibility;
- competent stance and weapon grip;
- no doll face, heeled footwear, exposed skin, or fantasy armor.

### Constructive equipment

Every visible part must answer how it exists and attaches:

- carrier has wall thickness and inner clearance;
- pouch has flap/zip/retention and MOLLE or belt attachment;
- webbing has tension and contact points;
- buckles and patches rest on surfaces;
- cloth compresses beneath straps;
- rigid armor preserves shape;
- helmet and eye/face protection contact head anatomy;
- body hide masks prevent clipping beneath garments.

## 4. Weapon art bar

Each family is mechanically plausible and silhouette-distinct:

1. Pistol
2. SMG
3. Assault Rifle
4. Shotgun
5. DMR
6. Sniper Rifle

### Required readable nodes

Receiver, handguard, barrel, muzzle, trigger/guard, grip, magazine and well, stock/brace, rail/slots, sling points, optic mount, lens, and class-specific action.

### Materials

- anodized/painted metal: controlled cold highlight and contact-edge wear;
- reinforced polymer: broad matte highlight;
- rubber: dark but not crushed black;
- optic glass: restrained reflection, no permanent cyan glow;
- steel controls: tighter metallic response;
- sling: woven response and physical attachment.

Wear is causal: grip zones, magazine well, rail contact, stock, sling points, and recess dust. No uniformly sprayed scratches or grunge.

### Originality and airsoft identity

Create original product families by altering receiver proportions, handguard language, stock, muzzle device, rail layout, magazine form, panels/cutouts, and accessory package while retaining functional logic. Use invented manufacturers and markings. Region-specific safety-tip treatment must be configurable. Airsoft-specific battery/gas/hop-up cues are shown only where appropriate and visible.

### MK progression

- **MK1:** utilitarian body, simple sighting, basic materials;
- **MK2:** refined furniture and ergonomic silhouette changes;
- **MK3:** premium machining/material breakup and presentation accessories.

Tiers must not imply unattached gameplay mechanics. `visualId` remains the single Shop/Battle identity.

## 5. Materials and lighting

### Stylized PBR library

- Skin: restrained micro-normal, warm subsurface response, non-waxy roughness variation.
- Woven uniform: broad soft response, multi-scale folds, stable camo scale.
- Cordura/webbing: coarser roughness, readable edges and stitches.
- Hard armor/polymer: broad matte lobe, molded edge treatment.
- Painted metal: narrow highlight with sparse edge wear.
- Rubber: soft response above absolute black.
- Glass: correct lens volume and controlled reflection.
- Hair: cards or geometry with low transparency cost; helmet-safe variants.

Use tileable fabric, trim sheets for webbing/hardware, color/camo masks, decal atlas for patches and original markings, and packed PBR maps. Avoid photographic noise as a substitute for form.

### Catalog rig

- perspective camera equivalent to **50–70 mm**;
- warm soft key, weaker cool fill, neutral rim;
- controlled AO/contact shadow;
- fixed exposure, LUT, and background across the catalog;
- three-quarter pose plus front/side validation views.

### Battle rig

- fixed near-orthographic gameplay angle;
- baked/environment fill plus one principal directional light;
- modest post-processing, no persistent bloom or colored fog;
- team identity via patches/tape and UI markers—not full-body neon tint.

## 6. Modular technical contract

```text
CharacterRoot
├── SharedHumanoidRig
├── Body_Fitted_Male | Body_Fitted_Female
├── Head
├── Hair
├── Uniform_Fitted
├── HeadProtection
├── FaceProtection
├── Carrier_Fitted
├── PouchSet
├── Gloves
├── Boots
├── ClubPatchDecals
└── WeaponRoot
```

Rules:

- one skeleton namespace, bind pose, scale, and coordinate convention;
- male/female-specific fitted meshes on the shared humanoid rig;
- common animation set, with small body-type corrective poses when required;
- large clothes/carriers use fitted variants and body hide masks;
- rigid equipment uses documented sockets;
- left-hand IK targets the chosen weapon;
- every weapon exposes `GripPrimary`, `GripSupport`, `StockShoulder`, `Muzzle`, `Magazine`, and `OpticRail`;
- sling is optional for the first slice because believable sling simulation is a separate gate;
- the legacy 2D rig ABI is retained only as fallback/migration evidence.

## 7. LOD and budget targets

These are starting budgets, validated on target hardware:

| Asset view | Character triangles | Purpose |
|---|---:|---|
| LOD0 | 60k–100k equipped | inspect / catalog render |
| LOD1 | 30k–50k | roster / recruitment |
| LOD2 | 15k–25k | near battle |
| LOD3 | 6k–12k | 16v16 |

- main character and hero weapon: up to 2K authored maps;
- minor equipment: 1K where appropriate;
- atlas battle materials and reduce skinned renderer count;
- no transparent micro-straps or hair noise in LOD3;
- preserve head, hands, weapon, helmet, carrier, and role silhouette through LOD transitions;
- profile a 32-character stress scene before catalog production.

## 8. Vertical slice scope

Build only this before scaling:

- one original male enthusiast;
- one original female enthusiast;
- one fitted uniform/casual-tactical set each;
- one helmet, protective eyewear/face option, carrier, pouch set, gloves, boots;
- one hero-quality original AR-style airsoft replica;
- black silhouette blockouts for all six weapon families;
- neutral, roster, aim, fire, hit, victory/defeat poses;
- inspect/roster/shop RenderTexture presentation;
- battle validation at 1v1, 8v8, and 16v16;
- automatic portrait and icon render.

## 9. Acceptance gates

### A — silhouette

- body type, build, role, helmet, carrier, and weapon separate in black fill;
- male and female are not one scaled mesh;
- all six weapon families are recognized at 96×48;
- hero weapon remains legible at 64×64 icon size.

### B — anatomy and pose

- front/side/three-quarter review passes;
- center of gravity is believable;
- stock contacts shoulder;
- head can align to optic;
- wrist/elbow/shoulder angles are achievable;
- no T-pose residue or broken knee/pelvis twist.

### C — construction and clipping

- every pouch/strap/buckle has an attachment path;
- cloth compresses, rigid gear holds shape;
- no floating or intersecting patches, optics, magazines, fingers, stocks, hair, or straps;
- approved Male/Female × helmet × carrier × AR combinations pass Aim and Hit poses.

### D — close-up finish

- faces retain anatomy and natural asymmetry;
- eyes, lips, ears, nostrils, and eyelids survive roster framing;
- male/female faces have equal fidelity;
- barrels/optics/cylinders are not visibly faceted;
- metal, polymer, rubber, glass, skin, and fabric separate in grayscale roughness review.

### E — game view

- 32 units maintain role/team/weapon readability;
- no dense microdetail shimmer;
- LOD changes do not pop the silhouette;
- measured frame time and allocations meet project targets;
- distant Animator update reduction does not damage important reactions.

### F — pipeline

- same approved assets appear in Shop and Battle;
- portrait/icon batch renderer is deterministic;
- Unity validation scene rejects missing sockets/material roles/LODs;
- no catalog expansion until A–F pass.

## 10. Immediate implementation sequence

1. Install and lock Blender LTS/tool version; record exporter settings.
2. Select a legally safe base-mesh strategy; store license/provenance with source assets.
3. Produce male/female anatomy blockouts and six black weapon silhouettes.
4. Integrate blockouts into Unity with shared rig, cameras, and RenderTextures.
5. Validate actual screen framing and 32-unit readability.
6. Finish one outfit/carrier/helmet and one AR to hero quality.
7. Approve anatomy, face style, material response, camera, lighting, and performance.
8. Only then build remaining weapon families, MK tiers, outfits, and cosmetics.

## 11. Hard rejection criteria

Reject the slice if any hero object appears complete only from afar but breaks down in roster/shop view into:

- visible primitive planes or faceted cylinders;
- one plastic material across all surfaces;
- structurally impossible equipment;
- implausible hand/weapon contact;
- a scaled male presented as the female base;
- copied reference identity or trademark-dependent design;
- mismatched Shop and Battle representations.
