# Airsoft design package v1

Обов'язкова [межа окремого проєкту](../PROJECT_BOUNDARY.md): цей пакет належить тільки Airsoft_Club_Game. Project Airsoft / Airsoft Manager не змінюється й не є його базою. Див. [реєстр повторного використання ідей](REUSE_DECISIONS.md).

Почати з [AIRSOFT RECONSTRUCTION PRODUCT DECISION PACK v1](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v1.md).

Пакет сформовано 2026-09-05 на основі PaintballWars_Research. Окремий management-first продукт для PC/Steam. Це design deliverable; код гри та production art не створювались. Числа — початкові гіпотези, не validated balance.

## Рішення й докази

- [Product Decision Pack](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v1.md)
- [24 product decisions](PRODUCT_DECISIONS.md)
- [Original → Airsoft mapping](PAINTBALL_TO_AIRSOFT_MAPPING.md)
- [Research basis](RESEARCH_BASIS.md) і [source hashes](SOURCE_MANIFEST.csv)

## Дизайн

- [Game design spec](AIRSOFT_GAME_DESIGN_SPEC_v1.md)
- [Core loop](AIRSOFT_CORE_LOOP.md)
- [UI flow](AIRSOFT_UI_FLOW.md)
- [Fighter system](AIRSOFT_FIGHTER_SYSTEM.md)
- [Equipment](AIRSOFT_EQUIPMENT_SYSTEM.md)
- [Combat](AIRSOFT_COMBAT_SYSTEM.md)
- [Economy](AIRSOFT_ECONOMY.md)

## Архітектура та реалізація

- [Steam social PvP](STEAM_SOCIAL_PVP_ARCHITECTURE.md)
- [Backend](BACKEND_ARCHITECTURE.md)
- [Data model](DATA_MODEL.md)
- [MVP implementation plan](MVP_IMPLEMENTATION_PLAN.md)
- [Перевірка пакета](VALIDATION.md)

Порядок читання: Pack → Decisions → Mapping → відповідні system specs → implementation plan. Числова зміна має бути узгоджена між Pack і тематичними документами. Historical archive зберігає власні терміни; production design використовує airsoft vocabulary.
