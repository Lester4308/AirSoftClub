# Airsoft_Club_Game

> **Current: Playable Visual Alpha014, 2026-09-07 — local PASS.** [Consolidated visual/build report](implementation/VISUAL_FOUNDATION_014.md) · [Monetization and40-cell balance matrix](implementation/MONETIZATION_BALANCE_014.md) · [Launch guide](implementation/DEVELOPMENT_RUNBOOK.md). Mono/native IL2CPP verified. Intentional premium advantage; progressive early unlock replaces blanket+3. Final art and Steam sandbox remain OPEN. Earlier checkpoint text below is historical.

> **Development pass 2026-09-06:** implementation003–010 now has local code and verification; current scope, remaining gates and native/package evidence are in [Verification011](implementation/VERIFICATION_011.md), with [launch instructions](implementation/DEVELOPMENT_RUNBOOK.md). Earlier documentation-only/future status text below records the pre-execution checkpoint. The user's explicit development-pass request authorizes this work; Project Airsoft remains untouched.


## Актуальна консолідація — Master v1, 2026-09-06

Почати з [Master Development Spec v1](AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md) та [Agent Implementation Pack v1](AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Усі Q01–Q15 закриті на рівні рішень; balance hypotheses і residual details виділені окремо. Implementation001–002 завершені, native IL2CPP PASS. Цей pass змінює лише документацію; виконання pack потребує окремого стартового доручення.

Нижче збережено попередні записи стану. Їхні старі OPEN/нестворені системи/обмеження окремих milestones читаються історично; актуальний статус і правила визначає master.

## Поточний стан — Implementation 002

Unity host: UnityHost/ — **6000.3.21f1 LTS**. Pure battle core підключено одним local package, без копії simulator. [Як відкрити проєкт, запустити тести й builds](implementation/UNITY_INTEGRATION_002.md), [точні результати](implementation/VERIFICATION_002.md).

.NET, EditMode, PlayMode та Windows Mono golden result збігаються. Повний Windows x64 IL2CPP native build/run пройдено; golden digest і regressions збігаються. [Blocker закрито](implementation/VERIFICATION_002_IL2CPP_FINAL.md). До наступних gameplay/UI/backend систем не переходили.


## Попередній стан — Implementation 001

Окремим запитом користувача дозволено й реалізовано pure C# deterministic headless battle core. [Architecture / formulas / scope](implementation/IMPLEMENTATION_001.md), [verification](implementation/VERIFICATION_001.md). Запуск усіх перевірок: powershell -ExecutionPolicy Bypass -File tools/verify.ps1.

Нижче збережено історичний стан design gate до дозволу на цей milestone. Дозвіл поширюється тільки на core/tests/harness; решта систем не дозволені.


Нова незалежна airsoft management game, натхненна management loop «Пейнтбольные войны», зі Steam social/asynchronous PvP. [Обов'язкова project boundary](PROJECT_BOUNDARY.md): Project Airsoft / Airsoft Manager не змінюється і не є основою.

**PRODUCT DESIGN GATE v3 — APPROVED FOR IMPLEMENTATION PREPARATION**
**IMPLEMENTATION NOT YET AUTHORIZED.**

Почати з [короткого summary v3](design/AIRSOFT_CLUB_GAME_UPDATED_PRODUCT_DECISION_SUMMARY_v3.md) та [повного Gate v3](design/AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md). [Індекс усієї документації](design/README.md), [roadmap](ROADMAP.md), [майбутній перший scope](design/MVP_IMPLEMENTATION_PLAN.md), [validation](design/VALIDATION.md).

Стек обрано: Unity/C# client, C#/ASP.NET Core backend, PostgreSQL, Steam-first. Це design-only repository: code, integration, production UI/art та live services ще не створені. Final art OPEN. Баланс і implementation-level питання явно позначені, не приховані як final constants.
