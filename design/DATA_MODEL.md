# Data model v1

Новий logical model; не твердження про schema оригіналу. UUID primary keys, UTC timestamps, integer currency/XP, SteamID64 decimal string у transport. SchemaVersion, balanceVersion і simulationVersion — різні поля.

| Entity | Key / важливі поля | Constraints |
|---|---|---|
| PlayerProfile | id,createdAt,status,settingsVersion | один live profile на linked identity |
| ExternalIdentity | playerId,provider,subject,verifiedAt | unique(provider,subject); nickname не key |
| GameSession | id,playerId,expiresAt,revokedAt,entitlementCheckAt | secret hash, не raw bearer у DB logs |
| ClubProfile | id,playerId,name,emblemId,clubXP,rankedEnabled,discoverable,revision | unique(playerId); public projection окрема |
| Fighter | id,clubId,name,appearanceId,baseA/M/E,xp,purchasedBudget,allocatedA/M/E,revision | total24+budget≤36; коженstat≤20; purchased≤min(12,floorXP/100) |
| ItemDefinition | id,version,slot,family,price,unlockLevel,combatProperties | immutable version; airsoft-only production labels |
| ItemInstance | id,clubId,definitionId/version,paidCredits,grantKind | owned; starter non-sellable |
| EquipmentAssignment | fighterId,slot,itemInstanceId | unique(fighterId,slot),unique(itemInstanceId) |
| SquadPreset | id,clubId,purpose,linkedToAttack,revision | purpose attack/defense; valid size3 |
| SquadMember | presetId,fighterId,anchor | unique(presetId,anchor),unique(presetId,fighterId) |
| Wallet | clubId,credits,revision | credits≥0; mutations ledger-backed |
| WalletEntry | id,transactionId,clubId,amount,reason,matchId?,purchaseId? | append-only; unique source/component |
| DefenseSnapshot | id,clubId,schemaVersion,rulesetVersion,balanceVersion,createdAt,inputHash,payload | immutable; payload owns effective stats/definitions/formation |
| CurrentDefense | clubId,format,snapshotId | one valid pointer per supported format; v1 only3v3 |
| RankedOffer | id,attackerId,defenderId,snapshotIds,expiresAt,status,matchmakingVersion | one live offer/attacker; stateful token validation |
| Match | id,mode,attackerId,defenderId,offerId?,revengeId?,snapshotA/B,seed,versions,arena,state,acceptedAt | frozen inputs after accept; one active attacker |
| MatchResult | matchId,outcome,roundScores,eventBlobHash,outputHash,bbUsage | unique(matchId); immutable |
| Settlement | matchId,settledAt,rewardPolicyVersion,eligibilityOrdinal,ratingVersion | unique(matchId); all effects transactional |
| Rating | clubId,seasonId,value,revision | unique club/season; value≥0 |
| RatingEntry | matchId,clubId,before,delta,after,revision | unique(matchId,clubId) |
| PairReservation | id,attackerId,defenderId,matchId,acceptedAt,state,rewardOrdinal | concurrent counts include reserved |
| RankedPairReservation | unorderedClubPair,matchId,acceptedAt,state | rolling24h validated transactionally, not simple permanent unique(pair) |
| DefenseQuotaReservation | defenderId,matchId,acceptedAt,state | max5incoming in rolling24h; failed releases |
| RevengeTicket | id,ownerId,targetId,originMatchId,createdAt,expiresAt,usedByMatchId? | one redemption; origin non-revenge; pair creation dedupe24h |
| BattleHistoryView | viewerId,matchId,perspective,displayIdentityRef | same match, attacks/defenses views; not duplicate simulation |
| Notification | id,recipientId,type,sourceId,createdAt,readAt | unique(recipient,type,source) |
| IdempotencyRecord | playerId,route,key,bodyHash,responseRef,createdAt | unique(player,route,key); differing body conflict |
| Job / Outbox | id,type,aggregateId,payloadVersion,state,leaseUntil,attempts | durable retry, consumer dedupe |
| Block / ModerationCase | ownerId,targetId,reason,status,createdAt | authorization checked before accept |

## Snapshot payload

Squad-local slots0..2; fighter IDs for provenance; effective A/M/E; training budget; owned equipment definition content at capture; kit modifiers; weapon tables; anchor assignments; format; ruleset/balance versions; captured strength; captured rating for display only. Cosmetic IDs/name optional display metadata, не mathematical input. Public preview не повертає private inventory, wallet, seed або unpublished reserve builds.

Attack snapshot також immutable і може використати ту саму payload schema. Наявність item у двох historical snapshots не означає подвійної live ownership. Club profile агрегує fighters/inventory/economy/history логічно; це не одна велика mutable JSON колонка.

## State transitions

Match accepted→queued→resolved→settled або accepted/queued/resolved→failed при internal error до settlement. Settled ніколи не повертається в accepted. Compensation створює окремий audited record. Snapshot published→superseded не мутує payload. Revenge available→reserved→used; failed resolution повертає available якщо TTL не минув, інакше expired.

## Transaction boundaries

Hire: wallet debit + fighter + baseline gear grant + roster revision. Train: XP/budget validation + wallet debit + allocation + new valid snapshot. Equip: ownership assignments + revision + new snapshot. Accept: pin snapshots + counters/offer/ticket + match/job. Settle: ledger + XP + ratings + history/inbox + quota state. Немає public endpoint «set wallet» або «upload result».

## Read projections і lifecycle

ClubPublicCard: SteamID64 для display lookup,clubId/name/emblem,clubLevel,rating,rankedW/L/D,strength,format,snapshotTimestamp. Last-active і settings приватні. Leaderboard projection rebuildable із canonical Rating, кеш не authority. При deletion/anonymization history показує «Deleted club», input numeric blob лишається без user-facing names за retention policy. Оригінальні account credentials/Steam tickets не є replay material.
