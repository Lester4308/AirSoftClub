# Airsoft fighter system — v2

Затверджено V2-03–06/10/11. [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). E002/E007/E010/E018: historical roster, stats, training; formulas UNKNOWN.

## Fighter і roster

Club owns максимум 16. Перший FREE, кожен наступний платний. Deployment окремий від roster:1–16 distinct owned available fighters. Будь-який valid count дозволений проти будь-якого valid count opponent. Немає trio presets як обов'язкової схеми.

Fighter має identity/appearance, starting stats, поточний training progress, Max HP/Current HP, gear, BB assignment. При HP<=0 — eliminated у battle та потребує recovery; не permanent death. HP>0 може бути ready без повного лікування. Energy bar відсутній.

Рекомендовані 3 основні stats (Q-01): Accuracy, Agility, Endurance. Accuracy→hit quality; Agility→avoidance/tempo; Endurance→Max HP. Точні functions/caps/назви OPEN. Damage/armor/cadence — combat-derived fields із equipment. Не переносити v1 round-stamina formulas або stat budget24/36.

## Recruitment market

При відкритті приблизно 6–7 offers, кожен з visible starting stats, HP/derived strength preview, Money price, appearance. Cost залежить і від hire progression, і від candidate quality. Cheaper recruit weaker; expensive stronger як загальна економічна тенденція, без гарантії superiority кожного stat.

Club Level зміщує pool distribution до сильніших candidates із overlap/variance. Upgrade існуючих бійців має залишатися осмисленою альтернативою найму, але рівність потенціалу/caps не затверджена. Development potential — OPEN; не додавати latent rarity або hidden permanent potential до схвалення.

OfferId, generatedClubLevel, stats, price quote і version фіксує backend. Повтор відкриття екрана не повинен непомітно давати безкоштовний unlimited reroll; refresh timing/trigger Q-02. Два кліки Hire не створюють двох бійців.

## Free-first і growth prices

Один free-first entitlement, after-use flag persistent. Чи всі starter candidates free, чи це спеціальний starter subset — Q-02; дорогий high-level offer не стає автоматично безкоштовним через слово first.

Для однакової якості successive recruitment дорожчає. Exact price curve невідома. Питання replacement:current roster size проти lifetime hires; recommendation lifetime progression guard, але не adopted. Не копіювати 100/300 з E025 як нові ціни.

## Training та lifecycle

Training підвищує обрані stats; payment route, increment/XP/caps, respec й compensation OPEN Q-01. Немає автоматичного схвалення premium training button. Credits→Money already creates indirect acceleration якщо base training paid Money.

Rename/dismiss можуть зберегтися як historical-supported capabilities. Не дозволити dismissal скидати free-first прапорець, market costs або farm counters. Політика останнього fighter/повернення gear/compensation має бути визначена Q-02; не зберігати старий min3.

## Health state

CurrentHP bounded0..MaxHP; recovery materializes за server timestamp. Після authoritative battle зберігається finalHP, а не reset. Proposed concurrency: deployed actors busy від acceptance до settlement, heal/train/equip для них serialize/reject; benched actors можуть відновлюватися. Це technical proposal, потрібно погодити Q-03/Q-06 до реалізації.

MaxHP upgrade не може неявно лікувати до 100%: absolute HP, deficit або ratio policy Q-03. Defense snapshot з oldHP не визначає live owner health сам по собі. Injury types, calendar, classes з Project Airsoft не додаються.
