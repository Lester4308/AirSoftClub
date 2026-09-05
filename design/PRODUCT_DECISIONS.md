# Product decisions — v2

2026-09-05. Канон: [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Джерело рішень: [затверджені corrections](USER_APPROVED_CORRECTIONS_v2.txt). V2-номер відповідає номеру секції користувача. Options нижче показують контекст вибору; не відкривають заново затверджене рішення.

**APPROVED** — правило/напрям схвалено користувачем. **APPROVED DIRECTION / OPEN DETAIL** — можливість дозволена, конкретика не обрана. **HYPOTHESIS** — число не фіналізоване. В усіх рядках Decision належить NEW AIRSOFT DESIGN, а Original evidence не є підтвердженням нових internals.

## Active approved register

| ID / Question | Original behavior / evidence | Options | Recommendation | Decision | Reason | Impact | Status |
|---|---|---|---|---|---|---|---|
| V2-00 Product principle? | Management loop E007–E020 | близький successor / tactical reinterpretation | successor | original-game-first | Відновити потрібну DNA | Перегляд v1 всіх систем | APPROVED |
| V2-01 Battle size? | Roster16 E002/E014; deployment range UNKNOWN | equal sizes / довільні sizes | довільні | 1–16 на сторону, asymmetry | Самостійний вибір ризику | No size equality validation | APPROVED |
| V2-02 Rounds? | Auto battle/result E012/E016; точна hidden structure UNKNOWN | один / series | один | challenge=battle=one round=result | Швидкий payoff | Без round score entities | APPROVED |
| V2-03 Damage model? | HP/damage/armor E010/E020; formulas UNKNOWN | one-hit / HP | HP | MaxHP, damage, BB modifier, mitigation, hit, eliminate<=0 | Ближче до reference | Перепроєктування sim contract | APPROVED; coefficients OPEN |
| V2-04 Roster/hire? | First free E007;16 E002; rising E025 | free trio / first free | first free | max16, перший 0, решта paid rising | Progression із малого клубу | Немає fixed free trio | APPROVED |
| V2-05 Recruitment? |8cards E007; exact generation UNKNOWN | fixed / equal budget / variable | variable | приблизно 6–7, різні stats/сила/ціна | Дешевий зараз vs сильніший потім | Market offers і quality pricing | APPROVED |
| V2-06 Level quality? | Exact relation UNKNOWN | fixed pool / level variance | level variance | Club Level поліпшує distribution | Довгий progression | Діапазони перекриваються | APPROVED |
| V2-07 Reward base? | Variable rewards E016/E017; formula UNKNOWN | flat / count / club level | club level | opponent level→Base Battle Value | Цінність розвитку opponent | Reward config version | APPROVED |
| V2-08 Power metric? | Visible stats E010/E014; metric UNKNOWN | count only / derived power | derived | count, stats, HP, gear, MK, armor, BB | Реальна нерівність ширша за count | Pricing reward/ranking aid | APPROVED; formula OPEN |
| V2-09 Disparity reward? | Formula UNKNOWN | заборона / scale | scale | underdog potential↑, overpower↓ | Не забороняти unequal battles | 16vs1 legal, small payout | APPROVED |
| V2-10 Energy? | E024–E026 CONTRADICTORY | energy / operational limits | no energy | persistent energy absent | Без artificial refill gate | HP/ammo/money лишаються | APPROVED |
| V2-11 Health persistence? | HP resource E013; timing UNKNOWN | reset / persistent | persistent | Money immediate heal або timed free recovery | Health як cost choice | Recovery domain/concurrency | APPROVED; rates OPEN |
| V2-12 Credit heal? | UNKNOWN | direct premium / Money only | Money only | Credits→Money→heal; no direct button | Не дублювати currency path | Окремий exchange flow | APPROVED |
| V2-13 Currencies? | Coins/karma E006/E008 | single / dual | dual | Money working soft + Credits premium | Economy+business | Multi-currency ledger | APPROVED |
| V2-14 Credits uses? | Paid-only/early bypass UNKNOWN | cosmetics only / paid acceleration | approved uses | real money, exchange, premium, early access | Commercial design | Special reward faucet pending | APPROVED DIRECTION / OPEN DETAIL |
| V2-15 Monetization goal? | Original payment model UNKNOWN | optional / integral | integral | faster progression+collection+limited advantage | Комерційна мета | Combined stack testing | APPROVED |
| V2-16 Early unlock? | Level gates E030; bypass UNKNOWN | level only / Credits alternative | alternative | частина items доступна раніше | Платне прискорення | 7→10 example не price table | APPROVED DIRECTION / OPEN DETAIL |
| V2-17 Unique premium? | UNKNOWN | cosmetic / modest advantage / extreme power | modest | expensive premium-only можливі | Collection/status/combat value | No invulnerability/guaranteed hit | APPROVED DIRECTION / OPEN DETAIL |
| V2-18 Weapon skin? | Gear stats E008; MK UNKNOWN | cosmetic paint / visual+stats | visual+stats | Base→MK1→MK2→MK3 | Visible progression | Tier ownership і appearance linked | APPROVED |
| V2-19 MK strength? | UNKNOWN | великі multipliers / small steps | small steps | Не міняє base family на інший tier сили | Limited advantage | Caps/testing before launch | APPROVED; values OPEN |
| V2-20 BB inventory? | Persistent E013/E015 | fee / stock | stock | buy→allocate→consume→refill | Operational economy | Per-class stock/reservations | APPROVED |
| V2-21 BB classes? | Original class system UNKNOWN | single /3–5 |3–5 direction |0/3/5/10/15% illustration | Graduated cost/advantage | Catalog scale ще не frozen | APPROVED DIRECTION; numbers HYPOTHESIS |
| V2-22 Premium BB? | UNKNOWN | soft top / Credits-only top | evaluate Credits-only | top може бути Credits-only;~15% target ceiling | Modest paid edge | Damage або equivalent, not double benefit | APPROVED DIRECTION; numbers HYPOTHESIS |
| V2-23 Armor? | Armor field E010/E011; formula UNKNOWN | cosmetic / mitigation | mitigation | armor rating→damage reduction; possible mobility cost | Survivability tradeoff | Penetration optional | APPROVED; formula OPEN |
| V2-24 Friends sizes? | Friend Attack E009 | equal / any valid | any valid |1–16 vs1–16 | Соціальний вибір | Не перевіряти equality | APPROVED |
| V2-25 Friends rating? | Rating UNKNOWN | ranked / social | social | non-ranked | Прибрати direct rating farm | Money/XP окремо від rating | APPROVED |
| V2-26 Revenge? | UNKNOWN | ranked / non-ranked | non-ranked | default non-ranked, normal rewards possible | Rivalry без rating loop | Dedupe/repeat shared | APPROVED; limits OPEN |
| V2-27 Ranked? | Opponent list E014; algorithm UNKNOWN | shared / separate pool | separate | backend matchmaking; rating тільки тут | Authority і fairness | Counts можуть різнитися | APPROVED; algorithm OPEN |
| V2-28 UI? | Hub/destinations E006/E022 | replace hub / modernize | modernize | retain structure; UI analysis→redesign later | Вподобаний flow | No production UI now | APPROVED |
| V2-29 Design order? | Evidence-based reconstruction | novelty first / original first | original first | мінімальна потрібна модернізація | Не міняти механіку заради новизни | Evidence check кожного рішення | APPROVED |
| V2-30 Economy business? | Partial currencies/sinks E006–E018 | progression only / dual goal | dual goal | модель всіх sources/sinks/prices | Комерційна життєздатність | Не фіналізувати числа | APPROVED |
| V2-31 Net loop? | Ammo cost/rewards E015–E017 | always-positive / operational net | operational | gross reward−ammo cost−optional heal | Cost choices | Cashflow≠inventory valuation | APPROVED |
| V2-32 Docs sync? | v1 design repo | append conflicts / replace active | replace active | packv2+dependent specs+roadmap | Один канон | v1 redirect only | APPROVED |
| V2-33 Invalidate? | Помилкові v1 assumptions | keep alternatives active / supersede | supersede | старі conflicting rules inactive | Не допустити implementation drift | Нижче audit table | APPROVED |
| V2-34 Gate? | Implementation not begun | start code / stop design | stop design | Updated Summary + explicit next decision | Пряма межа задачі | Жодного production code | APPROVED |

## Open questions register

Recommendation у цьому розділі — **PROPOSED**, не approved. Усі вирішуються тільки для цієї гри, без Project Airsoft defaults.

| ID | Question / options | Recommendation для розгляду | Impact / gate |
|---|---|---|---|
| Q-01 | Stats naming, training payment/caps/XP/respec: original3stats чи додаткові? Money чи Credits? | Три близькі original stats; base training за Money; curves окремо | Не задавати veteran power curve до balance design |
| Q-02 | Refresh time/action, first-free pool, replacement price, potential | First free choice зі starter subset; free-first entitlement once lifetime; rising cost за total hires запобігає dismiss reset | Product review, особливо free choice серед різної якості; potential не активний |
| Q-03 | Recovery rate,0HP, recovery during sim, MaxHP upgrade interaction | Server timestamp; pause recovery for participant у accepted battle; no free full heal через stat change | Combat/recovery contract до prototype |
| Q-04 | Starter BBs/gear, ammo capacity, mixing, soft-zero fallback | Starter supplies для першого loop; no auto Credits spending; basic resupply route до paid release | Free recovery сама не вирішує empty ammo |
| Q-05 | No-ammo/stalemate/timeout/simultaneous elimination | Explicit draw/timeout policy замість series replay | Не дозволяти нескінченну simulation |
| Q-06 | Defense live resources vs isolated snapshot vs reserved defense pool | Порівняти live serialized defense і isolated defense на drain/concurrency; жоден не adopted | Human async settlement implementation заблокований до вибору |
| Q-07 | Early entitlement чи direct purchase; MK normal acquisition, level bypass depth | Явний SKU entitlement/owned item; visible normal path; bounded bypass | Commerce/pricing до activation |
| Q-08 | Slots/families, penetration, art | Compact weapon/clothing/protection; armor tradeoff; avoid extra slots без користі | Combat/UI layouts next stage |
| Q-09 | Power weights/reference HP, reward caps, XP, repetition | Actual deployment risk плюс reference-power abuse guard; monotonic level value, power-aware reduction | Не приймати v1 flat table/pair factors |
| Q-10 | Ranked rules/revenge TTL, snapshot, caps | Non-ranked revenge; current valid snapshot і one-use72h як candidate; separate ranked offers | K/TTL/quota поки не затверджені |
| Q-11 | Prices, F2P/upfront, special Credits, refund debt, combined premium cap | Моделювати paid/free cohorts і threshold effects; no special Credits faucet by default | No live paid economy до review |
| Q-12 | Offline/DB/engine/storage/moderation/next implementation scope | Modular authority і read-only cache; окреме technical ADR після дозволу | Не переносити календар/stack іншого проєкту |

## SUPERSEDED v1 register

Всі PD-01–PD-24 v1 як набір замінені цим реєстром. Сумісні принципи (окремий продукт, Steam verified auth, immutable inputs) збережені в актуальних specs; старі coefficients не успадковуються.

| Prior rule / ID | Status | Replacement / reason |
|---|---|---|
| PD-06 mandatory3v3; PD-17 size equality | SUPERSEDED | V2-01/24/27:variable1–16, asymmetric |
| PD-05 max6/MVP4/free starters | SUPERSEDED | V2-04/05:16, one free, paid recruits |
| PD-04 one-hit | SUPERSEDED | V2-03:HP/Damage/Armor |
| PD-14 best-of-three/round wins | SUPERSEDED | V2-02:one battle |
| PD-13 full reset/no paid heal | SUPERSEDED | V2-11/12:persistent HP, Money heal/time |
| PD-09 fixed15cost/no BB inventory | SUPERSEDED | V2-20:persistent classes/consumption |
| PD-10 Credits=soft/single currency | SUPERSEDED | V2-13/14:soft+premium Credits |
| PD-11 no purchasable advantage/upfront-only recommendation | SUPERSEDED | V2-15–17:integral monetization, limited advantage; entry model OPEN |
| No planned monetization direction | SUPERSEDED | Не активний; v1 насправді згадував upfront sale, але його модель не v2 |
| Purely cosmetic weapon skins / cosmetic-only paid gear | SUPERSEDED | V2-18/19:MK visual+stats; не забороняє окрему чисту косметику пізніше |
| PD-03/12 equal budget24, max36,12upgrades, fixed XP | SUPERSEDED | V2-05/06; training caps/curves Q-01 |
| PD-02 dashboard replacing hub | SUPERSEDED | V2-28/29:original structure first |
| Flat60/40/30 rewards, guaranteed nonnegative net | SUPERSEDED | V2-07–09/31:level/power/actual costs |
| Ranked K24, ratio0.8–1.25, quota5/24h as default | SUPERSEDED | Q-10:новий policy review, без forced symmetry |
| Fixed AR/SMG/DMR tables, slot/maxXP gates | SUPERSEDED | Q-07/08:new content/balance after review |
