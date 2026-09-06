> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Updated product decision summary — v3

**PRODUCT DESIGN GATE v3 — APPROVED FOR IMPLEMENTATION PREPARATION**
**IMPLEMENTATION NOT YET AUTHORIZED.**

[Повний gate](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md), [Pack v3](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v3.md), [boundary](../PROJECT_BOUNDARY.md).

| Область | Остаточне product рішення v3 |
|---|---|
| Scope | Нова незалежна Airsoft_Club_Game; Project Airsoft не змінюється і не успадковується |
| Core | Management → preparation → opponent → automatic battle → result → progression; один battle/round |
| Roster | Max 16; усі боєздатні owned fighters автоматично; no manual active squad; асиметрія |
| Fighters | Accuracy/Endurance/Agility; XP → Level → training cap → soft-paid upgrades; немає training currency/hidden genetic caps |
| Recruitment | Один free з 3 starter candidates; normal 6–7 varied offers; Club Level покращує середню якість |
| HP | Multi-hit HP/Damage/Armor; offensive Current HP persists; soft heal/free recovery; Max HP upgrade не лікує; no Energy |
| Gear | 4 slots; 6 weapon families; armor protection/Agility tradeoff |
| BB | Shared club stock, один active class, actual shots debit attacker; defense без live debit |
| Outcomes | Draw дозволений; draw Club XP 35%, Ranked rating 0; loss Money 0/Fighter XP 25% |
| Friends | Directed 8h/одна rating win; після win rating 0; rewards full/reduced/reduced/zero з четвертої |
| Defense | Immutable full-HP snapshot; max 4 incoming rating impacts/24h |
| Shield | Credits: 8h/1d/3d/7d; blocks new attacks, accepted finish; own Ranked interaction OPEN |
| Ranked | 3–5 candidates, вибір гравця; приблизна сила без exact numeric Team Power |
| Revenge | 24h/3 attempts; success 120% origin rating lost + standard reward |
| Monetization | Paid Steam + optional Credits; early unlock +3 levels лише access; MK2 soft/MK3 Credits-only |
| Balance | Premium gear ~5–8%, BB до ~15%, combined stack ~15–20% — targets, не доказ балансу |
| Stack | Unity/C#, C#/ASP.NET Core, PostgreSQL, Steam-first; backend authoritative |
| Offline | Read-only cache/launch; isolated PvE potentially later, без authoritative progression |
| Art | FINAL STYLE OPEN; 2D/3/4/bigger-head concepts reference-only |

Remaining: [Q01–Q15](IMPLEMENTATION_QUESTIONS_v3.md), зокрема combat/readiness/ammo, rating-window/loss limits, Revenge edge cases, MK1, shield interaction, versions і balance curves. [First milestone scope](MVP_IMPLEMENTATION_PLAN.md) підготовлено, не реалізовано.
