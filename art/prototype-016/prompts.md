# Built-in image_gen prompt record

All calls used the built-in image_gen tool. The approved reference was inspected before edits. Originals retained, copies used non-destructively.

## male-base.png

Initial prompt: Production art test: ONE male base sprite isolated on genuinely transparent RGBA background. Use approved first male in reference for exact adult face, enlarged head/body proportions, olive uniform, tan boots and soft low-poly shading. Full body, portrait 1024x1536 canvas, generous empty padding, feet baseline at 94 percent height, centered around x=46 percent. SAME relaxed three-quarter facing right as reference, hands relaxed down at sides like the reference first male; this is roster idle pose. No gear, no gun, no helmet, no mask, no armor, no thigh holster. Keep low-poly matte broad facets and detail from approved reference, not painterly. All hair and both boots fully visible. No ground shadow, no background, no text. Clean transparent edges. This base will receive independently generated clothing and equipment overlays so do not include equipment.

Final extraction prompt: Use case: background-extraction. Keep this exact male character unchanged with exact canvas 1024x1536, position, size, pose and colors. Remove ALL the soft brown haze, glow, gradient, shadow and background surrounding the figure. Output a clean game sprite cutout with truly fully transparent pixels everywhere outside character silhouette, including the space between legs and around head. No drop shadow, no outer glow, no ambient background, no ground. Preserve sharp anti-aliased character edges. Do not restyle or change character. The only opaque pixels should belong to the physical body, clothes, hair, hands, boots.

## armor.png

Create ONLY a tan tactical plate carrier overlay to fit exact male body in image 1. Full unchanged 1024x1536 canvas, TRANSPARENT RGBA. Position carrier on upper torso in same location it would occupy worn by image1: shoulder straps around y350-440, vest chest/pouches y420-670, waist ends above original belt y690. Center around x495, approximate outer width 300px. Shape in three-quarter view facing right and fitted to image1 shoulder and ribcage. Use approved equipped male plate carrier design and soft low-poly style from image2. Only render the carrier and its shoulder straps, front and side magazine pouches; no person, no shirt, no head, no hands, no legs, no background, no mannequin. Empty spaces between straps truly transparent. No leg equipment. Do not enlarge or center the vest on the full canvas: it is a positioned paper-doll overlay matching image1's torso 1:1.

Result did not obey scale/position: viewer uses explicit fitted display transform.

## helmet.png

Isolated game equipment sprite on genuine transparent RGBA background: ONLY the tactical helmet from equipped male in reference, same soft low-poly olive/tan faceted military helmet with side rails and straps, three-quarter view facing right matching reference male. No head, no hair, no face, no mannequin. Helmet brim and side protection complete, empty interior truly transparent wherever skin would appear. Tight framed standalone helmet, 1024x1024 square with 15 percent padding. Same approved game-like stylized shading, no photoreal fabric. No text, no checkerboard pattern, no ground or shadow. Actual alpha transparency outside helmet. This will be scaled onto separate male head.

Actual output is 1254x1254; original retained.

## face-protection.png

Create isolated airsoft face protection sprite, only goggles and lower-face mesh mask as one wearable assembly in THREE QUARTER VIEW FACING RIGHT to fit approved male face in reference. Same olive/tan soft low-poly style. No head, no face, no skin, no helmet, no mannequin. Goggles lenses mildly smoky translucent, separated goggles and mesh lower mask positioned as worn together. Include thin side straps only. Actual transparent RGBA background, not an opaque checkerboard. No text. Centered equipment sprite with padding. Same camera angle as male reference. This is a game overlay for separate character head; empty eye openings must allow base eyes to remain visible.

## fire-pose.png

Initial prompt: Generate ONE full body male airsoft fighter in firing-ready pose, genuine transparent RGBA background no checkerboard. Use exact approved equipped male style and costume of image1, and same face/head-to-body ratio as image2. Portrait 1024x1536, boots baseline at y1480, all boots and rifle muzzle visible with generous side padding. Legs planted as image2, upper body THREE QUARTER SIDE facing right. Both arms raised naturally, right hand on pistol grip trigger, left hand supporting fore-end, realistic airsoft assault rifle shouldered and aimed horizontally to RIGHT. Wear olive/tan camo, tan plate carrier, protective helmet, goggles, mesh mask and tan boots. NO thigh holster, NO pistol. Preserve soft faceted low-poly detailed game style, not photoreal. No muzzle flash or projectiles in asset, no ground shadow, no backdrop. This is a single firing-pose reference sprite for motion feasibility test, not a sprite sheet.

Final extraction prompt: Extract this character as an isolated transparent-background PNG cutout. Erase everything outside the character and gun silhouette into actual alpha transparency. Preserve the soldier, gun, entire body, pose, pixels and original canvas. Deliver RGBA PNG with transparent background. No design changes.

## rejected/camouflage-opaque.png

Matching clothing-only overlay requested from base; extraction retried. Final prompt: Extract the clothing as an isolated transparent-background PNG cutout. Erase everything outside the jacket and trousers silhouette into actual alpha transparency, including gaps between arms and torso and between legs. Preserve clothing, pose, original size and position on canvas. Deliver RGBA PNG with transparent background. No design changes.

Result failed alpha and changed collar position; not loaded by viewer and not production approved.
