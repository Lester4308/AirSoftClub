# Project boundary — mandatory

This repository is a NEW, INDEPENDENT GAME, created from scratch. Working project identifier: Airsoft_Club_Game; this is not its final commercial title.

It is inspired by the historical management loop of the VK game documented in PaintballWars_Research, reinterpreted in an airsoft setting with Steam social discovery and asynchronous PvP.

Do not continue, modernize, merge with, or modify Project Airsoft / Airsoft Manager. Those are separate projects. Do not read their decisions as inherited requirements or use their repository as this project's foundation.

Do not automatically reuse their code, architecture, data model, fighter classes, combat rules, economy, calendar, UI flow, progression, repository structure, save system, balance, or terminology. Do not import their Git history, worktree, shared state, database, or runtime dependencies.

Any proposed reuse of an idea must first have an explicit entry in design/REUSE_DECISIONS.md describing its source, alternatives, justification for THIS game, impact and decision status. Until explicitly adopted for this game, it is not a requirement. Recording an idea does not authorize copying code or assets or modifying the source project.

Use this repository's own design documents, architecture, data model, roadmap and history. The user boundary in PROJECT_BOUNDARY.md takes precedence over earlier ambiguous naming or assumptions. Existing design numbers are prototype hypotheses, not approved or validated balance.

PaintballWars_Research is read-only historical evidence. Preserve ORIGINAL CONFIRMED / ORIGINAL STRONGLY SUPPORTED / RECONSTRUCTION DECISION / NEW AIRSOFT DESIGN distinctions. Create all production code, art, audio and branding independently.

Current stage: design only. Do not start game implementation merely because documentation exists; follow the user's current task. Do not publish a remote repository or deploy without task authorization.

## Current canonical product decisions

Use design/AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md and design/PRODUCT_DECISIONS.md. User-approved corrections in design/USER_APPROVED_CORRECTIONS_v2.txt supersede v1 assumptions. V1 product files are historical pointers only.

Mandatory constraints: roster max16; selectable1–16 fighters per side with asymmetric battles allowed; one challenge/one battle/one round/one result; HP/Damage/Armor and persistent health; first recruit free, later recruits paid; approximately6–7 varied recruitment offers; persistent BB classes; soft Money plus premium Credits; mandatory commercial monetization with limited power advantage; MK means visual AND small gameplay upgrade; preserve original hub/destinations. Friends and default Revenge are non-ranked; Ranked uses its own backend pool. No persistent Energy and no direct heal-for-Credits.

Distinguish USER APPROVED product rules from historical evidence, balance hypotheses and open questions. Do not invent final formulas or silently select offline-defense resource policy. This correction pass ends at the product/design gate; implementation requires a separate subsequent decision.
