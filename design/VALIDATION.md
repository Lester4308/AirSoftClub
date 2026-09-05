# Documentation validation — v2

2026-09-05. Перевірка correction pass у незалежному Airsoft_Club_Game. **Це перевірка документів, не виконані тести гри, балансу чи платежів.**

## Виконана перевірка

- У PRODUCT_DECISIONS.md є рівно35 рядків V2-00–V2-34; усі секції затверджених corrections мають відповідний запис.
- Updated Summary містить усі4 потрібні розділи: CONFIRMED, BALANCE HYPOTHESES, OPEN QUESTIONS, SUPERSEDED DECISIONS.
- Pack v2 і Game Design Spec v2 — активні. V1 файли — тільки SUPERSEDED pointers; попередні тексти збережені у Git history.
- Оновлені core loop, fighters/recruitment, equipment/MK/BB, combat, economy, Steam/social/commerce, backend, data model, UI flow, mapping, MVP plan, roadmap та індекси.
- Markdown links на локальні файли перевірені: broken links0. Таблиці перевірені на кількість колонок: errors0. Evidence IDs звірені з ledger: unknown IDs0.
- Усі26 джерел original SOURCE_MANIFEST.csv мають незмінні SHA-256. Архів PaintballWars_Research не змінено.
- USER_APPROVED_CORRECTIONS_v2.txt точно збігається з attachment за SHA-256: 60D8BEE75773AB34B25FD70A737B43EBF7AA235B61CA502DAD967F756481B1D0.
- Перевірено скасовані правила: mandatory3v3, roster6, one-hit, equal sizes, multi-round, full HP reset, fixed ammo fee, single soft Credits, cosmetic-only MK та flat rewards згадуються лише як скасовані/заперечені або історичні.
- Власні файли нового проєкту — єдина область записів; Project Airsoft не відкривався для імпорту і не змінювався. Змінені/нові артефакти — тільки документація та точна текстова копія user corrections.

## Semantic checks

| Topic | Узгоджений результат |
|---|---|
| Size | Max16 owned; selectable1–16 per side; усі256 комбінацій — future validation target, не вже виконаний simulation test |
| Battle | Один challenge/battle/round/result; HP/Damage/Armor; elimination HP<=0 |
| Recruitment | First free, later paid;6–7 varied offers; level-dependent quality; potential OPEN |
| Health | Persistent damage, Money immediate heal/free timed recovery; no Energy/direct Credits heal |
| Ammo | Persistent stock/classes; purchase і consumption не створюють подвійного money debit |
| Rewards | Opponent Club Level foundation, Team Power scaling; unequal fights legal |
| Commerce | Soft Money + premium Credits; ранній доступ, modest premium edge, MK visual+stats |
| Balance | BB0/3/5/10/~15% лише hypotheses; сумарний premium cap OPEN |
| Social | Friends/default Revenge non-ranked; separate backend Ranked pool |
| UI | Hub/destinations збережені; redesign окремим майбутнім етапом |
| Gates | Q-01–Q-12 видимі; особливо defense resource policy та combined monetization effect не обрані приховано |

## Не виконувалося

Production code, engine setup, database migrations, game simulation, automated gameplay tests, economy simulation, Steam login/payment integration, actual purchases, UI implementation, production art, deployment або зміни Project Airsoft.

Steam commerce statements звірені з офіційною документацією; це не підтвердження робочої інтеграції. Числових балансних гарантій і фінальних цін немає.

**Результат: correction pass документації завершено; зупинка на PRODUCT / DESIGN GATE. Implementation потребує окремого наступного рішення.**
