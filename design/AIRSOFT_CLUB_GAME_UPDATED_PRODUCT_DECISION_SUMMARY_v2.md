# AIRSOFT CLUB GAME — UPDATED PRODUCT DECISION SUMMARY v2

2026-09-05 · Correction pass завершено на рівні документації · **PRODUCT / DESIGN GATE**.

Канон: [Product Decision Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Працюємо лише в незалежному Airsoft_Club_Game. Project Airsoft не використовується як foundation і не змінюється.

## CONFIRMED

Тут CONFIRMED = затверджено користувачем для нової гри; історичні докази окремо у [Research basis](RESEARCH_BASIS.md).

- Original-game-first: близькі management loop, progression, operational economy та hub flow, нові airsoft assets/code, Steam замість VK.
- Roster максимум 16; кожна сторона обирає 1–16 доступних бійців; асиметричні бої дозволені.
- Один challenge = один battle / один раунд = один результат.
- HP + weapon damage + BB modifier + armor mitigation + hit calculation; elimination HP <= 0, декілька влучань допустимі.
- Перший fighter FREE, решта платні. Market приблизно 6–7 різних за stats/силою/ціною recruits; Club Level покращує пул із variance.
- Damage/HP зберігаються після бою. Immediate healing за Money або безкоштовне відновлення з часом. Persistent Energy немає.
- Soft currency (назва відкрита) + premium **Credits**. Credits купуються за реальні гроші та обмінюються на Money. Прямого heal-for-Credits немає.
- Монетизація обов'язкова: швидша progression, collection, limited power advantage. Early level unlock за Credits та premium-only items допустимі напрями; оплата не гарантує win.
- Weapon Base → MK1 → MK2 → MK3: visual skin + невелике gameplay покращення; Credits можуть відкривати MK.
- BBs — persistent consumables; напрям 3–5 classes, можливий Credits-only top class.
- Base reward насамперед від Opponent Club Level; actual Team Power коригує underdog/overpower value. Unequal sizes не забороняються; weak-target farming економічно знецінюється.
- Friends та default Revenge — non-ranked; Ranked — окремий backend opponent pool.
- Hub / Shop / Training / Recovery / Club зберігаються як структура; modern UI redesign — окремий майбутній етап.
- Оновлено залежні design/architecture/data/roadmap документи. Implementation потребує окремого рішення.

## BALANCE HYPOTHESES

- BB illustration: +0%, близько+3%, +5%, +10%, максимум близько +15% damage або еквівалент. Це не фінальна таблиця товарів чи доказ балансу.
- MK step sizes, premium-item advantage та сумарний stack ceiling ще без затверджених чисел. BB ceiling не є ceiling усього loadout.
- HP/stat curves, damage/armor/hit/penetration, target cadence, recruitment prices/quality distributions, XP/training, rewards/power scaling, recovery time/cost — визначатимуться перевірками.
- Money/Credits exchange rate, real-money packs, BB/weapon/armor/MK/early-unlock prices, recurring costs і ROI не фіналізовано.
- Старі v1 таблиці шансів, нагород і цін не є стартовим балансом v2. Не гарантується positive net після operational costs.

## OPEN QUESTIONS

| ID | Потрібне окреме рішення |
|---|---|
| Q-01 | Три stats: точні назви/ефекти; training currency, XP curves, caps, respec |
| Q-02 | Recruitment refresh; free-first candidate selection; replacement cost; development potential і роль ветеранів |
| Q-03 | Free recovery rate/start/cap; heal pricing; як Max HP upgrade впливає на Current HP |
| Q-04 | BB allocation/mixing/capacity; starter supplies; zero-money/zero-BB fallback без paid-only тупика |
| Q-05 | Timeout, simultaneous elimination, no-ammo/stalemate outcome |
| Q-06 | Offline defense: live persistent HP/ammo drain чи ізольований snapshot; concurrent fights і heal/equip conflicts |
| Q-07 | Early unlock purchase vs entitlement; bypass depth; MK soft route; конкретні premium-only items/BB |
| Q-08 | Final equipment slots, armor tradeoffs/penetration; weapon families й art direction |
| Q-09 | Team Power/reference power, reward coefficients, XP scaling, repeat/collusion policy |
| Q-10 | Ranked algorithm/limits/exposure; Revenge expiry/consumption/snapshot policy |
| Q-11 | Credits special rewards, paid-vs-F2P entry, prices/refunds policy та cumulative monetization advantage ceiling |
| Q-12 | Engine/DB, online/offline behavior details, retention/moderation та scope наступного implementation дозволу |

Запропоновані варіанти Q-01–Q-12 не є прихованими затвердженнями. Особливо Q-06: не записано автоматичного «безкоштовного defense», що суперечило б задуму persistent costs.

## SUPERSEDED DECISIONS

| Скасовано | Заміна |
|---|---|
| Mandatory3v3, fixed symmetric sizes | Будь-які 1–16 проти 1–16 |
| Max roster6 / MVP max4 | Max16; onboarding починається з 1 |
| Три безкоштовні starters і безкоштовний четвертий | Тільки перший FREE; подальші recruits купуються |
| One-hit elimination | HP/Damage/Armor, HP <= 0 |
| Best-of-three / round wins | Один battle/раунд/result |
| Instant full health reset / no paid recovery | Persistent HP, Money heal або timed free recovery |
| Fixed supply fee, no persistent BB stock | Persistent BB inventory та actual expenditure |
| Credits як єдина soft currency | Money + premium Credits |
| Відсутність запланованої комерційної монетизації / paid-power prohibition | Обов'язкова monetization з limited advantage |
| Cosmetic-only weapon skin interpretation | MK = visual + невеликий stat upgrade |
| Equal-budget recruit pool | Різні сила/ціна, level-dependent variance |
| Dashboard replacing original hub | Original UI structure first; окремий redesign |

Уточнення історії v1: він містив рекомендацію upfront premium sale, але виключав куповану силу. Тому «no planned monetization» тут позначає відкинутий напрям, а не твердження, що v1 взагалі не згадував business model.

**На цьому зупинено роботу. Код, UI implementation та commerce integration не починалися.**
