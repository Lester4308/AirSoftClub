# Revision 2 generation prompts

Built-in image_gen; project-local originals inspected before editing. Source images remain unchanged. Selected generated outputs copied into assets/ as versioned PNGs. No external project assets used.

## helmet-shell-v2.png

Source: assets/helmet.png. Prompt: Edit this isolated tactical helmet sprite for paper-doll compositing. Keep the exact helmet shell, rails, mount and outer size/position unchanged on same canvas. REMOVE all black interior padding, all chin straps, all hanging straps, all buckles below the rim. Keep only the outer hard helmet shell including its ear-side protection. Under the front rim everything must be truly transparent so a separate character's eyes and goggles will be visible beneath. Actual transparent PNG background. No mannequin, no head, no new elements. Only remove interior and hanging straps.

Follow-up on generated shell: Remove the background. Transparent background.

## camouflage-v2.png

Source: rejected/camouflage-opaque.png. Prompt: Remove the background. Transparent background.

Generator changed scale and garment details; explicit roster placement is stored in rig.js. This is not a pixel-identical extraction.

## combat-body-v2.png

Source: assets/fire-pose.png. Prompt: Game animation layer extraction. From this male fighter KEEP ONLY head with helmet/face protection, torso with plate carrier, waist, both legs and boots. REMOVE the entire rifle and BOTH arms from shoulder to fingers, including all hands, gloves, sleeves and elbows, replacing removed areas with transparency outside torso and reconstructing the chest armor where gun/arms hid it. Arms will be separate animated sprites. Preserve original body silhouette, exact pose, location and size on 1024x1536 canvas. Do not recenter. Transparent background. No new equipment, no holster. This is an armless body layer for a 2D cutout animation rig; no injury, no gore, clean garment shoulder openings.

## arm-front-v2.png

Source: assets/fire-pose.png. Prompt: Extract ONLY the foreground shooting arm (the large bent sleeve visible on viewer-left, from shoulder around x210 y290 down to elbow x190 y460 and up to the trigger hand at x460 y320). Keep that one camouflage sleeve plus tan gloved hand as one continuous bent arm sprite, same firing pose, same soft low-poly style. Remove everything else: no person, no head, no torso, no other arm, NO rifle, no background. Reconstruct glove fingers that were hidden by gun, loosely curled as gripping pistol grip. Isolated bent sleeve-and-glove cutout on transparent background. This is a clean game animation piece, not injury or gore. Generous padding around sprite.

Follow-up: Remove the background. Transparent background.

## arm-rear-v2.png

Source: assets/fire-pose.png. Prompt: Isolated background support arm from this male fighter, the arm on viewer-right supporting rifle fore-end: camouflage upper sleeve begins near shoulder x480 y300, descends to elbow x585 y435 then forearm rises to tan glove at x675 y290. Extract only this one bent sleeve-and-glove arm. Palm faces up holding imaginary fore-end, fingers curled over top. No rifle, no other arm, no body, no torso, no head, no backdrop. Transparent background. Preserve low-poly olive/tan style and angular pose. Game animation sprite part, no injury or gore.

## rifle-v2.png

Source: assets/fire-pose.png. Prompt: Extract only the complete airsoft assault rifle from this image, reconstructing parts hidden by hands. Isolated side view pointing exactly horizontally RIGHT, full tan stock at left, black receiver and curved magazine, tan ventilated fore-end, short optic, black barrel and suppressor at right. Preserve original realistic shape and stylized low-poly matte olive/tan/black game shading. No hands, no arms, no person, no sling. Transparent background. Entire gun visible with padding. Game weapon sprite to attach to animated hands.

## smg-v2.png

Source: assets/rifle-v2.png. Prompt: Create a compact airsoft SMG game sprite in the exact same grounded matte soft low-poly art style, same left side view pointing horizontally right, isolated transparent background. Different weapon family from reference: compact black receiver, short olive/tan handguard, straight slender magazine, compact adjustable stock, small simple reflex sight and short muzzle device. Realistic airsoft shape, no sci-fi. No hands, no people, no sling. Full weapon visible. This is a standalone interchangeable weapon sprite; preserve style only, do not copy rifle proportions. Transparent background.
