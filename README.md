# Airsoft_Club_Game

## Поточний стан — Implementation 001

Окремим запитом користувача дозволено й реалізовано pure C# deterministic headless battle core. [Architecture / formulas / scope](implementation/IMPLEMENTATION_001.md), [verification](implementation/VERIFICATION_001.md). Запуск усіх перевірок: powershell -ExecutionPolicy Bypass -File tools/verify.ps1.

Нижче збережено історичний стан design gate до дозволу на цей milestone. Дозвіл поширюється тільки на core/tests/harness; решта систем не дозволені.


Нова незалежна airsoft management game, натхненна management loop «Пейнтбольные войны», зі Steam social/asynchronous PvP. [Обов'язкова project boundary](PROJECT_BOUNDARY.md): Project Airsoft / Airsoft Manager не змінюється і не є основою.

**PRODUCT DESIGN GATE v3 — APPROVED FOR IMPLEMENTATION PREPARATION**
**IMPLEMENTATION NOT YET AUTHORIZED.**

Почати з [короткого summary v3](design/AIRSOFT_CLUB_GAME_UPDATED_PRODUCT_DECISION_SUMMARY_v3.md) та [повного Gate v3](design/AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md). [Індекс усієї документації](design/README.md), [roadmap](ROADMAP.md), [майбутній перший scope](design/MVP_IMPLEMENTATION_PLAN.md), [validation](design/VALIDATION.md).

Стек обрано: Unity/C# client, C#/ASP.NET Core backend, PostgreSQL, Steam-first. Це design-only repository: code, integration, production UI/art та live services ще не створені. Final art OPEN. Баланс і implementation-level питання явно позначені, не приховані як final constants.
