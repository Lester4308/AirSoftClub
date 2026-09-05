# MVP implementation plan

Статус: план, implementation не почато. [Decision Pack](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v1.md) сформовано перед будь-яким кодом. Milestones мають gates, а не вигадану дату release.

## M0 — Design baseline (цей пакет)

Результат: evidence mapping,24decisions,game/system/Steam/backend/data specs. Перевірка: усі 7 особливо названих research documents прочитані; невідомі original formulas не видані за факти; нові choices мають options/recommendation/impact. Production art/code не переноситься з архіву.

## M1 — One complete loop

Порядок: foundation → Steam adapter stub → profile → club → fighters → inventory/equipment → shop/economy → opponent profile → immutable team input → deterministic sim → Result → progression → dev save/reload.

Concrete scope:3starters,4thchoice,max4;3v3+1v1drill;Primary/Protection/Kit;AR/SMG,3kit styles;one arena;watch/skip;Credits+training;one NPC opponent set;debug presentation. No social/ranked backend exposed to external players. Dev SQLite/file adapter допустимий лише як disposable non-production persistence, profile namespace dev.

Перед основною реалізацією foundation spike порівнює два engine кандидати за headless simulation reuse, Steam adapter integration, UI iteration і deploy constraints. За замовчуванням дослідити C#-based shared domain library; engine brand/version не зафіксовано без наявних навичок команди й integration evidence. Timebox2 робочі дні команди, не поточна обіцянка виконання. Gate output — короткий ADR з engine/runtime/DB/version choices та перевіреним minimal build.

M1 exit: новий profile → club → купити/equip → battle → reward → upgrade → repeat → restart відновлює state. Same seed/input identical result; skip однаковий; missing protection виправляється безкоштовно. Designer може змінити balance table без rewrite системи. Не переходити до polish, поки гравець не розуміє build effect.

## M2 — Persistence і Steam authority

Порядок: production DB schema/migrations → command validation/ledger → durable sim jobs/settlement → Steam ticket/ownership integration → cache/offline practice → backup/restore.

Auth stub не переноситься у production. Real Steam integration потребує game AppID, publisher credentials і тестових entitlement accounts. User-visible MVP післяM2 дозволяє trustworthy account/profile/equipment/battle/progression/save. Social PvP ще може бути прихований feature flag; NPC snapshots server-owned.

M2 exit: invalid auth rejected; no local save import; simultaneous purchase не робить negative wallet;duplicate submit/worker/settlement не дублює reward;disconnect відновлює той самий match;old balance replay відтворюється;restore drill відновлює canonical data. Exactly-once effect перевіряється fault injection між кожними двома state transitions.

## M3 — Social beta / public v1 candidate

Порядок: async human defense → Steam Friends → rating offers/paired rating → revenge → leaderboards → inbox → privacy/moderation → UI/3D polish. Це відповідає пріоритету стабільного battle core перед social.

Gate для кожної частини: offline B не потрібний для A challenge;friend SDK empty/error не блокує Rating/PvE;pair counts cross-mode;revenge one-use72h/no chain;ranking pool constraints видно;concurrent defense changes не мутують pinned inputs;Global/Friends/Around Me мають consistent rating;unverified social IDs не відкривають private data.

Перед external beta: allowlisted tester population, monitoring/rollback, feedback про battle length/build clarity, report/block/rename tools. Public v1 не називати готовим лише тому, що M1 playable. Steam push, Steam leaderboard mirror, Steam Deck certification не критичний шлях.

## M4 — Content expansion

До 6roster,DMR,до 3arenas,new art/audio,cosmetic options і achievements після balance gates. Не додавати 4v4,secondary,attachment tree чи persistent fatigue в цей milestone без окремого PD update. Умови для Deck/controller — реальний input/text/performance test на пристрої.

## Validation matrix

| Risk | Перевірка | Gate |
|---|---|---|
| Нецікава підготовка | Human playtest після 3matches | Може пояснити рішення та побачений ефект |
| One-hit RNG/домінантний build | Seed sweep + side swaps + win matrix | Review перед freeze balance, без заяви про вже доведений баланс |
| Economy trap | Zero wallet,10loss streak,caps,repeat pairs | Завжди доступний новий матч; ledger consistent |
| Client tampering | Modified stats/price/XP/result | Server rejects або ignores authoritative fields |
| Async race | Equip+accept,2workers,2settlements | Frozen inputs,one reward/paired rating |
| Population fragmentation |100/1k/10k synthetic clubs,quota saturation | No-offer rate виміряний; fallback чесно маркований |
| Privacy failure | Forged friend IDs,blocked target,private profile | Лише public cards; no sensitive data |
| Patch/replay break | Match across balance migration | Old event hash і audit build доступні |

## Команда, витрати, відкриті залежності

Потрібні ролі domain/backend engineer,client/UI engineer,designer/balance,3D/generalist art та QA; одна людина може поєднувати ролі. Без чисельності/досвіду/бюджету оцінка тижнями ненадійна. Найдорожчі невідомі:3Dproduction throughput,Steam integration credentials,retention/storage,online operations і actual battle fun. Після M1 оцінити scope на фактичному throughput, а не підміняти його кількістю документів.
