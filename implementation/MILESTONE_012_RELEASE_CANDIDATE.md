# Milestone 012 — local release-candidate review

Date: 2026-09-09. **LOCAL RELEASE-CANDIDATE PASS WITH EXPLICIT EXTERNAL/ART GATES.**

This milestone closes the approved local development sequence through 012. It does not claim Steam publication, live commerce, production hosting, final-art approval or legal clearance.

## Product and UX completion

- The playable client uses the modern UGUI/Canvas presentation with responsive 1600×900 reference scaling.
- Idle screens are retained until visible state changes instead of rebuilding the hierarchy every frame; keyboard selection/focus remains stable between idle frames.
- Buttons expose highlighted, pressed and disabled states; server transactions disable actions and show authoritative busy/error/retry feedback.
- Credits conversion, early access, premium purchases and shields require explicit confirmation with exact cost and consequences.
- Daily UI shows UTC reset/claimed state. Shield UI explains its cancellation conditions.
- Long recruitment, shop, ammunition, opponent and history lists are clipped and scrollable.
- Male017 layered prototype sprites represent fighter, camouflage, head protection, vest and weapon visibility in roster/recruitment/shop/battle.
- Dense battle presentation uses stronger HP bars and redundant side markers. Replay remains presentation-only.

## Final balance evidence

See `FINAL_BALANCE_012.md` and `evidence/012/final-balance-012.json`.

- Starter 60 BB: 5.476 measured equivalent battles, inside the approved 5–7 target.
- Free zero-to-readiness: configured 10 minutes; measured post-battle mean 5.995 minutes.
- Economy: 100 accounts / 2,000 battles; no negative Money/BB and no premium spending.
- Premium: user-approved intentional advantage preserved; measured peak full-stack ratio 1.216789× at Level 2.
- Asymmetric rosters: 2,500 battles across 25 size pairs; all terminate, but large roster differences are decisively one-sided and must not be presented as balanced.
- No balance constants changed from this audit alone.

## Art acceptance boundary

See `ART_PRESENTATION_AUDIT_012.md`.

Current presentation is a coherent prototype, not final art. Open gates include:

- female fighter family;
- truthful weapon-family and MK1/MK2/MK3 visuals;
- character/face diversity;
- authored animation, final VFX/audio and environment art;
- licensing/rights review and explicit final-art approval;
- complete localization and accessibility review.

## Fresh local verification

- Release build: 0 warnings, 0 errors.
- Battle harness: **46/46**.
- Club harness: **37/37**.
- PostgreSQL harness: **27/27** (latest Milestone 011/commerce run).
- Unity EditMode: **15/15**.
- Unity PlayMode: **7/7**.
- Windows Mono build: **PASS**.
- Windows IL2CPP build and native run: **PASS**.
- Native golden digest: `22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f`.
- Final package: `Artifacts/AirsoftClub-Development-Windows-x64.zip`.
- Package SHA-256 from Milestone 011: `0061C6184BAD03665BC185C860A491DE607198357C40D5BCE48DEF4E7BD5CBEF` (must be regenerated after this commit for a final delivery hash).

## Gates not closed locally

1. Real Steam AppID, publisher key, entitled sandbox accounts and partner configuration.
2. Live payment provider, final prices/SKUs and spent-balance refund/debt policy.
3. Production moderator authorization provisioning.
4. Hosting, load/security review, monitoring and production deployment.
5. Final art/legal approval, localization/accessibility approval and final commercial balance approval.

The repository is ready for continued product/art work and external integration without representing any of the above as completed.
