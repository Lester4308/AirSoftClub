# Критична межа нового проєкту

Зафіксовано за прямою інструкцією користувача 2026-09-05.

**ЦЕ НОВА ОКРЕМА ГРА, створювана з нуля.** Робочий ідентифікатор папки: Airsoft_Club_Game. Остаточна назва не визначена. Це не продовження, модернізація або перейменування Project Airsoft / Airsoft Manager.

## Незалежність

- Власна папка та окремий локальний Git-репозиторій без спільної історії з іншими іграми.
- Власні design documents у design/.
- Власна архітектура: design/BACKEND_ARCHITECTURE.md та design/STEAM_SOCIAL_PVP_ARCHITECTURE.md.
- Власна модель даних: design/DATA_MODEL.md.
- Власний roadmap: design/MVP_IMPLEMENTATION_PLAN.md.
- Актуальний roadmap v2: [ROADMAP.md](ROADMAP.md); затверджені corrections — [Product Pack v2](design/AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md).
- Власні combat, economy, progression, UI та майбутній save implementation.

Project Airsoft не змінювати. Його код, архітектура, data model, fighter classes, combat rules, economy, calendar, UI flow, progression, repository structure, save system, balance та terminology не успадковуються.

Можливе повторне використання ідеї потребує окремого обґрунтування саме для нової гри у [реєстрі](design/REUSE_DECISIONS.md). Схожість загального патерну не доводить походження з іншого проєкту; рішення має спиратися на потреби цієї гри. Наразі жодного такого повторного використання не прийнято.

## Походження матеріалів

Поточні design documents створені в попередньому кроці цього завдання на основі PaintballWars_Research та явно позначених нових продуктових рішень. Вони перенесені з тимчасового розташування Airsoft_Design до design/ цього незалежного репозиторію. Git-історія тимчасової папки не імпортована. Репозиторії та матеріали Project Airsoft для цього перенесення не використовувалися.

Слова «Airsoft Club Manager» у ранньому design pack — опис жанру, а не посилання на існуючий Airsoft Manager. У разі неоднозначності пріоритет має цей документ.

Завершений design pack не означає початку implementation чи затвердження всіх гіпотез балансу. Remote repository, engine, календар розробки та production code ще не створені.
