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
        const uint SpacewarAppId = 480;
        Action<string> onTicket, onError;

        public static bool TryValidateAppId(string configuredValue, uint runningAppId, out string error)
        {
            if (!uint.TryParse(configuredValue, out uint configuredAppId) || configuredAppId == 0 || configuredAppId == uint.MaxValue)
            {
                error = "Steam AppID is not configured.";
                return false;
            }
            if (configuredAppId == SpacewarAppId)
            {
                error = "Steam sample AppID 480 is not accepted for this game.";
                return false;
            }
            if (runningAppId != 0 && runningAppId != configuredAppId)
            {
                error = "Steam AppID mismatch.";
                return false;
            }
            error = null;
            return true;
        }

        public void Begin(Action<string> success, Action<string> failure)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));
            if (failure == null) throw new ArgumentNullException(nameof(failure));

            string configuredAppId = Environment.GetEnvironmentVariable("STEAM_APP_ID");
            if (!TryValidateAppId(configuredAppId, 0, out string configurationError))
            {
                failure(configurationError);
                return;
            }
            if (!initialized)
            {
                initialized = SteamAPI.Init();
                if (!initialized) { failure("Steam client/app entitlement unavailable."); return; }
                if (!TryValidateAppId(configuredAppId, SteamUtils.GetAppID().m_AppId, out configurationError))
                {
                    SteamAPI.Shutdown();
                    initialized = false;
                    failure(configurationError);
                    return;
                }
                callback = Callback<GetTicketForWebApiResponse_t>.Create(Received);
            }
            Cancel();
            onTicket = success;
            onError = failure;
            ticket = SteamUser.GetAuthTicketForWebApi("airsoft-club-server");
            if (ticket == HAuthTicket.Invalid)
            {
                Fail("Steam could not create an authentication ticket.");
            }
        }
        void Received(GetTicketForWebApiResponse_t response)
        {
            if (response.m_hAuthTicket != ticket) return;
            if (response.m_eResult != EResult.k_EResultOK)
            {
                Fail("Steam rejected ticket request.");
                return;
            }
            if (response.m_cubTicket == 0 || response.m_cubTicket > response.m_rgubTicket.Length)
            {
                Fail("Steam returned an invalid authentication ticket.");
                return;
            }
            Action<string> success = onTicket;
            onTicket = null;
            onError = null;
            success?.Invoke(BitConverter.ToString(response.m_rgubTicket, 0, response.m_cubTicket).Replace("-", ""));
        }
        void Fail(string error)
        {
            Action<string> failure = onError;
            Cancel();
            onTicket = null;
            onError = null;
            failure?.Invoke(error);
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
