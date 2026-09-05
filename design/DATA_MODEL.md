# Data model — v2

[Pack v2](AIRSOFT_RECONSTRUCTION_PRODUCT_DECISION_PACK_v2.md). Logical design, не production schema. UUID identities, UTC server timestamps, integer quantities/currencies. SteamID64/order IDs transport decimal string. Value scales/curves OPEN.

## Entities

| Entity | Key / fields | Invariants / status |
|---|---|---|
| PlayerProfile | id, createdAt, status, settings | No local economy import |
| ExternalIdentity | playerId, provider, subject, verifiedAt | unique provider+SteamID64; not nickname |
| Club | id, playerId, name, level, xp, firstRecruitClaimed, hireProgression, revision | roster≤16; hireProgression semantics Q-02 |
| RecruitOffer | id, clubId, poolVersion, generatedLevel, stats, quality, priceMoney, expiry? |6–7 market direction; immutable quoted payload |
| Fighter | id, clubId, stats, trainingProgress, appearance, revision | stats budget/caps OPEN; no fixed equal24 |
| FighterHealth | fighterId, currentHP, maxHP, materializedAt, recoveryPolicyVersion, busyBattleId? |0≤HP≤MaxHP; persistent |
| HealQuote | id, fighterIds, stateRevision, missingHP, moneyPrice, expiresAt | Currency soft only; server recomputes validity |
| ItemDefinition | id, version, family, slot, baseStats, armor, normalLevelGate, acquisitionOptions | New content; exact catalog OPEN |
| ItemInstance | id, ownerId, definitionId/version, mkTier, sourceTransactionId | One live owner/assignment |
| WeaponTierDefinition | baseItemId, tier, version, visualVariant, statModifiers, costOptions | Base/MK1/MK2/MK3; small modifiers TBD |
| EarlyAccessEntitlement | id, playerId, itemOrTierScope, sourceTransactionId, status | Acquisition vs ownership semantics Q-07 |
| PremiumCatalogEntry | skuId, version, priceCurrency, price, unlockScope | Credits/Money distinct; real packs separate |
| AmmoClassDefinition | id, version, damageOrBallisticModifier, priceOptions |3–5 direction;~15% target not final |
| AmmoStockLot | id, ownerId, classId, quantity, acquisitionCost/currency, sourceTransactionId | Nonnegative available; purchase provenance |
| AmmoReservation | battleId, ownerId, lotId, quantity, spent, released, status | Reserve does not duplicate stock |
| DeploymentPreset | id, clubId, purpose, revision |1–16 distinct fighters; countA need not equalB |
| DeploymentMember | presetId, fighterId, position?, ammoClass/allocation | Positions/ammo mixing Q-04/Q-08 |
| WalletBalance | ownerId, currencyCode, balance, revision | separate SOFT / CREDITS; available≥0 |
| WalletEntry | id, ownerId, currency, amount, transactionId, reason, sourceRef | append-only; unique source/component |
| CurrencyExchange | id, ownerId, quoteVersion, creditsDebit, softCredit, status | Atomic two-currency transaction |
| CommerceOrder | id, ownerId, provider, providerOrderId, providerTxnId, realCurrency, amount, sku, status | Verified finalization→one grant |
| CommerceAdjustment | id, orderId, providerEventKey, type, affectedLot/entitlement, status | Refund/reversal dedupe; policy Q-11 |
| TeamSnapshot | id, clubId, createdAt, versions, inputHash, payload | Immutable; payload1–16, HP, MK, armor, BB |
| DefensePointer | clubId, snapshotId, resourcePolicyVersion | Policy Q-06 must be selected before human async |
| PowerAssessment | snapshotId, metricVersion, actualPower, referencePower?, context | Internal derived; not count-only |
| OpponentOffer | id, mode, attackerId, defenderId, snapshotRefs, quoteVersions, expiresAt | Ranked separate pool; no equality field |
| Battle | id, mode, attackerId, defenderId, snapshotA/B, seed, versions, state, acceptedAt | One challenge→one battle, no roundSeries |
| BattleResult | battleId, outcome, finalHP, ammoSpent, eventsHash, input/outputHash | unique battle; no roundWins |
| BattleSettlement | battleId, healthPolicyVersion, rewardVersion, grossMoney, xp, ratingRefs, settledAt | Once; net display not duplicate BB debit |
| PairActivity | attackerId, defenderId, battleId, mode, acceptedAt, status | Cross-mode abuse context; size not reset key |
| Rating / RatingEntry | clubId, ruleset, value, revision; before/delta/after, battleId | Only ranked approved ruleset changes |
| RevengeTicket | id, ownerId, targetId, originBattleId, expiresAt?, usedByBattleId? | Default non-ranked; TTL/chain policy Q-10 |
| Notification / HistoryView | recipientId, battleId, perspective, type, readAt | Show actual resource effects only |
| Idempotency / Job / Outbox | scopedKey, bodyHash, resultRef; lease/state | Durable retry, one authoritative effect |

## Snapshot payload

Owned actor IDs зі stable snapshot-local ordering; current/maxHP; effective stats; weapon definition content і MK; armor/protection modifiers; ammo class+allocated quantity; deployment count/positions; Club Level на acceptance; power assessment/reward context; simulation/ruleset/balance/PRNG/resourcePolicy versions.

BB allocation поле концептуальне до Q-04. Defender resource payload фіксується для конкретного battle, але permanent owner write правила Q-06. Capture не дає клієнтові право завантажувати arbitrary effective stats.

Public opponent card віддає дозволений summary, не wallets, purchase history, Steam tickets або весь reserve inventory. Names/appearance display не mathematical key.

## Lifecycle і concurrency

Battle draft/preview→accepted→resolved→settled; internal failure before settlement→failed+reservation release. Settled не повертається до accepted. Paid transaction flow окремий від battle flow.

Heal/equip/train/MK та battle acceptance використовують revision/locks на тих самих live resources; freeze inputs не розв'язує race записів. Остаточний concurrent-defense policy Q-06 потрібний до implementation. Single-round schema не містить RoundResult або roundScore.

## Migration/status

Попередня v1 модель була тільки документом; реальної DB чи save для міграції немає. Не створювати автоматичний exchange «старих Credits→premiumCredits». Future schema versioning і immutable balance versions окремі. Оригінальний PaintballWars_Research і Project Airsoft schemas не імпортуються.
