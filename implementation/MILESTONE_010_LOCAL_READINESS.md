# Milestone 010 — Credits, retention, moderation and commerce local readiness

Date: 2026-09-08. **LOCAL PASS; LIVE COMMERCE / MODERATOR PROVISIONING OPEN.**

## Implemented and verified

### Credits and retention

- Exactly 10 starter Credits.
- Credits → Money conversion is explicit, confirmed by the player and cannot be reversed. No Money → Credits path exists.
- Daily claim uses the authoritative UTC day, resets at 00:00 UTC, rejects pre-epoch time without mutation, and resets the streak after a missed full day.
- Seventh-day, achievement and Club Level Credit grants are idempotent.
- Progressive early access depth remains 0/1/2/3 by Club Level. Access and item ownership are separate: the player unlocks access with Credits, then purchases the item separately with Money.
- Exact retries of a completed early-access operation return safely without a second debit. Conflicting payload reuse and ledger/access divergence are rejected.
- Shield purchases use explicit Credits confirmation and show cancellation consequences before purchase.

### Modern UGUI

- Credits purchases, early unlocks, conversion and shields require an explicit confirmation overlay.
- The overlay states the amount, consequence and authoritative refresh behavior.
- Daily UI distinguishes “claimed today”, displays the UTC reset, and does not offer a duplicate claim button.
- Failed/stale server operations expose a retry/refresh action; commands remain disabled while a transaction is pending.
- PlayMode tests cover confirmation staging without server contact, daily/conversion/shield explanations and retryable transaction failures.

### Commerce foundation

- Trusted-provider observation validates owner and SKU.
- Wallet grants/debits/reversals are append-only and idempotent through the immutable ledger.
- Payment-observed / DB-failure reconciliation grants exactly once.
- Refund after spent Credits enters `NeedsReview`; no invented debt, ban or suspension policy is applied.

### Moderation foundation

- Club name validation, emblem selection and report commands are server-authoritative.
- Reports are deduplicated within the configured window and support Name, Emblem and Cheating reasons; there is no open-chat reason or open chat feature.
- Warning, forced rename, temporary restriction and suspension actions are implemented and auditable.
- Unauthorized sanctions are rejected without audit mutation.
- Production moderator role provisioning remains intentionally disabled until a real authorization source exists.

## Verification receipt

- Release build: 0 warnings, 0 errors.
- Battle harness: **46/46**.
- Club harness: **37/37**.
- 10,000 deterministic simulation battles: zero invariant failures.
- PostgreSQL server harness: **27/27**.
- Unity EditMode: **15/15**.
- Unity PlayMode: **6/6**.
- Windows Mono build: **PASS**.

## External gates

- Real Steam commerce/payment provider credentials and sandbox execution.
- Final production SKU/pricing table and commercial balance approval.
- Spent-balance refund/debt policy.
- Production moderator role provisioning and operational review process.

No live commerce or production moderation claim is made by this report.
