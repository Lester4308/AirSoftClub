# Airsoft game design spec v1

Стан: NEW AIRSOFT DESIGN baseline. Основний документ: [Decision Pack](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v1.md); рішення PD-01–24 мають пріоритет над деталями, якщо виявлена суперечність.

## Promise і межі

«Моя трійка перемогла завдяки моїй підготовці». Гравець управляє клубом, людьми, спорядженням і суперництвом. Combat — коротка автоматична демонстрація результату. Аудиторія: гравці PC management/auto-battler, яким цікаві builds та знайомі суперники; це продуктова гіпотеза, не market research result.

Club — персональна команда одного player, не guild з кількома власниками. Internal UUID прив'язує Steam identity; Club name окреме від Steam nickname. Club profile містить level, rating, separate ranked/casual records, lineup, історію й achievements. Logo v1 — конструктор із дозволених емблем, без довільних uploads.

## Функціональна поверхня v1

| System | Baseline | Деталі |
|---|---|---|
| Fighters | три stats, bounded progression, rename | [Fighter system](AIRSOFT_FIGHTER_SYSTEM.md) |
| Team |3 deployed, max6 roster; MVP4 | активний і defense presets |
| Equipment | Primary/Protection/Kit | [Equipment](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| Economy | Credits + XP; automatic supply cost | [Economy](AIRSOFT_ECONOMY.md) |
| Combat |3v3, до 3 rounds, one-hit OUT | [Combat](AIRSOFT_COMBAT_SYSTEM.md) |
| Opponents | Friends/Rating/Revenge + PvE fallback | [Social architecture](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| Persistence | authoritative backend, isolated offline practice | [Backend](BACKEND_ARCHITECTURE.md) |

Основні екрани: Club, Team, Fighter Detail, Recruit, Training Center, Shop, Opponents, Club Profile, Battle, Result, History, Leaderboards, Settings. Ready Room — частина Club/Result, не окрема господарська система. [UI flow](AIRSOFT_UI_FLOW.md).

## Progression structure

Перша сесія: trio → вступний матч → вибір reserve fighter → перше покращення → наступний opponent. Пізніше — training distribution, alternate gear, defense composition та rating. Club levels відкривають різноманіття; fighter upgrades bounded. Кількість друзів, покупки за реальні гроші та offline очікування не дають combat power.

Victory/defeat не знищують gear і не травмують бійців. Losing player бачить конкретну причину і може відразу повторити цикл. Тривалий прогрес не перетворюється на обов'язок регулярно забирати timers.

## UX, art, audio

Нова visual identity: клубний спорт, номерні нашивки, захисне спорядження, контрастні командні accent colors. Не копіювати композиції старих меню/графіку. В оглядовому 3D кадрі мають читатися всі 6 учасників. Стани ACTIVE/OUT відрізняються текстом/іконкою, не тільки кольором. Налаштування масштабу тексту, reduced motion, окремих audio sliders. Tooltips дублюються focus/click panels.

Audio — короткі нові сигнали shot/reload/hit/end, без overlaid музики дослідницьких відео. Combat replay має пояснювати механіку навіть з muted audio. Controller-friendly focus model передбачити, Steam Deck verification відкладена.

## Success gates

M1 демонструє purchase/equip/train → змінений battle behavior → reward → save/reload. M2 витримує retry/crash/tampered-state. M3 працює з offline defender і нульовим friend list. Public release потребує moderation/report/block, privacy states, підтримки/відновлення DB і перевірки balance. Календарний план без даних про команду не встановлюється.
