# Fighter system — v3

Канон: [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md). Числові targets: [Balance hypotheses](BALANCE_HYPOTHESES_v3.md). Відкриті деталі: [Q01–Q15](IMPLEMENTATION_QUESTIONS_v3.md).

## Roster та участь

Клуб володіє максимум 16 fighters. Усі куплені боєздатні fighters автоматично беруть участь у власному бою. Ручного active squad, резерву для довільного виключення або обов'язкового 3v3 немає. 1v2, 2v5, 7v16 та 16v16 допустимі. Погана чи відсутня екіпіровка не виключає бійця; точна дія без Weapon — Q03.

Offense використовує live Current HP після належного server-time recovery. Fighter на 0 HP не допускається до власної атаки до readiness threshold (~10% Max HP, hypothesis). Q02 визначить застосування порога до решти поранених. Не створювати окремий persistent Energy ресурс.

Defense формує доступних owned fighters із 100% Max HP у snapshot; live wounds не знижують цей HP. Рекомендована інтерпретація доступності — чинний roster без звільнених fighters, незалежно від live 0 HP; остаточна lifecycle validation — Q14. Defense не лікує та не ранить live roster.

## Три базові характеристики

| Stat | Роль |
|---|---|
| Accuracy | Chance to hit |
| Endurance | Max HP |
| Agility | Evasion та action/initiative tempo |

Max HP, Current HP, Damage, Protection, penetration та derived tempo не є додатковими базовими stats. Не вводити fighter classes або генетичні стелі, успадковані з іншої гри.

## XP та training

Фактична участь → Battle XP → Fighter Level → підвищення доступного training cap → покупка конкретного stat upgrade за soft currency. Окремої training currency немає. Учасники, які вибули під час бою, також eligible; неучасникам XP не нараховується. Рівень сам не купує stat upgrades. Дешевий recruit може стати сильним ветераном; дорожчий економить час і витрати, але не має недосяжної генетичної переваги.

XP curve швидша спочатку і поступово сповільнюється; числа відкриті. Outcome multipliers: [Economy](AIRSOFT_ECONOMY.md). Endurance upgrade не лікує: 80/100 → Max HP 110 → 80/110.

## Recruitment

Одноразово обрати одного з 3 FREE starter candidates. Звичайний центр показує 6–7 candidates з різними starting stats, силою, ціною та male/female visual variants. Club Level підвищує середню якість, зберігаючи variance. Купівля збільшує автоматично залучений roster, потребу в gear, BB і лікуванні.

Free refresh: ~1 година АБО ~10 матчів, що раніше (prototype); immediate refresh — soft currency. Dismiss/sell повертає лише частину вартості, target ~25–40%. Не дублювати перший безкоштовний entitlement через dismissal. Resale base, gear return та refresh reset — Q13.

## Health lifecycle

Після offensive battle зберігається фактичний Current HP: 35/100 без лікування означає старт наступної атаки з 35 HP плюс законне відновлення за минулий час. Immediate healing — soft currency; free recovery — ~1% Max HP/хв, включно з офлайном. Offline elapsed time обчислює сервер, локальний clock не є доказом.

Перед battle acceptance сервер фіксує версію roster/health. Після результату застосовує HP та XP один раз. Не дозволяти старому результату перезаписати пізніший heal/equip; порядок команд і recovery timestamps — Q14.
