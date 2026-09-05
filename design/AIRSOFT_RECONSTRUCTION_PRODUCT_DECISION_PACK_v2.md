# AIRSOFT RECONSTRUCTION PRODUCT DECISION PACK v2

2026-09-05 · **CORRECTION PASS / PRODUCT-DESIGN GATE** · Airsoft_Club_Game.

Канонічна версія. Затверджені користувачем corrections замінюють суперечливі рішення v1. Джерело: [USER_APPROVED_CORRECTIONS_v2.txt](USER_APPROVED_CORRECTIONS_v2.txt). Проєкт незалежний; [Project boundary](../PROJECT_BOUNDARY.md) обов'язкова. Implementation потребує окремого наступного рішення.

## 1. Пріоритет і статуси

**Modernized successor:** максимально близькі core gameplay, progression, economy та UI flow дослідженої VK-гри, із новим airsoft-сетингом, новими assets/code та Steam social/asynchronous PvP.

Порядок кожного design decision: перевірене історичне правило → чи працює воно для цього продукту → зберегти, якщо так → модернізувати лише необхідне. Модернізація не є підставою автоматично замінювати механіку складнішим варіантом.

| Позначка | Значення |
|---|---|
| ORIGINAL CONFIRMED | Історичне спостереження з EVIDENCE_LEDGER; не схвалення нової формули |
| ORIGINAL STRONGLY SUPPORTED | Сильні непрямі докази, не встановлені internals |
| NEW AIRSOFT DESIGN — USER APPROVED | Прямо затверджений користувачем напрям/правило нового продукту |
| RECONSTRUCTION DECISION — PROPOSED | Технічна пропозиція для потрібної поведінки, ще не implementation |
| BALANCE HYPOTHESIS | Число/крива/поріг для майбутньої перевірки |
| OPEN QUESTION | Не погоджена деталь; жоден із варіантів не є активним правилом |
| SUPERSEDED | Скасована v1 вимога, не застосовувати |

CONFIRMED у [Updated Summary](AIRSOFT_CLUB_GAME_UPDATED_PRODUCT_DECISION_SUMMARY_v2.md) означає user-approved product decisions; історична доказовість там позначена окремо. UNKNOWN/CONTRADICTORY архіву не підвищуються через бажання схожості.

## 2. Межа реконструкції

Підтверджені: free first fighter, найм/тренування, три основні stats, gear slots, обов'язкова маска, HP/damage feedback, persistent ammunition, автоматичний бій і Skip, hub та великі destinations, opponent/friend cards, rewards (E002, E007–E023). Наступні 100/300 за recruit у гайді — STRONGLY SUPPORTED E025; не нові ціни.

16 у roster підтверджено E002/E014. **Deployment до 16 та всі асиметричні комбінації — тепер пряме USER APPROVED рішення**, а не новий доказ, що всі вони були історично перевірені. Exact hit/armor/reward/recovery formulas UNKNOWN. Historical asynchronous model STRONGLY SUPPORTED; конкретний snapshot schema — наша реконструкція.

Coins/karma видно, але paid-only currency, real-money purchase й early level bypass оригіналу не доведені наявними матеріалами. У новій грі Credits, early unlock і premium advantage затверджені користувачем незалежно від цієї прогалини. Докази: [RESEARCH_BASIS.md](RESEARCH_BASIS.md).

## 3. Склад і масштаб бою — V2-01, V2-02, V2-04

- Максимум клубу: **16 fighters**.
- Deployment: гравець обирає **будь-яку кількість доступних власних бійців від 1 до 16**.
- Opponent defense selection також має 1–16; сторони не повинні бути однаковими.
- Дозволені 1v1, 1v2, 1v3, 2v1, 2v3, 3v1, 3v5, 5v8, 10v16, 16v16 та всі інші допустимі пари.
- Один challenge → **один battle / один раунд → один результат**.
- Немає серій раундів, round wins чи обов'язкової трійки.

Unavailable fighter може означати HP=0, відсутній потрібний gear/ammo або зайнятість прийнятим боєм за майбутньою concurrency policy. Частково поранений fighter з HP>0 може брати участь; не вимагати full heal перед кожним challenge. Точний gear/ammo readiness threshold — Q-04.

Мала досвідчена/споряджена команда може бути ціннішою за велику слабку. Quantity — лише один чинник; найм не дає автоматичної перемоги. Зменшення deployment не обнуляє anti-farming history.

## 4. Найм і розвиток — V2-04–06

Перший fighter безкоштовний; кожен наступний — платний. Slot-related частина вартості зростає: другий дешевший, третій дорожчий і далі. Одночасно ціна залежить від якості кандидата: дорогий сильний другий recruit може коштувати більше за слабкого третього. Порівнювати зростання slot cost при однаковій якості, не нав'язувати монотонність кожній фактичній покупці.

Recruitment screen показує приблизно **6–7 candidates** з різними starting stats, силою, ціною та appearance. Дешевий зараз або накопичення на значно кращого — центральний вибір. Equal-stat-budget candidates v1 більше не використовуються.

Club Level покращує розподіл якості пулу: на високому рівні можливі сильніші recruits, але діапазони перекриваються. Не кожен новий кандидат кращий за розвиненого старого. Development potential — **OPEN**, його не генерувати як приховану характеристику до схвалення.

Рекомендація, не нове затвердження: залишити три trainable stats близько до Accuracy / Agility / Endurance; Endurance пов'язати з Max HP, Agility із hit avoidance/tempo, Accuracy із hit calculation. Naming, training payment, XP curve, caps, respec та retention цінності ветерана — Q-01/Q-02. Старі caps 12 upgrades/20 stat/36 budget і fixed XP table вилучені з активного baseline.

## 5. Combat: HP, damage, armor — V2-03, V2-23

**HP + Damage + Armor / Protection** замінюють one-hit elimination. Бійці витримують декілька влучань; elimination при HP <= 0. Це arcade airsoft abstraction, не твердження про правила реального спорту або захисні властивості спорядження.

Потрібний причинний pipeline без фінальних коефіцієнтів:

1. Визначити action/target за затверджуваними ruleset та stats.
2. Перевірити і витратити ammunition для fire action.
3. Hit calculation із fighter accuracy, weapon/MK modifiers та target avoidance.
4. На hit отримати weapon damage з MK contribution.
5. Застосувати BB damage/ballistic modifier за обраним порядком.
6. Armor mitigation; penetration interaction, якщо її окремо обрано.
7. Зменшити HP; зафіксувати damage, remaining HP та elimination.
8. Завершити один battle, коли сторона вибула; edge outcomes Q-05.

Потрібні Max HP, Current HP, damage, armor rating, effective hit chance та ammunition class. Немає фінальної hit chance, tick rate, damage floor, mitigation cap, target weighting чи damage multipliers у v2. Deterministic server simulation і versioned replay залишаються технічним напрямом.

Система повинна завершувати zero-ammo/stalemate/simultaneous elimination cases, але timeout/draw policy ще не затверджена. Не підміняти її старим round survivor scoring.

## 6. Persistent health і Recovery — V2-10–12

Damage залишається після бою: fighter повертається з фактичними HP, а не автоматично healthy. **Persistent Energy відсутня.**

Два шляхи відновлення: immediate heal за **soft currency** або free automatic recovery із часом. HP=0 не означає permanent death; recovery має дозволяти повернення, точний старт/rate — Q-03. Server time — джерело часу, не local clock.

**Немає Heal for Credits.** Дозволений ланцюг Credits → soft money → healing; conversion має бути окремою зрозумілою операцією, без прихованої автоматичної premium витрати.

Поранення, BB stock, money, readiness та opponent availability формують operational pressure. Не гарантуємо positive net після кожного бою. Free recovery не створює BBs: emergency ammo/soft-zero fallback потребує окремого рішення Q-04, не paid-only тупика.

Offline defense HP/ammo persistence — Q-06: рішення про збереження post-battle damage обов'язкове для керованої участі гравця; як саме застосувати його до множинних offline challenges проти snapshot, користувач не визначив. Не додавати мовчки безкоштовний virtual defense або глобальний drain.

## 7. Дві валюти і бізнес-модель — V2-13–17, V2-30

Soft currency: робоча позначка **Money / soft money**, final name OPEN. Premium currency: **Credits**. Credits більше не назва звичайних грошей v1.

Credits купуються за реальні гроші, конвертуються в Money, використовуються для premium purchases та раннього доступу до частини progression. Special Credits rewards — лише якщо окремо схвалено. Звичайна battle reward — Money/XP, не автоматичний Credits faucet.

Монетизація — обов'язкова частина product/economy architecture з початку: **pay to progress faster + premium collection + limited power advantage**. «Не гарантована перемога» не означає, що грошової переваги немає: conversion прискорює recruitment/training/healing, early unlock дає доступ до gear, а paid modifiers можуть складатися. Потрібно вимірювати увесь ефект.

Early unlock: item, що normally відкривається за Club Level, можна отримати раніше за Credits. Приклад Level 7 → item gate 10 — **ілюстрація, не каталог AR або готова ціна**. Модель unlock entitlement vs direct purchase, глибина bypass та доступність MK за Money — Q-07.

Expensive premium-only items дозволені як напрям: collectible look/status + невеликий combat advantage. Конкретні предмети/ціни/advantages не затверджені. No guaranteed hit, invulnerability, x2 damage або guaranteed kill. Business entry price (paid game чи F2P) OPEN; колишня premium-upfront-only рекомендація не є default.

## 8. Weapon MK і equipment — V2-18, V2-19, V2-23

Weapon Base → MK1 → MK2 → MK3; подальші tiers можливі тільки після окремої оцінки. Кожний MK змінює visual skin та дає невелике gameplay покращення: damage, accuracy або handling. Це **не cosmetic-only skin system**. Credits можуть відкривати tiers.

MK зберігає характер базової weapon family; не перетворює її на інший клас сили через великі множники. Exact modifiers та cumulative ceiling — BALANCE HYPOTHESES, ще без чисел.

Рекомендована компактна слотна структура: Weapon, tactical clothing/armor, eye/face protection; точний набір — Q-08. Armor зменшує damage; сильніший armor може знижувати mobility. Eye protection requirement зберігається як readiness principle; не прив'язувати весь armor до «платного захисту очей». Weapons families/unlock cadence не успадковують AR/SMG/DMR level table v1.

## 9. Persistent BB classes — V2-20–22

BBs купуються, зберігаються, призначаються бійцям/команді, витрачаються в battle і поповнюються. Немає fixed match supply fee замість inventory. Не списувати двічі ammo cost: purchase витрачає money, combat споживає stock.

| Working class | Ілюстративний modifier | Статус |
|---|---|---|
| Base BB | +0% | BALANCE HYPOTHESIS |
| Improved BB | приблизно +3% | BALANCE HYPOTHESIS |
| Advanced BB | приблизно +5% | BALANCE HYPOTHESIS |
| High-end BB | приблизно +10% | BALANCE HYPOTHESIS |
| Premium BB | максимум приблизно +15% damage або еквівалент | BALANCE HYPOTHESIS / target ceiling |

Напрям 3–5 classes; таблиця показує п'ять можливих, не зобов'язує одразу продавати всі. Найсильніша BB може бути Credits-only; exact catalog decision OPEN. Penetration — альтернатива або окремо обмежений внесок, не +15% damage плюс повне ігнорування armor.

Ціль +15% для BB **не є автоматичним cap для всього premium loadout**. Damage/MK/accuracy/cadence/armor/early unlock комбінації можуть дати нелінійний ефект і перейти threshold hits-to-eliminate. Потрібні combined-stack тести перед цінами й release.

## 10. Rewards і Team Power — V2-07–09, V2-31

Foundation: **Opponent Club Level → Base Battle Value → power disparity adjustment → outcome/repetition/abuse policy → gross reward**.

Team Power враховує actual deployed count, fighter stats, поточні/максимальні HP, weapons, MK tier, armor, ammunition class та інші combat-relevant modifiers. Формула internal/versioned; це assistance metric, не прогноз точної перемоги.

Перемога над сильнішим opponent може підвищувати potential reward. Overpower win значно зменшує reward. **16 сильних проти 1 слабкого дозволено**, але payout дуже малий. Рівність sizes/power не є передумовою законного challenge. Високий Club Level з навмисно слабким defense не гарантує вигідного farm.

Рекомендація для захисту від health sandbagging: рахувати current battle power для actual risk і separate full-health/reference power для abuse guard, не дозволяти навмисне недолікування автоматично підвищувати payout. Формули, caps і XP-vs-Money scaling — Q-09, не завершений баланс.

Battle → reward → фактичний ammo consumption cost → можливий heal → заощадження на recruit/gear/upgrade → progression → next battle. Result відокремлює gross Money, inventory consumption, replacement cost estimate, optional healing quote та фактичний wallet delta. Premium BB cost не переводити в Money без явно вказаного valuation; cashflow і economic cost різні.

## 11. Social, Ranked, Revenge — V2-24–27

Steam Friends — ключовий **non-ranked social mode**, будь-які доступні 1–16 на сторону, друг offline допустимий. Avatar/Steam name/Club name окремі, показані deployed count, level, power estimate та reward preview.

Revenge — default non-ranked; може давати normal Money/XP за загальною anti-farm policy. Не обнуляє повтори й не створює rating. Ranked — окремий backend-selected opponent pool; лише тут rating changes, якщо інший ruleset не буде окремо схвалено.

Matchmaking може ранжувати пропозиції за level/power/rating, але не нав'язує equal counts. Точні ranked bands/algorithm/defense exposure caps — OPEN; жодні старі 3v3, ratio0.8–1.25, K24, five-defenses/24h не є активними затвердженими числами.

Revenge 72h, once-per-ticket і current snapshot — перенесені **PROPOSED** технічні/product defaults, а не нові USER APPROVED constants. Expiry/rating policy розділені; зміна team size не скидає pair counters.

## 12. Backend і paid economy integrity

Verified Steam ticket → internal UUID / SteamID64 → club/profile. Client не authority для HP, BBs, wallet, reward, rating або payments. Domain boundaries: Auth, Club/Fighters, Recruitment, Training, Gear/MK, Ammo, Recovery, Money/Credits, Commerce, Snapshots, Battle/Settlement, Matchmaking, Rating, History/Inbox.

Acceptance atomically pins immutable attacker/defender snapshots, reserves attacker BBs і керованих бійців за обраною policy. Current HP матеріалізується на server acceptance time; далі input не пливе під час replay. Settlement once applies spent ammo, post-battle HP, reward/XP та ranked delta. Healing/equip після acceptance не змінюють старий input. Defense resource concurrency залишається Q-06 і блокує human-async resource implementation до рішення.

Commerce: окремі order/transaction IDs, verified provider state, idempotent grants, Money/Credits conversion ledger, early-access entitlement, weapon tier history, refunds/reconciliation. Документовані Steam purchase mechanics і наші proposed boundaries: [Steam architecture](STEAM_SOCIAL_PVP_ARCHITECTURE.md). Поточний correction pass не здійснює платежів.

Offline cache/practice без імпорту wallet лишається технічною пропозицією. Free recovery може нараховуватись сервером за elapsed offline time при reconnect; local clock не джерело authority. Architecture/DB vendor/engine, retention і moderation details не переносяться з Project Airsoft.

## 13. UI: original structure first — V2-28, V2-29

Зберегти зрозумілий hub, великі locations/screens, команду/персонажа в центрі, явні Money/Credits/BB/HP, destinations Shop / Training / Recovery / Club. Team/recruitment — окремий зрозумілий шлях.

Наступний окремий етап: **ORIGINAL UI ANALYSIS → MODERN AIRSOFT UI REDESIGN**. Типографіка, layout, animation, responsive/focus behavior сучасні, art новий. Короткі переходи чи shortcuts можуть доповнювати hub; не оголошувати city/hub скасованим на користь mandatory dashboard tabs. Production UI, 3D direction та нові макети зараз не реалізуються.

## 14. Roadmap і gate — V2-32–34

Поточний результат: v2 pack + synchronized dependent documents + Updated Summary з CONFIRMED / BALANCE HYPOTHESES / OPEN QUESTIONS / SUPERSEDED DECISIONS.

Майбутня послідовність: product clarification → окремий UI analysis → balance/economy scenario design → окреме рішення implementation → foundation → one-fighter complete loop → variable deployments до 16 → durable HP/ammo/two-currency economy → Steam/social → rated pool/revenge → commerce verification → release QA. Commercial model у документах/схемі від початку; real-money checkout не потрібний для першого local prototype, але монетизація не факультативне post-launch доповнення.

MVP не фіксує 3v3: початковий 1v1 — onboarding slice; asymmetric1v2/2v1 та boundary16v16 — обов'язкові подальші acceptance cases. [ROADMAP](../ROADMAP.md), [implementation plan](MVP_IMPLEMENTATION_PLAN.md).

## 15. Залежні рішення і завершення correction pass

Всі V2-00–V2-34 простежуються до пунктів corrections у [PRODUCT_DECISIONS.md](PRODUCT_DECISIONS.md). Open questions Q-01–Q-12 та superseded register містяться там само. v1 лишається тільки redirect/SUPERSEDED notice; попередній текст доступний у Git history.

**Зупинка на product/design gate. Production code, simulation code, платіжна інтеграція й UI implementation у цьому завданні не створюються.**
