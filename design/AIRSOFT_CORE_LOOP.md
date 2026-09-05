# Airsoft core loop

NEW AIRSOFT DESIGN. Historical anchor: E007–E020; докази не встановлюють наші таймінги чи нагороди.

## Повторюваний цикл

Club → перевірити трійку → за потреби train/shop/equip → Opponents → preview → accept → automatic match → settled Result → одна upgrade decision → Club.

Training і Shop — необов'язкові гілки кожної ітерації. Гравець зі спорядженою трійкою може йти прямо до Opponents. Result пропонує «Наступний суперник» і «Змінити команду»; не змушує обходити всі management screens.

| Крок | Рішення гравця | Системна відповідь | Target |
|---|---|---|---|
| Club | продовжити чи змінити squad | readiness, доступні кошти, defense reports |5–10s |
| Prepare | fighter/gear/stat/anchor | readable deltas, valid squad |15–60s |
| Opponents | friend/revenge/rated offer | format,map,reward policy, strength proxy |10–20s |
| Match | watch/speed/skip | server result і replay |45–90s watched |
| Result | наступна зміна | net reward, XP, unlock, причини |10–20s |

## Перша сесія

1. Steam authentication або dev adapter; створити Club name і дозволену емблему.
2. Вибрати/прийняти трьох starters з готовими безкоштовними комплектами. Розподіли stats різні, budgets однакові.
3. Короткий необов'язковий 1v1 drill пояснює HIT/OUT, без повторної economy reward.
4. Зіграти scripted3v3 на starter arena, показати наслідок gear/anchor choice.
5. Отримати одноразовий milestone reward і вибір четвертого бійця. Показати інвентар без обов'язку наймати більше.
6. Купити альтернативу або виконати training, якщо XP gate відкритий; новий match. Перший tutorial milestone окремо дає 100fighterXP deployed trio для відкриття першого training.

Ціль: до 5 хвилин до team battle. Tutorial не дає штучної гарантованої перемоги, що приховується під виглядом чесної симуляції; NPC склад підібраний для навчання. Повтор milestone не дає виплату.

## Довгий цикл та повернення

XP → bounded training → нова композиція → складніший opponent → rating/особистий rivalry. Return session показує зведений Defense Report, а не чергу модальних вікон. Revenge доступний 72h. Втрати на defense не змінюють wallet/готовність.

Порожній wallet не блокує гру: cost утримується з gross reward. Нуль друзів → Rating Match або PvE. Відсутність мережі → cached club/practice без online progression. Відсутність valid protection → повернення безкоштовного базового комплекту, без платної пастки.

## Метрики перевірки

Time-to-first-team-battle; prepare time; next-match navigation count; частка skip; частка гравців, що пояснюють свій build; repeated-opponent share; no-offer rate; credits net/session. Велика частка skip не є сама по собі провалом: перевірити, чи гравець усе ще розуміє причинність результату.
