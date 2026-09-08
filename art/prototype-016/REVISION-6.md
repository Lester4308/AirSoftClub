# Revision 6 — shoulder correction

Built-in image_gen edited combat-body-v2.png into assets/combat-body-v3.png. The hollow viewer-left shoulder socket is replaced with closed camouflage cloth behind the animated arm. The generated figure shifted horizontally; rendering offsets it by -32 source pixels to preserve the established joints and feet placement. Small texture changes from generation remain visible. No source PNG was edited programmatically.

Added subtle procedural contact shadows below combat boots. Existing grip and pocket corrections remain active.

## Prompts and source

1. Edit this exact modular game sprite. Make ONLY this local correction: replace the dark hollow circular arm socket on the viewer-left shoulder with a closed, natural rounded camouflage cloth shoulder cap matching the uniform. Remove the thick rolled rim of the hole. This is a torso layer used behind separately animated arms: do NOT add arms or hands. Preserve exact original 1024x1536 canvas, character pixel position, pose, silhouette elsewhere, helmet, face, vest, legs and boots. No redesign, no new objects, no pistol or holster. Keep the soft low-poly faceted style and existing colors. Transparent background, genuine alpha outside the character, no glow or shadow background.

First output had a baked checkerboard and was not adopted. Original: C:/Users/Ihor/.codex/generated_images/01a07b64-1b96-7472-92b6-61342fdf6f7c/exec-178d17c3-30fd-4c8a-80df-c484123bbc0b.png.

2. Remove the checkerboard background. Transparent background. Preserve the character exactly.

Adopted source: C:/Users/Ihor/.codex/generated_images/01a07b64-1b96-7472-92b6-61342fdf6f7c/exec-54410abf-cc68-48e2-a239-7ab535a32fe3.png.

## Verification

PNG 1024x1536; sampled corner alpha = 0. Browser inspected Rifle settled on graphite and SMG at 90 ms on light. The shoulder opening no longer shows in the assembled character. No browser error logs; node --check preview.js passed. No new gameplay changes. Combat gear remains baked into the torso image; this revision does not claim to separate equipment slots or finish production art.
