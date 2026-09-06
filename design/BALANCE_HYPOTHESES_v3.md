> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Balance hypotheses — v3

Це targets для майбутнього налаштування, **не final constants і не підтверджений баланс**. Не змішувати з hard rules у [реєстрі](PRODUCT_DECISIONS.md).

| ID | Prototype target / невизначена величина |
|---|---|
| B01 | Recruitment refresh через ~1 годину АБО ~10 матчів, що раніше; ціна immediate soft refresh відкрита. |
| B02 | Dismissal повертає ~25–40% визначеної бази вартості; база та rounding відкриті. |
| B03 | Free recovery ~1% Max HP за хвилину; readiness після 0 HP ~10% Max HP. |
| B04 | Basic starter supply приблизно на 5–7 перших матчів; тестувати на початковому roster і shot distribution. |
| B05 | Emergency: Basic stock <~15% club capacity, недостатньо soft для мінімального refill; +500 Basic раз на ~3 години. Capacity/overflow та min refill ще визначити. |
| B06 | 3–5 BB tiers: Basic 0%; Improved ~3%; Advanced ~5%; High-End ~10%; Premium до ~15% effective advantage. Це не обов'язково чистий damage bonus. |
| B07 | Premium-only weapon/armor приблизно +5–8% проти доступного без купівлі Credits аналога. |
| B08 | Повний paid stack: ~15–20% effective combat advantage над нормально екіпірованим аналогом того ж progression tier без купівлі Credits. |
| B09 | XP curve швидка на початку, повільніша далі; training caps/prices, recruit distribution, capacity curve та unlock levels відкриті. |
| B10 | Hit/damage/armor/penetration, Agility penalties, tempo, power formula/category thresholds, reward scaling і rating coefficients відкриті. |
| B11 | Пропозиція для reward table: draw Money 10% win-equivalent, draw Fighter XP 35%, loss Club XP 0. Ці три числа НЕ user-approved hard rules. |
| B12 | Second/third friend win discount і всі Credit faucet/exchange/shop prices ще відкриті. |

Hard rules включають roster 16, 3 base stats, 3 free starter candidates (обрати одного), стандартні 6–7 offers, 4 slots, 6 weapon families, draw Club XP 35%, loss Money 0, loss Fighter XP 25%, Ranked draw rating 0, directed friend window 8 годин/одна rating win/нульова нагорода з четвертої перемоги, defense exposure 4/24h, shields 8h/1d/3d/7d, Ranked list 3–5, Revenge 24h/3 attempts/120%, early unlock +3 levels і 7-day streak.

Окремі targets не додаються як дозволений максимум: premium BB ~15% плюс gear ~8% плюс MK3 не доводять дотримання загальної межі. Майбутня перевірка має порівняти повні builds за однакових progression, roster, стартових HP, сценарію і множини seeds. Обрати метрику (наприклад, еквівалент сили за outcome curves) до висновку про відсоток переваги; raw win-rate delta не прирівнювати автоматично до effective power.
