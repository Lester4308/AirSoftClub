# Original → Airsoft mapping

Історичні назви нижче використовуються виключно для traceability. E### розшифровані у [RESEARCH_BASIS.md](RESEARCH_BASIS.md). Keep/Modify/Remove — рішення нової гри, а не правова оцінка. Нові механіки не називаються підтвердженими механіками оригіналу.

| Original mechanic | Evidence / status | Keep? | Modify? | Remove? | Airsoft replacement | Reason |
|---|---|---|---|---|---|---|
| Hire → equip → auto battle → reward | E007–E020 CONFIRMED | так | терміни/UX | ні | Club preparation loop | Основний емоційний контракт |
| School candidates,8 cards, first free | E007 CONFIRMED | вибір бійця | 3 starters +4th choice,4 cards | literal8 | Recruit у Team | Швидкий перший 3v3 |
| Другий 100/третій 300 | E025 STRONGLY SUPPORTED | rising opportunity cost | нова price table | literal prices | slots5/6 за Credits | Старі числа не баланс нової гри |
| Roster16 | E002/E014 CONFIRMED; deploy UNKNOWN | постійний roster | max6/deploy3 |16 v1 | active trio + reserves | Читабельність |
| Name/rename/dismiss/portrait | E002, Fighter/Team docs CONFIRMED | так | нові pools, min3 rule | ні | персональні club members | Прив'язаність до команди |
| Endurance/Agility/Accuracy | E010/E018 CONFIRMED | три параметри | ефекти | буквальні формули невідомі | Endurance/Mobility/Accuracy | Build clarity |
| Fighter HP | E010/E020 CONFIRMED | зрозуміле вибуття | round status | persistent HP | ACTIVE/OUT; per-round stamina | Один зарахований hit |
| Damage/speed/armor | E008/E019/E020 CONFIRMED | gear tradeoffs | cadence/accuracy/range/weight | damage sponge/armor | weapon and kit properties | Спортивний контекст |
| Karma training | E002/E010/E018 CONFIRMED | цільовий stat upgrade | Credits+XP budget | karma | Training Center | Без premium-like другої валюти |
| Trainer periodic gift | E006 CONFIRMED; timer UNKNOWN | early encouragement | one-time milestone | mandatory collection timer | first-match reward | Не вставляти chores |
| Marker slot | E010 CONFIRMED | слот | airsoft сім'ї/нові names | старі models/names | Primary | Clean-room content |
| Mask mandatory | E021 CONFIRMED | readiness check | eye/face protection | armor effect | Protection, free baseline | Gear completeness без paid safety |
| Camouflage | E011 CONFIRMED | loadout choice | consolidated kit | old art/stat armor | Tactical kit | Три meaningful slots |
| Set relationship | E011 CONFIRMED; bonus UNKNOWN | майбутня можливість | sidegrade synergy | set bonus v1 | explicit individual tradeoffs | Немає прихованого обов'язкового set |
| Shop price/resale/unlock | E008/E023/E030 CONFIRMED | порівняння/cost/unlock | нові tables; resale25% | old prices | Credits shop | Контроль витрат |
| Persistent balls | E013/E015 CONFIRMED | operational expense | fixed match supply cost | manual stock | BB allowance each round | Не зупиняти матчі |
| Ammo capacity growth | Economy doc, cause UNKNOWN | ні | ні | з v1 | fixed per-round loadout | Не винаходити old formula |
| Coins + karma | E006/E008 CONFIRMED | resource choice | Credits+XP | exchange/premium assumptions | one spendable currency | Проста економіка |
| XP/level gates | E016/E017/E029/E030 CONFIRMED | так | own curves/caps | old thresholds | Club/Fighter XP | Розвиток без unlimited power |
| Resp | карта CONFIRMED; heal purpose STRONGLY SUPPORTED | ready feedback | instant reset | paid/timed recovery | Ready Room | Більше боїв |
| Energy | E024–E026 CONTRADICTORY | ні | ні | не вводити | no persistent energy | Не переносити суперечливе |
| Opponents/Allies lists | E014 CONFIRMED | opponent discovery | tabs Friends/Rating/Revenge | ally count as power | Opponents | Friends count не power |
| VK friend attack/profile | E009 CONFIRMED | rivalry | Steam adapter | VK integration | Steam Friends | First-class платформа |
| Automatic fixed positions | E012/E020 CONFIRMED | autonomous watch | anchors/rounds/exposure | ручне керування не додається | deterministic automatic3v3 | Підготовка визначає дії |
| Static map variants | E020 CONFIRMED; modifiers UNKNOWN | visual variety | authored lane distances | old backgrounds | new low-poly arenas | Sidegrade context |
| MISS/damage/colored HP | E020 CONFIRMED | feedback clarity | Miss/Blocked/Hit/Out/stamina | damage labels/HP | readable event replay | Пояснення airsoft outcomes |
| Skip | E012 CONFIRMED | так | speed/replay | ні | Skip to settled result | Гравець контролює час |
| Rewards44/44,36/32,97/84 | E016/E017 CONFIRMED examples | explicit reward | own eligible tables | literal formula inference | settled reward breakdown | Відділити evidence від tuning |
| Battle history rows | E017 CONFIRMED | так | attacks/defenses/replay | ні | versioned MatchRecord | Audit + rivalry |
| Server fight endpoint | E026 CONFIRMED route; replay weakness STRONGLY SUPPORTED | authoritative domain | transactions/idempotency | historic endpoints | new battle commands | Повтор не дає нагороду |
| Async match model | Social doc STRONGLY SUPPORTED | так | immutable defense | ні | offline defense | Snapshot schema — нове рішення |
| Team photo to VK album | E002/E022 CONFIRMED | pride/team view | local lineup view | automatic social posting | Club lineup; export later | Не залежати від публікацій |
| Kremlin-like city object | E002 object CONFIRMED; function UNKNOWN | ні | ні | з нового дизайну | none | Не вигадувати призначення |
| Music/SFX | Original audio UNKNOWN | feedback role | new sound design | extracted soundtrack | own assets | Відеомузика не game audio |
| Flash/Flex/VK shell | E002 CONFIRMED | domain separation | PC client/backend | obsolete runtime | Steam-ready client | Сучасна підтримка |
| Revenge/rating/leaderboards | UNKNOWN | немає established mechanic | цілком нові | — | PD-17/18/23 | Не видавати нове за реконструкцію |
