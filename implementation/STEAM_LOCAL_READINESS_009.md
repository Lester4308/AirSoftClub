# Milestone 009 — Steam identity / Friends local readiness

Date: 2026-09-08. Scope: local implementation and fixtures only. **LOCAL PASS; LIVE STEAM SANDBOX OPEN.**

## Implemented and verified locally

### Unity client adapter

- Steamworks.NET remains pinned to `2025.164.1` / package-lock commit `c21a8f0...`.
- Steam is explicit opt-in; development login stays separate.
- Missing/zero/invalid AppID and Valve sample AppID `480` are rejected.
- The configured AppID must match the running Steam client AppID before a web API ticket is requested.
- Invalid ticket handles, rejected callbacks, empty/oversized ticket payloads and timeout paths fail explicitly and clean up the ticket/callback state.
- The client does not mark the session as Steam-authenticated until `/steam/login` succeeds.
- Successful Steam login displays a distinct connection status.
- Client friend enumeration is discovery-only. It never authorizes a Friend attack.

### Server authority

- Ticket validation uses `AuthenticateUserTicket`, requires the authenticated SteamID to equal the owner SteamID, rejects publisher-banned identities, and verifies app ownership.
- Server accounts derive from the verified identity as `steam-{SteamID}`.
- Ticket exchange is replay-protected with a stored ticket hash; sessions are random, hashed and expire after one hour.
- Friend attacks from Steam identities call the server-side `GetFriendList` endpoint and require a verified current relationship.
- Returned friend IDs are filtered to valid 17-digit Steam identities; private/empty lists remain empty.
- Development identities cannot cross into Steam-owned profiles or use development tools as Steam owners.
- Publisher credentials remain server-side and absent from Unity assets/source.

## Verification receipt

- Unity EditMode: **15 passed, 0 failed**.
- Unity PlayMode: **3 passed, 0 failed**.
- Windows Mono build: **PASS**.
- PostgreSQL server harness: **27 passed, 0 failed**.
- `git diff --check`: PASS before commit.

## External gate still open

No real `STEAM_APP_ID`, server-only `STEAM_PUBLISHER_KEY`, entitled sandbox accounts or Steam partner configuration are available in this local development environment. Therefore these items are **not claimed**:

- live Steam client ticket issuance;
- live ownership verification;
- live friend-list discovery/authorization;
- production entitlement or release readiness in Steamworks.

To close the external gate later, configure a real non-480 sandbox AppID, keep the publisher key only on the backend, use two entitled test accounts with a verified friendship, and execute the full sign-in → discovery → Friend battle → reconnect path while capturing server logs without URLs/tickets/keys.
