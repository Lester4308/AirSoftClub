# Mandatory independent-project boundary

Read and obey [PROJECT_BOUNDARY.md](PROJECT_BOUNDARY.md). This is Airsoft_Club_Game, a new game with its own repository, history, design, architecture and roadmap. Do not continue, modify, import or inherit Project Airsoft / Airsoft Manager. Proposed idea reuse requires a justified, explicit adoption in [REUSE_DECISIONS.md](design/REUSE_DECISIONS.md); recording a proposal does not authorize copying.

## Canonical design

Use [Product Design Gate v3](design/AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md), [Pack v3](design/AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v3.md), [PRODUCT_DECISIONS.md](design/PRODUCT_DECISIONS.md) and the user's latest instructions. V1/v2 product documents are SUPERSEDED pointers; source records are historical evidence, not active rules.

Hard rules: max roster 16; all purchased combat-ready fighters automatically participate, with no manual active squad; asymmetric battles; one challenge = one battle = one round. Exactly Accuracy, Endurance, Agility. HP/Damage/Armor; offensive Current HP persists; defense starts at full HP and never debits live HP or BB. No persistent Energy. Four equipment slots. Shared club BB stock and one active BB class. XP unlocks training caps; soft currency buys upgrades.

Steam Friends have a directed 8-hour rating/reward window: one rating-eligible win, subsequent matches have zero rating impact; repeated losses are bounded. First victory full eligible reward, second and third reduced, fourth onward zero Money/Club XP/Fighter XP. Never reset that reward exhaustion by a loss, draw, mode switch or reconnect. Defense exposure max 4 rating-impacting attacks per profile per 24 hours. Shields block new attacks; whether own Ranked attack cancels a shield remains OPEN. Revenge: 24 hours, max 3 attempts, one success restores 120% of rating actually lost in the originating defense battle, plus standard battle reward.

Paid Steam game plus optional Credits. Early unlock at most +3 Club Levels grants access only; buy item separately for soft currency. MK2 expensive soft; MK3 Credits-only. Final art OPEN. Unity/C# client, C#/ASP.NET Core backend, PostgreSQL, Steam-first. Backend is authoritative; shared C# contracts do not make the client trusted.

Preserve USER APPROVED, BALANCE HYPOTHESIS, IMPLEMENTATION PROPOSAL, OPEN and historical evidence distinctions. Do not turn approximate targets into final constants. See [open questions](design/IMPLEMENTATION_QUESTIONS_v3.md).

**Implementation 002 is explicitly authorized:** minimal Unity host, shared-core integration, versioned serialization, golden fixtures, EditMode/PlayMode tests and Windows Mono/IL2CPP validation only. See [integration scope](implementation/UNITY_INTEGRATION_002.md). Do not rebalance or add production UI/art, Steam, backend/database, economy, recruitment, healing, matchmaking or live PvP. Q05–Q15 remain open except verified technical facts appended to Q12. Stop after this gate and commit.
