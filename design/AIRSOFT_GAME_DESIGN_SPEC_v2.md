# Airsoft game design spec — v2

Канон: [Product Decision Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). USER APPROVED modernized successor; цей документ узгоджує systems, не додає implementation дозвіл.

## Product promise

«Я найняв цих бійців, розвинув їх, підібрав спорядження й боєприпаси, обрав суперника і дивлюся, чи спрацювала підготовка». Management-first, автоматичні бої, соціальний rivalry. Нова окрема гра Airsoft_Club_Game, не Project Airsoft.

Core DNA:малий старт, поступовий найм, різна якість recruits, HP/ammo operational costs, вибір ризику й короткий battle→reward→upgrade loop. Весь production content новий; історичні names/art не каталог нової гри.

## Затверджений system scope

| System | Правило | Document |
|---|---|---|
| Roster | Max16; перший FREE, потім paid | [Fighters](AIRSOFT_FIGHTER_SYSTEM.md) |
| Market | Approx6–7 varied candidates; level improves pool | [Fighters](AIRSOFT_FIGHTER_SYSTEM.md) |
| Deployment | Будь-які 1–16 vs1–16 available; asymmetric legal | [Combat](AIRSOFT_COMBAT_SYSTEM.md) |
| Combat | One battle/round, result; HP/Damage/Armor | [Combat](AIRSOFT_COMBAT_SYSTEM.md) |
| Gear | Weapon/armor; MK visual+small stats; early unlock | [Equipment](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| BBs | Persistent3–5 class direction; premium top possible | [Equipment](AIRSOFT_EQUIPMENT_SYSTEM.md) |
| Recovery | HP persists; Money heal/time free; no Energy | [Economy](AIRSOFT_ECONOMY.md) |
| Economy | Money+premium Credits; level/power rewards, operational net | [Economy](AIRSOFT_ECONOMY.md) |
| Monetization | Integral; faster progression, collection, limited advantage | [Economy](AIRSOFT_ECONOMY.md) |
| Social | Steam Friends/Revenge non-ranked; separate Ranked pool | [Steam](STEAM_SOCIAL_PVP_ARCHITECTURE.md) |
| Navigation | Hub + Shop/Training/Recovery/Club destinations | [UI flow](AIRSOFT_UI_FLOW.md) |

## Прогресія і business

Recruitment ширина та якість конкурують за Money із gear/training/ammo/healing. Credits можуть прискорювати через exchange, early access і MK/premium routes. «Pay не гарантує win» — design constraint, не доказ відсутності paid edge. Compound advantage та sustain costs мають бути перевірені до commercial activation.

Club Level — база цінності opponent і якості market, а Team Power описує реальну deployed strength. Quantity alone не reward formula. Exact XP, prices, normal unlocks, training stats/caps, combined advantage ceiling OPEN.

## UX / presentation

Original UI structure first, нове візуальне виконання. Команда і fighter profile у центрі; Money/Credits/BB/HP добре видимі. Watch/speed/skip можливі як presentation controls, без direct combat input. Формат 2D/2.5D/3D не затверджено цим correction pass; не примушувати low-poly3D як успадкований default.

Modern PC input/focus/text readability, clear currency labels, purchase previews і gross-vs-net breakdown. Responsive behavior та layout для 16actors/market cards — окремий UI analysis/redesign етап. Немає production mockups/code зараз.

## Authority і release boundary

Steam verified profile, server-authoritative wallets/HP/BB/purchases/rewards/rating, immutable battle inputs/replay. Defense live resource impact Q-06; commerce reversals Q-11; engine/save/offline specifics Q-12. Дані цих питань не брати з іншого проєкту.

[Roadmap](../ROADMAP.md) зупиняється на product/design gate до окремого дозволу. Public gameplay/monetization не називаються готовими через наявність документів.
