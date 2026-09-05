# Airsoft equipment system

NEW AIRSOFT DESIGN, PD-07/08. Reference E008/E010/E011/E019/E021: історичні slots/stats/mandatory mask; нові числа не взяті з item databases оригіналу.

## Core equipment set v1

| Slot | Вміст | Combat effect | Acquisition |
|---|---|---|---|
| Primary | одна airsoft replica | hit bonus за range, cadence, magazine, reload, mobility load | starter AR безкоштовно; alternatives за Credits |
| Protection | eye/face protection bundle | validity requirement, без stat bonus | free baseline; cosmetic variants later |
| Tactical kit | uniform/rig/boots bundle | stamina vs mobility tradeoff | starter balanced; alternatives150 |

Protection не можна продавати за гроші; базовий комплект відновлюється безкоштовно для кожного owned fighter. Gear не псується. Cosmetics не міняють silhouette настільки, щоб приховувати team/state. Plate carrier не дає імунітету до hit. Secondary, accessories, attachments, set bonuses не входять у v1.

## Початкова таблиця для simulation prototype

Кожна fire action — абстрактний burst із фіксованим BB budget і одним trial на зарахований hit. Це tuning гри, не реальна характеристика спорядження.

| Family / item working ID | Price | Unlock club level | hit bonus bp short/medium/long | cadence ticks | magazine bursts | reload ticks | mobility load | BBs/burst |
|---|---:|---:|---|---:|---:|---:|---:|---:|
| AR / ar_standard | starter або 150 |0 |+300/+300/0 |20 |8 |30 |0 |3 |
| SMG / smg_compact |150 |0 |+500/0/-500 |16 |6 |24 |0 |3 |
| DMR / dmr_precision |250 |3 |0/+400/+800 |30 |5 |40 |2 |1 |

1tick=100ms; bp=1/100 percentage point. Позитивний бонус не означає final chance; формула у [Combat](AIRSOFT_COMBAT_SYSTEM.md). Tables потребують simulation tests: SMG може виявитися домінантною через cadence; не заявляти баланс без вимірювання.

| Kit | Price | stamina bonus | mobility adjustment |
|---|---:|---:|---:|
| balanced | starter або 150 |0 |0 |
| light |150 |-15 |+2 |
| endurance |150 |+25 |-2 |

AR/SMG/три kits — MVP каталожний максимум, protection одна модель. DMR — v1 expansion. Каталог labels не використовує protected/старі brand names. No damage stat у новому UI.

## Inventory rules

ItemInstance має UUID, owner, definition/version. Один instance може бути equipped тільки на одного fighter поточного live roster. При trade іншому fighter команда оновлюється транзакційно і отримує новий snapshot. У старому snapshot зберігається combat definition, не live ownership.

Buy не дорівнює equip, якщо гравець не вибрав explicit Buy & Equip. У command вказуються expected price/catalogVersion, ownership і expected revision. Server перевіряє ціну/level/Credits. Starter grant instances позначені non-sellable; куплені resale=floor(paidCredits×0.25), без refund XP/upgrade. Sell equipped item спочатку потребує valid replacement у тому самому command або відхиляється. Немає player trading/marketplace v1.

## UX comparison

Показувати range strengths, cadence/reload та mobility/stamina delta, а не агреговане «краще на 17%». Strength estimate — лише грубий index; не автоматично equip highest price. Preview не запускає reward-bearing simulation або server seed fishing.
