> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Product decisions — v3

Канон: [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md). Джерело: [точна копія user request](USER_APPROVED_DECISIONS_v3.txt), додатково [friend reward clarification](USER_CLARIFICATION_v3.md). V3-00–V3-76 відслідковують кожен пункт запиту; це нові product decisions, не нові historical confirmations.

CONFIRMED позначає продуктну вимогу. MIXED означає підтверджений напрям із числовими hypotheses або явно OPEN деталями; деталі у пов'язаному документі та [B01–B12](BALANCE_HYPOTHESES_v3.md), [Q01–Q15](IMPLEMENTATION_QUESTIONS_v3.md).

| ID | User section | Статус | Канонічне покриття |
|---|---|---|---|
| V3-00 | PROJECT BOUNDARY | CONFIRMED | [Spec](../PROJECT_BOUNDARY.md) |
| V3-01 | PRODUCT DNA | CONFIRMED | [Spec](AIRSOFT_CORE_LOOP.md) |
| V3-02 | CORE ROSTER / BATTLE SIZE | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-03 | FIGHTER STATS | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-04 | FIGHTER XP / TRAINING | MIXED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-05 | RECRUITMENT | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-06 | RECRUITMENT REFRESH | MIXED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-07 | RECRUIT DISMISSAL | MIXED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-08 | CLUB LEVEL → RECRUIT QUALITY | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-09 | HP / DAMAGE / ARMOR | CONFIRMED | [Spec](AIRSOFT_COMBAT_SYSTEM.md) |
| V3-10 | PERSISTENT CURRENT HP | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-11 | HEALING | MIXED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-12 | MAX HP UPGRADE RULE | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-13 | ZERO HP | MIXED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-14 | ENERGY | CONFIRMED | [Spec](AIRSOFT_FIGHTER_SYSTEM.md) |
| V3-15 | BB INVENTORY | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-16 | BB SELECTION | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-17 | BB CONSUMPTION | CONFIRMED | [Spec](AIRSOFT_COMBAT_SYSTEM.md) |
| V3-18 | BB STARTER SUPPLY | MIXED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-19 | BB AUTO-BUY | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-20 | EMERGENCY BB | MIXED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-21 | BB CLASSES | MIXED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-22 | PREMIUM BB | MIXED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-23 | BATTLE END CONDITIONS | CONFIRMED | [Spec](AIRSOFT_COMBAT_SYSTEM.md) |
| V3-24 | DRAW CONDITIONS | CONFIRMED | [Spec](AIRSOFT_COMBAT_SYSTEM.md) |
| V3-25 | DRAW REWARD | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-26 | LOSS REWARD | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-27 | EQUIPMENT SLOTS | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-28 | HEAD PROTECTION | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-29 | LOAD-BEARING / ARMOR | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-30 | ARMOR TRADEOFF | MIXED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-31 | WEAPON FAMILIES v1 | CONFIRMED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-32 | ART DIRECTION | OPEN (explicit) | [Spec](AIRSOFT_UI_FLOW.md) |
| V3-33 | TEAM POWER | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-34 | TEAM POWER UI | CONFIRMED | [Spec](AIRSOFT_UI_FLOW.md) |
| V3-35 | REWARD FOUNDATION | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-36 | REWARD TYPES | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-37 | REPEAT FRIEND REWARD | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-38 | FRIEND RATING WINDOW | MIXED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-39 | DEFENSE SNAPSHOT | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-40 | DEFENSE HP | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-41 | DEFENSE BB | MIXED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-42 | CONCURRENT DEFENSE | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-43 | DEFENSE RATING EXPOSURE | MIXED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-44 | PROTECTION SHIELD | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-45 | SHIELD / ATTACK INTERACTION | OPEN (explicit) | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-46 | RANKED OPPONENT SELECTION | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-47 | RANKED DISPLAY | CONFIRMED | [Spec](AIRSOFT_UI_FLOW.md) |
| V3-48 | RANKED DRAW | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-49 | REVENGE | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-50 | REVENGE ATTEMPTS | MIXED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-51 | REVENGE RATING REWARD | MIXED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-52 | REVENGE NORMAL REWARD | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-53 | MONETIZATION MODEL | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-54 | PREMIUM CURRENCY | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-55 | FREE CREDIT SOURCES | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-56 | EARLY UNLOCK | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-57 | EARLY UNLOCK DEPTH | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-58 | WEAPON MK SYSTEM | MIXED | [Spec](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| V3-59 | PREMIUM-ONLY ITEMS | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-60 | GLOBAL PREMIUM ADVANTAGE CEILING | MIXED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-61 | CREDIT REFUNDS | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-62 | COSMETICS | CONFIRMED | [Spec](AIRSOFT_ECONOMY.md) |
| V3-63 | TECH STACK | CONFIRMED | [Spec](BACKEND_ARCHITECTURE.md) |
| V3-64 | SHARED C# CONTRACTS | MIXED | [Spec](BACKEND_ARCHITECTURE.md) |
| V3-65 | BACKEND AUTHORITY | CONFIRMED | [Spec](BACKEND_ARCHITECTURE.md) |
| V3-66 | ONLINE REQUIRED | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-67 | OFFLINE MODE | CONFIRMED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-68 | RETENTION SYSTEMS | CONFIRMED | [Spec](AIRSOFT_CORE_LOOP.md) |
| V3-69 | MODERATION | MIXED | [Spec](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| V3-70 | BALANCE STATUS | BALANCE POLICY | [Spec](BALANCE_HYPOTHESES_v3.md) |
| V3-71 | UPDATE ALL DEPENDENT DOCUMENTS | DELIVERY REQUIREMENT | [Spec](README.md) |
| V3-72 | SUPERSEDE CONFLICTING OLD RULES | DELIVERY REQUIREMENT | [Spec](PRODUCT_DECISIONS.md) |
| V3-73 | PRODUCT DESIGN GATE OUTPUT | DELIVERY REQUIREMENT | [Spec](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md) |
| V3-74 | GATE STATUS | DELIVERY REQUIREMENT | [Spec](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md) |
| V3-75 | FIRST IMPLEMENTATION PREP SCOPE | DELIVERY REQUIREMENT | [Spec](MVP_IMPLEMENTATION_PLAN.md) |
| V3-76 | VERIFICATION | DELIVERY REQUIREMENT | [Spec](VALIDATION.md) |

## Superseded register

| Старе правило / припущення | Статус і чинна заміна |
|---|---|
| Mandatory 3v3, max roster 6 | SUPERSEDED: max 16, automatic eligible roster, asymmetry |
| Manual active squad / selectable subset | SUPERSEDED: усі purchased combat-ready беруть участь |
| One-hit elimination | SUPERSEDED: HP/Damage/Armor, multi-hit |
| Non-ranked-only Steam Friends | SUPERSEDED: directed rating-eligible window 8h |
| Default non-ranked-only Revenge | SUPERSEDED: 120% origin rating loss recovery |
| Purely cosmetic-only skins/MK | SUPERSEDED: visual + small stats; pure cosmetics later |
| 3 equipment slots | SUPERSEDED: 4 slots |
| Final 3D art direction | SUPERSEDED: final art OPEN |
| Project Airsoft reuse assumptions | REJECTED / SUPERSEDED: independent project, no automatic reuse |
| Select engine/backend later | SUPERSEDED: Unity/C#, ASP.NET Core/C#, PostgreSQL, Steam-first |
| Live defense wounds/debit policy undecided | SUPERSEDED: full-HP snapshot, no live HP/BB debit |
| Friend reward cutoff ambiguous | RESOLVED: full, reduced, reduced, zero from fourth |
| Credits early unlock directly grants item | SUPERSEDED: access only, item purchased soft separately |

V1/v2 product documents у робочому дереві — лише SUPERSEDED pointers. Попередні тексти збережено Git history на 3f76545. USER_APPROVED_CORRECTIONS_v2.txt — historical source; нові правила мають пріоритет. SOURCE_MANIFEST.csv та historical evidence не переписуються.

## Gate

PRODUCT DESIGN GATE v3 — APPROVED FOR IMPLEMENTATION PREPARATION.
**IMPLEMENTATION NOT YET AUTHORIZED.** Перший майбутній scope визначено у [MVP plan](MVP_IMPLEMENTATION_PLAN.md), без виконання.
