# Airsoft_Club_Game — agent rules

Read and obey [PROJECT_BOUNDARY.md](PROJECT_BOUNDARY.md). This is an independent game. Do not modify, continue, import or inherit Project Airsoft / Airsoft Manager, its code, assets, rules, architecture or Git history. No reuse is adopted in [REUSE_DECISIONS.md](design/REUSE_DECISIONS.md); a proposal is not authorization to copy.

## Current canonical documentation — 2026-09-06

Start with [Master Development Spec v1](AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md) and [Agent Implementation Pack v1](AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). The master consolidates Gate v3, Implementation 001–002 and the user's subsequent Q01–Q15 approvals. All Q01–Q15 are CLOSED as decisions; balance values and residual implementation details remain explicitly distinguished. Earlier OPEN labels, milestone-specific STOP instructions and native IL2CPP blockers in historical documents do not override this consolidation or a later explicit user request.

Hard rules: max16 automatic ready roster, asymmetry, one battle/round; exactly Accuracy/Endurance/Agility; persistent offensive HP, full-HP isolated defense; no Energy/crit/injury latch; four slots; shared club BB and one tier; defense virtual budget100%capacity; XP unlocks caps and soft buys training. Preserve all social, reward, premium and authority invariants I01–I24 in the master. Base=MK1. Final art OPEN.

Verified game-code checkpoint: branch `codex/implementation-002-unity-host`, commit `d553cd3d9fee998a9764be04b63f8881658d299f`. Implementation001–002 are complete; native Windows x64 IL2CPP passed. [Final verification](implementation/VERIFICATION_002_IL2CPP_FINAL.md). Game systems after002 are not implemented at that checkpoint.

## Authorization boundary

The current master-consolidation task authorizes DOCUMENTATION ONLY. Do not start game implementation merely because the execution pack exists. A later explicit request to execute the pack authorizes its bounded development pass and supersedes old milestone-only stops within that scope. Green Gate must preserve known documentation-only changes or descendants of the verified checkpoint; do not reset them away. No automatic permission for live payments, production publishing, destructive data changes or final art approval.

Keep USER APPROVED, VERIFIED IMPLEMENTED, BALANCE HYPOTHESIS, GROUNDED IMPLEMENTATION DECISION, OPEN PRODUCT/ART and HISTORICAL/SUPERSEDED distinct. Never claim mocked or old verification as a fresh live result.
