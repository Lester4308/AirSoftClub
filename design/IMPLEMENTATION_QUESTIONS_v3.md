# Remaining implementation-level questions — v3

Статус: OPEN або IMPLEMENTATION PROPOSAL. Це не схвалені приховані правила. Жодне питання нижче не змінює підтверджені межі [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md).

| ID | Питання / потрібне рішення | Коли закрити |
|---|---|---|
| Q01 | Формули Accuracy/evasion/tempo, damage/penetration/protection, стартові HP; fixed-point чи інший відтворюваний числовий формат; tie resolution та safety cap. | Перед реалізацією battle core |
| Q02 | Readiness: поріг для кожного пораненого чи latch після 0 HP; округлення порога, відновлення, Max HP upgrade та часових меж. 0 HP точно не допускається до власної атаки. | Перед health model |
| Q03 | Поведінка fighter без Weapon: він обов'язково бере участь, але чи може атакувати і як виникає no-progress draw; не видавати неузгоджену безкоштовну зброю. | Перед battle core |
| Q04 | Скінченний simulated BB budget захисту незалежно від live stock, ємність по tiers/загальна, overflow emergency grant, burst/pellet облік, поводження при вичерпанні обраного класу. | Перед battle core |
| Q05 | Reward curves, draw Money/Fighter XP, loss Club XP, reduced-friend коефіцієнт, округлення та bounded power modifier; окремо налаштування XP кожного учасника. | До економічної реалізації |
| Q06 | Початок 8-hour friend window і 24-hour exposure window, точний ліміт rating loss до першої friend win, rating formula/floors. Пропозиція: максимум одна rating loss на directed pair/window; не вважати затвердженим числом. | До live PvP |
| Q07 | Post-cap discovery, reservation/settlement exposure, конфлікт глобального cap з Revenge. Не перевищувати 4 incoming rating impacts; не обіцяти Revenge recovery до перевірки eligibility. | До live PvP |
| Q08 | Чи власна Ranked attack знімає shield — OPEN продуктне уточнення; не додавати auto-cancel. Час activation/acceptance вирішує backend. | До shields або Ranked launch |
| Q09 | Revenge draw як attempt; reservation/expiry під час бою; rating іншої сторони, округлення 120%, захист від циклів Revenge-on-Revenge та штучного створення tickets. | До live Revenge |
| Q10 | MK1: чи існує окремо від Base, його naming/acquisition/upgrade path. V3 визначає Base, MK2 і MK3; не вигадувати правило MK1. | До item catalog |
| Q11 | Price tables, starter grants, soft/Credits conversion, free Credit faucets, платні spend limits, reward ceilings і premium-stack метрика ефективної переваги. | До економічного прототипу |
| Q12 | Версії Unity/.NET/PostgreSQL, C# Steamworks layer, serialization, backward compatibility і shared-library target, хостинг, observability, migrations. | За відповідним milestone |
| Q13 | Recruitment generation/refresh resets, match counter policy, resale base, доля gear при dismissal; одноразова free-recruit entitlement не відновлюється через resale/reset. | До recruitment |
| Q14 | Recovery clock/remainder при зміні Max HP, concurrent offensive commands, snapshot publication після committed змін, мінімальна валідна defense roster. | До backend state lifecycle |
| Q15 | Daily/streak timezone і пропуск дня, moderation workflow/санкції/UGC scope, reconciliation і refund support. | До social/economy launch |

Уточнення користувача від 2026-09-06 закрило межу friend reward: **перша повна, друга й третя зменшені, з четвертої нульові**. Після вичерпання нагород нулі зберігаються до завершення вікна; loss/draw не відкриває новий reward budget. Точний discount ще не затверджено.

До старту першого code milestone достатньо окремого дозволу та конкретизації Q01–Q04 для тестових fixtures. Решта питань блокує відповідні наступні системи, а не підготовку документації. Art рішення залишається окремим майбутнім pass.
