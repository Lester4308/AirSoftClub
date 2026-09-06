# Steam social / asynchronous PvP — v3

[Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md); [reward table](AIRSOFT_ECONOMY.md); [implementation questions](IMPLEMENTATION_QUESTIONS_v3.md). Це архітектурний design, інтеграції ще немає.

## Steam identity та online boundary

Steam-first, Unity/C# client, Steamworks через C# integration layer (конкретний layer OPEN). Сервер перевіряє Steam identity/ownership і видає власну session. Steam friend enumeration допомагає discovery; client SteamID/friend list не є доказом дозволу атакувати чи отримати Credits. Ключі publisher та privileged API лишаються на backend. Джерело: [Steam authentication](https://partner.steamgames.com/doc/features/auth).

PvP, Ranked, Steam Friends, Credits, purchase/conversion, matchmaking, leaderboards і authoritative economy online-only. Offline launch/read-only cache можливі; isolated PvE лише пізніше, без локального authoritative XP/rating/economy. Network retry не повторює нагороду.

## Вибір opponent

Ranked backend пропонує **3–5 candidates**, гравець обирає слабшого/рівного/сильнішого. Показувати Club Level, rating, fighter count, approximate strength і potential reward category. Exact numeric Team Power та його formula не показуються. При недостатньому eligible pool потрібен чесний empty/limited state, а не вигаданий opponent.

Steam Friends — social discovery із rating eligibility за directed pair window, а не виключно non-ranked mode. Unlimited attempts проти одного friend підпорядковані shield та іншим eligibility checks. Вікно 8h: одна rating-eligible win; losses допускають retry, але rating loss bounded; після win жоден наступний battle у цьому вікні не змінює rating. Reward: перша win повна, друга/третя reduced, з четвертої нулі Money/Club XP/Fighter XP до кінця window. Discount/window anchor/loss cap OPEN.

## Immutable defense

Snapshot включає fighter IDs/stats/Max HP, всі 4 equipment slots/MK, active BB class, rating, combat modifiers, ruleset/balance versions та generation timestamp. Defense fighters стартують із 100% Max HP попри live wounds; defense result не змінює live Current HP і не списує live BB. Симуляційний ammo budget незалежний від реального debit, конкретне правило Q04.

Кілька attackers можуть використовувати ту саму immutable version. Heal/equip та інші committed combat-relevant зміни створюють новішу snapshot для нових battles, не змінюють accepted input. Match acceptance фіксує version і eligibility; settlement використовує цей input і актуальні transactional quotas, не покладається лише на snapshot rating.

## Rating exposure і shield

Не більше **4 rating-impacting incoming attacks на profile за 24h** сукупно, незалежно від entry point. Draw із нульовим impact не повинен списувати фактичний rating-impact count. Concurrent acceptance потребує reservation, щоб два matches не зайняли останнє місце. Release/commit, window model та post-cap visibility OPEN; не дозволяти fifth impact через race або mode switch.

Shield за Credits: **8 hours / 1 day / 3 days / 7 days**. Active shield блокує **всі нові incoming attacks**, включно з Friends і Revenge. Accepted battle не скасовується при подальшій activation. Чи власна Ranked attack знімає shield — **OPEN**; не вводити автоматичного скасування.

## Revenge

Defense loss може створити origin-linked opportunity на **24h**, максимум **3 attempts**. Перша win consumes ticket, losses допускають retry до win/3 attempts/expiry. Draw attempt accounting та in-flight expiry OPEN.

Successful Revenge повертає **120% rating, фактично втраченого в origin defense battle**: -10 → +12. Не 120% current rating; при origin loss 0 база recovery дорівнює 0. Додатково standard battle reward без extra multiplier. Не нараховувати ще й звичайний Ranked-win gain поверх recovery без окремого рішення.

Rating debit іншій стороні, округлення, origin eligibility при zero rating, cross-mode farm prevention і quota interaction — Q07/Q09. Реєстр ticket не дозволяє подвійної successful consumption. Shield може тимчасово заблокувати target; не обіцяти обхід shield або продовження 24h без рішення.

## Commerce foundation

Paid Steam license + optional Credits. Purchase intent містить server-defined SKU/price/currency і unique order ID; client не задає award amount. Сервер ініціює Steam transaction, отримує user authorization, виконує Finalize і видає Credits лише після підтвердженого success. Невідомий outcome звіряється з platform status; retries idempotent. Refund/chargeback reconciliation коригує ledger за політикою, не повторює grant. Джерело: [Steam Microtransactions Implementation Guide](https://partner.steamgames.com/doc/features/microtransactions/implementation).

Leaderboard є projection authoritative rating; client не задає score. Steam display/sync не замінює backend truth. Daily rewards/7-day streak/recruit refresh/free recovery/Revenge/Club Level/social rivalry — retention pillars. Club names, reports, sanctions та emblem moderation if UGC мають окрему foundation; не overbuild їх у першому code milestone.
