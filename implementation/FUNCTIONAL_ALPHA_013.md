# Functional Alpha — verification013

2026-09-06. **FUNCTIONAL ALPHA: PASS for the local functional acceptance checklist.** This is not release readiness or balance approval. The full paid-access balance audit is **outside the intended ceiling at early levels**; see Economy below. Steam sandbox, final art and manual Windows input QA remain separate.

## Scope and starting point

Only C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game. Start: codex/implementation-003-development-pass at5dfad5c7195cf2d06b078e5a6dc549a29d8a9170; working tree clean. Canonical master/pack were found at repository root, not the attachment's outputs/ paths; existing root documents remain canonical. Q01–Q15 and the approved Revenge clarification were preserved. No new product questions.

## Playable and systems completed

Launch/create or reconnect to a stable development identity; choose one of3 free candidates; hire from7-offer pools; manage up to16 fighters; train Accuracy/Endurance/Agility with Money after XP opens caps; buy/equip/unequip all4 slots; buy/select shared BB; heal fully or up to10HP; wait for free offline recovery; select Practice/Ranked/local Friend; run an authoritative battle; view gross rewards,XP,rating and attack/defense history; use Revenge and shields; reconnect and repeat.

| Acceptance system | Evidence |
|---|---|
| Club/profile/progression |Versioned server projection includes wallet,XP,level,rating,inventory,roster,emblem,streak,BB,timers,defense version,history,tickets,shield |
| Recruitment |Permanent free entitlement,3 choices,7 varied offers,level quality,1h OR10 completed refresh,paid refresh,both anchors reset,original-price resale and returned gear |
| Training/recovery |Three stats,XP caps,soft training; old-rate recovery before Endurance change; full/partial soft heal,offline rational recovery; defense isolated full HP |
| Inventory/shop |Ownership and slot validation,unique assignment,level/access checks,6 weapon families,MK1/2/3,armor tradeoffs,explicit prices; no new item resale policy |
| BB |5 named tiers,capacity growth,actual attacker spend,virtual defense budget,60 starter Basic,emergency and opt-in auto Basic |
| Economy/retention |Append-only wallet ledger; conversion one-way;starter10Credits;daily UTC/streak,first-battle and bounded level faucets |
| Rating/defense/Revenge/shields |Existing approved policy plus auditable actual deltas;immutable accepted inputs and final history;cap/race tests;no Revenge chains |
| Persistence/reconnect |Actual PostgreSQL tests,HTTP reconnect and a separate native client process restoring state |
| Unity/native |Functional shell and full native IL2CPP pipeline/run; unchanged deterministic golden |
| Double spend/reward |Versioned serialized transactions,idempotency hashes,once-only settlement,DB ledger/history protections and race tests |

## Changes in this pass

- Centralized prototype lifecycle/PvP/retention/MK/BB configuration in AlphaConfig (alpha-013-v1). Catalog version catalog-013-v1; weapon family unlocks progress through levels1–6, instead of all at1. Existing owned equipment is preserved. MK costs/damage modifiers and battle formulas unchanged.
- Seventh UTC consecutive claim grants1Credit; normal days grant Money100+10×(streak−1). This is a versioned prototype faucet, not final pacing. No repeat same-day grant.
- Added partial healing. Whole-HP requested amount is capped to missing HP; price rounds upward at1Money/HP; partial recovery remainder retained,full cap clears remainder. No Credit heal.
- Replaced constant opponent label with relative power categories. Internal approximation accounts for HP,protection,evasion,Accuracy,damage,projectiles,tempo,BB tier and bounded ammunition. Numeric power remains server-only. Reward config version economy-013-v1; accepted reward inputs remain captured.
- New forward migration20260906162957_FunctionalAlphaAudit: accepted attacker/defender versions,catalog version,settlement receipt and ledger timestamps. Final match update/delete and accepted combat-input mutation rejected by PostgreSQL trigger. Old matches retain empty/unknown new audit fields; no invented historical deltas or destructive backfill.
- Actual settlement stores both rating deltas,reward amounts,UTC time and schema1 alongside existing complete wire input/result(seed,rules,BB,HP/events). History distinguishes offense/defense. Current defense source version is exposed.
- Added identity/session,platform social/profile and commerce ports. Concrete Steam auth/friends adapter remains; profile lookup is an interface foundation, not a verified avatar service. Development commerce provider is fixture-only,not registered in production.
- Added16KB request bound and240requests/minute/IP limiter. Development and Steam opponent scopes cannot cross. Null command fields rejected. API errors distinguish insufficient Money/Credits;client handles rate/network/session errors.
- DEV endpoints registered only when Development AND AIRSOFT_DEV_AUTH=1; require authenticated dev owner and loopback. Fixed test grants,recovery simulation,seeded refresh,opponent preparation,test Revenge and ledger inspection. No arbitrary wallet/timestamp mutation endpoint. UI reset creates a NEW dev identity; old profile/free entitlement/history remain intact.

## UI and visual evidence

Main navigation: Club,Roster,Recruitment,Supply,Opponents,History,Status. Training/healing/gear are in Roster;shop/BB inSupply. Status exposes profile/emblem,defense,shield,leaderboard and visibly marked DEV fixtures. All art remains neutral IMGUI/HP bars and numbered emblem placeholders; no final fighter/map/art direction chosen.

Server prices,healing quotes,recovery countdowns,refresh timing,streak and auto-buy state now reach UI. Early-access entitlement shows Available; BB refill labels show the actual capped amount. History/result show gross reward and signed rating delta; legacy unknown delta is labeled. Defensive replay header now says ATTACK/DEFENSE instead of incorrectly labeling the attacker YOUR CLUB.

Mono visible walkthrough screenshots reviewed:Club,recruitment,supply,roster,result,Revenge,history,status,relaunch. Native repeated these9 captures; native result/status/relaunch also inspected. No overlap/readability defect observed at1280×800 in these screens. [Saved native screenshots](evidence/013/club-club.png), [recruitment](evidence/013/club-recruitment.png), [supply](evidence/013/club-supply.png), [roster](evidence/013/club-roster.png), [result](evidence/013/club-result.png), [Revenge](evidence/013/club-revenge.png), [history](evidence/013/club-history.png), [status](evidence/013/club-status.png), [relaunch](evidence/013/club-relaunch.png).

Both visible automated runs exited0; a separate player process reconnected and compared identity/version/Money/Credits/BB/history/shield/tickets to saved prior state. Native example includes an eliminated fighter remaining not-ready on relaunch,without a free heal. These are real visible Unity renders and automated actions,not manual mouse QA. Existing Windows capture0x80004002 was not re-engineered or claimed fixed. Keyboard/accessibility,other resolutions and manual16v16 layout remain unverified; per the user these do not block Functional Alpha.

## Backend and local PvP

ASP.NET Core/.NET10,EF Core10.0.8,Npgsql10.0.2,PostgreSQL18.6. Existing versions preserved. Modular monolith;global advisory transaction lock remains a bounded-alpha throughput choice. Production partitioning/load work remains future scope.

Routes: /dev/login,/steam/login,/api/club,/api/command,/api/opponents,/api/leaderboard,/api/match/{id},/api/dev/command,/api/dev/ledger,/health,/ready. Battle worker resolves captured inputs outside transaction then settles atomically. Incoming attacks cannot consume live defensive HP/BB. Shield/start ordering and pending offense versus heal/equip are tested.

Local Friend means the policy test identity layer,not proof of Steam friendship. Steam Friend commands still require server-verified friend list. Ranked candidate pool is rating-nearby3–5 when available. Revenge:24h/3attempts,draw consumes,expiry after accepted start valid,frozen rating eligibility,floor120%actual origin loss,no target debit,shared4incoming cap,non-rated fallback without recovery,win closes,no chains,standard rewards. No approved rule reopened.

## Economy and simulations

Prototype values:1000starterMoney,10Credits,60BasicBB;training25Money;recruit price5×sum(stats),resale30%;refresh50Money;MK1=100Money,MK2=600Money,MK3=6Credits;BB500 refill for50/70/90/110Money or1Credit Premium;conversion1Credit→100Money. Shield8/24/72/168h costs1/2/4/7Credits. Fighter level per100XP;Club level per1000XP;capacity1000+500/level capped1million. Configurable here means versioned server code/config,not a hot-reload admin editor.

Combat10000:4913attacker wins,5001defender wins,86draws;0 invariant failures,100%termination,mean6089.70ms simulation duration. BB mean81.05/81.30.

[Economy evidence](evidence/013/economy.json):100accounts×20=2000battles;Money rewards106240,healing170430,refills5000,free recovery wait15300minutes total,minimumMoney0,premium spent0. Continued free play exists; waiting/pacing is not final. Starter1000seed estimate remains9.526BB/battle,60BB≈6.30battles.

[Matched-build premium audit](evidence/013/balance.json):5400paired simulations,maximum expected DPS×effectiveHP ratio1.1902496,4213premium wins(78.02%),347draws. This metric passing20% does not establish win-rate fairness.

**New full-access audit FAILS the intended early-level ceiling:** [early-access evidence](evidence/013/early-access-013.json). At equal base stats,best currently accessible normal build versus paid build including+3-level early unlock,armor andBB gives:

| Club level | Paid/normal DPS×relative effectiveHP |
|---|---|
|1 |2.4449025 |
|2 |1.2167894 |
|3–6 |1.0608344 |

Level1 compares Pistol-MK2/armor1/High-End BB to early-access AssaultRifle-MK2/armor3/PremiumBB. This is a sustained analytical metric,not a simulated win rate. It exposes catalog progression/access stacking missed by same-build comparison. No final balancing,monetization-ceiling redefinition or release approval was performed. This is an explicit balance/release risk; it is not hidden behind Functional Alpha PASS. User directed functional development to continue despite balance tuning remaining open.

## Verification results

| Gate | Fresh result |
|---|---|
| Release solution |0 compiler warnings/errors |
| Combat suite |46/46 |
| Domain suite |32/32 |
| Actual PostgreSQL suite |25/25 |
| EditMode |9/9 |
| PlayMode |1/1 |
| Mass combat |10000completed,0invariant failures |
| Mono full build/run |PASS,exit0;300smokes,100each1v1/8v8/16v16 |
| Native IL2CPP full build/run |PASS,exit0;300smokes,100each1v1/8v8/16v16 |
| Extended HTTP/reconnect |PASS,version27/history2/18ledger entries in final fixture |
| Visible Mono/native + separate process reconnect |PASS |
| Production security fixture |PASS |
| Core formulas/RNG/wire diff |unchanged |

Golden all deterministic hosts:
`22446b64986d5a77c42fe827552cd1c28ceb31aedd6583f03cea9081bd6e2f9f`.
SplitMix64,fixed arithmetic,serialization roundtrip,termination/invariants and16v16 remain included. Unity6000.3.21f1,.NET Standard2.1,Windows x64 IL2CPP,SDK10.0.26100.0;native C++ compile/link completed,not just conversion. Native100-battle timings1v1=6.154ms,8v8=20.546ms,16v16=31.936ms.

New database coverage includes refresh/purchase races,stale offers,one-version purchase collision,pending battle/heal/equip,shield versus incoming acceptance,offline recovery projection,actual immutable history trigger,receipt deltas,dev/Steam scope isolation and fixture commerce retry. Existing idempotent Revenge/expiry and4th-slot races remain green. Production rejects dev login with flag1,requires auth on protected paths,and returns429 under excess traffic. This is foundation testing,not a production penetration/load certification.

Commands:tools/verify.ps1;tools/verify-server.ps1;tools/verify-alpha-http.ps1;tools/verify-alpha-security.ps1;tools/verify-unity.ps1 forEditMode,PlayMode,MonoBuild,MonoRun,IL2CPPBuild,IL2CPPRun;tools/verify-ui.ps1 -Backend Mono/IL2CPP -Visible;domain runner --balance,--economy,--early-access. All scripts use the repository root.

Warnings:initial new migration failed on malformed SQL dollar delimiter;transaction rolled back,delimiter corrected,and forward migration plus25real DB tests passed. No reset/drop needed. Nonblocking Unity licensing-token and development shutdown allocator diagnostics persist. LF/CRLF notices are handled by the repository whitespace check;no compiler warnings/errors.

## Steam, blockers and final boundary

Steam auth/ownership/friends adapters and session replay guard are code-ready foundations. Identity/profile/commerce interfaces and dev/test reconciliation exist. Actual sandbox auth,friends,avatars and commerce are NOT verified. Requires real AppID,entitled test accounts,partner key/configuration and commerce sandbox setup. No sampleAppID480 substituted;no real charge,public deployment orproduction setup.

No true blocker remains for the stated local Functional Alpha acceptance. Production gates remain:Steam/commerce external setup,full provider transport,spent-credit refund policy before live commerce,moderator provisioning,production load/security,manual/accessibility QA and final art/balance. No new product answer is requested for this pass. Premium early-access ceiling must be resolved before monetization/balance approval.

Commits:138f0c6 authority/config/audit;b1bfc8d Unity flows,reconnect/security;6aa37f4 paid-access audit. Documentation/evidence/package checkpoint follows separately. Final HEAD and clean working-tree status are returned in the task response.

Project Airsoft / Airsoft Manager **untouched**. No code,assets,data,architecture orGit history imported. All modifications are in the independent repository.

## Development package

Artifacts/AirsoftClub-Development-Windows-x64.zip contains the native Client, published Windows .NET Server, Compose, launcher, runbook, this report and evidence. Existing framework-dependent runtime requirements remain Windows x64/.NET10 and Docker Desktop. The published bundle backend applied the forward migration to its separate PostgreSQL database on55433, started successfully, and passed the extended HTTP loop/reconnect. It was then stopped for packaging. Source DB remains on55432; both volumes and passwords were preserved.

Extract/open Artifacts/DevelopmentBundle, run Start-Backend.ps1, then Client/AirsoftClubIntegration.exe. Steam is unnecessary for the development identity loop. No backend/game process is intentionally left running after this pass. Local PostgreSQL containers remain available. The archive allow-list excludes generated local passwords and .env files. Archive hash is delivered separately to avoid a self-referential hash inside the packaged report.
