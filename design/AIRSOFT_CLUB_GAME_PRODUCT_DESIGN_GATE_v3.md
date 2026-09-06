> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# AIRSOFT CLUB GAME — PRODUCT DESIGN GATE v3

Дата: 2026-09-06. База pass: 3f76545. **PRODUCT DESIGN GATE v3 — FINALIZED FOR IMPLEMENTATION PREP.**

Джерела рішень: [текст користувача, пункти 0–76](USER_APPROVED_DECISIONS_v3.txt) та [уточнення friend rewards](USER_CLARIFICATION_v3.md). [Реєстр покриття](PRODUCT_DECISIONS.md). Позначення: CONFIRMED — рішення користувача; BALANCE HYPOTHESIS — не final balance; OPEN / IMPLEMENTATION PROPOSAL — не затверджена реалізаційна деталь.

## 1. PROJECT BOUNDARY

Airsoft_Club_Game — нова незалежна гра з власними repository, folder, Git history, architecture, roadmap, design, combat, economy, backend та Steam integration. [PROJECT_BOUNDARY.md](../PROJECT_BOUNDARY.md) обов'язковий. Project Airsoft / Airsoft Manager не змінювати та не використовувати як успадковану основу. Reuse code/data/rules/assets/terminology не дозволено автоматично; [реєстр reuse](REUSE_DECISIONS.md) не містить прийнятого перенесення.

## 2. CONFIRMED PRODUCT RULES

Management → preparation → opponent selection → automatic battle → result → progression. Clean-room successor management DNA «Пейнтбольные войны» у airsoft setting. Один challenge = battle = round; ручної стрільби немає.

Roster максимум 16. Усі purchased combat-ready fighters беруть участь автоматично; manual active squad немає, асиметрія 1v2/2v5/7v16/16v16 природна. Немає виключення через слабку/відсутню екіпіровку.

Accuracy, Endurance, Agility — рівно 3 base stats. XP → Fighter Level → training cap → soft-paid stat upgrades; окремої training currency немає. XP лише фактичним учасникам, включно з eliminated. Дешевий recruit може стати ветераном, hidden unreachable development caps заборонені.

Перший fighter: один з 3 free candidates. Звичайний pool 6–7 varied candidates, male/female variants, ціна/starting strength tradeoff. Club Level покращує середню якість зі збереженням variance. Refresh time/matches, resale share та XP curve — hypotheses.

Retention: Daily Rewards, 7-day consecutive streak, recruitment refresh, recovery timer, Revenge, Club Level, Steam rivalry. Moderation foundation для club names, reports, sanctions, emblem якщо UGC; без overbuild першого milestone.

## 3. CONFIRMED COMBAT RULES

HP/Damage/Armor; elimination Current HP <= 0, кілька влучань допустимі. Offense зберігає Current HP. Soft immediate healing або free timed recovery, включно з offline elapsed time. Max HP upgrade не лікує: 80/100 → 80/110. Fighter з 0 HP не атакує до readiness threshold; величина і точна readiness policy ще визначаються. Persistent Energy відсутня.

4 slots: Weapon, Camouflage, Head Protection, Load-bearing/Armor. Head — goggles/masks/helmet; load-bearing — chest rig/plate carrier/tactical vest. Light/medium/heavy protection має зростаючий Agility tradeoff. Проста penetration interaction. Weapon families v1: Pistol, SMG, Assault Rifle, Shotgun, DMR, Sniper Rifle, gradual unlocks.

BB shared club resource, capacity росте з Club Level, один active class на всю команду. Actual simulated shots атакуючого списують real BB. Defense використовує snapshot class/budget без live debit.

Win: противник втратив усіх боєздатних fighters. Draw: одночасна остання elimination обох сторін, обидві без ammo/можливості завершити бій, safety cap без winner. Cap внутрішній. [Combat spec](AIRSOFT_COMBAT_SYSTEM.md).

## 4. CONFIRMED ECONOMY RULES

Нагороди: Money, Club XP, Fighter XP. Base Battle Value насамперед Opponent Club Level, потім actual power difference. Internal power враховує count/stats/HP/weapon/MK/armor/head/camo якщо релевантно/BB; exact formula OPEN, UI тільки приблизна категорія.

Draw Club XP **35% standard win-equivalent**; Money невелика часткова, Fighter XP за [v3 table](AIRSOFT_ECONOMY.md). Draw Money 10% і Fighter XP 35% у таблиці — лише пропозиції. Loss Money **0**, Fighter XP **25%**; loss Club XP 0 — окремо позначена пропозиція, не затверджений коефіцієнт.

Starter Basic на 5–7 матчів — target. Later AUTO-BUY BASIC BB. Emergency дає тільки Basic за low-stock + insufficient-soft conditions; thresholds/quantity/cooldown hypotheses. Dismissal лише часткова компенсація. Не вводити direct healing за Credits замість схваленого soft healing.

## 5. CONFIRMED PVP / STEAM RULES

Friends unlimited attempts за eligibility. Directed attacker → friend window **8h**, **одна rating-eligible win**. Після першої win усі наступні pair battles мають нульовий rating impact. До win losses retry, але rating loss bounded, exact cap OPEN.

Уточнена reward sequence: **перша win full, друга/третя reduced, четверта+ 0 Money/Club XP/Fighter XP до window end**. Exhaustion не скидається loss/draw; discount OPEN.

Defense immutable snapshot містить fighters/stats/gear/BB class/rating/ruleset/balance. Доступні defense fighters стартують **100% Max HP**, навіть live wounded. Defense не змінює live HP/BB. Concurrent attackers можуть використовувати одну version; committed heal/equip дає новішу version лише для нових battles.

Exposure максимум **4 rating-impacting incoming attacks/profile/24h**. Concurrency accounting/post-cap visibility OPEN. Credits shield **8h/1d/3d/7d** блокує нові incoming attacks; accepted battles завершуються. Own Ranked attack cancels shield — **OPEN**.

Ranked: backend list **3–5 candidates**, вибір гравця; Club Level/rating/count/approximate strength/potential reward category, без exact Team Power. Ranked draw **0 rating**.

Revenge після defense loss: **24h**, до **3 attempts**, success consumes. Success recovery = **120% фактичної origin rating loss**, -10 → +12; standard battle reward без extra multiplier. Counterparty rating, draw-attempt accounting, cross-mode abuse і cap interaction OPEN. [Steam/PvP architecture](STEAM_SOCIAL_PVP_ARCHITECTURE.md).

## 6. CONFIRMED MONETIZATION RULES

**Paid on Steam + optional Credits.** Порівняння з «F2P» означає власника платної гри без купівлі Credits. Credits: real-money purchase, limited gameplay grants, conversion у soft, premium systems. Free sources: achievements, daily, 7-day consecutive streak, Club Level ups; events/milestones later, controlled faucets.

Early unlock через Credits: не більше **+3 Club Levels**, лише access entitlement; сам item окремо купується soft. Base normal, **MK2 expensive soft**, **MK3 Credits-only**, visual + small stats; MK1 OPEN.

Найсильніший BB Credits-only, target effective advantage ~15%, без x2 damage/guaranteed hit/ignore-all-armor. Premium-only weapons/armor дозволені з target ~5–8%. Combined paid stack target ceiling ~15–20%, а не сума незалежних дозволених максимумів.

Коректно використані consumables/shields/BB/unlocks не повертаються за product policy; exceptions technical failure/unapplied transaction/platform Steam refunds. Pure cosmetics later, не пріоритет v1.

## 7. CONFIRMED TECH STACK

| Layer | Підтверджене рішення |
|---|---|
| Client | Unity / C# |
| Backend | C# / ASP.NET Core |
| Database | PostgreSQL |
| Platform | Steam-first |
| Integration | Steamworks через відповідний C# integration layer |

Shared C# DTO/contracts/domain primitives допустимі там, де безпечно; client не trust source. Backend authoritative для identity, Credits, sensitive economy, PvP/rating/matchmaking, battle resolve/verify, progression, snapshots, leaderboards.

Critical operations online required. Offline launch/read-only club cache можливі, isolated PvE потенційно later; offline не видає authoritative economy/XP/rating/Credits. Exact versions/adapters OPEN. [Backend](BACKEND_ARCHITECTURE.md), [data model](DATA_MODEL.md).

## 8. BALANCE HYPOTHESES

[Повний реєстр B01–B12](BALANCE_HYPOTHESES_v3.md): refresh ~1h/10matches; resale ~25–40%; recovery ~1%/min; readiness ~10%; starter Basic 5–7matches; emergency <~15%, +500/~3h; BB 3–5 tiers 0/~3/~5/~10/~15%; premium gear ~5–8%; combined ~15–20%; XP/training/reward/price/power/rating curves.

Вони не є hidden final constants. Hard coefficients draw Club XP 35%, loss Fighter XP 25%, Revenge 120% та explicit product limits не переводити назад у «довільний баланс».

## 9. OPEN ART QUESTIONS

Final production art style **OPEN**. Попередні concepts reference-only. Preferences для майбутнього pass: classic 2D, side/3/4 fighters, stylized/low-poly-inspired 2D, head ~30% bigger, readable weapon/helmet/chest rig, універсальні male/female bases, maps для 16v16. Це не затверджена final direction. Повернутися наприкінці core design / implementation foundation; production art зараз не створюється.

## 10. REMAINING IMPLEMENTATION-LEVEL QUESTIONS

[Q01–Q15](IMPLEMENTATION_QUESTIONS_v3.md) містять формули/детермінізм/readiness/беззбройних fighters, defense ammo budget, reward coefficients, friend-window anchor/loss cap, global exposure races, shield-own-attack, Revenge corner cases, MK1, prices/faucets, versions та persistence lifecycle.

Питання shield-own-attack і MK1 потребують явного подальшого продуктного уточнення перед відповідними системами. Відкриті питання не означають дозвіл самовільно обрати остаточні правила. Перед майбутнім core implementation конкретизувати Q01–Q04 у versioned prototype fixtures.

## 11. SUPERSEDED DECISIONS

Неактивні: mandatory 3v3; max roster 6; manual active squad; one-hit elimination; non-ranked-only Steam Friends; default non-ranked-only Revenge; purely cosmetic-only MK; 3 slots; final 3D direction; Project Airsoft reuse assumptions; невизначений stack; live defense HP/BB drain; direct purchased early-unlock item без soft purchase.

V1/v2 product files — SUPERSEDED pointers. Точні попередні тексти доступні в Git history на 3f76545; historical source evidence не переписується у новий «факт оригіналу».

## 12. GATE STATUS

**PRODUCT DESIGN GATE v3 — APPROVED FOR IMPLEMENTATION PREPARATION**

**IMPLEMENTATION NOT YET AUTHORIZED.**

Пакет узгоджено на рівні затверджених product rules; hypotheses та OPEN питання явно відділені. Approval стосується підготовки scope, а не коду чи твердження про готовий баланс. [Validation](VALIDATION.md) фіксує перевірки цього pass.

Рекомендований майбутній перший milestone: Unity/C# foundation, pure C# Domain, deterministic headless battle core, fighter/weapon/armor/BB models, MatchConfig/MatchResult, seeded RNG, reproducible tests. **Не включати** production UI, Steamworks integration, live ASP.NET Core backend, PostgreSQL, monetization/Credits purchases, matchmaking, production art. [Повний scope](MVP_IMPLEMENTATION_PLAN.md). Жодної реалізації в цьому pass.
