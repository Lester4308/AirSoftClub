# Airsoft equipment system — v2

USER APPROVED V2-03/16–23. [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Historical E008/E010/E011/E019/E021 підтверджують gear fields/slots; MK/BB classes нові.

## Слоти та основні поля

Compact proposal Q-08: Weapon, Clothing/Armor, Eye/Face Protection. Mandatory protection зберігає historical readiness principle. Final slots, weapon families і каталог відкриті; дев'ять слотів автоматично не вводяться.

Weapon definition:family, base damage, accuracy/handling/cadence, reload/ammo rules, level gate, normal Money price, optional Credits acquisition route. Armor:rating, mitigation contribution, possible agility/mobility penalty. Eye protection — validity slot; HP/armor — arcade game abstraction, не реальна safety simulation.

Більш сильний armor може зменшувати mobility; дорога weapon не обов'язково підходить до build. Penetration тільки якщо має зрозумілу користь; не додавати одночасно складну ballistics/attachments систему без рішення.

## MK = appearance + small gameplay progression

Weapon Base → MK1 → MK2 → MK3. Кожен tier змінює appearance/skin і дає невелике покращення damage, accuracy або handling. Credits можуть відкривати tier. Further tiers лише після review.

TierDefinition прив'язаний до baseWeaponDefinition та balanceVersion; visualVariant і effective stat modifier разом. Не видавати MK за cosmetic-only item. Можлива окрема чиста косметика пізніше не змінює MK rule.

Межі:base family identity зберігається; no large multipliers, no guaranteed hit, invulnerability або guaranteed win. Exact stat improvements, step costs, cumulative cap і soft progression route OPEN Q-07/Q-11. Не встановлювати ціну або приріст із прикладів попередньої версії.

## Normal / early / premium acquisition

Normal:Club Level gate + Money purchase. Early:Credits доступ до item до ordinary level; Level7→gate10 — illustrative, не production unlock table. Entitlement лише unlock чи вже owned item Q-07.

Premium-only collectible items допустимі з modest advantage, status та distinct appearance. Конкретні SKUs не approved. Credits-only top BB — можливий catalog direction. Власність premium item не підміняє entitlement перевірку, не дає права на impossible stats.

## Persistent BB classes

Окремий inventory resource за class, купівля Money або Credits згідно catalog. Напрям 3–5 classes:Base/Improved/Advanced/High-end/Premium. Приклад 0/+3/+5/+10/~+15% — prototype hypotheses. Верхній target близько +15% damage або equivalent armor penetration/ballistic benefit; не повне ігнорування armor.

Ammo assignment фіксується до battle. Змішування classes, per-fighter capacity, shared pool і magazine semantics OPEN Q-04. Сервер reserves обрану quantity; spent з event log; unspent повертається після settlement. Це proposal transaction model, не готовий balance.

## Combined power review

Оцінювати base item+early access+MK+BB+armor+fighter stats+чисельність разом. Окремий «невеликий» modifier може змінити кількість hits до elimination;+accuracy і faster cadence теж damage throughput. BB15% не дозволяє автоматично ще стільки ж penetration поза cap.

Future test matrix:free normal path /early unlock /premium item /MK only /BB only /combined, різні Club Levels, HP ratios, team sizes, armor builds. Записати opportunity cost боєприпасів і survival cost. До цього жодна ціна/aggregate cap не final.

## Ownership і зміни

ItemInstance owner UUID, definition version, tier, acquisition source/commerce transaction. Upgrade витрачає дозволену currency і змінює один instance atomic; retry не додає tier. Assigned item не дублюється між двома live fighters. Historical snapshot copy не live ownership.

Sell/dismiss/refund behavior OPEN; не переносити v1 resale25% або free-baseline gear promises. Accepted battle pins definitions; пізніша покупка/upgrade не змінює replay або power used for reward.
