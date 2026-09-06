> **CONSOLIDATED / historical v3 reference — 2026-09-06.** Чинний канон: [Master Development Spec v1](../AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md); порядок подальшої роботи: [Agent Implementation Pack v1](../AIRSOFT_CLUB_GAME_AGENT_IMPLEMENTATION_PACK_v1.md). Текст нижче збережено як v3 source record: пізніші Q01–Q15 CLOSED, актуальна reward table, MK1, shields, refresh, daily/ledger та verified Implementation001–002 беруться з master. Старі OPEN, balance proposals і milestone authorization/status не є активними альтернативами. Поточний pass — лише документація; final art OPEN.

# Recommended first implementation scope — v3

**IMPLEMENTATION NOT YET AUTHORIZED.** Цей документ лише готує наступне рішення користувача. [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md), [boundary](../PROJECT_BOUNDARY.md).

## Future milestone 1: reproducible battle foundation

Після окремого дозволу рекомендовано:
- Unity/C# solution foundation для цієї незалежної гри.
- Pure C# Domain без залежності від Unity scenes, Steam, database або network.
- Fighter/weapon/armor/BB domain models із 3 base stats, 4 slots, HP та shared BB.
- Immutable MatchConfig / MatchResult, seed і ruleset/balance version.
- Deterministic headless battle core: automatic roster до 16, asymmetric combat, multi-hit elimination, draw conditions.
- Reproducible battle tests і documented prototype configuration.

Перед написанням core визначити Q01–Q04: формули та числове представлення, readiness, no-weapon behavior, simulated defense ammo. Це versioned experimental fixtures, не фінальний баланс.

## Acceptance evidence для майбутнього milestone

Однакові inputs/seed/version відтворюють outcome, HP та shots. Перевірити 1v2, 7v16, 16v16; inclusion всіх eligible fighters без ручного squad; unequipped участь; multi-hit/HP <=0; simultaneous elimination, ammo draw, safety cap; club stock не йде нижче нуля; defense simulation не мутує live profile; Max HP upgrade не лікує. Presentation frame rate не змінює resolution.

Достатньо локальних fixtures/adapters для перевірки domain invariants. Production economy, persistent state, live ratings, purchases чи мережеву авторизацію в цьому milestone не симулювати як готові системи.

## Excluded

Production UI; Steamworks integration; ASP.NET Core live backend; PostgreSQL; monetization; Credits purchases; matchmaking; production art. Не встановлювати/не створювати їх у поточному documentation pass.

## Subsequent preparation

Пізніше окремо погодити health/recruit/economy prototype, authoritative backend/DB, Steam identity/social/PvP, payments/retention/moderation, art pass та production release. Кожен потребує своїх gates і закриття відповідних [Q01–Q15](IMPLEMENTATION_QUESTIONS_v3.md). Це не прихована згода реалізувати roadmap.
