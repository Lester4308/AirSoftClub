# Milestone 011 — integrated hardening and reproducible Windows package

Date: 2026-09-08. **LOCAL PASS.** External Steam/commerce/hosting and final product approval remain outside this milestone.

## Hardening completed

- Full Release solution build and deterministic core/club harnesses pass with warnings treated as errors.
- PostgreSQL tests exercise migrations, persistent restart, ownership/FK/nonnegative constraints, last-resource races, stale versions, operation-key payload conflicts, rollback, once-only battle settlement, expired work fencing, exposure races, shield races, Steam ticket replay, daily-claim races and commerce reconciliation.
- Modern UGUI uses explicit transaction-busy/error/retry surfaces and explicit Credits confirmation.
- Steam identity and Friends adapters reject unsafe/sample AppIDs, malformed tickets, wrong owners/non-ownership and invalid friend identities; development identities remain isolated.
- Native Windows x64 IL2CPP build and runtime digest regression pass.
- Balance and economy simulations run without invariant/resource failures or automatic premium spending.

## Fresh verification receipt

| Gate | Result |
|---|---|
| Release solution / formatting / purity | PASS; 0 warnings/errors |
| Battle harness | 46/46 |
| Club harness | 37/37 |
| PostgreSQL harness | 27/27 |
| Unity EditMode | 15/15 |
| Unity PlayMode | 6/6 |
| 10,000 deterministic combat simulations | complete; zero invariant failures |
| Premium matrix | 5,400 battles; max declared ratio 1.1902496; 4,213 premium wins; 347 draws |
| Economy loop | 100 accounts / 2,000 battles; minimum Money 0; premium spent 0 |
| Windows Mono build | PASS |
| Windows IL2CPP build | PASS |
| Windows native IL2CPP run | PASS |

Native digest:

`22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f`

Native 100-battle timings (environment observations, not a production rendering benchmark):

- 1v1: 6.399 ms
- 8v8: 21.566 ms
- 16v16: 34.176 ms

## Reproducible package

`Artifacts/AirsoftClub-Development-Windows-x64.zip`

SHA-256:

`0061C6184BAD03665BC185C860A491DE607198357C40D5BCE48DEF4E7BD5CBEF`

The archive contains 488 entries and includes:

- Windows x64 IL2CPP client;
- published ASP.NET server;
- pinned PostgreSQL Compose configuration;
- `Start-Backend.ps1`;
- `Start-Game.ps1`, which opens the modern UGUI client;
- runbook, build manifest and visual/balance evidence.

Verified archive constraints:

- Bundle DB port is 55433; source development DB remains 55470 on this Windows host.
- No `.env`, `local-db-password.txt`, `steam_appid.txt`, publisher key or bearer token is present.
- The bundle generates its local database password at first backend launch.

## Remaining external/product gates

- Real Steam AppID/publisher key and entitled sandbox accounts.
- Real payment provider execution, final SKU/pricing and spent-balance refund policy.
- Production moderator provisioning.
- Hosting, load/security review and production publishing.
- Final art approval, localization/accessibility review and final product balance approval.

These open gates are not represented as local PASS.
