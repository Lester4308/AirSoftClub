# Airsoft game design specification — v3

**Канон:** [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md) та [Product Decision Pack v3](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v3.md). **IMPLEMENTATION NOT YET AUTHORIZED.**

Це navigation specification усіх активних систем нової незалежної гри. Чинні product правила зібрані в gate; subsystem documents деталізують їх без окремих суперечливих версій.

| Система | Специфікація |
|---|---|
| Gameplay та screens | [Core loop](AIRSOFT_CORE_LOOP.md), [UI flow](AIRSOFT_UI_FLOW.md) |
| Roster, XP, recruitment, HP | [Fighter system](AIRSOFT_FIGHTER_SYSTEM.md) |
| Чотири slots, weapons/MK, BB | [Equipment](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| Automatic battle, outcomes, deterministic contract | [Combat](AIRSOFT_COMBAT_SYSTEM.md) |
| Rewards, Money/Credits, premium limits | [Economy](AIRSOFT_ECONOMY.md) |
| Friends/Ranked/defense/Revenge/Steam | [Social PvP](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| Server authority / persistence concepts | [Backend](BACKEND_ARCHITECTURE.md), [Data model](DATA_MODEL.md) |
| Prototype targets / undecided details | [Balance](BALANCE_HYPOTHESES_v3.md), [Questions](IMPLEMENTATION_QUESTIONS_v3.md) |
| Future scope | [MVP plan](MVP_IMPLEMENTATION_PLAN.md), [Roadmap](../ROADMAP.md) |
| Provenance / traceability | [Mapping](PAINTBALL_TO_AIRSOFT_MAPPING.md), [Decisions](PRODUCT_DECISIONS.md), [Research](RESEARCH_BASIS.md) |

Порядок пріоритету: поточні вказівки користувача → project boundary → v3 confirmed rules → явно позначені hypotheses/proposals. Historical originals не встановлюють правила нової гри. Final art OPEN; обраний Unity client не означає фінального 3D style.
