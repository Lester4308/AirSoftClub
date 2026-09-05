# Airsoft core loop — v2

USER APPROVED V2-00/01/02/04/10–14/20/28/31. [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Historical anchors E007–E022; exact timings/rewards не встановлено.

## Основний loop

Hub → Team / Recruit → Training за потреби → Shop → Equip / BB loadout → Club/Opponents → обрати власних 1–16 → opponent preview → один automatic battle → Result → reward → оцінка ammo/health costs → Recovery або заощадження/upgrade → next battle.

Shop/Training/Recovery — destinations, а не обов'язкові кліки кожного проходу. Гравець з ready fighters може одразу вибрати opponent. Поранена команда не зобов'язана лікуватися до 100%; кількість deployed не вирівнюється з opponent.

## Перший клуб

1. Steam identity/profile → Club → Recruitment.
2. Market орієнтовно 6–7 candidates; вибір free-first має бути конкретизовано Q-02. Тільки один free entitlement на клуб/акаунт; немає free trio або четвертого подарунка.
3. Один owned fighter; базове gear/ammo onboarding забезпечення — Q-04 до playable scope. Жодних фінальних стартових balances.
4. Перший 1v1 показує hit/miss, damage, armor, HP та витрату BB. Це навчальний slice, не глобальний format lock.
5. Result показує gross Money/XP, залишок HP/BB і costs. Перший бій не створює автоматично другого fighter.
6. Вибір: heal Money зараз / free timed recovery / битися пораненим, купити BB, тренувати/купити gear або збирати на другого recruit.
7. Розширення roster відкриває новий власний масштаб без обов'язку виставити всіх.

## Економічна ітерація

Gross reward визначається opponent Club Level і relative power. Ammo вже придбано раніше: battle споживає inventory, а не списує ще один fee. На екрані окремо показати replacement cost як estimate, не debit. Healing необов'язкове і списує Money лише при підтвердженні.

Credits→Money — окремий exchange flow; без direct premium heal та без автоматичної конвертації при нестачі Money. Net після ammo/healing може бути низьким або від'ємним; жодної inherited flat-positive таблиці v1.

## Повернення й соціальний цикл

Defense Report → погляд на opponent/current snapshot → optional non-ranked Revenge → result → підготовка. Friends працює з offline opponent; rating тільки в окремому ranked pool. Defense вплив на live HP/stock — Q-06, не прихований free reset.

Немає persistent Energy. Free recovery працює з server elapsed time і при відсутності гри, але offline authoritative changes не приймаються з cache. Offline practice та emergency resupply — proposed details Q-04/Q-12.

## Наступна UX перевірка

Перевірити час до першого бою, кліки між Club і Result, читабельність HP/currencies, очевидність recruitment tradeoff, частку витрат на ammo/heal, скільки боїв доступно без Credits. Конкретні seconds/session targets відкриті: v1 targets не видаються за перевірені.
