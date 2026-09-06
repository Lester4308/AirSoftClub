> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Equipment / BB — v3

Канон: [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md). Параметри: [Balance hypotheses](BALANCE_HYPOTHESES_v3.md).

## Чотири slots

| Slot | Вміст / роль |
|---|---|
| Weapon | Одна equipped weapon; family, damage/tempo/penetration profile, MK |
| Camouflage | Видимий комплект; combat modifier лише якщо явно визначено |
| Head Protection | Goggles, masks, helmets, інший head/face protection |
| Load-bearing / Armor | Chest rigs, plate carriers, tactical vests та armor-related gear |

Light: нижчий protection, низький/відсутній Agility penalty. Medium: середній protection, невеликий penalty. Heavy: сильніший protection, більший penalty. Проста взаємодія weapon/BB penetration з armor; формули та stacking head/body protection ще відкриті. Gear не є умовою включення owned боєздатного fighter.

Дозволені сімейства v1: Pistol, SMG, Assault Rifle, Shotgun, DMR, Sniper Rifle. Відкриваються поступово; не всі доступні на старті. Точний каталог, range model та unlock levels ще не обрано.

## Acquisition та MK

Base — normal progression. MK2 — дорого за soft currency. MK3 — Credits-only. MK дає visual change і невелике stat improvement. Статус MK1 та upgrade path OPEN (Q10); попереднє правило не переносити мовчки.

Credits early unlock допускає максимум +3 Club Levels і дає лише entitlement: сам предмет після цього купується за soft currency. Premium-only weapon/armor допустимі з target ~+5–8% проти звичайного аналога. Усі paid modifiers разом обмежуються target ~15–20% effective advantage; не балансувати кожен ізольовано. Pure cosmetics можливі пізніше, не пріоритет v1.

## Shared club BB

BB stock належить клубу; не fighter. Ємність зростає з Club Level. Один active BB class обирається для всієї команди/бою; немає per-fighter BB selector. Класи зберігаються в club inventory; точна загальна/роздільна capacity policy — Q04.

Усі реальні simulated shots атакуючих списуються зі спільного обраного stock. Defender використовує snapshot BB class і окремий simulated budget без debit live BB. Defender budget має бути скінченним і версійованим; кількість OPEN. Не прирівнювати кількість pellets до кількості пострілів без рішення Q04. Немає мовчазного auto-switch на premium чи auto-spend Credits.

Prototype 3–5 tiers: Basic +0%, Improved ~3%, Advanced ~5%, High-End ~10%, Premium до ~15% effective advantage. Можливі damage/penetration tradeoffs. Найсильніший tier Credits-only; x2 damage, guaranteed hit та ignore-all-armor заборонені.

Basic starter supply target 5–7 перших матчів. Пізніше progression відкриває AUTO-BUY BASIC BB за soft currency; unlock level, opt-in та spend cap ще визначити. Emergency Basic: stock <~15% capacity і soft недостатньо для min refill → +500 Basic раз на ~3 години; всі числа hypotheses. Альтернатива — свідоме поповнення через Credits. Emergency ніколи не видає premium BB.
