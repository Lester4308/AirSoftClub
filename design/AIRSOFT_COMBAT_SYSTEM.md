# Airsoft combat system

PD-04/14/15. Automatic watch/skip і fixed anchors — ORIGINAL CONFIRMED E012/E020. Уся математика нижче — **NEW AIRSOFT DESIGN / tuning v0**, deterministic execution — RECONSTRUCTION DECISION.

## Input і результат

Input: MatchConfig(mode,format3v3,arenaId), SnapshotA/B, server-generated seed, simulationBuildHash, rulesetVersion, balanceVersion, mapVersion, PRNGVersion. Hash канонічного input зберігається. Sim не читає wall clock, DB, Steam, account rating або client FPS під час виконання.

Output: round results, match outcome, ordered events, BB usage, summary facts, inputHash/outputHash. Wallet/rating settlement — окремий authoritative domain, не частина shot RNG.

## Raундова структура

До 3 rounds;2 round wins завершують match. Round budget300ticks; усі бойові стани reset між rounds: ACTIVE, full stamina, magazine, reserves. Вибрані fighters/gear/anchors упродовж match незмінні. Round win: лише одна сторона має ACTIVE; simultaneous elimination → draw. Timeout: більше ACTIVE виграє; однакова кількість → draw. Після 3 rounds порівняти wins; рівність → match draw. No respawn всередині round.

Arena MVP має три anchors на бік і симетричну range matrix: matching anchor short, сусідній medium, крайній long. Згодом таблиці геометрії можна замінити authored distance/cover matrices. Starting side seed-selected у round1, alternating у round2/3. Side bias перевіряється окремо. Visual lean не є physics input.

## Candidate deterministic model

Використати integer/fixed-point math; hit probabilities у basis points0..10000. Нормалізація і округлення всюди floor, якщо не вказано інше.

1. Round start: stamina Smax=100+3E+kitBonus. Кожний fighter має phase0..9 із versioned PRNG; global10tick cycle: перші 4ticks його shifted cycle = exposed, решта covered. Візуально це короткий peek; жодної ручної зміни exposure.
2. Початковий ready tick=10+phase. Якщо shooter не exposed, відкласти до наступного власного exposed tick. Alive і ready shooter обирає uniform random серед exposed alive opponents, sorted за snapshot-local slot. Якщо нікого немає, retry через 1tick, RNG/ammo не витрачаються.
3. Під час action S<0.25Smax дає fatiguePenalty500bp і cadence multiplier1.25. MobilityEffective=max(0,M+kitAdjustment−weaponLoad). Chance=clamp(1800+60A+weaponRangeBonus−40×targetMobilityEffective−fatiguePenalty,300,4500).
4. Одне random integer0..9999; нижче chance → HIT, target стає OUT наприкінці цього tick. Інакше MISS. Немає damage roll, armor absorption або hitpoints. Один trial — abstract fire opportunity; не стверджуємо реалістичну ймовірність попадання кожної BB.
5. Action витрачає 1magazine burst, BBs/burst з таблиці,8stamina. Stamina не нижче 0 і сама не відновлюється до наступного round. Наступний ready tick=current+cadence або floor(cadence×1.25) при fatigue. Якщо magazine0, наступний ready tick додатково відсувається на reloadTicks і магазин поповнюється. Дозволено 2 повні запасні magazines; якщо всі 3 витрачені, стан OUT_OF_AMMO до кінця round, але fighter лишається ACTIVE для timeout count.
6. Усі actions одного tick обчислюються від state на початок tick. Sorted actions: side A slot0..2, side B slot0..2. Hits apply разом; два shooters можуть одночасно вибити один одного. RNG call schedule незмінний для однакового input, включно з пропусками.

Цей baseline спеціально малий. Якщо exposure/phase породжує патологічні deadlocks або надмірні draws, змінювати одну versioned table за раз; не додавати runtime random retry. Timeout гарантує завершення навіть без shots.

## Determinism contract

PRNG: конкретний алгоритм і test vectors обираються на M1; до їх фіксації не заявляти bit-identical implementation. Seed256bit генерує сервер, клієнт не пропонує seed. Stable serialization: versioned field order, UTF-8, integers, sorted arrays, no platform-dependent hash. Tie and RNG ordering як вище. Batch resolution не використовує floating physics/navigation/animations.

Combat logs: tick,round,eventIndex,actorSlot,targetSlot,eventType,hitChanceBp,roll,staminaAfter,magazineAfter,statusAfter. Full audit log server-only до settlement; replay payload після settlement може опускати rolls. Client summary показує тільки факти: «2 вибуття на далекій лінії», «темп знизився через stamina», а не недоведене причинне «саме це коштувало перемоги».

## Replay і версії

Сервер resolve передає event timeline і result. Pause/speed/skip змінюють playback, не sim state. Replay зі старим ruleset не оновлює equipment definitions із live catalog. Зберігати старий sim build, таблиці та input для audit; event log окремо для playback. Відсутній старий renderer → текстовий timeline, а не нова simulation з іншими правилами.

## Acceptance

Однаковий input дає тотожний output hash на supported server builds. Для seed suite немає invalid actors, negative ammo, shots після OUT із попереднього tick, infinite rounds. Simultaneous hits/draw/ammo exhaustion/timeout мають golden cases. Watch/skip/disconnect settlement однаковий. Seed sweeps із side swap порівнюють composition/range/training; тільки після цього висновки про balance.
