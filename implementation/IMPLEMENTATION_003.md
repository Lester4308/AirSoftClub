# Implementation 003 — rewards and wallet policies

Activated by the user's development-pass request, from docs commit 4c79e182a9f5c5f81c1f9346c8b38fc9ecef7d15 on codex/implementation-003-development-pass. Known 24 documentation-only changes were inspected, local links validated and committed separately. No remote configured. SDK10.0.302, Unity6000.3.21f1, Windows SDK10.0.26100.0 detected. Docker29.7.2 became available after starting installed Docker Desktop.

Airsoft.Club is a server-only net10.0 domain assembly referencing the unchanged netstandard2.1 battle core. Unity will use network DTOs; it does not receive wallet authority. Approved outcome/repeat percentages, bounded accepted-snapshot power, participant XP, one-time10 Credits, checked balances, ledger operation identity and one-way conversion are implemented. Atomic persistence is the next backend milestone; Wallet itself is not a concurrent database.

Prototype economy-003-v1: base100 plus10/opponent level after1; power ratio uses prepared stats/HP/protection/weapon damage and BB, clamped0.1–1.5; integer final floor; starter1000 Money; conversion100 Money/Credit. These are configurable hypotheses, not approved final prices. Internal power remains server-only.

Verification: dotnet run --project tests/Airsoft.Club.Tests -c Release:6 named scenarios passed. Existing baseline tools/verify.ps1:46/46,10000 simulations,zero invariant failures,golden22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f. Unity EditMode9/9. No combat source/golden changes. Full native gate will rerun after client integration. All writes remain in this independent repository.
