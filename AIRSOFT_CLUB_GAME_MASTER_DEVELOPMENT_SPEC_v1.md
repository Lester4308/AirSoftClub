> **USER APPROVED direction014:** progression + monetization driven; premium combat advantage intentional. Blanket +3 and strict total stack15–20% are SUPERSEDED. Current policy and measurements: [Monetization014](implementation/MONETIZATION_BALANCE_014.md).

> **Functional Alpha013 checkpoint:** local functional acceptance verified; see [Functional Alpha report](implementation/FUNCTIONAL_ALPHA_013.md). Steam sandbox/final art remain external/later. Early-access premium-stack audit exceeds target at levels1–2 and is explicitly NOT balance approval. Earlier implementation statuses are historical.


> **Approved reconciliation 2026-09-06:** the user confirmed that the Revenge target loses no rating. Rated attacker win restores floor(actual origin loss × 120 / 100), without normal rating gain; it still consumes the target’s shared incoming exposure slot. At cap, accept non-rated with no recovery and retain own shield. Any win closes the ticket. Existing friend-window anti-farm limits remain; no extra reward multiplier. The previous counterparty-debit blocker is superseded. See implementation/VERIFICATION_012.md.
# AIRSOFT CLUB GAME — MASTER DEVELOPMENT SPEC v1

> **Development pass 2026-09-06:** implementation003–010 now has local code and verification; current scope, remaining gates and native/package evidence are in [Verification011](implementation/VERIFICATION_011.md), with [launch instructions](implementation/DEVELOPMENT_RUNBOOK.md). Earlier documentation-only/future status text below records the pre-execution checkpoint. The user's explicit development-pass request authorizes this work; Project Airsoft remains untouched.


Дата консолідації: **2026-09-06**. Проєкт: **Airsoft_Club_Game**. Мова продуктних рішень: українська; identifiers коду збережені англійською.

**Єдине актуальне джерело правил розробки.** Документ об'єднує Product Design Gate v3, Implementation 001–002 та наступні затвердження Q01–Q15. Закриття питань не означає готовності всіх систем або фінального балансу. Цей pass — лише документація; подальша реалізація запускається окремим дорученням із companion execution pack.

## 0. Канон, статуси та походження

### 0.1. Порядок пріоритету

1. Останні явні вказівки власника продукту в активному завданні.
2. Незалежна project boundary: **не змінювати Project Airsoft / Airsoft Manager**.
3. Цей master: останні затвердження Q01–Q15 мають перевагу над старими OPEN/proposals Gate v3.
4. Product Design Gate v3 та чинні subsystem specs — у частині, що не суперечить master.
5. Implementation 001–002 і verified source — доказ реалізованої поведінки, а не дозвіл довільно змінювати product rules.
6. Позначені гіпотези та grounded implementation decisions.
7. V1/v2, старі prompts і research archive — історичні джерела, не активні команди.

Якщо код суперечить approved rule, зафіксувати discrepancy і виправити в дозволеному implementation scope; не переписувати правило під код. Історичні STOP після 001/002 виконані й не є вимогою зупиняти майбутній окремо запущений великий pass після кожного milestone.

| Позначка | Значення |
|---|---|
| USER APPROVED | Погоджена вимога продукту; агент не змінює самостійно |
| VERIFIED IMPLEMENTED | Поведінка підтверджена кодом і наявними verification reports |
| BALANCE HYPOTHESIS | Versioned prototype value; можна досліджувати й налаштовувати з доказами |
| GROUNDED IMPLEMENTATION DECISION | Оборотний технічний вибір у межах затверджених правил; записати підставу й тести |
| OPEN PRODUCT / ART | Не видане затвердження; не підміняти фінальним рішенням агента |
| HISTORICAL / SUPERSEDED | Зберігається для traceability; не виконується як актуальна вимога |

### 0.2. Source manifest цієї консолідації

Repository root: `C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game/`. Нижче всі repo paths відносні до цього root; це також спосіб знайти джерела, якщо master передано окремим файлом.

| ID | Джерело | Що встановлює |
|---|---|---|
| S01 | Розмова «Пошук інформації про гру», ID `6a9be018-10f4-83eb-8dac-855d180a9c23` | Product corrections, затвердження Q01–Q15, дозвіл підготувати два master-файли |
| S02 | `PROJECT_BOUNDARY.md`, `AGENTS.md`, `design/REUSE_DECISIONS.md` | Незалежність проєкту, відсутність approved reuse |
| S03 | `design/USER_APPROVED_DECISIONS_v3.txt`, `design/USER_CLARIFICATION_v3.md`, `design/PRODUCT_DECISIONS.md` | Пункти V3-00–V3-76; clarified friend reward cutoff |
| S04 | `design/AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md`, `design/AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v3.md`, `design/AIRSOFT_GAME_DESIGN_SPEC_v3.md`, `design/AIRSOFT_CLUB_GAME_UPDATED_PRODUCT_DECISION_SUMMARY_v3.md` | Узгоджений product foundation до пізніших Q closures |
| S05 | `design/AIRSOFT_CORE_LOOP.md`, `design/AIRSOFT_UI_FLOW.md`, `design/AIRSOFT_FIGHTER_SYSTEM.md`, `design/AIRSOFT_EQUIPMENT_SYSTEM.md`, `design/AIRSOFT_COMBAT_SYSTEM.md`, `design/AIRSOFT_ECONOMY.md` | Деталі систем і UX, із замінами з цього master |
| S06 | `design/STEAM_SOCIAL_PVP_ARCHITECTURE.md`, `design/BACKEND_ARCHITECTURE.md`, `design/DATA_MODEL.md` | Authority, persistence та social lifecycle |
| S07 | `design/BALANCE_HYPOTHESES_v3.md`, `design/IMPLEMENTATION_QUESTIONS_v3.md` | Попередні B/Q registers; актуальні статуси нижче |
| S08 | `implementation/IMPLEMENTATION_001.md`, `implementation/VERIFICATION_001.md` | Реалізовані формули, core contracts, 45 початкових tests |
| S09 | `implementation/UNITY_INTEGRATION_002.md`, `implementation/VERIFICATION_002.md`, `implementation/VERIFICATION_002_IL2CPP_RETRY.md`, `implementation/VERIFICATION_002_IL2CPP_FINAL.md` | Unity integration; final report скасовує обидва environment blockers |
| S10 | `src/Airsoft.Battle/Runtime/Rules.cs`, решта Runtime, `global.json`, UnityHost settings, Git HEAD/status | Source/config cross-check та перевірка repo checkpoint |
| S11 | `ROADMAP.md`, `design/MVP_IMPLEMENTATION_PLAN.md`, `design/VALIDATION.md`, `design/RESEARCH_BASIS.md` | Історія підготовки та provenance; не поточний implementation status |

Прочитано сторінки розмови до початку історії; для актуальних правил використано останні user approvals та локальні документи. Первинні відео й увесь PaintballWars_Research повторно не досліджувалися. Нові історичні або платформні факти не заявляються. Посилання на Steam у старих specs не замінюють перевірку офіційної документації під час майбутньої інтеграції.

У старій розмові **Q-01–Q-12** — ранні product questions; у цьому master **Q01–Q15** — пізні implementation questions після Gate v3. Не змішувати ці дві нумерації.

## 1. Project boundary і product DNA

Airsoft_Club_Game — нова окрема гра з власними repository, folder, Git history, architecture, data model, economy, combat, roadmap, backend, Steam integration та assets. Назва робоча, не фінальна комерційна.

Project Airsoft / Airsoft Manager не модернізувати, не продовжувати, не використовувати як codebase. Не переносити автоматично code, classes, календар, save system, термінологію, UI, balance, assets, dependencies або Git history. Жодного approved reuse у чинному реєстрі немає. Майбутня ідея reuse потребує окремого обґрунтування й adoption; запис proposal не дозволяє копіювання.

Product DNA: management/social rivalry loop, натхненний «Пейнтбольные войны», з незалежною реалізацією в airsoft setting. Гравець керує клубом, готує бійців, обирає ризик і суперника, спостерігає автоматичний бій та розвивається. Це не FPS, не manual tactical shooter і не продовження іншого Airsoft-проєкту. Комерційна модель: **paid Steam game + optional Credits**.

## 2. Core loop та екрани

**Management → Preparation → Opponent selection → Automatic battle → Result → Progression → Repeat.**

1. Club: recruitment, roster, training, inventory, Club Level.
2. Preparation: live HP/recovery або soft heal, чотири gear slots, refill shared BB, один active BB tier.
3. Opponent: Steam Friends, Ranked candidates або Revenge opportunity; видимий risk/reward.
4. Automatic battle: один challenge = один battle = один round. Немає серій раундів і ручного керування стрільбою.
5. Result: win/draw/loss, фактичні HP/BB, Money/Club XP/Fighter XP, rating і причина eligibility/обмежень.
6. Progression: XP відкриває можливості, soft currency оплачує розвиток; повернення до клубу.

| Screen / flow | Мінімальна поведінка майбутнього клієнта |
|---|---|
| Entry / reconnect | Steam session; чесний offline/read-only та connection error state |
| First recruit | Обрати одного з 3 FREE candidates; не видавати повторно |
| Club hub | Roster до 16, wallets, Club Level/XP, health, BB, navigation |
| Fighter / Training / Recovery | 3 stats, level/caps, ціна/preview training, 4 slots, HP і heal quote |
| Recruitment | 6–7 offers, time/match refresh progress, soft refresh, dismissal refund |
| Shop | MK1/MK2/MK3, progression, early unlock як access окремо від soft purchase |
| BB supply | Tier stocks/capacity, один active class, refill, emergency, later auto-buy Basic |
| Opponents | Friends; Ranked 3–5; Revenge; limited/empty pool без fake opponents |
| Pre-battle | Автоматичний склад, health exclusions, BB tier, reward/rating eligibility |
| Battle / Result | Presentation authoritative events; result і settlement status; skip presentation не скасовує battle |
| History / Inbox | Attack/defense, read state, origin loss, Revenge attempts/expiry |
| Credits / Shield | Явні суми, duration, conversion, ранній доступ; без automatic premium spend |
| Retention / Social | Daily/streak, achievements, leaderboard Global/Friends/Around Me, report flow |

У roster немає checkboxes manual active squad. Точний Team Power прихований; wallet delta, gross reward та орієнтовна вартість заміни BB не змішуються. Stale quote/snapshot вимагає оновлення, а не мовчазної покупки за іншою ціною. Placeholder UX дозволяє functional testing без затвердження final art.

## 3. Roster, recruitment, dismissal

### 3.1. Участь

- Максимум **16 active owned fighters у клубі** та **16 на сторону**.
- Усі придбані combat-ready fighters беруть участь автоматично; немає довільного reserve/active subset.
- Асиметрія природна: 1v2, 2v5, 7v16, 16v16; core підтримує всі 1..16 × 1..16.
- Gear quality або відсутність Weapon не виключає fighter.
- Offensive eligibility визначається readiness кожного fighter; порожній eligible attack roster відхиляється до acceptance.
- Defense включає active roster незалежно від live HP; мінімум **1 active fighter**. Без валідного defense roster клуб недоступний як PvP target до відновлення стану.

### 3.2. Recruitment

Перший recruit — **один із 3 free candidates**, одноразово назавжди на account/club entitlement. Reset/dismissal/resale не повертають free grant. Новий account отримує окремо 10 Credits та starter resources (§10).

Звичайний pool — **6–7 candidates**: різні starting stats, ціни, strength і male/female variants. Дорожчий recruit економить training time/cost; дешевий може стати ветераном без hidden unreachable genetic caps. Club Level підвищує середню якість зі збереженням variance.

Free refresh доступний через **1 годину АБО після 10 завершених authoritative battles, що раніше**. Кожен refresh повністю генерує новий pool. Будь-який refresh, включно з immediate soft-paid, скидає **і timer, і match counter**. Started/cancelled/failed starts не рахуються. Завершені Friend/Ranked/Revenge можуть рахуватися; retries одного settlement не збільшують counter повторно. Числа 1h/10 — затверджена стартова policy Q13, а не досі OPEN; майбутня зміна — явна versioned balance revision.

Offer acquisition перевіряє offer version, price, ownership/expiry, wallet та roster cap транзакційно. Recruitment distribution, exact prices і deterministic generation details — configurable implementation/balance work.

### 3.3. Dismissal

Повернення — частка **початкової recruitment purchase price**, target **25–40%** залишається гіпотезою. XP, Fighter Level і training investment не відшкодовуються. Все equipped gear повертається у club inventory. Fighter може лишитися inactive/archive record для історії, але не бере участі в нових snapshots. Для free fighter initial purchase price = 0, отже resale не створює soft faucet. Повтор dismissal має той самий результат без повторного refund.

## 4. Stats, XP та training

Рівно три base stats:

| Stat | Ефект |
|---|---|
| Accuracy | Hit chance |
| Endurance | Max HP |
| Agility | Evasion + action tempo |

HP, Damage, Protection, penetration, derived tempo — не додаткові base stats. Crits у v1 немає.

**Battle participation → Fighter XP → Fighter Level → training cap → soft-paid stat upgrade.** Рівень відкриває cap, а не автоматично купує +stats. Окремої training currency немає. XP отримують фактичні учасники, включно з eliminated; неучасникам XP не видається. Exact curves, per-participant base XP, caps та prices — balance config, не успадковані числа старої гри.

Club XP → Club Level → unlocks, BB capacity, кращий середній recruitment pool, обмежені Credit grants. Не додавати respec/refund training як приховану нову механіку.

## 5. HP, readiness, healing, recovery

Combat — **HP / Damage / Armor**, кілька hits допустимі; elimination при `HP <= 0`.

Offensive Current HP зберігається після authoritative settlement. Наприклад, 35/100 після бою означає наступний старт із 35 HP плюс законне recovery/heal. Defense starts at **100% Max HP**, незалежно від live wounds/онлайн-стану; defense не лікує й не ранить live fighter.

Readiness перевіряється **окремо для кожного fighter перед власною атакою**. Prototype threshold **10% Max HP**: нижче 10% NotReady, від 10% Ready; обов'язково HP > 0. Точне integer/fixed comparison `CurrentHP * 10 >= MaxHP`, не округлення відображеного відсотка. **No injury latch**: recovery до threshold автоматично повертає readiness. Helper вже реалізований; threshold лишається versioned prototype target.

Immediate healing купується за **soft currency**. Free timed recovery враховує offline elapsed time; target **~1% Max HP/хв** не фіналізований. Прямого healing за Credits немає: Credits можна явно обміняти на soft, а потім оплатити heal. Немає persistent Energy.

Max HP upgrade не масштабує Current HP і не лікує: **80/100 → 80/110**. Recovery після зміни працює за новими параметрами. Server-authoritative clock зберігає timestamp + fractional remainder; client clock не впливає на HP.

Майбутня grounded implementation policy: матеріалізувати recovery до часу зміни за старими параметрами, потім застосувати new Max HP/rate; зафіксувати units remainder, rounding, full-HP cap і timestamp boundary у decision log. Не нараховувати весь минулий час за новим Max HP. Цей алгоритм уточнює Q14, не видається за вже реалізований recovery service.

Власні offensive commands захищені serialization або optimistic concurrency/version token. Battle settlement не може старим snapshot перезаписати пізніший heal/train/equip; дозволено обрати одну pending offense на club як просту технічну реалізацію, але не прихований daily attack limit.

## 6. Equipment, weapons, MK

Рівно **4 slots**:

| Slot | Вміст |
|---|---|
| Weapon | Одна зброя, family/profile/MK |
| Camouflage | Камуфляж; combat effect лише через явний config |
| Head Protection | Goggles, mask, helmet, head/face protection |
| Load-bearing / Armor | Chest rig, plate carrier, tactical vest, armor |

Light armor: lower protection, low/no Agility penalty. Medium: medium protection, small penalty. Heavy: higher protection, larger penalty. Penetration проста й bounded; складної фізичної балістики не додавати. Head/body stacking і catalog coefficients — future implementation/balance decisions.

Weapon families v1: **Pistol, SMG, Assault Rifle, Shotgun, DMR, Sniper Rifle**; gradual unlocks, не всі з першого рівня. Exact item catalog не готовий.

**Base weapon = MK1.** Окремого Base → MK1 проміжного upgrade немає. MK1 — standard base version, MK2 — дорогий soft upgrade, MK3 — Credits-only upgrade. MK tier може давати visual change і невелике stat improvement. Exact transition costs/stacking конфігуруються; не створювати прихованих нових premium tiers.

Ready fighter без Weapon бере участь і є valid target, але **не атакує**. Автоматичного pistol, fists, melee або fallback damage немає.

## 7. Shared BB economy та combat accounting

BB — shared **club inventory**, не per-fighter resource. Capacity росте з Club Level. Stock за tiers, один active tier на команду/бій; per-fighter selector відсутній.

- Offense споживає **реальні BB за фактичними simulated projectiles**.
- Defense бере snapshot active tier та **finite virtual budget = 100% captured maximum club BB capacity**.
- Defense не читає live stock для budget і не списує real defender BB.
- Один projectile = один BB, включно з burst bullets та shotgun pellets.
- Реалізована нестача для volley: `min(remainingBB, plannedProjectiles)` — partial volley. Negative BB, free projectiles і silent tier switch заборонені.
- Коли одна сторона без ammo, інша продовжує, якщо здатна damage. Обидві без можливості damage → NoProgress Draw.

Prototype 3–5 tiers: Basic 0%, Improved ~3%, Advanced ~5%, High-End ~10%, Premium до ~15% effective advantage. Test fixtures використовують damage multipliers 1/1.03/1.05/1.10/1.15; це не final catalog. Допустимі damage/penetration tradeoffs. Найсильніший tier — **Credits-only**. Немає x2 damage, guaranteed hit чи ignore-all-armor.

Starter Basic target: **5–7 боїв** стартового roster. Later progression unlock **AUTO-BUY BASIC BB за soft**; opt-in, unlock level, refill threshold та soft spend guard — implementation decisions, premium auto-spend заборонений.

Emergency Basic: low Basic stock **<~15% capacity** і недостатньо soft на minimum refill → **+500 Basic раз на ~3 години**. Threshold/quantity/cooldown — hypotheses. Лише Basic; не premium. Альтернатива — свідоме поповнення через Credits.

Q04 закрив combat budget, але не total-vs-per-tier inventory capacity, overflow та мінімальний refill SKU. Ці деталі залишаються grounded implementation work (§16); обраний варіант має зберігати finite capacity, не створювати negative/duplicate grants і не перетворювати free recovery path на paid-only dead end.

## 8. Deterministic battle core — фактичний контракт

### 8.1. Ізоляція та вхід

`Airsoft.Battle` — pure C#, target `netstandard2.1`, без Unity/Steam/DB/network/wall clock. Entry point `BattleEngine.Run(MatchConfig)`. Однакові **MatchConfig + TeamSnapshot A/B + Seed + RulesetVersion + algorithm/versioned coefficients** дають той самий result. Одного version label без повних inputs недостатньо для replay.

Contracts immutable/get-only; arrays copied, fighters sorted ordinal ID; IDs unique всередині side, actor identity = side + ID. TeamSnapshot валідний для 1..16. Caller готує roster/readiness, live offense HP, full defense HP і finite budgets. Core не виконує recruitment, healing clock або settlement.

### 8.2. Numeric model та існуючі формули

**Int64 fixed-point: 10000 raw = 1.0000.** Checked arithmetic; multiply/divide truncate toward zero. Overflow — explicit failure, не wrap. Authoritative float/double заборонені. RNG unsigned wraparound окремо явно визначений.

Нижче **VERIFIED IMPLEMENTED, ruleset `prototype-001`, NOT FINAL BALANCE**:

| Значення | Формула / default |
|---|---|
| MaxHP | `50 + Endurance * 5` |
| EffectiveAgility | `max(0, Agility - EquipmentAgilityPenalty)` |
| Evasion | `EffectiveAgility * 0.005 + EquipmentEvasionBonus` |
| HitChance | `clamp(0.50 + Accuracy * 0.01 + WeaponAccuracyContribution - TargetEvasion, 0.05, 0.95)` |
| Speed | `1 + EffectiveAgility * 0.02` |
| Action interval | `max(10ms, ceil(WeaponIntervalMs / Speed))` |
| RawDamage | `WeaponDamage * BbDamageMultiplier` |
| EffectiveProtection | `max(0, ArmorProtection - WeaponPenetration - BbPenetration)` |
| Mitigation | `min(0.80, EffectiveProtection / (50 + EffectiveProtection))` |
| HitDamage | `max(0.0001, RawDamage * (1 - Mitigation))` |
| Maximum simulated duration | `120000ms`, exclusive deadline |
| Technical projectile-event guard | `1000000`, explicit TechnicalFailure |

Порядок fixed operations зберігати: algebraic rearrangement може змінити rounding і digest. Hit caps 5–95% затверджені як prototype target, не фінальний tuning. MaxBattleTime у розмові 300s був прикладом; фактичний перевірений default — **120s**, final duration OPEN balance.

Input validation bounds у core: stats ≤10000; weapon damage/protection/penetration ≤10000; BB multiplier >0 і ≤2; HP ≤1000100; weapon interval 1..60000ms; projectiles 1..64; BB budget 0..1000000; duration 1..3600000ms. Це representational limits, не design recommendation купувати multiplier 2 або million BB.

### 8.3. RNG, targets та scheduling

SplitMix64: seed 0 valid; increment `0x9E3779B97F4A7C15`; mixing multipliers `0xBF58476D1CE4E5B9`, `0x94D049BB133111EB`. NextUInt32 — upper 32 bits. `NextInt(bound) = (UInt64(nextUInt32) * bound) >> 32`, один RNG draw; accepted tiny bucket quantization bias, не cryptographic RNG. Seed-zero first vectors: `E220A839 / 6E789E6A / 06C45D18`.

Target — uniform seeded selection із ordinal-ID sorted alive enemy list на початку timestamp batch, один target на volley, окремий hit roll `[0,10000)` на projectile. Немає tactical AI або залежності від unordered collections.

Simulation перескакує до next action time, не використовує frame time чи round-robin. Initial action = interval. На одному timestamp:

1. Freeze alive actors/targets; перевірити весь batch проти technical guard.
2. Attacker side, потім Defender side; у side ordinal fighter ID order.
3. Розподілити shared BB, виконати target/hit RNG, записати projectile events, призначити next time.
4. Застосувати aggregated damage **одночасно**, clamp HP до 0.
5. Оцінити elimination, потім no-progress, потім наступний timestamp.

Fighter alive на початку batch може вистрілити, навіть якщо в тому ж batch його eliminated; на наступному timestamp — вже ні. Overkill events допустимі, resurrection — ні. Stable order визначає отримувача останніх BB і RNG sequence; це не доказ competitive symmetry при side swaps.

### 8.4. Outcomes, termination та failure

| Умова | Результат |
|---|---|
| Обидві сторони втратили останніх fighters у same-time batch | Draw / simultaneous elimination |
| Лише одна сторона eliminated | Перемога іншої |
| Обидві не мають alive armed actor з BB | Draw / NoProgress |
| Next action time >= MaxDuration | Draw / DurationLimit точно на deadline, action не виконується |
| Technical event guard перевищено | TechnicalFailure, Outcome = null, diagnostic `TECHNICAL_EVENT_GUARD` |

Немає Team Power tie-break. Technical failure не є gameplay draw і не дає ordinary rewards. Guard спрацьовує до зміни поточного batch HP/BB/RNG; result містить попередні committed batches. Майбутній backend звільняє reservations/reconciles failure, а не стягує повторну плату за retry.

### 8.5. Result і serialization

MatchResult: status/outcome/reason/diagnostic, simulated duration, fighter MaxHP/final HP/elimination, BB consumed/remaining per side, ordered projectile events, seed, rules version. Economic rewards/rating поза battle engine.

`BattleWire` schema **1**: explicit binary, little-endian; Int32 magic `0x41434257`, schema, kind 1=config/2=team/3=result. String — Int32 UTF-8 byte count + strict UTF-8, max4096; bool byte 0/1; Fixed — Int64 raw; seed — UInt64. Config зберігає повні coefficients/limits і обидві teams. Result включає ordered events. Max packet 64MiB, fighters1..16, events≤1000000. Unknown schema/kind, malformed fields, bad counts/order/HP, truncated/trailing bytes відхиляються; немає silent migration.

Wire result bytes і `MatchResult.ToCanonicalBytes()` digest — різні representations, обидві перевіряються. Decode result не доводить його авторитетність. Archive complete config/snapshots + result + versions; backend сам resolve/verify.

Golden seed: `0xFEDCBA9876543210`, fixture 3v2. SHA-256 canonical digest:

`22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f`

Golden files: `src/Airsoft.Battle/Runtime/Resources/golden-input.bytes`, `golden-result.bytes`, `golden-digest.txt`. Нормальні тести їх не переписують. Не regenerate для приховування regression.

## 9. PvP, Friends, Ranked, exposure, shields, Revenge

### 9.1. Authority та defense snapshots

Steam-first asynchronous PvP. Backend приймає intent, перевіряє identity/ownership/eligibility, обирає seed, фіксує inputs, рахує result і робить один settlement. Client replay — presentation.

Defense snapshot immutable/versioned: active fighters, stats, MaxHP, equipment/MK, active BB class, captured capacity, rating, combat modifiers, ruleset/balance version. Defense starts full HP, live HP/BB недоторканні. Concurrent attacks можуть читати одну version.

Новий snapshot публікується **тільки після committed equip/heal/recruit/dismiss/train та інших релевантних changes**. Уже accepted battles не перемикаються на нього. Failed transaction не публікує стан. Мінімум 1 active fighter; live readiness не criterion для defense.

### 9.2. Ranked

Backend дає **3–5 candidates**, гравець сам обирає слабшого/рівного/сильнішого. Якщо eligible pool менший — чесний limited/empty state. Opponent card: Club Level, rating, fighter count, approximate strength, potential reward category. Не exact Team Power. Багато власних атак дозволені; defense cap не є лімітом «4 власні матчі/день».

Rating floor **0**. Зміна залежить від rating difference; **±5…±20** — prototype range, exact curve лишається balance decision. Ranked Draw = **0 rating**.

### 9.3. Steam Friends

Friend battles не завжди non-ranked: можуть мати rating eligibility. Directed key **A→B**, B→A окремий. **8h window починається з першого рейтингового бою A→B**, backend фіксує anchor; candidate view/reconnect не починають/не скидають вікно.

До першої win максимум **одна rating loss** атакуючого від цього friend у window. Першу win можна винагородити rating gain; після неї всі наступні pair battles до кінця window мають **0 rating в обидва боки**. Unlimited attempts залишаються підпорядковані shield/readiness/іншим checks і витрачають offensive HP/BB.

Reward win ordinal: **100% → 50% → 25% → 0% з четвертої win**. Exhaustion триває до кінця window: loss, draw, reconnect, mode switch не відновлюють budget. Після exhaustion zero Money/Club XP/Fighter XP, а не лише zero win money. Win counter не означає streak, який можна скинути навмисною поразкою.

Для ще не anchored non-rated friend sessions та repeat draw/loss до exhaustion потрібен explicit anti-farm implementation rule; це залишкова деталь, не привід повторно відкривати весь Q06. Можна вибрати окремий conservative server budget, якщо він не змінює approved rated window/reward sequence; матеріальний новий reward режим вимагає рішення власника (§16).

### 9.4. Global incoming exposure

Максимум **4 incoming rating impacts на profile за rolling 24h**, сукупно між entry points. Це не calendar UTC day. Candidate display не резервує slot.

- Reserve atomically лише при **actual rated battle start**.
- Settlement підтверджує actual rating impact один раз.
- Zero-impact draw не витрачає фактичний impact count; reservation release потрібний.
- Active reservations враховуються для concurrency capacity; TTL запобігає вічному блокуванню.
- Якщо cap вичерпано до start — reject rated start або explicit non-rated path, без п'ятого impact.
- Revenge cap не обходить. При втраті rating eligibility до start можлива non-rated Revenge без recovery.
- Eligibility закріплюється на start; TTL/recovery design не може просто видалити reservation і згодом допустити п'ятий settlement. Потрібна lease/fencing або інша перевірена transaction policy.

Exact TTL, half-open interval boundaries, projection visibility — grounded choices; count invariant обов'язковий.

### 9.5. Protection shields

Credits shield durations: **8h / 1 day / 3 days / 7 days**. Backend визначає activation/expiry. Active shield блокує **всі нові incoming attacks**, включно з Friends/Revenge; accepted battle не скасовується.

Own **Ranked attack знімає shield**. Own **Revenge з rating impact** теж знімає. **Friend / non-ranked** battle не знімає. Розділяти mode і rating eligibility: Friend exception діє як погоджено, навіть коли friend attempt має rating eligibility. Failed validation до acceptance не повинна продаватися як успішна атака; atomic cancellation timing уточнюється й тестується в backend.

### 9.6. Revenge

Один eligible defense loss → максимум один origin-linked ticket. **Friend/non-ranked battles не створюють tickets**. **24h expiry**, максимум **3 attempts**, draw споживає attempt. Success закриває ticket; exhausted/expired ticket недоступний для нового start.

Eligibility, attempt reservation і право на recovery фіксуються на start. Якщо expiry настає під час accepted battle — він нормально завершується. Retried start idempotent; не дублює ticket/attempt. Successful Revenge не створює Revenge у відповідь; **Revenge-on-Revenge chain заборонений**.

Successful eligible Revenge повертає **120% actual rating lost у конкретному origin defense battle**. Приклад: -10 → +12. Не 120% current rating, не додатковий normal Ranked win gain зверху. Origin actual loss після floor — база; нульова втрата не дає recovery. Grounded integer rounding: floor(actual loss × 120 / 100), checked Int64 intermediate.

Плюс **standard battle reward без extra multiplier**, із загальними eligibility/anti-farm restrictions. Non-rated Revenge при cap не обіцяє 120% recovery. Shield не обходиться, expiry не продовжується автоматично. Ціль Revenge не втрачає рейтинг (user clarification 2026-09-06). Recovery використовує floor(actual loss × 120 / 100); rated recovery займає спільний exposure slot. Чинний friend-window anti-farm budget зберігається між modes.

## 10. Rewards, wallets, monetization, retention

### 10.1. Reward calculation contract

Base Battle Value насамперед залежить від **Opponent Club Level**; далі bounded **actual battle power difference** modifier. Використовувати accepted inputs, а не післяматчеве unequip. Внутрішній power враховує count, stats, starting HP, weapon/MK, armor/head/camo якщо combat-relevant, BB. UI показує категорії (наприклад Very Weak / Weak / Balanced / Strong / Very Strong), не число/формулу.

Затверджена таблиця Q05:

| Outcome | Money | Club XP | Fighter XP кожному фактичному учаснику |
|---|---|---|---|
| Win | **100%** | **100%** | **100%** |
| Draw | **25%** | **35%** | **50%** |
| Loss | **0%** | **10%** | **25%** |

Проценти — від відповідного standard win-equivalent. Friend repeat multipliers застосовуються після outcome/mode eligibility; rewards не є частиною core combat. Prototype power modifier target **0.1x–1.5x**. Exact base curves, power formula, clamp behavior та integer rounding — configurable/versioned, але не довільна заміна approved outcome percentages.

Не оголошувати loss farming, repeated draws або mode switching нескінченними faucets. Потрібні meaningful abuse tests і telemetry, без автоматичного бану лише за одиничний unusual result. Виплата offline defenders Money/XP не затверджена як faucet; не додавати її непомітно.

### 10.2. Currency flows

| Resource | Sources | Sinks |
|---|---|---|
| Soft / Money | Battle rewards, daily/achievements, approved progression grants, Credits conversion | Recruit, training, healing, gear, MK2, BB, refresh |
| Credits | Real-money purchase; limited achievement/daily/7-day streak/Club Level grants; **10 starter Credits** | Early unlock, MK3, premium items/BB, shields, conversion to soft |
| Club XP | Eligible battle/approved progression rewards | Progression, не spendable wallet |
| Fighter XP | Actual participation і outcome/eligibility | Level/cap progression, не training currency |

Credits → soft **дозволено за server-side configurable fixed rate**. Soft → Credits **заборонено**. Hard daily Credits spend cap немає. Не робити automatic Credits exchange/top-up/heal або premium tier switch.

Starter grants: **1 free fighter (choice of 3), 10 Credits, soft на базове спорядження, Basic BB приблизно на 5–7 боїв**. Exact soft/BB quantities і basic catalog — prototype config. Free weapon автоматично не видається без нового approval; гроші на gear не дорівнюють gifted Weapon.

### 10.3. Commercial rules

Гра платна у Steam; optional Credits. «F2P-аналог» у balance discussion — власник платної гри без купівлі Credits.

Credits early unlock: Level1 unavailable; Level2 +1; Level3–4 +2; Level5+ +3. Credits buy permanent access; item purchased separately with Money. Depth pricing configurable relative1×/2×/4×. Before natural unlock, individual gear contribution targets +20–25% versus best current-band normal gear; cap releases automatically at natural level without rebuy. Total premium stack may exceed this individual limit; measure pathological multipliers. The old strict combined15–20% ceiling is SUPERSEDED by user direction014. Core loop must remain playable without payment.

Ceiling — approved design constraint, але виконання ще не доведено. Треба визначити метрику, порівнювати повні builds, однакові roster/progression/HP, багато seeds/scenarios; raw win-rate delta не тотожна effective power. Price/faucet tables, exact metric і premium item catalog не фінальні. Pure cosmetics — later, не priority v1.

### 10.4. Ledger, purchase та reconciliation

Усі wallets/grants/debits server-authoritative. Steam/Credits operations мають **idempotency key + immutable ledger**; retry не створює duplicate grant. Client не задає award amount чи trusted balance.

Payment success без entitlement → automatic retry/reconciliation. Технічно незавершена internal transaction не списує Credits/entitlement; зовнішня confirmed payment не губиться через timeout локального grant. Unknown platform status звіряється, а не трактується як failure або дозвіл повторної покупки.

Коректно використані consumables/shields/BB/unlocks не мають unconditional manual Credits refund. Technical failure/unapplied transactions та Steam platform refunds/chargebacks мають backend reconciliation path. Reversal — окремі audit entries, не переписування ledger history. Exact spent-balance reversal treatment має бути визначений до реального commerce; не вигадувати production debt/ban policy.

### 10.5. Daily, streak, moderation

Daily period: **UTC, новий день о 00:00 UTC**. **Один daily claim на account на день**, authoritative server. Пропуск повного UTC-дня ламає streak; наступний claim починає **Day 1**. Client timezone/clock не впливають. Seven-day streak із контрольованими Credit faucets; reward amounts та поведінка наступного циклу після Day 7 — versioned retention config, не нові приховані entitlement grants.

Moderation v1: club names, emblems, report flow, sanctions. Мінімальний UGC, **без відкритого текстового чату у v1**. Sanctions: warning, rename reset, temporary restriction, account suspension за характером порушення. Записи та authorized moderator actions auditable; не включати надмірний social platform scope.

## 11. Backend / Steam / database / tech stack

| Layer | Approved / verified стан |
|---|---|
| Client | Unity **6000.3.21f1 LTS**, C#; API **.NET Standard 2.1** |
| Shared combat | Pure C# `netstandard2.1`, один source package |
| Standalone build/test baseline | SDK **10.0.302**, tests `net10.0`; не плутати з остаточним backend runtime pin |
| Backend | **ASP.NET Core**, актуальний **.NET LTS**; exact version pin після compatibility check на bootstrap |
| Database | **PostgreSQL**, stable major pin на backend milestone |
| ORM / migrations | **EF Core**, versioned controlled migrations |
| Platform | Steam-first, Steamworks через C# integration layer; exact wrapper ще вибрати |
| Battle serialization | Explicit binary schema v1, full input versions |
| Deployment shape | Docker-friendly; hosting provider не обраний |
| Operations | Structured logs, health/readiness, metrics/tracing foundation; secrets поза repo |

Shared DTO/domain primitives/core дозволені там, де сумісні з Unity; shared code не робить client trusted. ASP.NET target framework не переносити автоматично в Unity. Не додавати microservices, Netcode/DOTS/Addressables без потреби.

Логічні модулі: Identity/Access, Club/Fighters, Inventory/WalletLedger, Battle/Settlement, PvPPolicy, Snapshot publication, Retention/Moderation, Leaderboard projection. Modular monolith — дозволений grounded starting choice, не затверджена вимога distributed architecture.

Backend authoritative для Steam identity/ownership/session, economy/XP/Credits, combat resolve/verify, matchmaking/rating, snapshots і leaderboard. Critical operations online-only. Offline — launch/read-only cache; isolated PvE можливий later, без authoritative rewards.

Steam friend enumeration допомагає discovery; client SteamID/friend list не є доказом права атакувати. Publisher keys/backend credentials не в client/repo. Реальна auth/commerce інтеграція перевіряється за актуальними офіційними Steamworks docs під час implementation; master не обіцяє працездатні live integrations.

### 11.1. Transaction lifecycle

1. Authenticate, authorize, validate idempotency key/request payload hash і expected version.
2. Матеріалізувати recovery; перевірити roster, BB, shield, pair eligibility, exposure, Revenge; атомарно accept/reserve.
3. Зберегти immutable attacker input + defender snapshot, seed, повні rules/balance versions, captured eligibility.
4. Resolve pure core; failed resolve і crash recovery мають explicit state, не rewards draw.
5. Atomic idempotent settlement: attacker HP/actual BB, eligible rewards/XP/rating, counters/exposure/attempts, ledger refs.
6. Після commit надійно опублікувати новий snapshot/projections. Outbox або еквівалент — grounded choice.

Зовнішній HTTP/Steam success і DB commit не є однією distributed transaction; потрібні recoverable states й reconciliation. Власні commands серіалізуються/versioned; defender snapshot може обслуговувати concurrent reads, live resources не дебітуються.

### 11.2. Мінімальні persistence concepts

| Concept | Ключові поля / constraint |
|---|---|
| Account / Club | Verified Steam mapping, owner, access, XP/Level/rating≥0, version, capacity |
| Fighter | Stable ID, owner, active/archive, 3 stats, XP/level/caps, HP, recovery timestamp/remainder, initial purchase price |
| RecruitmentOfferSet / StarterEntitlement | Pool version, offers, generation, refresh anchors/counter, permanent claimed flag |
| ItemDefinition / Instance / EquipmentAssignment | Catalog version, owner, slot/family/MK, price/gates; ownership без double equip |
| ClubBBInventory | Tier quantities, capacity policy, active tier; no per-fighter stock |
| Wallet / LedgerEntry | Money/Credits, operation/reason, immutable deltas, unique grant/debit/refund references |
| DefenseSnapshot | Immutable version, roster/full HP/gear/BB/capacity/rating/rules, committed source version |
| Match / Config / Result / Settlement | Captured inputs, seed/versions, status, lease/fencing, unique once-only settlement |
| FriendPairWindow | Directed key, 8h anchor, loss used, win ordinal, rating disabled, reward exhaustion |
| DefenseExposure | Timestamped committed impacts + active reservations, rolling cap ≤4, lease state |
| Shield | Owner, start/end, purchase ref, cancellation reason/time |
| RevengeTicket / Attempt | Unique origin, actual loss, expiry, attempts≤3, start eligibility, consumed success |
| PurchaseOrder / Reconciliation | Platform IDs, trusted SKU, state, grant/reversal references, retry scheduling |
| RetentionClaim | Account/date/achievement/level unique grant keys, streak position |
| Moderation / Projections | Reports/sanctions audit, rating history/leaderboard versions |

Це requirements до schema, не твердження, що таблиці вже існують. Unique keys, FK/ownership, nonnegative quantities, concurrency constraints та migrations треба перевіряти на реальному PostgreSQL, а не лише in-memory provider.

## 12. Verified implementation status і checkpoint

Локально під час цього documentation pass перевірено root, **чистий стартовий working tree**, branch та HEAD:

```text
Branch: codex/implementation-002-unity-host
Commit: d553cd3d9fee998a9764be04b63f8881658d299f
```

Це точка продовження game code. Нові master documents можуть бути незакоміченими або окремим docs-only descendant; не reset-ити їх, щоб штучно отримати старий clean HEAD.

| Етап | Фактичний статус |
|---|---|
| Product Design Gate v3 | Completed; пізні Q closures накладені цим master |
| Implementation 001, commit `943d987147b82e63e8f26fe46f2f1cf002c19d09` | Pure deterministic battle core, contracts, fixtures/tests/harness implemented |
| Implementation 002, commit `7c50261a88ac752e70471032420cb4527f64a3aa` | Minimal Unity host, shared source, binary wire, golden/runtime gates |
| SDK retry `1ae6fb8dc0857e171939b574aa175d3e2e46049c` | Historical docs-only blocker report |
| Final gate `d553cd3d9fee998a9764be04b63f8881658d299f` | Native Windows x64 IL2CPP PASS, blocker CLOSED |
| Economy, recruitment, recovery service, backend/DB, live Steam/PvP, real commerce | **NOT IMPLEMENTED** у verified checkpoint |
| Production UI/art/release | **NOT IMPLEMENTED**, art OPEN |

UnityHost — technical bootstrap, не готова playable game. Core підключений local package `file:../../src/Airsoft.Battle`; standalone компілює ті самі Runtime files. Не копіювати simulator в Unity. Versioned `.meta`, ProjectSettings, manifest/lock; ignored Library/Temp/Logs/builds/Artifacts.

Final report S09 зафіксував Windows SDK **10.0.26100.0**, full C# → IL2CPP C++ → native compile/link → executable, launch **exit 0**. Identical digest у **.NET / EditMode / PlayMode / Mono / native IL2CPP**.

| Reported final verification | Результат |
|---|---|
| Standalone suite | **46/46** |
| EditMode | **9/9** |
| PlayMode | **1/1** |
| Repeat deterministic result | **100/100 identical** |
| Mass simulation | **10000/10000**, zero invariant failures |
| Native smoke | **300 battles**, 100×1v1 + 100×8v8 + **100×16v16** |
| Compiler warnings/errors | **0/0**; nonblocking licensing-token refresh message recorded |

Mass stats: attacker4913, defender5001, draw86 (0.86%); duration min1552 / mean6089.70 / max16506ms; mean BB81.05/81.30. Це mixed fixture sanity, не final balance або premium ceiling proof. Native smoke не означає, що всі 46 .NET tests виконані нативно. У цьому documentation pass gameplay tests/builds повторно не запускалися; evidence походить із final verified report.

## 13. Invariants і verification obligations

| ID | Invariant / потрібний доказ |
|---|---|
| I01 | Лише Airsoft_Club_Game; жодних imports/writes Project Airsoft |
| I02 | Roster≤16, усі ready automatically; no manual subset; empty offense rejected |
| I03 | Рівно Accuracy/Endurance/Agility; XP лише participants; training soft та cap-bound |
| I04 | HP у [0,MaxHP], no resurrection; Endurance upgrade не лікує |
| I05 | No injury latch; exact readiness threshold; server recovery без втрати remainder |
| I06 | Weaponless не атакує; same-time alive exception лише в поточному batch |
| I07 | BB≥0; consumed дорівнює projectiles; no tier auto-switch/free ammo |
| I08 | Defense full HP/finite virtual BB, zero live HP/BB mutation |
| I09 | Same input/seed/version → same bytes/digest; monotonic simulated time; guaranteed termination |
| I10 | TechnicalFailure не rewarded Draw; malformed/unsupported wire rejected |
| I11 | Immutable accepted snapshots; publish лише після commit |
| I12 | Idempotent acceptance/settlement/purchase/grants; conflicting payload with reused key не виконується |
| I13 | Own state concurrency не допускає double spending або stale HP overwrite |
| I14 | Directed friend 8h: ≤1 rating loss до win, ≤1 rating win, після win zero impact |
| I15 | Friend rewards 100/50/25/0; exhaustion не reset через loss/draw/mode/reconnect |
| I16 | Global rolling exposure≤4, включно з race за останній slot і Revenge |
| I17 | Shield blocks new incoming; accepted finish; own cancellation за mode rules |
| I18 | Revenge≤3 attempts, Draw counts, start eligibility, one origin ticket/success, no chains |
| I19 | Rating≥0; Ranked Draw=0; Revenge recovery від actual origin loss, не stack normal gain |
| I20 | One permanent free recruit; dismissal gear return; training/XP не refund |
| I21 | Refresh resets both counters; settlement retry не додає completed battle вдруге |
| I22 | Daily unique UTC account/date; missed day resets; client clock не authority |
| I23 | Progressive early depth0/1/2/3 by level; access only; item soft separately; no soft→Credits |
| I24 | Paid stack tested разом; no client grants/secret keys; immutable ledger/reconciliation |

Tests мають охоплювати exact boundaries, invalid inputs, crash/retry, concurrency, rollback і cross-mode abuse. Passing unit tests не є доказом live Steam або production database operations.

## 14. Реєстр закритих Q01–Q15

**Усі Q01–Q15 CLOSED як узгоджені питання.** Residual values/implementation details не оголошуються ані фінальними, ані вже implemented.

| ID | Актуальне закриття | Implementation status / residual |
|---|---|---|
| Q01 | Accuracy hit; Agility evasion+tempo; Endurance HP; 5–95% prototype; no crit; fixed integers, seeded RNG, action-time; duration Draw, guard failure | Core verified; exact tuning prototype (§8) |
| Q02 | Per-fighter 10% readiness, no latch, MaxHP change не scales CurrentHP | Helper verified; recovery service pending |
| Q03 | Ready weaponless participates/targetable, no attack/fallback; both no-progress Draw | Verified |
| Q04 | Defense BB=100% max capacity, finite/virtual; actual projectile accounting | Verified core + partial volley; inventory capacity/overflow pending |
| Q05 | Win100/100/100; Draw25/35/50; Loss0/10/25; friend100/50/25/0; power bounded target0.1–1.5 | Economy pending; curves/rounding config |
| Q06 | First rated directed-pair battle anchors8h; max1 loss; first win freezes later rating; rolling24h; floor0 | Pending; rating curve prototype ±5…20 |
| Q07 | Reserve at rated start, not discovery; settle once; cap4 shared with Revenge; TTL | Pending; exact lease/post-cap UX implementation |
| Q08 | Own Ranked removes shield; rated Revenge removes; Friend/non-rated does not; accepted incoming finishes | Pending |
| Q09 | Revenge24h/3; draw attempt; in-flight expiry valid; start eligibility; deterministic120%; unique origin/start; no chains; no Friend/non-ranked tickets | Implemented012; floor rounding, target rating unchanged; domain/PostgreSQL verified |
| Q10 | Base=MK1; MK2 expensive soft; MK3 Credits | Catalog pending |
| Q11 | Starter10 Credits; no soft→Credits; fixed configurable Credits→soft; no hard daily spend cap; intentional premium advantage; starter resources | Implemented014; exact prices/faucets and final balance remain hypotheses |
| Q12 | Unity6000.3.21f1/C#/Standard2.1; ASP.NET Core current LTS; PostgreSQL/EF Core; wirev1; shared pure core; observability; Docker; secrets external | Unity/wire verified; backend version pin/hosting pending |
| Q13 | Pool6–7 full regeneration;1h OR10 completed; reset both counters; one-time free; original-price resale; gear returns | Pending; refund fraction/distribution config |
| Q14 | Server timestamp+remainder; no MaxHP scaling; concurrency control; post-commit snapshots; min defense1 independent live HP | Pending backend lifecycle |
| Q15 | UTC00:00 daily/account; missed day→Day1; minimal UGC/no open chat; sanctions; idempotent ledger/reconciliation/refunds | Pending |

## 15. Balance hypotheses — актуальна карта

| Область | Стартова гіпотеза / залишок | Як перевіряти |
|---|---|---|
| Core | Формули §8, readiness10%, duration120s; no crit rule fixed | 1v1–16v16, seeds, armor/tempo pathologies, runtime parity |
| Recovery | ~1% MaxHP/min, heal prices | Time boundaries, fractional elapsed, economy pacing |
| Dismissal | 25–40% initial price | No free-recruit farm, gear return, progression cost |
| Starter | Basic5–7 battles, exact soft/BB | Onboarding має придбати gear і дійти до циклу refill/heal |
| Emergency | <~15%, +500, ~3h, min refill, overflow | No paid-only trap, no duplicate/cap overflow |
| BB/catalog | 3–5 tiers, 0/3/5/10/15%; armor tradeoffs | Complete builds, finite ammo, weakest/strongest tiers |
| Rewards | Base curves, power formula, target0.1–1.5 | Level/power mismatch, asymmetry, anti-farm |
| Rating | Difference-based, prototype ±5…20 | Floors, repeated pairs, zero origin loss, capped targets |
| Progression | XP/caps, unlock levels, recruitment quality, capacity | Early/late pacing, viable cheap veteran |
| Credits | Prices, fixed exchange rate, faucet amounts | Ledger conservation, sustainable sinks/faucets |
| Premium contribution014 | Individual early item~20–25%; total stack ceiling superseded | Paired full-build simulations; report bounded stacking and F2P viability |

Не лишаються гіпотезами старі альтернативи Draw Money10%, Draw FighterXP35%, Loss ClubXP0 або friend discount OPEN: Q05 їх замінив. 10 starter Credits — точне затвердження, не «small amount TBD».

## 16. Residual decisions та межа автономності

Це не повторне відкриття Q01–Q15. Закриті відповіді не покривали буквально кожну початкову піддеталь; нижче вони збережені чесно для агента.

| Деталь | Дозволений шлях |
|---|---|
| Reward/rating rounding, price tables, curves | Grounded versioned prototype config, tests; не змінювати approved multipliers |
| Capacity total/per-tier, overflow, emergency min refill | Grounded reversible policy з invariant tests; явно задокументувати |
| Recovery remainder/full-cap/time ordering | Grounded deterministic algorithm; no free HP from upgrade/time manipulation |
| TTL/lease, pair anchor transaction, post-cap reject vs non-rated | Grounded policy, atomic checks; прозоре UI, no fifth impact |
| Exact .NET/PostgreSQL/EF/wrapper | Verify official compatibility, pin; не міняти затверджений stack |
| Shield renewal/stacking, auto-buy soft safeguards, Day7 next cycle | Conservative configurable local implementation; не вигадувати premium spending/нові promises |
| Revenge counterparty/cross-mode | CLOSED: target rating unchanged; shared exposure cap and existing friend anti-farm budget preserved. |
| Refund після витрачених Credits, debt/suspension policy | Reconciliation foundation дозволена; реальну фінансову policy не вигадувати |
| Final commercial prices, production hosting/publishing, final art | Окреме рішення/дозвіл власника |

Невизначеність одного live integration не блокує незалежні domain/UI/test milestones. Не запитувати дозволу на назви класів, DTO, папки, indexes, fixture values чи звичайні fixes.

## 17. Art OPEN

**Final production art style OPEN.** Unity не означає final 3D. Попередні concept images — reference-only.

Відомі preferences: closer to classic 2D, side/3/4 fighters, stylized/low-poly-inspired 2D, голови приблизно на30% більші від реалістичних пропорцій, читабельні weapon/helmet/chest rig, універсальні male/female bases, сцени масштабуються до16v16. Це не затверджена production art bible. Повернутися до art pass після core/functional foundation; placeholder assets не оголошувати фінальними.

## 18. Roadmap від перевіреного стану

Milestone sequence нижче — план наступного окремо запущеного development pass; номери003+ не означають уже наявні commits.

| Milestone | Результат / exit gate |
|---|---|
| 001 | DONE: deterministic pure battle core |
| 002 | DONE: Unity host + native IL2CPP parity |
| 003 | Rewards/economy domain: outcome table, wallets/ledger semantics, curves/config, anti-farm tests |
| 004 | Club lifecycle: recruitment, stats/training, recovery/heal, inventory/MK/BB, snapshots, starter |
| 005 | ASP.NET Core/PostgreSQL/EF bootstrap: migrations, persistence, concurrency/idempotency, diagnostics |
| 006 | Authoritative battle orchestration: accept/resolve/settle/recover, resource isolation, durable snapshots |
| 007 | PvP domain/application: Friends/Ranked/rating/exposure/shields/Revenge/history/leaderboards |
| 008 | Unity playable vertical slice: full management→battle→result loop, readable16v16 placeholders |
| 009 | Steam identity/Friends adapters, isolated test mode, official sandbox checks where available |
| 010 | Credits/retention/moderation: grants/conversion/shields/unlocks, purchase reconciliation, sandbox commerce |
| 011 | Integrated hardening: crash/race/retry, Windows native regressions, economy/paid-stack simulation, reproducible package |
| 012 | Separate final art/product balance/real commerce/hosting/release approvals and execution |

Не зупиняти великий pass після003 лише через стару пораду «next milestone economy». Детальні дозволи, sequencing, gates, git discipline і stop boundaries — у **AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md**.

## 19. Superseded rules і узгодження документації

| Старий текст / припущення | Актуальна заміна |
|---|---|
| Mandatory3v3 / max6 / manual squad | Max16, automatic ready roster, asymmetry |
| One-hit / round series / persistent Energy | HP/Damage/Armor, one battle/round, no Energy |
| Gear absence excludes / free fallback weapon | Weaponless participates, no attack/fallback |
| Injury latch / full heal from Endurance | Exact threshold/no latch; CurrentHP unchanged by MaxHP upgrade |
| Live defense wounds/BB drain / infinite defense ammo | Full HP snapshot, virtual finite100% capacity, no live debit |
| Action-count gameplay draw / Team Power tie-break | Simulated duration Draw; technical guard failure; no tie-break |
| Draw Money10%, FighterXP35%; Loss ClubXP0 | Q05: Draw25/35/50, Loss0/10/25 |
| Reduced friend percentage OPEN / reward reset after loss | 100/50/25/0, exhaustion persists through window |
| Unlimited repeated friend rating losses / calendar defense day | Max1 pair loss; rolling24h cap4 |
| Friends non-ranked-only / Revenge non-ranked-only | Conditional friend rating; eligible Revenge120% origin loss |
| Discovery reserves exposure / Revenge bypasses cap | Reserve at actual rated start; global cap includes Revenge |
| Shield own Ranked interaction OPEN | Own Ranked cancels; rated Revenge cancels; Friend/non-rated keeps |
| Draw Revenge attempt OPEN / in-flight expiry cancels | Draw consumes attempt; accepted battle finishes; no chains |
| Separate Base and MK1 / cosmetic-only MK | Base=MK1; MK2soft/MK3Credits; small stats allowed |
| Early unlock grants item / unlimited bypass | Access only; soft purchase separately; progressive depth0/1/2/3 |
| Zero/unspecified starter Credits / bidirectional exchange | Starter10; only Credits→soft at configurable fixed rate |
| Final3D art / automatic Project Airsoft reuse | Art OPEN; independent project; no approved reuse |
| Backend stack unspecified | Unity/C#/ASP.NET Core/PostgreSQL/EF; exact backend pins deferred |
| Q05–Q15 OPEN / Q12 native blocker | Q01–Q15 CLOSED decisions; IL2CPP final PASS |
| Unity/core not created / implementation never authorized | 001–002 implemented and verified; future pass separately activated |

Master є consolidation overlay для S02–S11: старі OPEN/STOP/status statements читаються лише у контексті відповідного історичного milestone. Історичні exact user requests і verification reports не переписуються під нову дату. Новий агент спершу читає цей master, а не реконструює поточну policy зі старих уривків.

## 20. Acceptance цього master

Документ охоплює project boundary, DNA/loop, roster/recruitment, stats/training, HP/recovery, BB, gear/MK, exact implemented formulas, deterministic/wire contract, PvP/Friends/Ranked/Revenge/shields, rewards/rating, Credits/economy, backend/Steam/DB/stack, verified status, invariants, balance, art OPEN, roadmap, усі Q closures і superseded rules. Відсутні final price table/production art/live credentials не маскуються вигаданими approved facts.

Поточний pass не починає implementation003, не змінює game code/config/golden fixtures, не запускає payments/deployment і не змінює Project Airsoft.

### Перевірка доставленої консолідації

Перевірено 24 документаційні файли в repository: 2 нові masters і 22 узгоджені navigation/status/reference documents; 37 таблиць, 258 локальних Markdown links, послідовне покриття Q01–Q15 — помилок не виявлено. Старі subsystem тексти збережені під явним historical/consolidated banner, актуальні questions та roadmap посилаються на master. Exact user source records, research manifest/reuse register, implementation reports і весь game code/config залишилися незмінними.

Обидві delivery-копії збігаються з repo-копіями. Git HEAD лишився `d553cd3d9fee998a9764be04b63f8881658d299f`; новий commit не створено, documentation changes залишено у working tree для review/окремого docs commit. Game tests не запускалися повторно, оскільки цей pass не змінює implementation.
