using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
namespace AirsoftClub.Unity
{
    // Explicit opt-in adapter. Never initializes Spacewar or fabricates an application entitlement.
    public sealed class SteamIdentityAdapter : MonoBehaviour
    {
        bool initialized;
        Callback<GetTicketForWebApiResponse_t> callback;
        HAuthTicket ticket;
        Action<string> onTicket, onError;
        public void Begin(Action<string> success, Action<string> failure)
        {
            if (!uint.TryParse(Environment.GetEnvironmentVariable("STEAM_APP_ID"), out uint appId) || appId == 0)
            { failure("Steam sandbox AppID is not configured."); return; }
            if (!initialized)
            {
                initialized = SteamAPI.Init();
                if (!initialized) { failure("Steam client/app entitlement unavailable."); return; }
                if (SteamUtils.GetAppID().m_AppId != appId) { SteamAPI.Shutdown(); initialized = false; failure("Steam AppID mismatch."); return; }
                callback = Callback<GetTicketForWebApiResponse_t>.Create(Received);
            }
            Cancel(); onTicket = success; onError = failure;
            ticket = SteamUser.GetAuthTicketForWebApi("airsoft-club-server");
        }
        void Received(GetTicketForWebApiResponse_t response)
        {
            if (response.m_hAuthTicket != ticket) return;
            if (response.m_eResult != EResult.k_EResultOK) { onError?.Invoke("Steam rejected ticket request."); return; }
            onTicket?.Invoke(BitConverter.ToString(response.m_rgubTicket, 0, response.m_cubTicket).Replace("-", ""));
        }
        public string[] DiscoverFriends()
        {
            if (!initialized) return Array.Empty<string>();
            var list = new List<string>(); int count = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            for (int n = 0; n < count; n++) list.Add(SteamFriends.GetFriendByIndex(n, EFriendFlags.k_EFriendFlagImmediate).m_SteamID.ToString());
            return list.ToArray(); // Discovery only; server independently verifies relationship.
        }
        public void Cancel() { if (initialized && ticket != HAuthTicket.Invalid) SteamUser.CancelAuthTicket(ticket); ticket = HAuthTicket.Invalid; }
        void Update() { if (initialized) SteamAPI.RunCallbacks(); }
        void OnDestroy() { Cancel(); callback?.Dispose(); if (initialized) SteamAPI.Shutdown(); }
    }
}
