# Implementation 010 — retention and commerce/moderation foundation

UTC daily unique date, 7-day cycle, missed-day reset, controlled first-battle/level grants, early unlock access-only≤3levels, name/emblem/report commands. Daily100+10 per streak day, first battle2Credits, levels2–10 one Credit each, unlock1Credit are versioned development hypotheses. The approved starter10 is unchanged. UI exposes daily/progression and separate early access purchase. Existing MK3/BB/shields/conversion use the same wallet ledger.

Durable order ID ownership/SKU, provider observation and reconciliation are separate transactions. No client-supplied payment callback or live payment endpoint exists. Unknown stays ungranted; Paid survives an injected DB failure, next reconciliation grants exactly once; refunds reverse once only when available, otherwise NeedsReview with no invented debt/suspension. Development SKU is dev-credits-10, no real currency price/transaction. Actual provider sandbox unavailable and NOT RUN.

Minimal UGC validates names and fixed emblem IDs, accepts categorized reports without open chat. Authorized moderation domain supports warning/rename/restriction/suspension with audit; ordinary commands reject a restricted account. No public moderation/admin issuance route or real sanctions were enabled. Production role provisioning remains a release gate.

Verification: domain23/23; PostgreSQL15/15 including payment-observed→DB-fails→fresh reconciliation→one grant/refund, and concurrent UTC claim. Build0warnings/errors. UI compilation and final runtime matrix follow011; no combat/golden changes. Project Airsoft untouched.
