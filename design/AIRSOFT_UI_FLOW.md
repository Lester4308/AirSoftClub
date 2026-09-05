# Airsoft UI flow — v2

USER APPROVED V2-28/29. [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Original hub/screens E006–E023 зберігаються як інформаційна структура; новий visual redesign — окремий етап.

## Navigation foundation

Launch / Steam auth → Club hub.
Hub → Team/Recruitment · Training · Shop · Recovery · Club/Opponents.
Opponents → Preview → One Battle → Result → Hub/Recovery/Shop/наступний opponent.

Shortcuts допустимі для швидкості, але не скасовують hub і destinations. Домашній Club hub відрізняється від Club/Opponents як місця пошуку суперника; final labels визначить redesign.

## Screen contracts

| Screen | Required information / action | Edge state |
|---|---|---|
| Hub | Team/fighter focus, Money, Credits, HP, BB stock, locations | Cached/offline status, без wallet upload |
| Team | Owned roster≤16, deployment1–16, health, gear/ammo readiness | Partial HP allowed;0/17 selection invalid |
| Recruitment | Approx6–7 recruits, different stats/strength/price, Club Level context | First-free rule явне; capacity/refresh Q-02 |
| Fighter | Stats, Max/CurrentHP, gear, MK, training | Без round-stamina або one-hit UI |
| Training | Обране покращення, витрати, preview | Ціна/XP/caps Q-01, без вигаданих фінальних чисел |
| Shop | Weapons, armor/protection, BB classes, MK, premium offers | Money й Credits не взаємозамінні |
| Early access | Ordinary Club Level gate + explicit Credits option | Приклад 7→10 не actual AR spec |
| MK upgrade | Base/MK1/MK2/MK3 appearance + stat deltas | Gameplay upgrade, не cosmetic-only label |
| Currency exchange | Credits amount, Money output, quote | Окреме підтвердження, без implicit exchange |
| Recovery | Per-fighter HP, free recovery estimate, Money immediate heal | Немає Heal for Credits; changed quote→refresh |
| Opponents | Friends / Ranked / Revenge, count, power, Club Level | No friends/social error окремо; unequal sizes valid |
| Preview | Own/defense counts, HP/gear/ammo, level/power, reward context | Stale snapshot/resource quote→refresh |
| Battle | HP bars, damage, miss, armor effect, ammo, remaining fighters | Layout для 32 actors; one round, no round wins |
| Result | Outcome, gross Money/XP, HP after, BB spent/remaining, replacement estimate | Wallet delta окремо; optional heal quote не auto debit |
| History | Attacks/Defenses, mode, snapshots, outcome, ranked changes | Defense resource effects за Q-06 |
| Leaderboards | Global/Friends/Around Me, ranked rating | Challenge non-ranked поза approved ranked flow |
| Inbox | Defense/revenge, read state | Не залежить від external push |

## First-use flow

Перший free fighter → ready gear/BB за Q-04 → automatic1v1 → damage/ammo/result → recovery/replenishment або saving на paid recruit. Не видавати готову трійку й не прив'язувати onboarding до серії раундів.

## Currency і monetization UX

Money — звичайні витрати, Credits — premium. Реальні гроші, Credits і Money показувати різними одиницями. Gameplay effects раннього доступу, MK та premium BB видимі, не приховані за словом «skin». Prices/modifiers TBD; production screens із вигаданими числами зараз не створюються.

Health cost і BB replacement estimate не маскують mandatory premium purchase. При нестачі Money free recovery доступна; empty ammo fallback Q-04 потрібний окремо. Немає automatic Credits top-up/exchange/heal.

## Next stage: ORIGINAL UI ANALYSIS → MODERN AIRSOFT UI REDESIGN

1. Зіставити historical screens з evidence frames, locations і кількістю переходів.
2. Визначити збережені task flows до visual choices.
3. Створити нові layout concepts для hub, recruit market, shop, fighter, recovery, asymmetric battle.
4. Перевірити Money/Credits, HP/BB, щільність 16 fighters, keyboard/focus, scale/tooltips.
5. Лише після окремого рішення — UI implementation/production assets.

Це flow requirements, не готовий redesign. Layout, typography, animation і rendering technology ще не вибрано.
