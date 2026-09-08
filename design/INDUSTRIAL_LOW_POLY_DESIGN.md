---
version: alpha
name: Airsoft Club Industrial Low-Poly
description: Industrial clubhouse management UI with high-salience actions and modular low-poly fighters.
colors:
  primary: "#151713"
  surface: "#242720"
  surfaceRaised: "#30352C"
  surfaceInteractive: "#3A4034"
  border: "#5A604F"
  textPrimary: "#F3EBDD"
  textSecondary: "#B8B1A3"
  action: "#F28C28"
  actionHover: "#FFAA45"
  info: "#5CC8D7"
  success: "#8FCB6B"
  warning: "#F1C453"
  danger: "#E35D4F"
  credits: "#D9B8FF"
  money: "#F3C760"
  teamFriendly: "#63BDD0"
  teamEnemy: "#F07855"
typography:
  display:
    fontFamily: Bahnschrift Condensed
    fontSize: 36px
    fontWeight: 700
    lineHeight: 1.05
    letterSpacing: "0.04em"
  heading:
    fontFamily: Bahnschrift
    fontSize: 22px
    fontWeight: 700
    lineHeight: 1.15
  body:
    fontFamily: Segoe UI
    fontSize: 16px
    fontWeight: 400
    lineHeight: 1.45
  data:
    fontFamily: Cascadia Mono
    fontSize: 14px
    fontWeight: 600
    lineHeight: 1.2
rounded:
  sm: 2px
  md: 4px
  lg: 6px
spacing:
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 32px
components:
  button-primary:
    backgroundColor: "{colors.action}"
    textColor: "{colors.primary}"
    rounded: "{rounded.sm}"
    padding: 14px
  button-primary-hover:
    backgroundColor: "{colors.actionHover}"
    textColor: "{colors.primary}"
  button-secondary:
    backgroundColor: "{colors.surfaceInteractive}"
    textColor: "{colors.textPrimary}"
    rounded: "{rounded.sm}"
    padding: 12px
  button-danger:
    backgroundColor: "{colors.danger}"
    textColor: "{colors.primary}"
    rounded: "{rounded.sm}"
    padding: 12px
  card:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.textPrimary}"
    rounded: "{rounded.md}"
    padding: 16px
  info-panel:
    backgroundColor: "{colors.surfaceRaised}"
    textColor: "{colors.info}"
    rounded: "{rounded.md}"
    padding: 16px
---

> **Status update:** retained for UI color, typography, and Industrial Clubhouse environment principles. The final character/weapon art direction is superseded by `REALISTIC_STYLIZED_3D_ART_DIRECTION_016.md`; visibly primitive low-poly people and weapons are not a production target.

## Overview

Industrial Clubhouse is an **Operate / Explore** game surface. The club is a converted warehouse: lockers, workbench, map table, supply cages and trophy walls. Low-poly art uses clear geometric planes and controlled material colors rather than painterly brushwork or photorealistic texture noise.

The information system must not blend into one brown-grey mass. Neutral materials establish the place; state colors establish meaning. Orange is reserved for the single primary action. Cyan is information and navigation. Green is readiness/success. Yellow is warning/progression. Red is damage, destructive action and danger. Purple is Credits. Gold is Money/reward.

## Colors

- Backgrounds use `primary`, `surface` and `surfaceRaised`; adjacent layers must differ in luminance, not only hue.
- Primary actions use `action` and dark text. Never place two orange primary buttons in one decision region.
- Credits always use `credits`; Money uses `money`; BB information uses `info` or the approved tier color.
- Friendly/enemy teams pair color with direction, side marker and emblem. Never rely on color alone.
- Body copy uses `textPrimary`; metadata uses `textSecondary`. Decorative text cannot be lower contrast than required controls.

## Typography

Industrial condensed type is used for screen titles, club signage and results. Neutral sans is used for readable copy. Monospace is limited to resource amounts, rating, timers, weapon stats and event logs. Uppercase is reserved for short labels and statuses, not paragraphs.

## Layout

- 1600×900 reference canvas with 8 px rhythm.
- Persistent top resource bar and left navigation remain functional chrome.
- The central surface should be one scene or one selected object, not a grid of equal cards.
- Contextual actions live beside the object they affect. A bottom action strip may contain one primary and up to three secondary actions.
- Character customization uses three columns: category rail, full-body preview, item/options inspector.

## Elevation & Depth

Depth is created with value steps, thin borders and local warehouse lighting. Avoid glass blur, blue-purple gradients and heavy drop shadows. Background props are darker/desaturated. Interactive panels are cleaner and brighter. Selected objects receive a focused local light plus a 2 px status edge.

## Shapes

- Corners are 2–6 px with one optional clipped corner on large command panels.
- Buttons are rectangular and tool-like; no pill controls.
- Equipment slots visually echo locker labels and workbench trays.
- Icons use solid low-poly silhouettes with one internal cut.

## Components

- **Primary button:** orange, minimum 48 px, dark bold label, optional shortcut hint.
- **Secondary button:** raised graphite, light label, cyan focus outline.
- **Critical button:** red, used only for dismissal/destructive confirmation.
- **Info panel:** raised neutral surface; important values highlighted with semantic colors.
- **Resource chip:** icon + label + tabular value; distinct token color and no full-card tint.
- **Fighter card:** low-poly portrait, callsign, readiness strip, level and three stats; selection uses light/edge, not a complete color flood.
- **Weapon card:** truthful silhouette, family, MK tier, damage/tempo role and ownership/access state.

## Do's and Don'ts

### Do

- Keep low-poly geometry crisp: 3–6 value planes per material, deliberate silhouette edges, restrained ambient occlusion.
- Preserve one shared skeleton/anchor contract for compatible character parts.
- Author male and female bases against the same named equipment anchors.
- Keep every weapon family recognizable at 64 px silhouette scale.
- Validate customization combinations automatically for clipping and missing anchors.

### Don't

- Do not bake equipment into the body base.
- Do not use one rifle image to represent all weapon families in final art.
- Do not tint the entire UI or fighter by rarity/team.
- Do not add painterly noise, fake scratches everywhere, gradients, bloom or camouflage behind text.
- Do not let premium MK presentation imply new gameplay attachments that do not exist.
