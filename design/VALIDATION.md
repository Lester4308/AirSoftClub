# Documentation validation — v3

Дата: 2026-09-06. Repository: Airsoft_Club_Game. Branch: codex/design-foundation. Verified pre-pass HEAD: 3f76545797e5c9bd594b0b6cb22e4822f87f06d7; початковий working tree був чистий.

**Це перевірка документації, не тести гри, balance simulations або підтвердження робочих інтеграцій.**

## Coverage і source integrity

- PRODUCT_DECISIONS містить рівно 77 унікальних послідовних рядків V3-00–V3-76 із посиланнями на покриття.
- Gate містить усі 12 required sections у порядку 1–12.
- Exact user request збережено у USER_APPROVED_DECISIONS_v3.txt. SHA-256: 25FB25CA645471DFE35BDE7700C9855E222C050595674968D0563EABF19733B4; збігається з attachment.
- Окремо зафіксовано відповідь користувача: перша friend win full, друга/третя reduced, четверта+ zero.
- SOURCE_MANIFEST.csv, REUSE_DECISIONS.md та historical USER_APPROVED_CORRECTIONS_v2.txt не змінені від verified base. Зовнішні research source bytes у цьому pass повторно не хешувалися.

## Consistency review

| Область | Результат |
|---|---|
| Boundary | Усі записи лише в незалежному Airsoft_Club_Game; без import/reuse або змін Project Airsoft |
| Roster | Max 16, автоматична участь усіх combat-ready, asymmetry; gear не eligibility gate |
| Health / BB | Offensive HP persists/real shots debit shared BB; defense full HP/no live HP or BB debit |
| Progression / gear | 3 base stats, XP → cap → soft upgrades; 4 slots, 6 families, MK2 soft/MK3 Credits-only |
| Rewards | Draw Club XP 35%; loss Money 0/Fighter XP 25%; інші proposed coefficients явно hypotheses |
| Friends | Directed 8h/one rating win; subsequent rating 0; fourth win onward zero rewards |
| PvP | Exposure 4/24h, shield durations/accepted-match semantics, Ranked 3–5 selection, Revenge 24h/3/120% origin loss |
| Commercial / stack | Paid Steam + Credits; entitlement-only early unlock +3; Unity/C#/ASP.NET Core/PostgreSQL |
| Open questions | Readiness, no-weapon, defense ammo budget, window anchors, loss cap, Revenge edge cases, MK1, own-attack shield, formulas/versions залишені видимими |
| Art / scope | Final art OPEN; лише recommended future foundation scope, без implementation |

Пошук legacy terms переглянуто в контексті: old squad/3v3/one-hit/non-ranked-only/3-slots/final-3D правила присутні лише як заперечені, SUPERSEDED або historical evidence. V1/v2 product files тепер pointers. Поточні docs посилаються на v3, historical research не оголошено активним продуктним каноном.

## Structural verification

Markdown file links перевірено на існування, включно з historical absolute references; broken links 0. Markdown tables перевірено на однакову кількість колонок у кожній таблиці; errors 0. Перевірка Git diff включає scope, file extensions та whitespace; тільки documentation files. Документ цього report проходить фінальний повторний link/table check перед commit.

## Межі виконаного

Не створювалися production code, Unity project, C# solution, live backend, migrations, database, Steam integration, purchases, matchmaking, UI, production art чи deployment. Game tests не запускалися, бо implementation не починався. Project Airsoft не змінювався; усі write operations цього pass адресовані Airsoft_Club_Game.

**PRODUCT DESIGN GATE v3 — APPROVED FOR IMPLEMENTATION PREPARATION**
**IMPLEMENTATION NOT YET AUTHORIZED.**

Commit із цим report фіксує завершений документаційний pass; його SHA наведено у фінальному повідомленні, без самопосилального hash у файлі.
