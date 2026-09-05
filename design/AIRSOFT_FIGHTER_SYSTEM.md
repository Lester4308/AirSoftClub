# Airsoft fighter system

NEW AIRSOFT DESIGN, PD-03/04/05/12/13. Historical comparison: E010/E018 підтверджують три stats і training costs; формули та caps не встановлені. Нижче всі числа — tuning v0.

## Модель

| Original field | Airsoft field | Ефект |
|---|---|---|
| Accuracy | Accuracy A | Бонус до shot hit chance |
| Agility | Mobility M | Зниження шансу противника під час exposure |
| Endurance | Endurance E | Більший запас round stamina |
| HP | ACTIVE/OUT | Один зарахований hit виводить до кінця раунду |
| Damage/Speed/Armor | weapon cadence/range; kit weight | Похідні параметри, без trainable damage/armor |

На старті A+M+E=24, кожен 4–12. Три starter archetypes: (10,8,6), (6,10,8), (8,6,10). Кандидати мають той самий total24; зовнішність, стать та ім'я не впливають на силу. Стартові розподіли фіксуються в Fighter record. Немає rarity або permanent hidden potential.

Fighter level = min(12,floor(fighterXP/100)). Це число відкриває до 12 придбаних +1 upgrades; stats cap20, total cap36. Level сам по собі не дає другого прихованого stat multiplier. Cumulative XP1200 відкриває весь budget. Придбання n-го upgrade, де n=0..11 уже придбаних, коштує 60+20n Credits. Максимальна сумарна ціна 12 upgrades2040. Куплений budget розподіляється між A/M/E; respec безкоштовний, не повертає Credits/XP, не знижує achieved power band.

Eligible PvP дає кожному deployed fighter20/15/10 XP за win/draw/loss до repetition factor. OUT не зменшує XP: немає kill-stealing incentive. Резерв не отримує match XP; PvE drill дає кожному deployed5XP. One-time tutorial milestone дає 100XP трьом starters. Bench catch-up boosters та instructor skill trees відкладено.

## Roster

MVP: старт 3 з базовими комплектами; після introductory3v3 безкоштовний четвертий на вибір. v1: slots5/6 доступні на club level3/5; hire ціна 300/600 відповідно. Replacement у вже відкритому слоті 300, щоб dismiss/recruit не давав безкоштовного reroll. Candidate list4, fixed until hire/club-level change; немає paid refresh. Усі candidates рівного budget, тому очікування «легендарного» не потрібне.

Dismiss дозволений тільки якщо лишаються мінімум 3 fighters і після атомарного збереження valid attack/defense trios. Призначені items повертаються inventory. Без Credit refund; UI показує втрату XP/upgrades і явне підтвердження. Snapshot прийнятого матчу залишається історичним навіть після dismissal, але не дає ownership предметів.

## Ready Room

Ready означає 3 valid distinct fighters, equipped Primary/Protection/Kit, valid anchors і підтриманий ruleset. Немає persistent health/fatigue. Round stamina =100+3E+kitBonus, відновлюється кожного раунду. Слабка stamina може сповільнювати action cadence, але ніколи не блокує наступний матч.

## Перевірки

Однаковий overall budget має давати різні профілі, а не один універсальний optimum. У seed sweeps порівнювати A-heavy/M-heavy/E-heavy, однаковий gear, side swaps, усі ranges. Higher training budget має допомагати в середньому, але не усувати gear/context tradeoffs. Конкретний win-rate target визначається за даними прототипу; він не доведений цією специфікацією.
