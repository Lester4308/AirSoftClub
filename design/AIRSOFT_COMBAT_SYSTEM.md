# Automatic combat — v3

[Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md) визначає product rules; [Q01–Q04](IMPLEMENTATION_QUESTIONS_v3.md) — деталі до майбутнього battle core.

## Вхід і виконання

Один challenge = один battle = один round = один результат. Участь автоматична: усі owned combat-ready fighters, максимум 16 на сторону, асиметрія дозволена. Гравець готує клуб і обирає opponent, не керує стрільбою. Екіпіровка не є eligibility gate.

Offensive roster бере live Current HP після recovery/readiness validation. Defense бере immutable snapshot з повним Max HP доступних fighters, gear, active BB class, rating та ruleset/balance version. Нові committed зміни публікують новішу snapshot; accepted battle використовує вже зафіксовану версію.

Accuracy впливає на hit, Agility на evasion/tempo, Endurance на Max HP. Weapon/BB/armor визначають damage та protection interactions. Бійці можуть пережити кілька влучань, вибувають при Current HP <= 0. Остаточні формули та simultaneous-event ordering OPEN. Не додавати базові stats чи складну балістику.

## Ресурси

Один club BB class на battle. Кожен simulated shot атакуючого споживає реальний shared club BB; нуль stock не дозволяє безкоштовного пострілу. Defense shots витрачають тільки simulated budget snapshot battle. Offensive HP зберігається після результату; defense не змінює live HP/BB. Max HP training не лікує.

## Outcome precedence

| Умова | Outcome |
|---|---|
| Останні боєздатні fighters обох сторін вибули одночасно | Draw |
| Лише одна сторона повністю втратила боєздатних fighters | Перемога іншої сторони |
| Обидві сторони без ammo і не можуть завершити бій | Draw |
| Safety cap досягнуто без переможця | Draw |

Safety cap захищає simulation від зависання, не є обов'язковим видимим таймером. Порожню власну readiness roster потрібно відхиляти до прийняття battle, а не продавати як гарантовану поразку. No-weapon/no-progress поведінка — Q03, не прихована one-hit або instant-win політика.

[Reward table](AIRSOFT_ECONOMY.md): draw Club XP 35%, Ranked draw rating 0; loss Money 0, Fighter XP 25%. Fighter XP отримують фактичні учасники включно з eliminated. Mode eligibility може зменшити/занулити reward, але не змінює combat outcome.

## Майбутня відтворюваність

Рекомендовані MatchConfig: immutable input, stable fighter/item IDs, стартові HP, два simulated BB budgets/classes, ruleset/balance version і seed. MatchResult: outcome/reason, кінцеві HP, shot counts/BB used, participant IDs, ordered event trace або digest, versions. Це специфікація майбутніх типів, не код.

Однакові input + seed + version мають давати однаковий результат. RNG не залежить від Unity frame rate; domain не залежить від Unity scenes/network/database. Backend надалі сам resolve/verify battle, client replay не доводить нагороду. Acceptance та settlement idempotent; пропозиція — один незавершений offensive battle на клуб до settlement, щоб уникнути подвійного витрачання. Деталі reservation OPEN.

Майбутні meaningful tests: 1v2/7v16/16v16, automatic inclusion без gear, HP <= 0, кілька влучань, одночасна остання elimination, ammo draw, cap draw, no negative BB, однакові seeds, defense isolation, Max HP upgrade без heal. Вони ще не реалізовані й не запускалися.
