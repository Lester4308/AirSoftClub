# MVP implementation plan — v2

**PLAN ONLY. Implementation потребує окремого наступного рішення.** [Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md), [ROADMAP](../ROADMAP.md).

## Завершений correction milestone D0

Затверджені corrections застосовано; v1 superseded. Active docs узгоджуються з variable1–16, одним боєм, HP/BB persistence, Money/Credits, MK, hub і monetization. Updated Summary відокремлює approved rules, hypotheses та open questions. Жодного runtime, database schema або production code зараз.

## Product/design gates перед реалізацією

D1: Q-01–Q-05 — stats/recruit/training/recovery/ammo/edge outcomes; Q-06 — offline defense resource policy до human async; Q-07/Q-09/Q-11 — acquisition, power/reward, business і cumulative paid edge.
D2: окремий original UI analysis → modern redesign, зі збереженням hub.
D3: узгодження balance scenarios й економічної моделі та scope наступного implementation decision.

Не потрібно фіналізувати всі coefficients, щоб згодом дозволити prototype. Невизначені health/resource semantics мають бути явно обрані або виключені зі scope такого дозволу. У цьому завданні зупиняємося після D0.

## Майбутня послідовність — тільки план

| Milestone | Scope | Exit evidence після окремого дозволу |
|---|---|---|
| I0 Foundation | Independent runtime ADR, auth adapter/dev stub, domain contracts | Minimal build окремого repo, без імпорту Project Airsoft |
| I1 One complete loop | Club, один free fighter, market, gear/BB, NPC battle, HP damage, result, Money, Recovery, save | Equip → battle → persistent HP/spent BB → heal/refill/save → repeat |
| I2 Variable roster/deployment | Paid recruits різної якості, level-dependent pool, 1–16 selection, asymmetric battles | 1v2/2v1/3v5/10v16/16v16; усі 256 пар кількості допустимі |
| I3 Durable authority | Transactions, Money/Credits ledgers, exchange, ammo reservation, health clock, snapshots | Retry/crash не дублює reward/ammo/heal |
| I4 Social | Real Steam auth, offline defense за Q-06, Friends, Ranked pool, Revenge/history | Unequal friends challenges; rating тільки Ranked |
| I5 Commerce validation | MK/early/premium SKUs, Steam sandbox, verified Credits grants, reversals | Combined balance, payment correctness, business review |
| I6 UI/release readiness | Approved modern UI, art/audio, accessibility, content, operations | Readability32 actors, currencies/costs, monitoring/restore/privacy |

Commercial model і потрібні domain boundaries закладені з I0/I1. Test currency може заміняти real checkout до I5; монетизація не optional. Перший 1v1 — technical slice, не public format lock. Boundary16v16 й асиметрія не відкладаються до невизначеного expansion.

## Validation plan

| Area | Cases |
|---|---|
| Roster/recruit | First free once; subsequent paid;6–7 varied offers; level quality overlap; reject17 owned |
| Combat sizes | Усі 1..16 ×1..16; reject0/17, duplicates; unequal count не блокується |
| Combat HP | Multiple hits, mitigation, MK/BB modifiers, HP<=0, persistent final HP |
| Operational economy | Actual stock spent, unused return, Money heal, free elapsed recovery, no direct Credits heal |
| Rewards | Opponent level base, power disparity, low payout16strong vs1weak, underdog manipulation |
| Monetization | Currency separation, exchange, early access, MK visual+stats, premium BB hypothesis |
| Stack balance | Free/paid cohorts, full gear/MK/BB/armor, roster size, HP sustain; no guaranteed win |
| Concurrency | Acceptance+heal/equip, worker retry, reservations, defense policy Q-06 |
| Steam/commerce | Wrong AppID/identity, unknown payment state, retry, refund after conversion |
| UX | Original hub, distinct destinations,32 actors, clear gross/cost/currencies |

Це список майбутніх перевірок, не test report. Немає final win rates, prices, release calendar чи engine choice. Власний roadmap не успадковує Project Airsoft calendar або milestones.
