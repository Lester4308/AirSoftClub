# Core loop — v3

**MANAGEMENT → PREPARATION → OPPONENT SELECTION → AUTOMATIC BATTLE → RESULT → PROGRESSION → REPEAT.**

Нова airsoft-гра використовує management DNA історичної VK-гри, з власним дизайном і Steam asynchronous PvP. [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md).

1. Club management: roster до 16, recruitment, training, inventory; перший fighter — вибір одного з 3 free candidates.
2. Preparation: лікування, чотири gear slots, shared club BB refill та один active BB class. Усі owned combat-ready fighters підуть автоматично; manual squad selection немає.
3. Opponent selection: Steam Friends, Ranked list 3–5 candidates або доступний Revenge. Видимі Club Level/rating/count та approximate strength/reward categories; exact power прихований.
4. Automatic battle: один challenge/battle/round, HP/Damage/Armor, shared shots. Гравець не керує стрільбою.
5. Result: win/draw/loss, нагороди з eligibility explanation, actual offensive HP/BB, rating effect і Revenge opportunity за умовами.
6. Progression: Fighter XP відкриває training cap, upgrades купуються soft; Club XP відкриває можливості. Повернення до club.

Offensive wounds зберігаються, free recovery діє за offline elapsed time; defense snapshot стартує full HP і не списує live ресурси. No persistent Energy. Нульова friend reward після третьої оплаченої перемоги означає: **четверта і далі нульові** до завершення 8h window; battle attempts залишаються можливими за eligibility.

Retention: daily rewards, 7-day streak, recruit refresh, free recovery timer, Revenge 24h, Club Level та Steam rivalry. Paid Steam game + optional Credits, не вимога купувати Credits для базового циклу.

Offline: launch і read-only cached club; critical actions потребують connection. Final art OPEN. [UI flow](AIRSOFT_UI_FLOW.md), [Economy](AIRSOFT_ECONOMY.md).
