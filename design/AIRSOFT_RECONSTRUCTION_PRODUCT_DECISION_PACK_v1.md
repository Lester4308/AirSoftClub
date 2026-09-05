# AIRSOFT RECONSTRUCTION PRODUCT DECISION PACK v1

**Критична межа:** нова незалежна гра у власному репозиторії Airsoft_Club_Game. Project Airsoft / Airsoft Manager не є базою цього продукту. Жодні його рішення автоматично не успадковуються. Канонічна [межа проєкту](../PROJECT_BOUNDARY.md).

Дата: 2026-09-05. Окрема продуктова лінія: **Airsoft Club Manager** — робочий опис, не остаточна назва. Платформа: PC / Steam. Стан: **сформований design baseline; баланс потребує прототипування**. Код гри не створюється в межах цього пакета.

## 1. Продуктове рішення

Створити швидку management-first гру: найняти власних бійців, розвинути їх, підібрати комплект, вибрати суперника, переглянути автоматичний матч і використати результат для наступної зміни команди. Важливо, щоб гравець міг пояснити: «Я змінив підготовку — і побачив її наслідки».

Рекомендований v1: три характеристики, 3v3 як єдиний рейтинговий формат, стартовий склад із трьох бійців та четвертим безкоштовним вибором після вступного матчу; максимум шість у клубі. Три слоти спорядження. Один матч — до трьох коротких раундів. Один зарахований hit виводить бійця до кінця раунду; між раундами склад відновлюється. Поза матчем немає energy gate, травм або платного відновлення.

Соціальний фундамент: Steam Friends для особистого суперництва, окремий Rating Match для чесного рейтингу та Revenge як одноразове повернення до кривдника. Друг може бути offline. Результат, інвентар і прогрес обчислює та зберігає backend.

Це не об'єднання з Project Airsoft: немає ручної стрільби, live PvP, наказів під час матчу, глибокої тактичної симуляції, відкритого світу чи управління великою організацією.

## 2. Як читати доказову базу

| Позначка | Значення |
|---|---|
| ORIGINAL CONFIRMED | Безпосередня UI/відео/текстова фіксація у дослідженні; діє тільки в межах конкретного спостереження |
| ORIGINAL STRONGLY SUPPORTED | Сильне зіставлення непрямих доказів; не встановлює прихованого алгоритму |
| RECONSTRUCTION DECISION | Нова технічна/поведінкова реалізація для відтворення підтвердженого досвіду |
| NEW AIRSOFT DESIGN | Свідоме продуктове рішення нової гри |

UNKNOWN, PROBABLE і CONTRADICTORY з архіву залишаються такими. Вони не підвищуються до CONFIRMED через повторення в master/spec. Посилання E### — записи оригінального ledger; доступ до джерел і журнал прочитаного: [RESEARCH_BASIS.md](RESEARCH_BASIS.md).

**Важливі уточнення архівного summary:** asynchronous model має статус STRONGLY SUPPORTED за спеціалізованим SOCIAL_AND_PVP; 16 підтверджено як максимум команди, але не як одночасний deployment; конкретні формули battle simulation у reconstruction spec — авторські рішення. Показані 44/44 та 36/32 rewards — приклади, а не формули. Energy не видно у первинних кадрах, гайди заперечують gate, окремий текст суперечить їм. Візуально перевірені три кадри також не дають підстав переносити один опис художнього стилю на всі версії.

Увесь новий production content створюється незалежно. Архів залишається зовнішньою доказовою базою. Не переносити старі item names, персонажів, графіку, звуки, логотипи, code/API payloads або baseline цін як контент нової гри.

## 3. Що залишаємо максимально близько

| Цінність оригіналу | Доказ | Рішення та наслідок |
|---|---|---|
| Команда складається з індивідуальних найманих бійців | E007, E010, E022; CONFIRMED | Імена, зовнішність, постійні builds; розвиток конкретного бійця має цінність |
| Три зрозумілі trainable stats | E002, E010, E018; CONFIRMED | Зберегти три; не множити підсистеми заради реалізму |
| Невеликий набір слотів + порівняння предметів | E010–E011, E021; CONFIRMED | Три слоти; обов'язковий захист очей; видимі tradeoffs |
| Вибір суперника і друзів перед боєм | E009, E014; CONFIRMED | Steam Friends та preview суперника; ranked selection обмежена fairness policy |
| Автоматичний спостережуваний payoff | E012, E020; CONFIRMED | Watch / speed / skip, без ручних наказів |
| Результат веде до наступного покращення | E016–E018; CONFIRMED | Нагорода, progress bar, пояснення матчу, одна наступна дія |
| Асинхронне суперництво | E009, E014, E026–E027; STRONGLY SUPPORTED | Immutable snapshots і offline defense — RECONSTRUCTION DECISION |

## 4. Що адаптуємо, модернізуємо і відкидаємо

**Адаптація:** weapon → airsoft replica; ammunition → BBs; safety slot → eye/face protection; clothing → tactical clothing; навчання → Training Center; місце повернення команди → Ready Room. Старі терміни використовуються тільки в історичних порівняннях.

**Модернізація:** швидка навігація без обов'язкового міського хабу; явна різниця roster/deployed squad; доступні без hover порівняння; пояснення hit/miss; серверні транзакції; повне відновлення після disconnect; сучасний незалежний art; текстове дублювання кольорів; Steam identity adapter.

**Відкидаємо:** literal HP/armor sponge, оплату за вихід із recovery, premium training, обов'язкову соціальну публікацію, перевагу від кількості друзів, ранній roster16, нову функцію невідомої центральної будівлі, перенос старих цін/брендів. Наявність premium purchase або friend combat bonus в оригіналі не доведена — їх не слід описувати як «видалені підтверджені функції».

## 5. Fighter model і airsoft hit model — PD-03, PD-04

**Original evidence:** E010/E018 підтверджують Endurance/Agility/Accuracy, HP і похідні поля; E020 — числову шкоду та MISS. Точні зв'язки невідомі.

| Варіант | Перевага | Ціна |
|---|---|---|
| Три stats + буквальні HP/armor | Найближча математична структура | Слабка відповідність airsoft; захист ніби дозволяє ігнорувати попадання |
| **Три stats + один hit + серія коротких раундів** | Зрозумілий спортивний контекст, builds і драматургія | Треба заново перевірити тривалість та випадковість |
| Шість stats + suppression/awareness/wounds | Більше спеціалізації | Різко дорожчий баланс, пояснення і контент |

**Рекомендація / рішення baseline:** Accuracy впливає на прицільність, Mobility — на ефективність коротких виходів з укриття, Endurance — на збереження темпу в раунді. Handling/reload/range — властивості зброї, не додаткові trainable stats. Стать і зовнішність косметичні. Один валідний hit → OUT; жодного зменшення шкоди бронею. Best-of-three зменшує вагу одного випадкового вибуття. Stamina діє тільки всередині раунду і не є health.

**Impact:** зберігаємо підготовку, але змінюємо модель витривалості найбільш суттєво. Gate: у першому прототипі порівняти читабельність і вплив builds з HP-подібним альтернативним макетом. Якщо один hit дає надто короткі матчі, спочатку змінювати exposure/hit cadence і раунди; не додавати приховану «броню».

## 6. Roster і формати — PD-05, PD-06

**Original evidence:** E002/E014 — 16 бійців у певному контексті; E007 — безкоштовний перший, вісім кандидатів; E025 — 100/300 за наступних тільки STRONGLY SUPPORTED. Active/reserve split та окремі формати UNKNOWN.

| Питання | 2–3 варіанти | Рекомендація | Impact |
|---|---|---|---|
| Максимум roster | 4 / **6** / 16 | 6 у v1; MVP максимум 4 | До трьох замін, читається одним екраном; не оплачувати силу кількістю |
| Основний формат | 2v2 / **3v3** / 4v4 | 3v3; 1v1 лише tutorial/drill | 2v2 слабше показує командну композицію; 4v4 дорожчий за контентом |
| Старт | один з довгим наймом / **три стартові** / готові шість | Три з комплектами; четвертий вибирається після вступного матчу | До першого team battle без grind; найм залишається раннім рішенням |

1v1 не має окремого рейтингу або повторних економічних rewards. 2v2 — внутрішній test harness, не додаткова публічна черга. 4v4 відкладено. Ranked 3v3 ніколи не зводиться з 2v3. Будь-які три valid бійці можуть бути defense squad. Резерв не збільшує нагороду.

Дешевий rookie не стає назавжди непридатним: однаковий стартовий stat budget, різні розподіли, однаковий максимальний budget тренування. Немає прихованої якості або платних rerolls. Розвинена трійка сильніша за шість нетренованих, бо на поле виходять тільки троє.

## 7. Core equipment і weapons — PD-07, PD-08

**Original evidence:** E008/E010/E011/E019/E021: три слоти, stats зброї, залежність спорядження, обов'язкова маска. Наявність tooltip set relationship підтверджена; його величина невідома. Принцип «дорожче не завжди краще» беремо як design intent, не як доведену властивість історичного балансу.

| Варіант | Рекомендація та impact |
|---|---|
| **Primary + Protection + Tactical kit** | v1: три слоти; зрозумілий gear choice; Tactical kit об'єднує clothing/rig/boots |
| Primary + Secondary + чотири gear slots | Post-v1 за результатами тестів; дорожчі loadouts і onboarding |
| Дев'ять окремих слотів | Відкинути у v1: багато слабких рішень перед боєм |

Protection — обов'язковий безкоштовний базовий комплект захисту очей/обличчя, без stat advantage дорожчих варіантів. Headgear — зовнішність. Немає захисту, який скасовує hit. Tactical kit дає темп/мобільність з взаємними компромісами. Secondary, gloves, окремі accessories, set bonuses і weapon attachments відкладено.

Зброя: MVP — AR та SMG; v1 — DMR після першого освоєння build system. AR універсальна; SMG має кращий handling і гіршу дальню ефективність; DMR точніша на далекій лінії, повільніша й вимогливіша до мобільності. Pistol/sniper/shotgun — лише кандидати для розширення. Unlock відкриває альтернативу, а не гарантований апгрейд. Реальні торгові назви не потрібні.

## 8. Economy та AMMO DESIGN DECISION PACK — PD-09, PD-10, PD-11

**Original evidence:** E015 — persistent ammo і ціни двох пакетів; E016/E017 — variable rewards; E006/E029 — стартові ресурси; E010/E018 — витрата training currency. Стару exchange rate і монетизацію не встановлено.

| Ammo option | Плюси | Мінуси | Рішення |
|---|---|---|---|
| Persistent BB stock і ручні покупки | Найближче до resource management | Порожній склад перериває loop; новачок може збанкрутіти | Не v1 |
| **Автоматичний operational cost на матч** | Зберігає витрати, прибирає shopping chores | Витрата абстрактна, не кожна BB списується з wallet | Baseline |
| BBs тільки всередині бою, без cost | Найшвидше | Менше економічного вибору | Резервний варіант, якщо cost не додає цінності |

Плата — **фіксований показаний бюджет матчу**, не покарання за кожну анімацію пострілу. Battle telemetry зберігає shots/BB expenditure, але wallet не залежить від довжини replay. BB loadout поповнюється кожного раунду; немає складу BB у persistent inventory v1.

Економіка: одна spendable валюта Credits. Fighter XP відкриває право на тренування, Club XP — каталожні альтернативи. Training витрачає Credits і доступний рівень training budget, без другої купованої валюти. Купівля гри — попередня business recommendation; ціну і контентні DLC тут не встановлено. Платна сила, прискорення XP, paid recruits та training відсутні.

Стартова tuning hypothesis: 300 Credits після видачі базових комплектів; sidegrade150; перше тренування 60; operational cost15; online eligible win gross60, draw40, loss30. Net:45/25/15. Третій і наступні бої з тим самим суперником за rolling24h — без cost, credits і XP; другий дає 50% gross та 50% cost з округленням униз. Лічильник directed attacker→defender спільний для всіх PvP режимів. Ніяких rewards за offline sandbox або повтор перегляду.

При win rate50% середній net =30; sidegrade за 5 eligible matches; перше тренування за 2. Чотири покупки по 150 після стартових 300 вимагають ще 300, тобто близько 10 таких матчів. Це арифметична ілюстрація, не прогноз поведінки аудиторії. Отримані 30/матч не доводять довгострокову стабільність економіки.

Cost віднімається при settlement із gross, upfront wallet не потрібен: при балансі 0 гравець може продовжувати. Defense не витрачає гроші та не створює пасивного доходу. Повторні training-PvE мають net10 і fighterXP5: безпечний, повільний спосіб продовжувати при малій online population. Перші scripted milestones одноразові. Spending не потрібний для доступу до наступного бою.

**Impact:** відпадає supply micromanagement; лишається вибір recruit/equipment/training. У v1 power budget обмежений, тому нескінченне накопичення Credits не дає нескінченної сили. Довгострокові косметичні sinks відкриті, не виправдовують P2W або repair tax.

## 9. Training та RECOVERY / ENERGY / FATIGUE — PD-12, PD-13

**Evidence:** E002/E010/E018 — тренуються три stats; precise increment UNKNOWN. Resp видно на карті, interior/cost UNKNOWN; E024/E025 кажуть, що energy gate відсутній; E026 має суперечливий вислів.

| Питання | Варіанти | Recommendation / impact |
|---|---|---|
| Training | таймери / **миттєвий drill після XP gate** / автоматичний розподіл | Гравець сам обирає stat; немає черги очікування або premium skip |
| Recovery | persistent fatigue / injuries + лікування / **reset після раунду** | Ready Room показує результати й readiness комплектації, не energy timer |
| Specialization | skill tree / **stat distribution + gear** / шість незалежних stats | MVP без skill tree; зрозуміти основу перед додаванням здібностей |

Один upgrade дає +1 обраному stat у межах caps. Безкоштовне переналаштування витрачених training points поза матчем запобігає permanent mistakes; ціни вже виконаних upgrades не повертаються. Бійці не стають недоступними через тренування. Stamina reset між раундами; injuries не вводяться. Резерв потрібний для builds, а не для обходу штучного очікування.

## 10. Бій та presentation — PD-14, PD-15

**Evidence:** E012/E020 підтверджують fixed positions, automatic attacks, MISS/HP та skip. Mechanical map effects UNKNOWN.

Варіанти presentation: 2D animated (дешевше); 2.5D (компроміс); **stylized low-poly 3D** (краща читабельність airsoft kit, дорожчі моделі/анімація). Рекомендація: 3D як ціль v1, але перший loop — primitives/debug presentation. Камера оглядова, сталі бойові anchors, короткі lean/peek/reload/OUT анімації. Без фізичної симуляції куль і pathfinding у MVP. Перехід від 3D до 2.5D можливий без зміни simulation.

Tuning target: раунд максимум 30 simulated seconds; матч до 3 раундів, звичайний перегляд 45–90 секунд, skip доступний одразу. Раунд виграє сторона з surviving fighters; на timeout — більше surviving fighters, при рівності draw. Після трьох раундів порівнюються round wins; рівність → match draw. Дві перемоги достроково закінчують матч.

Підготовка: вибір трійки й призначення трьох anchors. MVP одна симетрична arena з короткою/середньою/далекою геометрією ліній, alternate starting sides кожного раунду; v1 до трьох арен після тестів. Немає платного вибору вигідної карти або RNG reroll. Preview показує формат, map, squad, cost/reward policy; seed прихований до settlement.

Технічний вибір: client-authoritative — неприйнятно для online; server-only nondeterministic — складніше audit; **deterministic server simulation + event replay** — baseline. MatchConfig + snapshots A/B + seed + simulation build + ruleset/balance/map versions → immutable result/events. Клієнт лише відтворює події; skip не змінює reward. Деталі: [AIRSOFT_COMBAT_SYSTEM.md](AIRSOFT_COMBAT_SYSTEM.md).

## 11. Steam Friends PvP — PD-16

**Original evidence:** E009/E014 — аватари, імена, attack/profile. Offline snapshot implementation — нова реалізація.

Варіанти: тільки friends (порожня гра без друзів); friends + rated challenges з однаковими rewards (ризик договорних боїв); **friends rivalry unranked + окремий ranked mode**. Обираємо третій. Friends мають дозволені progression rewards за загальним repetition policy, але ніколи не передають rating. Абсолютно unfarmable social rewards не обіцяємо; caps прогресу і abuse telemetry обмежують наслідки.

Friends card: avatar, Steam display name, окреме Club name, rating, strength estimate, format3v3, View Club / Challenge. Online presence не є умовою. Last active не показуємо у v1. Нуль друзів, помилка SDK і private social access мають окремі empty/error states.

Steam client може перелічити друзів через ISteamFriends; це не гарантує server-verifiable friendship. Client-supplied IDs використовуємо для discovery відкритих club cards, не як право читання приватного profile або отримання бонусу. Відсутність підтвердження friendship не дає доступу до friends-only секретів. Деталі й офіційні джерела: [STEAM_SOCIAL_PVP_ARCHITECTURE.md](STEAM_SOCIAL_PVP_ARCHITECTURE.md).

## 12. Rating PvP і Revenge — PD-17, PD-18

**Original evidence:** списки opponents E014; rating/revenge/cooldown формули UNKNOWN. Усе нижче — NEW AIRSOFT DESIGN.

Rating options: спільний рейтинг усіх challenges (collusion); рейтинг лише активного нападника (незвичний асиметричний ladder); **симетричний рейтинг лише server-issued ranked matches**. Baseline — третій. Steam Friends зберігає особистий rivalry record окремо.

Ranked3v3: поточний rating ±100; при нестачі ±200, потім ±300; ніколи не послаблюємо формат, readiness або progression compatibility. Додатково power estimate ratio0.8–1.25 та одна progression band; перевіряти sandbagging через досягнутий training budget, а не поточні зняті предмети. Це tuning, не обіцянка ідеального matchmaking.

Сервер видає одну rated offer на 10 хвилин. Повторне відкриття повертає ту саму; новий offer після expiry або завершення. Для вільного вибору є нерейтингові challenges. Offer не показує seed/outcome. Немає бонусів за кількість friends. При порожньому пулі — явний PvE drill без rating; не маскувати ботів під людей.

Rating старт 1000; Elo-style K24, draw0.5; зміни обох гравців атомарні. Один rated match на unordered pair за rolling24h включно з обома напрямками. До 5 rated incoming defenses на гравця за rolling24h — початкова захисна політика від великої кількості пасивних втрат (теоретично не більше 120 points через incoming defense). Після 5 він виходить із ranked defense pool до звільнення квоти; friendly challenge лишається. Це не cooldown на гру. Зафіксувати effect на pool size і переглянути ліміт на beta; за браку пулу не приховувати проблему.

Defense ranked availability вмикається під час вступу в ranked; UI пояснює offline rating effects. Після 7 діб без входу клуб виключається з ranked discovery. Уже прийняті matches завершуються. При деактивації ranked одночасно недоступні ranked attacks; повторне ввімкнення не скидає quota/pair history.

Revenge options: історичний snapshot (точне порівняння, але старий баланс); **актуальний valid snapshot** (живе суперництво); ranked revenge (farm loop). Baseline: актуальний snapshot, завжди unranked, ticket живе 72 години, один на qualifying incoming non-revenge battle. При зміні трійки UI попереджає, що це новий склад. Немає bonus rewards, repetition policy спільний з friends/ranked, нуль rating обом. Revenge не породжує новий revenge ticket, тому нескінченний ланцюг не утворюється. Старий матч можна повторно переглянути як replay без нагород.

## 13. Backend, identity, snapshot та offline — PD-19–PD-23

Варіанти backend: client saves із синхронізацією (підробка прогресу); **modular monolith + transactional DB + worker**; мікросервіси (передчасна операційна складність). Рекомендація — monolith із чіткими доменами: Auth, Profiles/Clubs, Fighters, Inventory, Economy, Snapshots, Matchmaking, Resolution, Rating, History, Leaderboards, Notifications.

Online PlayerProfile має internal UUID і зв'язок SteamID64; decimal string у JSON, не floating-point Number. Steam nickname змінний. Steam ticket перевіряється сервером; production ніколи не приймає dev identity stub. Для paid ownership передбачено окрему перевірку entitlement. Profile migration між платформами — майбутній account-linking flow з підтвердженням обох identities; не merge за nickname.

DefenseSnapshot містить троє бійців, effective stats, item definitions/versions, anchors, ruleset/balance/map compatibility, timestamp і hash. Display rating/strength можна зберегти як historical context; current rating для settlement читається з DB. Snapshot створює backend з owned state; клієнт не завантажує довільні stats. Усі наступні зміни створюють новий immutable snapshot. Обидва snapshot IDs фіксуються при acceptance матчу. Зняття спорядження після цього не змінює battle input і не дублює предмет як власність.

Crash/retry: accepted → queued → resolved → settled; idempotency keys, wallet ledger, unique settlement(matchId), durable jobs/outbox. Rating, wallet, progression, history і defense quota узгоджуються транзакційно. Client claim не потрібний; Result — вже зарахований стан.

Offline options: нічого не відкривається; повний offline progression з merge (небезпечний); **cached club + ізольований practice**. Baseline — третій. Без мережі можна оглянути останній cache та зіграти practice з ним/вбудованим squad; жодних online rewards, progression, recruits або training. При reconnect сервер замінює online cache, а не імпортує локальні гроші. Якщо cache немає — starter practice, без створення online profile. Для MVP практика може бути primitives, але online/out-of-sync status завжди явний.

Replay reproducibility: зберігати seed недостатньо. Потрібні frozen input, PRNG algorithm, rounding/order rules, simulation build і всі tables. Старі records залишаються прив'язані до їх версій; balance patch впливає на нові snapshots і нові matches. Event logs дають playback без повторного запуску старого build; archive build дає audit. Storage/retention budget — відкритий ризик, не обіцянка необмеженого public replay hosting.

## 14. MVP і порядок реалізації — PD-24

Варіанти: відразу повний Steam social launch; **один завершений loop → durable backend → social beta**; production UI спочатку. Обрати другий.

**M1 — one complete loop:** project foundation; Steam-ready adapter зі stub для dev; profile/club; три starters + recruit choice; fighter training; inventory і purchase/equip; NPC opponent snapshot; deterministic3v3; result/reward; save/reload у dev persistence. Без production art, рейтингу і справжніх online accounts. Це playable prototype, не secure release.

**M2 — authoritative foundation:** реальна persistence й транзакції, Steam authentication/ownership integration, crash/retry tests, immutable snapshots, робочий offline cache/practice. Тут закривається user-facing MVP state persistence. Зовнішніх economy-sensitive users не підключати до dev save.

**M3 — social beta:** asynchronous human defense; Steam Friends; server matchmaking/rating; revenge; leaderboards Global/Friends/Around Me; in-game notifications; privacy/moderation; потім 3D/UI polish. Public v1 включає social loop, хоча M1 його ще не має.

**M4 — expansion:** до 6 roster, DMR, додаткові карти/косметика за результатами балансу. Steam Deck — наступна compatibility перевірка, не автоматична сертифікація.

Детальні dependency gates: [MVP_IMPLEMENTATION_PLAN.md](MVP_IMPLEMENTATION_PLAN.md).

## 15. Критерії якості й відкриті ризики

Числові targets — NEW AIRSOFT DESIGN hypotheses: перший team battle до 5 хвилин; повторна підготовка до 60 секунд; типовий watched match45–90 секунд; після Result до наступного challenge не більше 3 навігаційних переходів. Гравець після трьох матчів пояснює хоча б один ефект build. Це треба перевірити на людях, не лише автоматичними тестами.

| Ризик | Перевірка / умова перегляду |
|---|---|
| One-hit мало нагороджує підготовку | Seed sweep, side swaps і user tests; порівняти modest stat advantage та gear counters |
| DMR або Mobility домінує | Контрольні рівні budgets; win-rate matrix на всіх аренах; коригувати таблиці, не ціни як маску балансу |
| Ranked caps розріджують пул | Симуляція population100/1k/10k; доля no-offer, повторів, defense exposure |
| Friendly farming і collusion rings | Перехресні pair counters, anomaly review, capped power progression; не заявляти повний захист від alt accounts |
| Credits втрачають сенс після cap | Виміряти time-to-cap і surplus; додавати sidegrades/cosmetics за попитом |
| Steam доступ/credentials не надані | Документований adapter contract; реальна інтеграційна перевірка на M2, зараз не заявляється виконаною |
| Replay storage/version cost | Замір bytes/match × battles/day × retention; окрема політика до public launch |
| Не відома команда та бюджет | Без обіцянки календарної дати; оцінка після M1 на фактичній швидкості |

Повний реєстр 24 рішень: [PRODUCT_DECISIONS.md](PRODUCT_DECISIONS.md). Mapping: [PAINTBALL_TO_AIRSOFT_MAPPING.md](PAINTBALL_TO_AIRSOFT_MAPPING.md). Назви, числовий баланс, engine і бізнес-ціна не вважаються затвердженими користувачем. Формування цього пакета завершене; implementation не розпочато.
