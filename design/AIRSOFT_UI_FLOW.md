# UI flow — v3

Це карта майбутніх екранів, не production UI чи остаточний art style. [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md).

| Екран | Дії / видима інформація |
|---|---|
| Entry / reconnect | Steam online session; offline read-only cache з очевидним статусом |
| First recruit | Рівно 3 FREE candidates, обрати одного; male/female variants |
| Club hub | Roster/count до 16, Money/Credits, Club Level, health summary, shared BB, шлях до інших destinations |
| Fighter profile | Accuracy/Endurance/Agility, HP, XP/Level/training cap; soft training/heal; 4 equipment slots |
| Recruitment Center | 6–7 varied priced offers, free refresh timer/match progress, soft immediate refresh, partial-value dismissal |
| Shop / equipment | Weapon/Camouflage/Head Protection/Load-bearing; Base/MK2/MK3; MK1 не презентувати як finalized |
| BB supply | Один club active class, class quantities/capacity, refill, later auto-buy Basic, eligible emergency Basic |
| Opponents | Friends, Ranked candidates 3–5, Revenge opportunities |
| Pre-battle | Автоматичний список учасників і причин health exclusion; один BB class; opponent/reward categories |
| Battle | Automatic HP/Damage/Armor presentation; без ручної стрільби; safety cap не обов'язково UI timer |
| Result | Outcome, Money/Club XP/Fighter XP, rating delta/eligibility, HP/BB витрати, повернення до hub |
| Defense history / Revenge | Origin loss, точна база rating recovery, expiry 24h, attempts до 3, shield/eligibility block |
| Credits / protection | Paid Steam + optional Credits, clear prices, shield durations; early unlock ≠ item purchase |
| Retention / social | Daily/7-day streak, leaderboard, report flow, moderation feedback |

У pre-battle немає checkboxes для manual active squad. Усі боєздатні owned fighters беруть участь, включно з неекіпірованими. HP/readiness policy деталізувати до UI implementation; не приховувати exclusion.

Opponent card: Club Level, rating, count, Very Weak / Weak / Balanced / Strong / Very Strong та reward category. Не показувати exact numeric Team Power або formula. Labels можна локалізувати.

Friends UI пояснює directed 8h window: одна rating win, після неї rating 0; перша win full reward, друга/третя reduced, четверта+ zero до кінця window. Бounded loss policy до першої win буде показана після уточнення. Draw Ranked rating 0.

Shield active блокує нові incoming attacks; accepted match finish. Own Ranked attack interaction OPEN — не створювати кнопку з вигаданою auto-cancel обіцянкою. Revenge success показує 120% origin rating loss і standard reward без bonus multiplier.

Final art OPEN. Reference preferences: класична 2D presentation, side/3/4 profile, stylized/low-poly-inspired 2D, голова приблизно +30%, читабельні weapon/helmet/chest rig, універсальні male/female bases, карти під 16v16. Це не production specification; concepts reference-only. Art pass після core design / implementation foundation.

## Navigation continuity

Збережена інформаційна структура hub/destinations попереднього pass: Launch → Club hub → Team/Recruitment · Training · Shop · Recovery · Club/Opponents. Shortcuts не скасовують destinations. Club hub і Club/Opponents мають різні задачі; labels уточнюються при redesign.

Training показує cap, ціну та preview; Recovery — HP, free recovery estimate і soft heal quote. Currency exchange показує Credits amount і Money output з явним підтвердженням. Leaderboards мають Global/Friends/Around Me; Inbox — defense/Revenge та read state. History зберігає attack/defense outcomes. Немає automatic Credits top-up/exchange/heal. У result gross reward, wallet delta і BB replacement estimate розділені.

First-use: один free recruit → доступне starter gear/Basic BB за майбутньою starter configuration → automatic battle з доступним opponent → result → recovery/refill/recruitment. Не примушувати 1v1, якщо eligible opponent має інший roster. Стан stale quote/snapshot потребує оновлення, offline/social error та empty candidates — окремого пояснення.

Майбутній ORIGINAL UI ANALYSIS → MODERN AIRSOFT UI REDESIGN зіставить historical task flows, створить незалежні layouts і перевірить читабельність 16 fighters, keyboard/focus, scaling/tooltips. Production style лишається OPEN.
