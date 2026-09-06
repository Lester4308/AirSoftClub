> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Economy / monetization — v3

Статуси: USER APPROVED — product rule; BALANCE HYPOTHESIS — target; IMPLEMENTATION PROPOSAL — не остаточний вибір. [Balance register](BALANCE_HYPOTHESES_v3.md), [open questions](IMPLEMENTATION_QUESTIONS_v3.md).

## Currency і progression

Soft Currency (Money) — recruits, training, healing, equipment, MK2, BB, immediate recruitment refresh. Credits — premium currency: real-money purchase, обмежені gameplay grants, conversion у soft currency, premium systems. Окремої training currency та persistent Energy немає.

Battle XP → Fighter Level → training cap; stat upgrades оплачуються soft. Club XP → Club Level → progression/unlocks, більша BB capacity, кращий у середньому recruit pool. Дешевий recruit не має недосяжного development cap. Перший з 3 starter candidates безкоштовний; далі платне розширення до roster 16.

## Reward table v3

Win-equivalent спочатку визначає **Opponent Club Level**, потім modifier за actual power difference; сильніший opponent потенційно вигідніший, значно слабший — значно менша нагорода. Використовувати прийняті battle inputs, не післяматчеве unequip. Формули, межі та rounding OPEN. Team Power внутрішній; UI показує лише категорії.

| Outcome | Money | Club XP | Fighter XP кожному фактичному учаснику |
|---|---|---|---|
| Win | 100% eligible win-equivalent | 100% eligible win-equivalent | 100% eligible win-equivalent |
| Draw | Невелика часткова; пропозиція 10% (hypothesis) | **35%** standard win-equivalent | Пропозиція **35%** (hypothesis) |
| Loss | **0** | Пропозиція **0**; не confirmed coefficient | **25%** standard win-equivalent |

Спочатку outcome table, потім mode/repeat eligibility. Draw Money/Fighter XP та loss Club XP не перетворювати на final constants. Ranked draw завжди 0 rating. Eliminated учасник лишається eligible for Fighter XP. Неучасник XP не отримує.

## Steam Friend window

Для directed attacker → friend діє 8-hour window. Спроби необмежені, поки немає shield/іншого eligibility block. Одна rating-eligible win у вікні; після першої перемоги всі наступні бої не додають і не знімають rating. Повторні losses до першої win не можуть безмежно знімати rating; точний cap OPEN.

Підтверджене користувачем уточнення:
1. Перша перемога — normal eligible reward.
2. Друга і третя — reduced reward (discount OPEN).
3. Четверта і наступні — 0 Money, 0 Club XP, 0 Fighter XP до завершення вікна.

Після вичерпання reward budget нулі діють до кінця window; loss/draw/reconnect не скидає лічильник і не відновлює XP farming. Для попередніх draw/loss діє outcome table та відповідна anti-farm eligibility; повний per-outcome budget до вичерпання — Q05. Повторні challenges не означають нескінченні безкоштовні ресурси. Opposite direction має окремий pair key, але не обходить global defense cap.

Revenge win дає **standard battle reward без додаткового multiplier**, плюс окреме rating recovery 120% фактичної втрати конкретного origin battle. Tickets не скидають friend windows. IMPLEMENTATION PROPOSAL: якщо target одночасно friend, застосовувати pair reward budget також до Revenge, щоб mode switch не відновлював farm. Це ще потребує Q09 і узгодження зі standard Revenge reward до live launch; не вважати новим confirmed винятком до standard reward.

## Costs і допомога

Recruit dismissal target ~25–40%; starter free entitlement одноразовий. Immediate healing soft; free offline recovery ~1% Max HP/хв. Max HP upgrade не лікує. BB — shared club stock; attacker платить за actual shots, defense live stock недоторканний. Starter Basic target 5–7 матчів; later unlock auto-buy Basic. Emergency Basic <~15% capacity та недостатньо soft для min refill → +500 Basic раз на ~3h, лише Basic; capacity edge cases OPEN.

## Paid Steam + optional Credits

Гра платна у Steam. «F2P-аналог» у balance discussion означає власника платної гри, який не купує Credits. Credits можуть надходити обмежено за achievements, daily rewards, 7-day consecutive login streak, Club Level ups; events/milestones пізніше. Server-controlled faucet rates відкриті.

Early unlock максимум +3 Club Levels лише відкриває доступ; item купується окремо за soft. Base normal progression, MK2 expensive soft, MK3 Credits-only; MK visual + small stats. MK1 OPEN. Найсильніші BB Credits-only, effective target до ~15%; premium-only weapon/armor ~5–8%. Повний paid stack gear + MK3 + BB + інші modifiers має target ceiling ~15–20% проти нормально екіпірованого аналога того ж progression tier. Це ціль майбутніх simulations, не доведений результат.

Credits shields: 8h/1d/3d/7d; блокують нові incoming attacks, accepted battles завершуються. Чи власна Ranked attack знімає shield — OPEN. Pure cosmetics пізніше.

Правило продукту: коректно використані consumables/shields/BB/unlocks не повертаються; exceptions — technical failure, transaction not applied, platform-level Steam refund behavior. Це не скасовує платформні вимоги. Нарахування та reversal проходять authoritative ledger; [Steam commerce design](STEAM_SOCIAL_PVP_ARCHITECTURE.md).
