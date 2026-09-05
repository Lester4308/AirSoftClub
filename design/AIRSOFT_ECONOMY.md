# Airsoft economy

PD-09–13. Original: E006/E008/E015–E018/E029. **Усі таблиці тут NEW AIRSOFT DESIGN, не historical formulas і не перевірений баланс.**

## Resources і policy

Credits — єдина spendable currency. Club XP відкриває sidegrades/roster slots. Fighter XP відкриває training budget, який купують Credits. Немає paid currency, premium training, energy, paid recovery, trade між гравцями або passive defense income.

Старт:300Credits,3fighters з non-sellable gear; четвертий безкоштовно після intro. Catalog: AR/SMG/kit150,DMR250; slots5/6 hire300/600; training60+20n. Club level=min(5,floor(clubXP/100)); clubXP earned за eligible match20/15/10 win/draw/loss, multiplied repetition policy. Tutorial team match додає звичайний reward і один milestone100clubXP +100XP starters, без додаткових Credits. Slot5/DMR unlock level3,slot6 level5. Levels не додають невидимий damage multiplier.

## Reward table

| Режим | Gross Credits W/D/L | Operational cost | Club XP W/D/L | XP на deployed fighter W/D/L | Rating |
|---|---|---|---|---|---|
| Eligible ranked/friendly/revenge,1st directed pair/24h |60/40/30 |15 |20/15/10 |20/15/10 | тільки ranked |
|2nd directed pair/24h |30/20/15 |7 |10/7/5 |10/7/5 | лише якщо ranked pair policy дозволяє |
|3rd+ directed pair/24h |0 |0 |0 |0 |0 |
| Repeat online PvE practice3v3 |10, outcome independent |0 |0 |5 |0 |
| Offline practice/replay/tutorial drill repeats |0 |0 |0 |0 |0 |
| Passive defense |0 |0 |0 |0 | paired ranked delta only |

Factor0.5 застосовується з floor до кожного поля окремо. Net reward=gross−cost; у second win23/draw13/loss8. Cost — match-level fixed supply allowance, не кількість shots. Бойові BB reserves — окрема per-round mechanical величина. Reward eligibility сервер фіксує при acceptance, quota reservation враховує concurrent requests.

Counter ключ(attackerUUID,defenderUUID), rolling24h, спільний для ranked/friendly/revenge. Ranked додатково має unordered pair limit1/24h. Revenge не обнуляє лічильник. Provider identity однакова незалежно від UI tab. Нульові matches дозволені як practice без штучного очікування. За rated match без reward eligibility запуск відхиляється як rated; UI може явно запропонувати unranked practice.

## Чому немає банкрутства

Не списувати 15 із wallet перед боєм. Gross і cost — два ledger entries одного settlement, net завжди≥0 у запропонованій таблиці. Balance0 не перешкоджає match. Покупка не може знизити balance<0. NPC practice дає мінімальний прогрес, навіть якщо population недостатній або всі pairs вичерпані. Перегляд одного match повторно не створює entries.

При win rate50% і full eligible matches: net avg30, перший upgrade60 приблизно 2matches, sidegrade150 —5. Loss streak10 дає 150net та 100XP/fighter: це захист від застою. Водночас 12upgrades коштують 2040 і вимагають 1200XP; з 15avgXP/match це 80matches на budget unlock, незалежно від того, що Credits можна отримати швидше. Tutorial зсуває перший XP gate на 100. Це сценарні розрахунки, не retention forecast.

## Abuse і saturation

No wallet gifting, no player market, no defense income, no rank rewards convertible to unlimited Credits. Обмежені stat caps захищають від unlimited combat power, але colluding accounts усе ще можуть швидше досягнути caps. Telemetry: repeated pairs, directed cycles, extreme win streaks, suspicious resign patterns, purchase/reward rate. Не карати автоматично тільки за спільну IP або друзів.

Sim server завершує матч незалежно від клієнта; disconnect/resign не економить cost чи rating. Один active resolution на attacker; rate limits захищають API, не продають energy. Якщо repetition policies виявляться недостатніми, додавати server challenge variety milestones або нормалізацію ranked budget після дослідження, не непомітно зменшувати rewards.

Після power cap Credits накопичуються. v1 приймає цей ризик; cosmetic sinks/catalog expansion — наступні рішення. Repair tax, lootboxes, оплачувані boosts не використовуються для боротьби з surplus.

## Ledger invariants

Wallet entries append-only з transactionId/reason/matchId/currency/amount. Unique(matchId,beneficiary,rewardComponent); compensation — новий entry, не редагування історії. Purchase/hire/train/sell виконується атомарно разом зі state changes. Ціна snapshot прив'язана до catalogVersion; request із застарілою ціною відхиляється. Duplicate key+same body повертає попередній результат, duplicate key+different body дає conflict. UI Result читає net ledger, не обчислює виплату.
