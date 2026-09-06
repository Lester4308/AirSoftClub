# AIRSOFT RECONSTRUCTION PRODUCT DECISION PACK v3

**PRODUCT DESIGN GATE v3 — FINALIZED FOR IMPLEMENTATION PREP**
**APPROVED FOR IMPLEMENTATION PREPARATION — IMPLEMENTATION NOT YET AUTHORIZED.**

Airsoft_Club_Game — окрема нова гра. [Обов'язкова межа](../PROJECT_BOUNDARY.md). Цей pack замінює v2 з verified base 3f76545. Авторитетний текст: [рішення користувача 0–76](USER_APPROVED_DECISIONS_v3.txt), [friend clarification](USER_CLARIFICATION_v3.md). Повний consolidated gate: [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md).

## Продукт

Management/preparation/opponent selection/automatic battle/result/progression; один challenge = battle = round. До 16 owned fighters, усі боєздатні автоматично беруть участь; асиметрія дозволена, manual active squad відсутній. Accuracy/Endurance/Agility; XP відкриває training cap, soft купує upgrades. Один free starter з 3 candidates, звичайний recruitment 6–7; variance і розвиток дешевих ветеранів зберігаються.

[Core loop](AIRSOFT_CORE_LOOP.md), [UI](AIRSOFT_UI_FLOW.md), [fighters](AIRSOFT_FIGHTER_SYSTEM.md).

## Бій та ресурси

HP/Damage/Armor, multi-hit, offensive HP persists; heal soft або free timed offline recovery. Max HP upgrade не лікує, zero HP вимагає readiness recovery, Energy немає. Чотири slots; 6 weapon families; light/medium/heavy protection versus Agility.

Shared club BB, один active class, debit actual attacker shots; defense full-HP immutable snapshot не витрачає live HP/BB. Win elimination, draw simultaneous elimination/ammo inability/safety cap. Draw Club XP 35%, Ranked rating 0; loss Money 0/Fighter XP 25%.

[Combat](AIRSOFT_COMBAT_SYSTEM.md), [equipment/BB](AIRSOFT_EQUIPMENT_SYSTEM.md), [reward table](AIRSOFT_ECONOMY.md).

## PvP і social

Ranked показує 3–5 opponents для вибору; exact Team Power прихований. Friends directed 8h: одна rating-eligible win, після неї rating 0; bounded loss до win. Перша win full reward, друга/третя reduced, четверта+ нульова до кінця window.

Defense exposure 4 rating-impacting attacks/profile/24h; shields 8h/1d/3d/7d блокують нові attacks, accepted finish. Own Ranked shield interaction OPEN. Revenge 24h/3 attempts; success restores 120% exact origin rating loss та standard battle reward, без extra multiplier.

[Steam/PvP](STEAM_SOCIAL_PVP_ARCHITECTURE.md).

## Комерційна модель та stack

Paid Steam game + optional Credits; limited gameplay faucets і conversion у soft. Early unlock до +3 Club Levels лише entitlement, item purchase soft окремо. Base normal, MK2 expensive soft, MK3 Credits-only, MK1 OPEN. Premium BB/gear дозволені в межах targets; combined paid advantage ~15–20% має перевірятися як stack. Pure cosmetics later.

Unity/C# client, C#/ASP.NET Core backend, PostgreSQL, Steamworks C# integration layer. Backend authoritative, shared contracts safe-only, offline read-only. [Backend](BACKEND_ARCHITECTURE.md), [data model](DATA_MODEL.md).

## Межі фіналізації

Final art OPEN, concepts reference-only. Approximate numbers у [Balance register](BALANCE_HYPOTHESES_v3.md) не є validated constants. [Q01–Q15](IMPLEMENTATION_QUESTIONS_v3.md) явно відділяють remaining decisions від confirmed rules. [Реєстр 0–76](PRODUCT_DECISIONS.md) дає покриття вимог і superseded decisions.

Майбутній перший milestone — лише [локальна deterministic battle foundation](MVP_IMPLEMENTATION_PLAN.md), після окремого дозволу. Код, live systems та production art не розпочаті.
