# Airsoft combat system — v2

USER APPROVED: variable1–16 deployment, one battle/round, HP/Damage/Armor, persistent HP і BBs. Exact formulas OPEN. [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md); E012/E020 — automatic shooting, damage/MISS/HP/Skip.

## Battle contract

Input:BattleConfig(mode, arena, ruleset), attacker snapshot1–16, defender snapshot1–16, server seed, simulation build, PRNG version, balance version, resource policy version. Немає validation countA=countB. Не нормалізувати вхід до трьох actors і не застосовувати hidden fixed-size scaling.

Один challenge дає один battle та один result. Немає roundWins, best-of-three, respawn/reset між rounds або повторів заради серії. Automatic watch/skip; гравець не стріляє й не надає tactical orders вручну.

Inputs містять starting CurrentHP/MaxHP, training-derived stats, weapon damage, MK, armor, BB class/quantity, formation якщо буде обрана. Acceptance pins input; не читати змінний live inventory під час replay. Клієнт не може запропонувати authority seed, result або свої effective stats.

## Causal combat pipeline — без final coefficients

| Крок | Вхід / відповідальність | Що ще відкрите |
|---|---|---|
| Max HP | Fighter stats/progression + дозволені gear modifiers | MaxHP function і upper caps |
| Starting HP | Authoritative health на acceptance timestamp | Recovery pause/concurrency policy |
| Action scheduling | Living fighters, weapon handling/cadence | Tick/event model, intervals |
| Target | Valid enemy із HP>0 | Uniform/weighted choice, formation effects |
| Fire availability | Selected BB stock/loadout | BBs per action, magazines, reload/mixing |
| Ammo debit in simulation | Витрата для performed fire action | Miss теж consumes за рекомендованою proposal |
| Hit calculation | Fighter accuracy, weapon/MK, defender agility/gear | Probability bounds, range/cover, rounding |
| Weapon damage | Base weapon + small MK changes | additive vs multiplicative, randomness |
| BB interaction | Class damage modifier або equivalent ballistic effect | order/penetration relationship |
| Armor mitigation | Armor rating зменшує received damage | curve, minimum damage, penetration option |
| Health transition | HPafter=max(0, HPbefore−appliedDamage) | Числовий scale/rounding |
| Elimination | HP<=0 | simultaneous event ordering |
| Battle end | Одна сторона повністю eliminated | both-zero, timeout, no-ammo/stalemate Q-05 |

HP transition — інваріант стану, не фінальна damage formula. Кілька hits можуть бути потрібні для elimination. Не встановлювати fire chance1800bp,300tick limit, magazines3 або будь-які старі constants v1.

Strong armor зменшує damage, може мати mobility tradeoff. No invulnerability або guaranteed hit для premium. Ammo ceiling~15% є hypothesis для BB, а не total premium advantage. Повний stack тестується на hits-to-eliminate thresholds і group focus-fire.

## Після бою

Result містить finalHP per fighter, spent/unspent BB per class, win/loss/edge result, event log. Persistent attacker health і stock оновлюються once через settlement. Skip не лікує й не повертає витрачені BB. UI memory/replay restart не повторює resource transitions.

Матеріалізувати free recovery від server clock перед acceptance, далі input frozen. Proposal: не нараховувати recovery під час accepted combat, почати наступний recovery interval після logical battle settlement. Точна semantics Q-03, особливо при затримці worker. Playback duration не повинна змінювати HP/reward.

Offline defender policy Q-06 не обрана. Варіанти:
- Live defender HP/ammo:серіалізація/reservations, ризик пасивного resource drain.
- Isolated defense snapshot:відсутність live debit, але це окремий відхід від persistent cost principle, потребує approval.
- Dedicated defense resource pool:складніший UI/economy, не default.

Будь-який варіант мусить зберігати finalHP/BB use у battle record. Жоден не реалізується мовчки. Повторні offline attacks не можуть неконтрольовано писати finalHP поверх іншого бою.

## Determinism / replay

Server-authoritative deterministic sim — RECONSTRUCTION DECISION. Pure input→result, event ordering, PRNG algorithm, serialization/fixed-point policy фіксуються в окремому technical design після дозволу. Старі ruleset/build/tables потрібні для replay/audit; seed сам по собі недостатній.

Event:sequence, simulationTime, actorId, targetId, type, ammoClass/quantity, hit/miss, rawDamage, mitigatedDamage, HPafter, elimination. Summary показує фактичні події, не гарантовану контрфактичну причину поразки. Presentation не використовує physics як authority.

## Future validation gates — не виконані зараз

Валідація всіх 256 пар countA/countB у 1..16; порожній/17/duplicate/foreign fighter відхилені. Приклади 1v5,16v2,10v16,16v16 не блокуються size equality. Performance виміряти на 32actors.

HP<=0 elimination, armor impact, BB damage/stock consumption, miss cost, zero ammo/timeout, both-zero, partial HP entry. Watch/skip/disconnect мають однаковий settlement. Seed sweeps:stats/quantity/MK/BB/armor/level, side swaps, underdog/overpower. До запуску цих перевірок balance не заявляється.
