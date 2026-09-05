# Airsoft economy — v2

USER APPROVED V2-04–09/11–22/30/31. [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). **Дві валюти; монетизація закладена від початку. Ціни й числові формули не final.**

## Currency ownership

Soft currency working name:Money, final label Q-11. Premium currency:Credits, не Credits-v1-soft. Money і Credits зберігаються окремо; ніякої автоматичної конверсії старих прикладів із документації в real balances.

| Resource | Sources | Sinks / use | Approval boundary |
|---|---|---|---|
| Money | Battle rewards; Credits exchange; onboarding grant якщо визначено | Recruit, normal weapon/armor/BB, immediate healing; training/upgrade routes після уточнення | Exact prices/grants/training Q-01/Q-04 |
| Credits | Real-money purchase через Steam; special rewards лише після approval | Exchange to Money, premium items, early unlock, MK routes, potential premium BB | Packs, exchange rate, SKUs OPEN |
| Club/Fighter XP | Normal battle progression | Level/unlocks/training gates | Не валюта; curves/reward allocation OPEN |
| BB classes | Shop purchase/grant, unspent reservation return | Fire expenditure | Persistent consumable, capacity/loadout OPEN |
| HP | Timed free recovery; Money heal | Battle damage | Persistent, не energy |
| MK/early-access entitlement | Approved purchase/upgrade route | Не consumable currency; впливає на доступ/gear | Exact entitlement semantics OPEN |

Немає direct heal-for-Credits. Exchange і healing окремі reviewed commands. Special Credits rewards не вмикаються лише тому, що поле currency може їх підтримувати. Player trading/gifting не затверджені.

## Recruitment economy

First hire free once. Subsequent cost зростає при порівнянній якості, інакше stronger candidate може коштувати значно більше за weak candidate на пізнішому hire. Approx6–7 offers; Club Level змінює distribution з overlap.

Ціну описують принаймні hire progression, candidate quality, catalog/recruitment version. Current roster count vs lifetime hire progression Q-02. Free-first dismissal reset та reopen reroll farming мають бути закриті до public market. Немає фіксованих 100/300/600 нових цін або equal budget quality.

## Reward foundation і power adjustment

Conceptual dependency,**не фіналізована числова формула**:

OpponentClubLevel → BaseBattleValue.
BaseBattleValue + Outcome + RelativeTeamPower + Repeat/AbuseContext → GrossMoney/XP.

Base value зростає за level як головним foundation, але слабкий навмисно stripped defense високого level не гарантує payout. Relative power зменшує value overpower wins, може підвищити potential underdog value. Number of fighters — лише компонент. У 16strong vs1weak reward дуже малий; не вводити size ban.

Power inputs:actual count, stats, HP, weapon/base family, MK, armor, BB class і real combat modifiers. Версія metric і input captures зберігаються. CurrentHP потрібний для actual risk; reference full-health/loadout power як possible anti-abuse guard Q-09. Не давати безкоштовний underdog boost через навмисне ушкодження/переодягання самого attacker.

Outcome/loss payout, XP vs Money scaling, cap underdog бонусу, рівень близькості, повторні пари/collusion thresholds OPEN. Малий reward за кожен unlimited scripted weak battle все одно масштабується у farm; потрібні repetition і automation protections, але не artificial energy bar. Friends/revenge/ranked поділяють abuse identity/pair context; rating лишається ranked-only.

## Operational net: purchase не дорівнює consumption debit

Cashflow view:wallet delta = gross Money reward − Money purchases/heal, які фактично виконані у періоді.
Economic view:gross reward − вартість витрачених BB − опціональний recovery expense.

BB stock оплачено під час купівлі; battle зменшує quantity. Replacement value у Result — estimate, не повторний money debit. Credits-only BB показує витрату quantity та Credits cost basis; не приховувати її в Money net за вигаданим курсом. Stock може бути старим/придбаним за іншу ціну, тому valuation basis має бути явним.

HP зберігається; instant Money heal — choice. Free timed recovery знижує cash cost ціною очікування, але не поповнює BBs. Empty money+BB може створити тупик; starter supply/emergency base-resupply route Q-04 є gate до paid launch. Guaranteed positive net кожного бою не затверджений.

## Commerce як частина progression

Pay to progress faster + premium collection + limited power advantage. Credits→Money прискорює найм/heal/gear; early unlock обходить частину level wait; premium item і MK дають modest edge. Тому balance comparison має враховувати швидкість roster growth і підтримку HP, а не лише weapon damage.

Early access scope, normal availability, MK Money alternative, premium-only top BB і вхідна модель гри Q-07/Q-11. Немає припущення free players отримають кожен premium-only item; немає й припущення, що без оплати неможливо виграти.

## Hypotheses і combined stack

BB класів 3–5; приклад 0/+3/+5/+10/~+15%damage або equivalent. Це не price list і не guarantee рівно+15%win rate. Armor interaction/hit thresholds можуть посилити або послабити effect. MK increments і premium item edge без чисел. Потрібен окремий combined ceiling:BB cap не поглинає automatic MK/early unlock/accuracy/cadence.

No x2damage, no guaranteed kill/hit, no invulnerability, no all-armor-ignore premium effect. Не використовувати price як доказ балансності. Верхній edge тестується free/paying cohort на однаковому Club Level і окремо на однаковому elapsed playtime.

## Business model worksheet для наступного design етапу

| Model component | Inputs без заданих constants | Outputs для review |
|---|---|---|
| Money flow | Level/power reward, outcome mix, BB class shots, healing choice | Net/session, zero-resource frequency, save time |
| Credits flow | Pack pricing, conversion, SKU costs, special grants якщо прийнято | Credits sources/sinks, balance liability |
| Recruit progression | Hire count, quality distribution, prices, training alternative | Time-to-next-recruit, roster width/quality |
| Equipment | Weapon/armor levels, early unlock, MK prices | Time saved, power distribution |
| Consumables/recovery | BB cost/use, HP damage, heal prices, recovery rate | Operating margin, payer sustain advantage |
| Commercial viability | Active players, payer conversion, spend, retention, platform/net receipts assumptions | Scenario revenue minus operating/content costs |
| Abuse | Repetition, high-level weak defense, underdog manipulation, alt rings | Payout leakage і policy tradeoffs |

Не прогнозувати дохід без даних і не фіналізувати ціни до scenario simulation/testing. В рамках correction pass моделі описані, не виконані.

## Transaction invariants

WalletBalance keyed(player, currency), append-only ledger. Exchange atomically дебетує Credits і кредитує Money за versioned quote; retry не дублює. Healing materializes server recovery, calculates missing HP/quote, дебетує Money і оновлює HP once. Stock nonnegative після reserves; settlement consumes actual spent та releases unspent.

Payment grant прив'язаний до verified Steam order transaction, а не client success callback. Refund/chargeback після Credits conversion/consumption вимагає provenance і явної reconciliation policy Q-11; не редагувати старі matches. Details: [Backend](BACKEND_ARCHITECTURE.md),[Steam](STEAM_SOCIAL_PVP_ARCHITECTURE.md).
