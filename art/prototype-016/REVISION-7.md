# Revision 7 — aiming pose range

Added three preview aim positions: -8, 0 and +8 degrees. Weapons rotate around the buttstock contact region at body-space (290,350). Both grips follow the same weapon matrix, driving the existing fixed-length arm rig. Recoil and BB travel follow the selected barrel direction. This is presentation only, with no gameplay targeting changes.

Verification: node verify-rig.cjs passed 1530 samples across both weapons, three aim angles, three times and recoil phases; all configured targets reachable, bone lengths unchanged. node --check preview.js passed. Browser inspected raised Rifle and lowered SMG with close-up; no obvious detached wrist or open shoulder seam at the inspected scale. Console error log empty. Server restarted after connection refusal. No raster assets changed.

Limits: only a small aiming range is supported. No claim of unrestricted anatomical motion or final art approval. Combat equipment still baked into body texture.
