# Product decisions — v1

2026-09-05. Канон: [Decision Pack](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v1.md). Кожен запис має Question, Original behavior, Options, Recommendation, Decision, Reason, Impact, Status. **BASELINE** означає запропоноване рішення для специфікації, не погодження власника і не перевірений баланс. **TUNE** — baseline з обов'язковою перевіркою прототипом. Усі числові параметри нові.

| ID / Question | Original behavior / Evidence | Options | Recommendation | Decision | Reason | Impact | Status |
|---|---|---|---|---|---|---|---|
| PD-01 Product DNA? | Hire/equip/watch/reward; E007–E020 CONFIRMED | literal remake; management successor; tactical manager | management successor | NEW AIRSOFT DESIGN, окремий продукт | Зберегти підготовку і payoff | Без Project Airsoft merge/FPS | BASELINE |
| PD-02 Навігація? | City hub; E006/E022 CONFIRMED | mandatory city; direct tabs; open clubhouse | direct tabs | NEW AIRSOFT DESIGN, Club dashboard | Менше переходів | City art не залежність MVP | BASELINE |
| PD-03 Fighter stats? | Три trainable; E002/E010/E018 CONFIRMED | три; шість; class-only | три | NEW AIRSOFT DESIGN: Accuracy/Mobility/Endurance | Зрозумілі builds | Derived stats тільки у gear/details | TUNE |
| PD-04 Hit model? | HP/damage/armor; E020 CONFIRMED, formula UNKNOWN | HP; one-hit rounds; suppression+wounds | one-hit rounds | NEW AIRSOFT DESIGN | Airsoft контекст | Найбільший відхід від старого бою | TUNE |
| PD-05 Roster? | 16 cap E002/E014; active split UNKNOWN | 4;6;16 | v1 max6 | NEW AIRSOFT DESIGN; MVP4, deploy3 | Резерв без overbuild | Тренована трійка важливіша за кількість | TUNE |
| PD-06 Formats? | Точна система UNKNOWN | 2v2;3v3;4v4 | 3v3 | NEW AIRSOFT DESIGN; tutorial1v1 без ladder | Командність і один пул | Не розпорошує population | BASELINE |
| PD-07 Slots? | Три + mask requirement E010/E021 CONFIRMED | 3;6;9 | 3 | NEW AIRSOFT DESIGN: Primary/Protection/Kit | Мало важливих рішень | Немає secondary/accessories v1 | BASELINE |
| PD-08 Weapons? | Різні stats E008/E019; balance superiority UNKNOWN | AR only; AR+SMG→DMR; усі сім'ї | AR+SMG→DMR | NEW AIRSOFT DESIGN | Відкривати tradeoffs поступово | DMR після core stability | TUNE |
| PD-09 BB economy? | Persistent ammo E013/E015 CONFIRMED | manual stock; auto cost; no cost | auto cost | NEW AIRSOFT DESIGN,15 за full eligible match | Operational sink без chores | Немає persistent BB inventory | TUNE |
| PD-10 Currencies? | Coins/karma E006/E008/E018 CONFIRMED | дві spendable; одна+XP; pure XP | Credits+XP gates | NEW AIRSOFT DESIGN | Прозора ціна рішень | Один wallet ledger | TUNE |
| PD-11 Monetization? | Real-money purchase UNKNOWN | premium; cosmetics F2P; paid power | premium candidate | NEW AIRSOFT DESIGN, без paid power | Сумісність з management ownership | Ціна/комерційна перевірка відкриті | PROPOSED |
| PD-12 Training? | Karma,1/2 приклади E010/E018; increment UNKNOWN | timers; instant XP-gated; auto | instant XP-gated | NEW AIRSOFT DESIGN,+1 stat, cap12 upgrades | Вибір без очікування | Без skill tree MVP | TUNE |
| PD-13 Recovery? | Resp purpose strongly supported; energy contradictory E024–E026 | fatigue; injury; round reset | reset | NEW AIRSOFT DESIGN, no persistent energy | Швидкий repeat loop | Ready Room = readiness/results | BASELINE |
| PD-14 Presentation? | Automatic fixed anchors E012/E020 CONFIRMED | 2D;2.5D;low-poly3D | low-poly3D target | NEW AIRSOFT DESIGN, primitives спочатку | Читабельність та нова identity | Арт-бюджет ще невідомий | PROPOSED |
| PD-15 Authority? | Server routes E026 CONFIRMED, formula UNKNOWN | client; server non-deterministic; server deterministic | server deterministic | RECONSTRUCTION DECISION | Audit/replay/anti-cheat | Версійні inputs та builds | BASELINE |
| PD-16 Friends? | Attack/Profile E009 CONFIRMED | friends-only; all-ranked; unranked rivalry | unranked + separate ranked | NEW AIRSOFT DESIGN | Соціальність без direct rating trade | Rewards limited за pair policy | BASELINE |
| PD-17 Rating? | Lists E014; rating UNKNOWN | all challenges; attacker-only; paired ranked | paired server-issued ranked | NEW AIRSOFT DESIGN,K24, pair24h, incoming5/24h | Знизити cherry-pick та offline exposure | Pool size треба виміряти | TUNE |
| PD-18 Revenge? | UNKNOWN | old snapshot; current unranked; ranked revenge | current unranked | NEW AIRSOFT DESIGN,72h, once, no chain | Зрозумілий соціальний comeback | Zero rating; shared reward counters | TUNE |
| PD-19 Backend shape? | Відомі routes, stack/schema UNKNOWN | local sync; monolith; microservices | modular monolith | RECONSTRUCTION DECISION | Мінімальний operational scope | API+DB+worker, не MMO | BASELINE |
| PD-20 Identity? | VK social context E009; Steam новий | nickname; claimed ID; verified ticket | verified ticket | NEW AIRSOFT DESIGN,UUID+SteamID64 | Змінні імена не account key | SDK credentials потрібні для M2 | BASELINE |
| PD-21 Offline? | Немає доказів offline | online-only; cache/practice; sync economy | cache/practice | NEW AIRSOFT DESIGN, isolated state | Гра запускається без довіри до local wallet | Немає earned offline progression import | BASELINE |
| PD-22 Snapshot? | Async STRONGLY SUPPORTED; schema UNKNOWN | live read; mutable snapshot; immutable | immutable per battle | RECONSTRUCTION DECISION | Зміни підготовки не змінюють минуле | Архів версій, atomic capture | BASELINE |
| PD-23 Leaderboards/notifications? | UNKNOWN | Steam-only; backend+mirror; custom push | backend first | NEW AIRSOFT DESIGN; in-game inbox, optional trusted mirror | Контроль authority та privacy | Steam push не v1 dependency | BASELINE |
| PD-24 Delivery? | Historical loop відомий, implementation нова | full launch; staged loop; UI-first | staged loop | NEW AIRSOFT DESIGN,M1→M2→M3→M4 | Рано перевірити preparation payoff | Без production UI до core gate | BASELINE |

## Припущення та review triggers

Мала команда, невідомий бюджет, PC-first input, відсутність existing engine choice. Вимоги Steam credentials, paid ownership policy, контентна локалізація й retention потребують конкретизації перед відповідним milestone; вони не блокують сформований design pack.

PD-04 переглядати після one-hit playtest; PD-05 після перевірки цінності reserves; PD-09 після economy usability; PD-17 після population simulation; PD-14 після art slice і profiling. Зміна рішення має оновлювати цей реєстр, головний pack і залежні specs одночасно. Стару версію balance не перезаписувати.
