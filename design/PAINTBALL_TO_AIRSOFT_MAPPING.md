# Original → Airsoft mapping — v2

Канон: [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). E### у [Research basis](RESEARCH_BASIS.md). Original terminology нижче лише для доказового порівняння. USER APPROVED не означає ORIGINAL CONFIRMED.

| Original mechanic | Evidence / status | Keep? | Modify? | Remove? | Airsoft replacement | Reason |
|---|---|---|---|---|---|---|
| Hire/equip/train/battle/reward | E007–E020 CONFIRMED | так | терміни та UX | ні | Management-first loop | Original-game-first |
| City hub / великі locations | E006/E022 CONFIRMED | структура | modern visual execution | ні | Hub, Shop, Training, Recovery, Club | Вподобаний користувачем flow |
| Перший recruit безкоштовний | E007 CONFIRMED | так | starter selection Q-02 | ні | Один free fighter | Малий старт |
| Наступні recruits дорожчають | E024/E025 STRONGLY SUPPORTED | принцип | якість + hire progression | old exact prices | Paid recruits після першого | Saving tradeoff |
| Вісім candidate cards | E007 CONFIRMED | market choice | приблизно 6–7 | equal-budget v1 | Різні stats, сила, ціна | Новий глибший recruitment |
| Club Level → recruit quality | Exact rule UNKNOWN | — | NEW AIRSOFT DESIGN | — | Better distribution with overlap | Довгий progression |
| Development potential | UNKNOWN | — | лише після approval | inactive зараз | Q-02 | Не додавати прихований параметр |
| Roster max16 | E002/E014 CONFIRMED |16 | deployment policy clarified | max6 v1 | Max16 owned | Повернення до reference scale |
| Asymmetric deployment1–16 | Повний range UNKNOWN | design spirit | USER APPROVED exact range | forced equality | Будь-які 1–16 на сторону | Самостійний risk choice |
| Три trainable stats | E002/E010/E018 CONFIRMED | рекомендовано | exact effects Q-01 | v1 fixed budgets | Accuracy/Agility/Endurance proposal | Мінімальна модернізація |
| Name/appearance/rename/dismiss | E002/E022 CONFIRMED | так за scope | нові assets/names, anti-reset policy | старий content | Personal fighters | Прив'язаність до команди |
| Training за karma | E010/E018 CONFIRMED; increment UNKNOWN | training role | currency/curve Q-01 | literal cost table | Training destination | Не домислювати payment route |
| HP/MaxHP | E010/E020 CONFIRMED | так | нові balance curves | one-hit v1 | Persistent CurrentHP/MaxHP | Можна пережити кілька hits |
| Armor/damage/speed | E008/E011/E019 CONFIRMED; formula UNKNOWN | combat role | own mitigation/hit model | old coefficients | Weapon damage, armor, handling | Ближчий combat payoff |
| Resp/recovery | Location CONFIRMED, exact behavior UNKNOWN | recovery role | USER APPROVED paid immediate/free time | full reset v1 | Money heal / timed recovery | Operational decision |
| Energy | E024–E026 CONTRADICTORY | ні | не копіювати | persistent energy | None | Без artificial refill |
| Marker/camouflage/mask slots | E010/E011/E021 CONFIRMED | compact principle | Weapon/Clothing-Armor/Protection proposal | old names/art | Q-08 slot design | Airsoft conversion |
| Missing-mask validation | E021 CONFIRMED | так | eye/face protection | ні | Readiness rule | Historical prerequisite |
| Set bonus relation | E011 reference CONFIRMED; amount UNKNOWN | не active requirement | future review | invented bonus | None until decision | Не додавати недоведений effect |
| Weapon level gates | E008/E030 CONFIRMED | normal gates | new catalog | old levels/prices | Club Level gates | Progression |
| Premium early level bypass | UNKNOWN у recovered sources | — | USER APPROVED | level-only v1 | Credits early access | Commercial acceleration |
| MK skins/upgrades | UNKNOWN | — | USER APPROVED | cosmetic-only interpretation | Base→MK1→MK2→MK3 visual+stats | Visible power progression |
| Coins/karma | E006/E008 CONFIRMED | two resource structure | Money/Credits roles | single soft Credits v1 | Soft + premium | Business+progression |
| Real-money currency | Original purchase screen UNKNOWN | — | USER APPROVED | no-paid-advantage v1 | Steam Credits purchase | Комерційна мета |
| Exchange | E026 request traces; rate UNKNOWN | conversion concept | Credits→Money | presumed old rate | Versioned exchange | No direct Credits heal |
| Persistent ammunition | E013/E015 CONFIRMED | так | BB classes/stock/loadout | fixed fee v1 | Buy/use/replenish BB | Operational economy |
| Ammo capacity growth | Observed values; cause UNKNOWN | capacity need | Q-04 | invented growth curve | Configurable later | Не переносити causality |
| BB quality classes/premium | UNKNOWN | — | USER APPROVED direction | single-class assumption |3–5 classes, capped hypothesis | Modest advantage/cost |
| Auto combat/fixed positions/Skip | E012/E020 CONFIRMED | automatic, Skip | own presentation/positions | manual FPS не додається | One automatic battle | Підготовка визначає payoff |
| HP zero / result | E016/E020 CONFIRMED | так | edge outcomes Q-05 | round series v1 | One battle/result | Швидкий social loop |
| Damage/MISS/HP feedback | E020 CONFIRMED | так | modern readable UI | one-hit-only UI | Armor/damage/HP/BB feedback | Пояснення результату |
| Maps | E020 visuals CONFIRMED; mechanics UNKNOWN | variety | new independent art | old backgrounds | Arena proposal | Не переносити hidden modifiers |
| XP/Money rewards | E016/E017 CONFIRMED examples; formula UNKNOWN | progression role | approved level+power basis | flat v1 payout | Gross reward + operational net | Anti-farm economy |
| Opponent/friend cards | E009/E014 CONFIRMED | rivalry | Steam metadata/level/count/power | VK dependency | Steam Friends | Offline opponent allowed |
| Asynchronous model | Social note STRONGLY SUPPORTED | так | authoritative snapshots | — | Modern async PvP | Internals нові |
| Ranked/Revenge/Leaderboards | UNKNOWN | — | USER APPROVED/new design | direct friend rating | Separate Ranked, non-ranked social | Не приписувати старій грі |
| Server fight API | E026 route CONFIRMED; replay weakness strongly supported | authority role | transactions/idempotency | historical code/routes | New independent backend | One resource/reward effect |
| Photo/social posting | E002/E022 CONFIRMED | team pride | optional later | mandatory posting | Club team view | Не блокувати loop соцдією |
| Central unknown building | E002 object; function UNKNOWN | лише hub organization | не вигадувати function | unsupported feature | No active purpose | Evidence discipline |
| Old art/audio/branding/runtime | Asset/source notes | reference only | new production | copied assets/code | New airsoft identity + Steam client | Clean-room boundary |
