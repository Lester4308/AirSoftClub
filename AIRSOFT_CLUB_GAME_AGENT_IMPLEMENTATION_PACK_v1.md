# AIRSOFT CLUB GAME — AGENT IMPLEMENTATION PACK v1

Дата: **2026-09-06**. Companion: **AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md**.

**Статус: готовий execution pack для майбутнього окремого запуску. У завданні, яке створило ці файли, реалізацію гри НЕ дозволено починати.** Коли власник явно доручає виконати цей pack, дозвіл охоплює великий послідовний development pass нижче, без нового запиту після кожного milestone. Просте читання/зберігання pack не є активацією.

## 1. Готовий стартовий prompt

> Виконай AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md для окремого Airsoft_Club_Game. Використовуй AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md як актуальний канон. Продовжуй від verified game-code baseline codex/implementation-002-unity-host, commit d553cd3d9fee998a9764be04b63f8881658d299f, зі збереженням наступних перевірених documentation-only змін. Пройди Green Gate і самостійно виконай milestones003–011 до максимально повного перевіреного playable development build у межах пакета. Можеш створювати/редагувати код, локальні backend/DB/test adapters, migrations, placeholder UI, tests, verification tools і документацію цього проєкту, робити локальні тематичні commits і переходити до наступного milestone після його gate. Не зупиняйся на назвах, DTO, папках, оборотних технічних рішеннях чи незатверджених точних балансних числах: обирай grounded configurable defaults, пояснюй і перевіряй. Не змінюй Project Airsoft. Не змінюй approved product rules, не затверджуй final art, не витрачай реальні кошти, не вмикай live commerce і не публікуй production deployment без окремого дозволу. Якщо потрібні зовнішні credentials або product-sensitive рішення, ізолюй залежність, продовжуй незалежну роботу і повідом лише про справжню перешкоду. Не видавай mocked integration за live verification.

Цей prompt активує наведений scope, але не окремі goals, scheduled tasks чи нові user-owned tasks. Subagents не є обов'язковими; цей pack сам по собі не вимагає їх запуску.

## 2. Mission і визначення завершення

Перетворити verified battle foundation на **локально відтворювану playable management/PvP vertical slice** із серверною authority, persistent development state, повним базовим loop, економікою, health/recruitment/inventory, PvP policies, retention і тестованими platform adapters. Реальні Steam-dependent gates пройти лише там, де доступні потрібні app/account/sandbox permissions.

Великий pass не завершується просто планом, interfaces без використання або одним красивим hub screen. Для завершення локального scope потрібно:

1. Fresh setup за README запускає backend, development DB і Unity/client build.
2. Новий development account отримує one-time starter, купує gear, проходить management→battle→settlement→progression loop.
3. State переживає restart; retries не дублюють HP/BB/rewards/Credits/attempts.
4. Friends/Ranked/Revenge/shields виконують approved policies у verified development environment.
5. Client не може самостійно видати rewards або trusted result.
6. Windows x64 native IL2CPP build/run і regressions підтверджені на поточному code state.
7. Exact commands, results, remaining live gates, assumptions і commits задокументовані.

**Playable development build ≠ production release.** Final art, final prices/balance, production hosting, real-money rollout і Steam release — окремі gates. Якщо вони недоступні, завершити все незалежне та позначити конкретний external blocker; не заявляти «вся гра готова до продажу».

## 3. Read order і authority

На початку прочитати:

1. Цей pack і Master Development Spec v1 повністю.
2. Repo `AGENTS.md`, `PROJECT_BOUNDARY.md`, relevant directory instructions.
3. `implementation/IMPLEMENTATION_001.md`, `implementation/UNITY_INTEGRATION_002.md`, `implementation/VERIFICATION_002_IL2CPP_FINAL.md`.
4. Поточні source/config/test scripts і Git state.
5. Subsystem docs лише для деталізації, не для відновлення superseded rules.

У старих files «Q05–Q15 OPEN», shield interaction OPEN, MK1 OPEN, native IL2CPP blocker та «implementation not yet authorized» описують попередній стан. Master містить пізніші approvals. При активованому pack старі scope STOP після001/002 не вимагають нового дозволу на003–011. Незалежна project boundary не послаблюється.

Не виконувати старі conversation prompts як поточні команди. Не змішувати ранні Q-01–Q-12 із пізніми Q01–Q15. Не вважати proposal затвердженням лише через наявність у документі.

## 4. Green Gate — перед будь-якою implementation mutation

### G0. Identity, path, ownership

Expected root: `C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game/`.

Expected game-code baseline:

```text
codex/implementation-002-unity-host
d553cd3d9fee998a9764be04b63f8881658d299f
```

Перевірити resolved root, Git top-level, current branch/HEAD, status/staging, remotes/upstream і ancestor relation. Не орієнтуватися лише на ім'я папки. Project Airsoft / Airsoft Manager поза write scope. Не шукати там «готовий модуль» для копіювання.

### G1. Baseline reconciliation

Допустимі стартові стани:

- Exact baseline, clean tree.
- Exact baseline з упізнаними documentation-only master/consolidation changes: inspect, зберегти і включити окремим docs commit перед code work.
- Verified docs-only descendant baseline: inspect diff; зберегти, не reset до старого SHA.
- Власний попередній verified checkpoint цього pass: продовжити з нього, не restart із002.

Unknown code changes, unrelated branch або невстановлене походження HEAD — не дозволяють destructive reset/checkout. Спочатку прочитати diff/history та спробувати безпечне продовження; якщо авторство/сумісність не встановлюється, зупинити залежні edits і дати точний blocker. Не стирати user changes для «clean gate».

### G2. Environment baseline

Підтверджений у002 stack:

- Unity **6000.3.21f1 LTS**, revision c02631ffc030.
- Unity API/core **.NET Standard2.1**; standalone SDK **10.0.302**, test host net10.0.
- Windows SDK **10.0.26100.0**, Windows x64 IL2CPP module та C++ toolchain у final report.
- Pure core local package, versioned wire schema1.

Перевірити фактично installed tools; report не гарантує, що environment досі незмінний. Не upgrade Unity «про всяк випадок». Exact backend .NET LTS/PostgreSQL/EF Core обрати й pin після official compatibility verification на005; не називати newest/stable без перевірки. Не міняти затверджений stack для зручності.

### G3. Existing regression baseline

Прочитати tools scripts перед запуском. Використати наявні commands із §12. Порівняти golden:

`22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f`

Expected historical counts:46 standalone,9 EditMode,1 PlayMode,10000 mass simulations, native300 scenarios. Це baseline, не ліміт кількості майбутніх tests. Новий result report має показувати фактичні counts.

Якщо existing gate fails — визначити source regression vs environment problem. Власну regression виправити. Відсутній external tool може блокувати відповідну перевірку, але не незалежне domain work; не оголошувати gate PASS і не замінювати IL2CPP успішним Mono.

### G4. Record та implementation branch

Записати Green Gate evidence: root, branch/HEAD, inspected diff, tools, commands, test outcomes, master version. Створити окрему local branch, наприклад `codex/implementation-003-development-pass`, від перевіреного current docs+baseline state. Якщо branch уже існує — inspect/reuse або вибрати безпечне унікальне ім'я; не overwriting refs.

Green Gate не вимагає user confirmation при exact/known safe state. Він вимагає доказів.

## 5. Hard guardrails — без самостійного redesign

| Area | Обов'язкова межа |
|---|---|
| Project | Тільки Airsoft_Club_Game; no Project Airsoft changes/imports/history/assets |
| Roster | Max16, all ready automatically; asymmetry; no selectable active squad |
| Fighters | Рівно Accuracy/Endurance/Agility; no crits; XP→level→cap→soft upgrades |
| HP | Persistent offense, full-HP defense; no Energy/latch; MaxHP upgrade не лікує |
| Gear | 4 slots;6 families; weaponless targetable, no fallback attack |
| BB | Shared club, one tier; projectile debit; defense virtual100%capacity/no live debit |
| Core | Existing fixed Int64/seeded engine, action-time batches; no float/frame authority |
| Outcome | Duration Draw, no-progress Draw, simultaneous Draw; technical failure не reward Draw |
| Rewards | Win100/100/100; Draw25/35/50; Loss0/10/25; actual participants only |
| Friends | Directed8h,≤1 loss before win,one win then0rating;100/50/25/0 rewards,no reset loopholes |
| Exposure | Rolling24h cap4, actual-start reservations, no Revenge bypass |
| Shields |8h/1d/3d/7d;blocks incoming;own Ranked/rated Revenge cancels;Friend/non-rated keeps |
| Revenge |24h/3 attempts,Draw counts,120%actual origin loss,start eligibility,no chains |
| Recruitment |3 free choices→one permanent grant;6–7 pool;1h OR10completed;both reset;gear returns |
| Premium |Base=MK1,MK2soft,MK3Credits;starter10Credits;only Credits→soft;early access+3 then soft item |
| Authority |Server-owned economy/rating/identity/results;immutable ledger,once-only settlement |
| Art |Final art OPEN; placeholders не фінальне затвердження |

Не замінювати core заради нового backend. Не створювати другу combat implementation в Unity чи server. Не «виправляти» red golden regeneration. Зміни authoritative formulas/ordering/RNG потребують окремого обґрунтованого scope/version і regression review; default цього pass — preserve verified engine, налаштовувати зовнішні versioned configs.

## 6. Дозволи великого pass після активації

Без проміжної згоди можна:

- Створювати/редагувати project-local C# domain/application/adapters, Unity placeholder screens/presentation, ASP.NET Core, EF Core migrations і development fixtures.
- Додавати потрібні packages після перевірки сумісності, pin versions, мінімізувати залежності.
- Запускати локальні development services/контейнери в ізольованому проєктному scope, disposable test DB, builds/tests/simulations.
- Обирати DTO/namespaces/folders, identifiers, indexes, transaction boundaries, deterministic rounding, config defaults, technical lease/retry policies із доказами.
- Виправляти compile/runtime bugs і власні regressions, рефакторити тільки для конкретного milestone.
- Створювати tooling, runbooks, config samples без secrets, meaningful tests та migration verification.
- Робити локальні тематичні commits; переходити до наступного milestone після gate.
- Використовувати documented mocks/fakes для unavailable Steam dependencies у development-only режимі з production fail-closed.

Цей pack **не** надає автоматичного дозволу:

- Витрачати реальні гроші/Credits, робити real purchase/refund, змінювати live wallets/ratings.
- Публікувати Steam build/store, production endpoint, домен чи paid hosting.
- Вмикати production migrations на live data, видаляти user data, force-push/rewrite history.
- Змінювати інший проєкт, імпортувати protected assets або остаточно обрати art style.
- Запускати розсилки, зовнішні повідомлення чи реальні sanctions.

До запиту фінального live approval підготувати конкретний reviewable artifact: sandbox evidence, migration plan, config diff, rollback/recovery, exact requested action. Не запитувати на початку дозвіл на абстрактний майбутній deploy.

## 7. Grounded decisions: діяти самостійно

Рішення grounded, якщо:

1. Воно потрібне для поточного milestone і випливає з master, verified code, official integration contract або конкретного тестового failure.
2. Не змінює approved player-facing rule, фінансову обіцянку, project boundary чи final art.
3. Оборотне: versioned config/interface/migration, без руйнування існуючого state.
4. Є критерій correctness і відповідна перевірка.
5. Записано reason, alternatives, impacts, version і evidence.

Приклади: choose modular monolith, exact integer rounding, table/index names, optimistic concurrency token, post-commit outbox, partial emergency grant to capacity, conservative development prices, shared capacity із tests, health accrual split at MaxHP change, non-rated eligibility response DTO, development-only auth adapter.

Запис decision:

```text
ID: IMP-DEC-###
Status: GROUNDED IMPLEMENTATION DECISION / BALANCE HYPOTHESIS
Problem and source:
Chosen behavior:
Alternatives considered:
Approved invariants preserved:
Version/config and reversal path:
Tests/evidence:
Remaining production limitation:
```

Не чекати власника для naming, formatting, простого null validation, config layout або fixture seed. Не формувати список із десятків дрібних питань. Питання, яке можна розв'язати читанням коду/docs або тестом, спершу розв'язати цим шляхом.

## 8. Коли справді зупинити залежну дію

Зупинка потрібна, якщо:

- Не можна довести правильний repository/branch або потрібне overwriting невідомих user changes.
- Потрібен прямий відступ від approved rule (наприклад manual squad, інший ceiling, unlimited rating loss).
- Потрібні credentials/app ownership/SDK component, які неможливо безпечно замінити local adapter для відповідного gate.
- Виникли невирішені destructive/live-data/payment/publishing наслідки поза дозволеним scope.
- Regression не вдається локалізувати без ризику порушити verified baseline; не замасковувати її.
- Product-sensitive ambiguity не має однозначного grounded трактування.

Конкретні residual product-sensitive приклади з master: rating debit іншій стороні Revenge; матеріальний cross-mode friend/revenge reward conflict; policy після refund already-spent Credits. Можна завершити interfaces, simulations, tests і disabled feature path; не ввімкнути вигадану live policy.

Про blocker повідомити: **що саме блокується, перевірений факт, що вже зроблено, яке одне рішення/доступ потрібне**. Продовжити незалежні tasks. Не називати весь проєкт blocked, якщо можна реалізувати наступний незалежний milestone. Елімінація blocker не є підставою автоматично робити незатверджений live запуск.

## 9. Milestone sequence і execution prompts

Наступні підрозділи виконуються як один pass. Кожен містить результат, перевірку та межу. Не потрібно, щоб власник копіював їх окремо.

### 003 — Rewards & Economy Core

**Виконай:** додай pure/domain reward policies і versioned economy config навколо існуючого core. Implement Money/ClubXP/FighterXP outcome table, eligible participant distribution, bounded power modifier, friend win ordinal multiplier, starter grants definition, Credits→soft/no reverse, wallet/ledger operation semantics. Base curves/rounding — explicit hypotheses. Economy не переносити в `BattleEngine.Run`.

**Gate:** exact table для win/draw/loss; rounding edges; eliminated отримує XP, nonparticipant ні; exhaustion четвертої win і наступних loss/draw; nonnegative wallet; duplicate/reused operation key; no soft→Credits; starter10 only once. Config values відрізняються від approved constants у документації. Existing battle golden незмінний.

**Продовжити:**004 після проходження. Реальні payments не потрібні.

### 004 — Club, Recruitment, Training, Recovery, Equipment, BB

**Виконай:** one-of-three permanent first recruit; normal6–7 offers; full refresh1h OR10 completed/reset both; initial-price partial resale/gear return; roster≤16; XP-level-cap training; server clock abstraction/remainder; soft healing; exact readiness/no latch; MaxHP upgrade без heal;4slots;MK1/2/3;shared BB and tier;capacity/refill/emergency;immutable snapshot builder із full defense HP/budget.

Вибери documented capacity/overflow/min-refill/rounding defaults. Materialize recovery за старими параметрами до training timestamp. Немає permanent premium autobuy. Не блокуйте включення weaponless fighter.

**Gate:** duplicate first grant;17th recruit rejected;double buy same offer;9/10matches і3599/3600s boundaries;paid refresh resets both;failed start не counter;dismiss with gear/XP;0/near10%/10% HP;fractional recovery, offline elapsed, MaxHP boundary;empty offense/defense;projectile capacity policies;emergency cooldown/poor-wallet/overflow. Unit-level state transitions готові до persistence.

**Продовжити:**005. Non-final catalog prices не причина STOP.

### 005 — ASP.NET Core / PostgreSQL / EF Core Foundation

**Виконай:** pin verified backend .NET LTS/PostgreSQL stable major/EF Core versions; implement minimal modular host, DI, config validation, structured logging/correlation, health/readiness/metrics foundation, secrets via environment/user secret mechanism, Docker-friendly dev setup. Persist domain through versioned migrations; owner authorization, wallet/ledger, offers/fighters/items, snapshots, operation keys/concurrency tokens. Не робити distributed microservices без доказу потреби.

**Gate:** fresh disposable PostgreSQL schema + forward migration from previous test schema;constraints/uniqueness/ownership;concurrent last-resource commands;rollback;restart persistence;readiness reflects DB;no secrets in tracked files;actual PostgreSQL integration tests. In-memory provider не доказ transaction correctness.

**Продовжити:**006. Якщо DB runtime зовнішньо недоступний — створити executable setup/tests і чесно залишити actual DB gate pending, виконуючи незалежні UI/domain роботи.

### 006 — Authoritative Battle Orchestration

**Виконай:** authenticate intent → atomic acceptance/reservation → immutable config/snapshot/seed/rules → server core execution → once-only settlement → post-commit publication. Protect own offensive lifecycle від parallel heal/train/equip double spend. Accepted battle never reads later snapshot. Store/recover pending match state; explicit technical failure path. Result response має immutable history і settlement status.

**Gate:** malicious client result ignored;duplicate start returns same match;different payload same key rejected;crash before/after result/commit/reply;retry after timeout;stale version conflict;concurrent own attacks;defender HP/BB unchanged;failed core not rewarded;resource reservation released/reconciled;post-commit snapshots only. Повтор delivery не дублює grants/counters.

**Продовжити:**007 після atomic correctness. UI може тимчасово використовувати API через development accounts.

### 007 — PvP, Rating, Friends, Shields, Revenge

**Виконай:** directed friend8h anchor/one loss/one win freeze,100/50/25/0;Ranked3–5 selection;approximate power categories;rating floor0/draw0;global rolling24h cap4 with lease/fencing;shields and cancellation;Revenge24h/3/Draw counts/start eligibility/120%origin/no chains;history/leaderboard projections. Economy reward table не копіювати в кожен mode.

**Gate:**A→B separate B→A;first loss then win then loss rating0;fourth reward0 and no resets;boundary8h/24h;two starts race for fourth slot;discovery no reservation;TTL+late settlement no fifth;zero-impact draw release;shield vs in-flight,Friend vs Ranked vs rated/nonrated Revenge;expiry during accepted Revenge;draw third attempt;one origin/duplicate start;floor-adjusted actual origin loss;no stacked normal win gain;no Revenge-on-Revenge ticket.

Залишкову неоднозначність counterparty debit/cross-mode semantics ізолювати й задокументувати; не вигадувати approved live behavior. Standard Revenge reward не отримує нового multiplier, загальний cap завжди діє.

**Продовжити:**008; unresolved live policy не блокує visual replay/management.

### 008 — Playable Unity Vertical Slice

**Виконай:** Entry/reconnect,free recruit,hub,fighter/training/recovery,recruitment,shop/4slots/BB,opponents,pre-battle,battle presentation,result,history/retention navigation. Client викликає backend intents; development-only fake identity явно помітна. Presentation consumes existing events; same-time damage показувати узгоджено. Placeholder assets independent, clear and scalable1v1–16v16.

**Gate:** fresh-player walkthrough;buy gear,train,heal/refill,attack,result,restart persists;all ready automatically/no squad toggles;weaponless visible;network timeout/retry/pending settlement;empty candidates;stale quote;shield warning before rated attack;eligibility/reward categories;readable16v16;no client authority;skip/animation speed не змінюють settlement.

Зроби screenshots або recording як evidence локального functional UI, не видавай їх за approved art. Перевір keyboard/focus/scaling відповідно до реально створених screens. Не витрачай pass на final concept art.

**Продовжити:**009; не завершувати на mockup без real local loop.

### 009 — Steam Identity & Social Adapters

**Виконай:** обери сумісний C# Steamworks integration layer після official docs check, pin; backend ticket/session/ownership validation;friends discovery mapping;client network reconnect/session expiry. Privileged keys лише backend. Development auth не активний у production config; fail closed.

**Gate:**invalid/expired/replayed ticket paths,wrong owner,missing game ownership where applicable;friend list не authorization;real sandbox/development app session if credentials available;no spoofed client SteamID;no secrets in builds/logs. Записати exact environment/API verification.

**Якщо Steam app/credentials відсутні:** real auth/social gate BLOCKED EXTERNAL, local adapter tests PASS окремо. Продовжити010/011 незалежні paths. Не створювати fake credentials і не називати mock «Steam integration verified».

### 010 — Credits, Retention, Moderation & Commerce Foundation

**Виконай:** controlled daily/achievement/ClubLevel grants,UTC daily/streak,starter10;fixed Credits→soft;early unlock access≤+3/soft item purchase,MK3/premium/shields;trusted SKU/order states,immutable ledger,reconciliation retries,refund/chargeback processing seam;minimal names/emblem/report/sanctions interfaces,no open text chat.

**Gate:**account/date unique claim;UTC midnight,missed full day→Day1;duplicate achievement/level grant;conversion stale quote;entitlement vs item purchase;payment verified but DB grant fails then recovers once;unknown payment state;duplicate callbacks;refund/reversal idempotency;unauthorized moderation denied/audited. Real-money transactions не запускати. Sandbox results відокремити від mocks.

**Продовжити:**011. Exact sale prices та policy spent-credit debt не фіналізувати самостійно.

### 011 — Integration, Balance Evidence, Packaging

**Виконай:** інтеграційні scenarios із новим account і тривалим state;crash/race/reconnect replay;economy simulation (starter viability,recruit/heal/BB sinks,faucets);matched full-build premium comparisons за визначеною метрикою;Windows x64 native verification;developer setup/runbook,development build artifact,final milestone report.

**Gate:**повний local loop,regression suite,actual DB tests,deterministic parity,10000 mass simulations,EditMode/PlayMode,Mono/native IL2CPP і16v16 smoke. Якщо core/wire untouched — old golden зберігається. Performance/profile тільки за фактичною потребою; не optimize через припущення.

Premium ceiling не доводиться одиничним win-rate: predeclare metric,matched tiers/rosters/HP,seeds/scenarios,full gear+MK3+BB stack. Якщо evidence перевищує constraint — налаштувати prototype config у дозволених межах і повторити affected checks; не приховувати результат або підняти ceiling.

**Завершення pass:** deliver verified local build/source/docs/commits та список конкретних production gates. Не переходити автоматично до012 live release/final art.

## 10. Architecture discipline

- Pure battle assembly лишається Unity/DB/network-independent; host/application роблять orchestration.
- Domain policy має бути testable без Steam/session/scene. Wall clock injected тільки в state lifecycle, не combat simulation.
- Shared contracts сумісні з Unity Standard2.1; server-specific .NET APIs лишаються backend.
- Versions для rules,balance,catalog,snapshots,requests/serialization. Archive повні match inputs.
- Backend validation не замінюється client validation; UI validation лише feedback.
- Не тримати DB transaction відкритою через невизначений external Steam call. Durable states/reconciliation/outbox за потреби.
- Stable ownership/uniqueness/nonnegative constraints; common service для wallet operations, не scattered writes.
- Не додавати generic framework/plugin architecture «на майбутнє» замість vertical slice.
- Production config не допускає development auth,seed cheats,grant commands або client result trust.

## 11. Cross-system verification matrix

| Risk | Required scenario |
|---|---|
| Double spend | Два simultaneous commands на останні soft/BB; один commit, другий conflict |
| Duplicate settlement | Response загублено після commit;retry повертає result без повторної нагороди |
| Stale state | Heal/equip/train під час pending offense;визначений порядок без HP overwrite |
| Snapshot race | New snapshot після acceptance;старий match replay незмінний |
| Exposure race | Два starts за slot4;TTL і late worker;committed impacts ніколи5 |
| Friend farming | Win1/2/3/4,loss/draw/mode/reconnect між ними;exhaustion не reset |
| Revenge duplication | Same origin/start retry;parallel attempts;draw/expiry boundary;single success |
| Time manipulation | Client timezone/backward clock не впливає;UTC claim і recovery checked |
| Grant recovery | Platform success→process crash→reconciliation→один grant |
| DB reliability | Fresh migrations,restart,rollback,concurrency на actual PostgreSQL |
| Runtime parity | Same golden у .NET,Editor,PlayMode,Mono,native IL2CPP |
| Scale | All roster sizes/domain,16v16 presentation/native smoke |
| Premium balance | Full build stack,predeclared metric,multiple scenarios,seeds |

Не писати tests, які лише повторюють реалізацію і не перевіряють invariant. Для docs-only зміни достатньо structural/diff checks; expensive full suite повторюється при змінах/нових ризиках, а не після кожної коми.

## 12. Verified commands і нові gates

З repository root, після читання scripts:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage MonoRun
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPBuild
powershell -NoProfile -ExecutionPolicy Bypass -File tools/verify-unity.ps1 -Stage IL2CPPRun
```

Underlying standalone checks:

```powershell
dotnet build AirsoftClubGame.sln -c Release
dotnet run --project tests/Airsoft.Battle.Tests -c Release --no-build
dotnet run --project tests/Airsoft.Battle.Tests -c Release --no-build -- --simulate 10000
dotnet format whitespace AirsoftClubGame.sln --verify-no-changes --no-restore
git -c core.whitespace=blank-at-eol,blank-at-eof,space-before-tab,cr-at-eol diff --check
```

Existing tests — custom console runner, **не `dotnet test` discovery**. Нові backend test projects можуть використовувати інший відповідний runner; задокументувати exact command і exit criteria. Не подавати неіснуючі scripts як готові.

`tools/verify-aot.ps1` може перевіряти conversion, але не замінює native compile/link/run. Full event-log guard може споживати значну пам'ять; summary-only harness оптимізація допустима лише з profiling і без зміни authoritative result.

## 13. Git discipline і документація

1. Green Gate перед першим edit; known dirty docs зберегти, unknown changes не чіпати.
2. Працювати на власній development branch від verified baseline/docs descendant.
3. Перед commit inspect diff/status, scope і whitespace. Stage explicit files; не blanket-add secrets/generated binaries.
4. Тематичні commits на meaningful boundaries:003 economy,004 club lifecycle,005 backend тощо. Не треба один гігантський commit на все.
5. Commit code разом із потрібними tests/config/migrations/updated docs. Не комітити failing implementation як completed milestone.
6. Не amend чужі commits, не rebase/reset/force-push без відповідного дозволу. Local commits дозволені; remote publish/merge не передбачений автоматично.
7. Version `.meta`, package manifests/locks,migrations; ignore Library/Temp/Artifacts/build output/credentials.
8. Після commit записати SHA і status. Uncommitted user files перелічити чесно; не заявляти clean, якщо вони є.

Для кожного milestone вести `implementation/IMPLEMENTATION_00N.md` та `implementation/VERIFICATION_00N.md` або рівнозначний однозначний report. Decision log і config hypotheses підтримувати актуальними. Master змінювати для verified status/grounded details, не для самовільного перезатвердження product rules.

Report має відрізняти **implemented / tested locally / verified against live sandbox / not run / blocked**. Збережені у002 test counts не копіювати як нове виконання.

## 14. Continuation без дрібних зупинок

Після green milestone: короткий report → thematic commit → наступний milestone. Не закінчувати завдання фразою «можу зробити наступне», якщо воно вже входить у scope.

Проміжні оновлення пояснюють досягнуту поведінку, важливу знахідку/ризик і наступну перевірку; не перераховують кожен файл. На довгій роботі повідомляти користувачу прогрес регулярно. Не використовувати busy polling або довгі мовчазні waits.

Перед context handoff залишити checkpoint:

```text
Current root/branch/HEAD:
Active milestone and completed gates:
Known uncommitted changes:
Master version and grounded decisions:
Last exact test results:
External/product blockers:
Next concrete action:
Do not redo:
```

Після resume прочитати checkpoint і поточний Git state, продовжити незавершене. Не відновлювати старі approvals/OPEN по пам'яті, не запускати001 заново. User steering уточнює поточний scope; пряме «stop» зупиняє.

## 15. Готовий prompt для продовження перерваного pass

> Продовжуй активований Airsoft_Club_Game development pass за Agent Implementation Pack v1. Прочитай останній checkpoint, Master Spec v1, Git diff і verification reports. Збережи відомі незавершені зміни. Перевір зв'язок поточного стану з baseline d553cd3d9fee998a9764be04b63f8881658d299f; не reset-ити до нього. Заверши поточний milestone, виправ regressions і переходь до наступних дозволених milestones без дрібних уточнень. Live payments/publishing/final art лишаються за межами дозволу; Project Airsoft не змінювати.

## 16. Готовий prompt для фінального verification pass

> Перевір фактичний Airsoft_Club_Game implementation за Master Spec v1 та invariants I01–I24. Пройди локальний onboarding→recruit/gear/train/heal/BB→opponent→battle→settlement→restart loop; перевір concurrency/idempotency,UTC daily,friend/exposure/shield/Revenge boundaries,actual PostgreSQL migrations,Windows native IL2CPP parity і premium-stack evidence. Виправ знайдені defects у дозволеному scope, повтори affected tests. Не regenerate golden для приховування regression, не називай mocks live integration. Підготуй development build/runbook і точний звіт із SHA,commands/counts та незакритими production gates. Не запускай production release.

## 17. Final delivery contract

Повернути стисло, з посиланнями на фактичні artifacts:

- Що вже працює end-to-end; які003–011 milestones завершено.
- Як запустити development build/backend/DB, де runbook.
- Test/build/native results із фактичними counts і commands у report.
- Grounded decisions та balance hypotheses, що впливають на гру.
- Що mocked,що sandbox verified,що blocked і який доступ/рішення потрібні.
- Branch,commit SHA(s),working tree state,scope змін.
- Підтвердження **Project Airsoft untouched** та відсутності unauthorized live payments/deployment.

Не завершувати формальним «усе готово», якщо залишилися regression або неперевірений критичний шлях. Водночас не залишати дозволену незалежну роботу недоробленою через final-art/live-Steam blocker.

## 18. Узгодженість із поточним documentation-only завданням

Pack підготовлений на підставі Gate v3,Implementation001–002 і закритихQ01–Q15. Master містить exact approved rules,actual formulas,source register,remaining implementation details і superseded map. Цей файл дає операційний спосіб їх виконати після явного старту.

**Підготовка двох master-файлів не активує наведені prompts. У поточному завданні game implementation не починається.**
