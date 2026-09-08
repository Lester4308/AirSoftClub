# Image provenance

Mode: built-in image_gen. Reference: user image codex-clipboard-1809ce14-8cc4-4e22-a9fd-13901cec54d0.png. No Project Airsoft content used.

Atlas prompt: production cutout atlas for the male reference, exact simple soft low-poly style and enlarged head proportions. Requested 4 columns x 3 rows: olive and camouflage armless/headless bodies, bare and fully equipped heads; right shooting arm olive/camo, left support arm olive/camo; standalone vest and rifle, two empty cells. Transparent background, no labels. No increased realism, noisy textures, holsters, pistols or visible left sleeve pocket. Identical body silhouette requested. Actual output layout and dimensions were inspected and explicit rectangles used instead of assuming a grid.

Extraction prompt: Remove the checkerboard background. Transparent background. Preserve all ten sprite parts exactly in their current positions, sizes and layout. No other changes.

Atlas initial: exec-788f19a6-2cc9-49ba-a472-4007b9c071fa.png. Alpha atlas: exec-92de48db-184e-4a74-8833-52a9fcf8f063.png.

Left-arm correction prompted after user identified wrong handedness and invisible supporting arm:
Generate one anatomically LEFT SUPPORT ARM, matching low-poly camouflage sleeve/tan glove; shoulder upper-left, elbow down-right, forearm rises to cupped upward left palm. No body, weapon, head, sleeve pocket or patch. Transparent background.
Initial exec-3a363d87-cf16-47e1-baac-b563e9972573.png rejected for wrong thumb side.

Correction prompt: Correct ONLY the glove's handedness: it must be a LEFT hand seen from its PALM side. The THUMB must be on the viewer-RIGHT side of the palm, not on the left. Remove the existing large thumb on the left edge, put the curled pinky there, and place a proper opposable thumb along the right edge. Four curled fingers plus one thumb total. Keep wrist position, shoulder, sleeve, elbow and arm silhouette exactly unchanged. No mirroring the arm. Transparent background.
Adopted camo: exec-651787b3-ad5d-4e70-9416-83dfabedf1d4.png.

Olive prompt: Change only the sleeve fabric color from camouflage to plain muted olive green, preserving the low-poly facets. Keep the entire left arm pose, silhouette, left palm, thumb on viewer-right, tan glove, shoulder and canvas exactly identical. Transparent background. No pocket or patch.
Adopted olive: exec-3965d55d-83cd-49c1-82df-d44c5017dff0.png. Generator changed canvas size; separate source anchors compensate.

All original outputs are under C:/Users/Ihor/.codex/generated_images/01a07b64-1b96-7472-92b6-61342fdf6f7c/. Adopted images copied into this pack. Atlas slicing is lossless pixel cropping only; no programmatic repainting or recoloring.
