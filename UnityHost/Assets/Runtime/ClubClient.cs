using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Airsoft.Battle;
using UnityEngine;
using UnityEngine.Networking;

namespace AirsoftClub.Unity
{
    [Serializable] public class ClubView
    {
        public string Id, Name, PendingMatch, CatalogVersion, ConfigVersion;
        public int ContractVersion, TrainingMoney, ConvertRate, RefreshMoney;
        public long RefreshAvailableAt, EmergencyAvailableAt, DefenseVersion;
        public bool DefensePublished;
        public BbView[] BbCatalog;
        public ShieldView[] Shields;
        public long Version, Money, Credits, Xp, ShieldUntil, ServerNow, OffersAt;
        public int Level, Rating, Capacity, ActiveBbTier, OfferVersion, CompletedSinceRefresh, Streak, Emblem;
        public long LastDay;
        public bool FreeRecruitClaimed, AutoBuyBasic;
        public int[] BbStock;
        public FighterView[] Fighters;
        public OfferView[] Offers;
        public ItemView[] Items;
        public CatalogView[] Catalog;
        public HistoryView[] History;
        public RevengeView[] RevengeTickets;
    }
    [Serializable] public class BbView { public int Tier, Money, Credits, Amount; public string Name; }
    [Serializable] public class ShieldView { public int Hours, Credits; }
    [Serializable] public class ErrorView { public string Error; }
    [Serializable] public class LeaderView { public string Id, Name; public int Rating; }
    [Serializable] public class LeadersView { public LeaderView[] Leaders; }
    [Serializable] public class FighterView { public string Id, Name, AppearanceId; public int Accuracy, Endurance, Agility, Level, TrainingCap; public long Hp, MaxHp, Xp, HealMoney, RecoveryRemainingMs; public bool Ready; public EquipmentView[] Equipment; }
    [Serializable] public class EquipmentView { public string Slot, Item, Definition; }
    [Serializable] public class OfferView { public string Id, Name, AppearanceId; public int Accuracy, Endurance, Agility; public long Price; }
    [Serializable] public class ItemView { public string Id, Definition, Slot; public bool Equipped; }
    [Serializable] public class CatalogView { public string Id, Slot; public long Money, Credits; public int Mk, Level, Damage, Interval, Projectiles, Protection, AgilityPenalty; public bool Access, EarlyAllowed, EarlyCapped; public int EarlyPrice; }
    [Serializable] public class RevengeView { public string Origin, Target; public int ActualLoss, Attempts; public long Expires; }
    [Serializable] public class HistoryView { public string Id, Status, Mode, Attacker, Defender, Outcome; public long AcceptedAt, Money; public int RatingDelta; public bool RatingKnown; }
    [Serializable] public class RivalView { public long PreviewWinMoney, PreviewWinClubXp; public string Id, Name, Category; public int Level, Rating, Fighters; public bool Protected; }
    [Serializable] public class RivalsView { public RivalView[] Opponents; }
    [Serializable] public class LoginView { public string Token; }
    [Serializable] public class SteamLoginRequest { public string Ticket; }
    [Serializable] public class LoginRequest { public string Account; }
    [Serializable] public class CommandRequest
    {
        public string Key, Type, Target = "", Value = "", Ticket = "", CatalogVersion = "";
        public long Version;
        public int Number, OfferVersion;
        public bool Flag;
    }
    [Serializable] public class AppearanceView { public string Id, Side, AppearanceId; public bool Head, Rig, Camo; }
    [Serializable] public class MatchView { public AppearanceView[] Appearance; public string Id, MatchId, Status, Input, Result, Digest; public long RewardMoney, RewardClubXp, RewardFighterXp; public int RatingDelta; public bool RatingKnown; }
    public sealed partial class ClubClient : MonoBehaviour
    {
        const string Endpoint = "http://127.0.0.1:5080";
        string account, token = "", page = "Club", status = "Connect to your local development server.", selectedFighter = "", lastPayload, selectedTarget = "";
        string mode = "Practice", rewardSummary = "", profileName = "", lastEndpoint = "/api/command";
        bool steamSession, showPremium;
        internal bool useModern; // when true, ClubClient renders via the UGUI/Canvas controller instead of legacy OnGUI
        public ModernUiController Modern; // UGUI/Canvas presentation controller (null when legacy)
        public void SetModernForTest(bool on) { useModern = on; if (on && Modern == null) ControllerBoot.Create(this); }
        AppearanceView[] appearance = Array.Empty<AppearanceView>();
        LeaderView[] leaders = Array.Empty<LeaderView>();
        ClubView club;
        RivalView[] rivals = Array.Empty<RivalView>();
        bool busy, smoke, failed, smokeFailure;
        Vector2 scroll;
        MatchView savedMatch;
        MatchConfig replayInput;
        MatchResult replayResult;
        float replayTime;
        int eventIndex;
        readonly Dictionary<string, long> hp = new Dictionary<string, long>();
        GUIStyle title, label, small, button;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Contains("--airsoft-smoke") || Application.isBatchMode && !args.Contains("--club-ui-smoke")) return;
            var cameraObject = new GameObject("Club presentation camera"); DontDestroyOnLoad(cameraObject);
            var camera = cameraObject.AddComponent<Camera>(); camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.055f, .085f, .11f);
            var go = new GameObject("Club development client"); DontDestroyOnLoad(go);
            var client = go.AddComponent<ClubClient>();
            // New UGUI/Canvas presentation is opt-in via command line so the legacy OnGUI
            // client stays as a fallback. Boot members are static; ModernUiController reads
            // the flag via the same args through a static hook.
            client.useModern = true; // Modern UGUI is the primary UI; legacy IMGUI retained only for smoke tests
            ControllerBoot.Create(client);
        }
        void Start()
        {
            Application.targetFrameRate = 60; Application.runInBackground = true;
            smoke = Environment.GetCommandLineArgs().Contains("--club-ui-smoke");
            account = PlayerPrefs.GetString("club-development-account", "dev-" + Guid.NewGuid().ToString("N").Substring(0, 12));
            if (Environment.GetCommandLineArgs().Contains("--club-reconnect-smoke")) { smoke = true; StartCoroutine(ReconnectWalkthrough()); return; }
            if (Environment.GetCommandLineArgs().Contains("--bb-visual")) { smoke = true; StartCoroutine(BbWalkthrough()); return; }
            if (Environment.GetCommandLineArgs().Contains("--visual-matrix")) { smoke = true; StartCoroutine(VisualWalkthrough()); return; }
            if (smoke) { account = "dev-smoke-" + Guid.NewGuid().ToString("N").Substring(0, 12); StartCoroutine(Walkthrough()); }
        }
        IEnumerator Request(string path, string payload, Action<string> success)
        {
            using (var request = new UnityWebRequest(Endpoint + path, payload == null ? "GET" : "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer(); request.timeout = 15;
                if (payload != null) { request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload)); request.SetRequestHeader("Content-Type", "application/json"); }
                if (token.Length > 0) request.SetRequestHeader("Authorization", "Bearer " + token);
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    failed = true; if (smoke) smokeFailure = true;
                    status = request.responseCode == 401 ? "Session expired. Reconnect to continue." : request.responseCode == 429 ? "Too many requests. Wait a minute, then retry the same operation." : request.downloadHandler.text.StartsWith("{") ? JsonUtility.FromJson<ErrorView>(request.downloadHandler.text).Error : "Server unavailable. Start the local backend, then retry.";
                    Debug.Log("CLUB_REQUEST_FAILED code=" + request.responseCode);
                    if (request.responseCode == 409 || request.responseCode == 400) lastPayload = null;
                }
                else { failed = false; success(request.downloadHandler.text); }
            }
        }
        IEnumerator Login()
        {
            if (busy) yield break;
            busy = true; steamSession = false;
            yield return Request("/dev/login", JsonUtility.ToJson(new LoginRequest { Account = account }), text => token = JsonUtility.FromJson<LoginView>(text).Token);
            if (!failed) { PlayerPrefs.SetString("club-development-account", account); PlayerPrefs.Save(); yield return Refresh(); if (!failed) status = "Connected. Server state restored."; }
            busy = false;
        }
        IEnumerator SteamLogin()
        {
            if (busy) yield break;
            busy = true;
            steamSession = false;
            string ticket = null; bool finished = false;
            var adapter = GetComponent<SteamIdentityAdapter>() ?? gameObject.AddComponent<SteamIdentityAdapter>();
            adapter.Begin(value => { ticket = value; finished = true; }, error => { status = error; finished = true; });
            float deadline = Time.realtimeSinceStartup + 15;
            while (!finished && Time.realtimeSinceStartup < deadline) yield return null;
            if (ticket != null)
            {
                yield return Request("/steam/login", JsonUtility.ToJson(new SteamLoginRequest { Ticket = ticket }), text => token = JsonUtility.FromJson<LoginView>(text).Token);
                if (!failed)
                {
                    steamSession = true;
                    yield return Refresh();
                    if (!failed) status = "Connected through Steam.";
                }
            }
            else if (!finished) status = "Steam callback timed out; request a fresh ticket.";
            adapter.Cancel(); busy = false;
        }
        IEnumerator Refresh()
        {
            yield return Request("/api/club", null, text => { club = JsonUtility.FromJson<ClubView>(text); if (profileName.Length == 0) profileName = club.Name; if (!club.Fighters.Any(f => f.Id == selectedFighter) && club.Fighters.Length > 0) selectedFighter = club.Fighters[0].Id; });
            if (!failed) yield return Request("/api/opponents", null, text => rivals = JsonUtility.FromJson<RivalsView>(text).Opponents);
        }
        CommandRequest Intent(string type, string target = "", string value = "", int number = 0, bool flag = false) => new CommandRequest
        { Key = Guid.NewGuid().ToString("N"), Version = club.Version, Type = type, Target = target, Value = value, Number = number, Flag = flag, OfferVersion = club.OfferVersion, CatalogVersion = club.CatalogVersion };
        IEnumerator Send(CommandRequest command, bool development = false)
        {
            if (busy) yield break;
            busy = true; lastPayload = JsonUtility.ToJson(command); lastEndpoint = development ? "/api/dev/command" : "/api/command";
            string match = "";
            yield return Request(lastEndpoint, lastPayload, text => { if (command.Type == "Attack") match = JsonUtility.FromJson<MatchView>(text).MatchId; status = "Saved by server."; lastPayload = null; });
            if (!failed) yield return Refresh();
            if (match.Length > 0) yield return LoadMatch(match);
            busy = false;
        }
        IEnumerator Retry()
        {
            if (busy) yield break;
            busy = true;
            if (lastPayload != null) yield return Request(lastEndpoint, lastPayload, text => { lastPayload = null; status = "Recovered server receipt."; });
            yield return Refresh();
            if (!failed && club.PendingMatch != null && club.PendingMatch.Length > 0) yield return LoadMatch(club.PendingMatch);
            busy = false;
        }
        IEnumerator LoadMatch(string id)
        {
            bool ownsBusy = !busy;
            if (ownsBusy) busy = true;
            MatchView view = null;
            for (int n = 0; n < 40; n++)
            {
                yield return Request("/api/match/" + id, null, text => view = JsonUtility.FromJson<MatchView>(text));
                if (failed || view.Status != "Pending") break;
                status = "Battle accepted. Waiting for authoritative settlement..."; yield return new WaitForSecondsRealtime(0.5f);
            }
            if (view != null && view.Status == "Completed")
            {
                rewardSummary = $"Money +{view.RewardMoney} • Club XP +{view.RewardClubXp} • Each participating fighter XP +{view.RewardFighterXp} • Rating {(view.RatingKnown ? view.RatingDelta.ToString("+0;-0;0") : "historical unknown")}";
                savedMatch = view;
                appearance = view.Appearance ?? Array.Empty<AppearanceView>();
                replayInput = BattleWire.ReadConfig(Convert.FromBase64String(view.Input)); replayResult = BattleWire.ReadResult(Convert.FromBase64String(view.Result));
                hp.Clear(); foreach (var f in replayInput.Attacker.Fighters) hp["A" + f.Id] = f.StartingHp.Raw;
                foreach (var f in replayInput.Defender.Fighters) hp["D" + f.Id] = f.StartingHp.Raw;
                replayTime = 0; eventIndex = 0; page = "Battle"; status = "Server result saved. Playback speed and skip only affect presentation.";
            }
            else if (view != null) status = "Match " + view.Status + ". Use refresh to recover pending work.";
            yield return Refresh();
            if (ownsBusy) busy = false;
        }
        void Update()
        {
            HandleModalEscape();
            if (club == null && Input.GetKeyDown(KeyCode.Return) && !busy)
            {
                StartCoroutine(Login());
                return;
            }
            if (page != "Battle" || replayResult == null) return;
            int duration = replayResult.SimulatedDurationMs;
            if (replayTime < duration)
                replayTime = Math.Min(duration, replayTime + Time.unscaledDeltaTime * 1000);
            while (eventIndex < replayResult.Events.Count && replayResult.Events[eventIndex].TimeMs <= replayTime)
            {
                int time = replayResult.Events[eventIndex].TimeMs;
                var damage = new Dictionary<string, long>();
                while (eventIndex < replayResult.Events.Count && replayResult.Events[eventIndex].TimeMs == time)
                {
                    var e = replayResult.Events[eventIndex++]; string key = (e.ActorSide == Side.Attacker ? "D" : "A") + e.TargetId;
                    if (!damage.ContainsKey(key)) damage[key] = 0; damage[key] += e.Damage.Raw;
                }
                foreach (var d in damage) hp[d.Key] = Math.Max(0, hp[d.Key] - d.Value);
            }
        }
        bool Btn(string text, params GUILayoutOption[] options) => GUILayout.Button(text, button, options);
        void Text(string value) => GUILayout.Label(value, label);
        IEnumerator LoadLeaders()
        {
            if (busy) yield break;
            busy = true;
            yield return Request("/api/leaderboard", null, text => leaders = JsonUtility.FromJson<LeadersView>(text).Leaders);
            busy = false;
        }
        void StatusPage()
        {
            Text("Server saved version " + club.Version + " • " + club.ConfigVersion);
            Text(club.DefensePublished ? "Defense published at version " + club.DefenseVersion + " • full HP / virtual BB" : "Recruit a fighter to publish defense.");
            Text(club.ShieldUntil > club.ServerNow ? "Shield active: " + Math.Ceiling((club.ShieldUntil - club.ServerNow) / 60000d) + " minutes left" : "No active shield");
            Text("Club name"); profileName = GUILayout.TextField(profileName, 24, GUILayout.Height(30));
            if (Btn("Save club name")) StartCoroutine(Send(Intent("Name", value: profileName)));
            GUILayout.BeginHorizontal(); for (int n = 0; n < 8; n++) if (Btn("Emblem " + n)) StartCoroutine(Send(Intent("Emblem", number: n))); GUILayout.EndHorizontal();
            Text("Current emblem: " + club.Emblem + " • neutral numbered placeholders.");
            if (Btn("Load leaderboard")) StartCoroutine(LoadLeaders());
            foreach (var leader in leaders) Text(leader.Name + " • Rating " + leader.Rating);
            if (!steamSession && club.Id.StartsWith("dev-"))
            {
                Text("DEVELOPMENT FIXTURES — not real currency or Steam. Reset means a NEW profile; existing profile and entitlements are preserved.");
                GUILayout.BeginHorizontal();
                if (Btn("Test Money / Credits")) StartCoroutine(Send(Intent("Grant"), true));
                if (Btn("Simulate full recovery")) StartCoroutine(Send(Intent("Recovery"), true));
                if (Btn("Seed fresh recruits")) StartCoroutine(Send(Intent("Refresh"), true));
                GUILayout.EndHorizontal();
                if (rivals.Length > 0 && Btn("Create test Revenge ticket")) StartCoroutine(Send(Intent("Revenge", rivals[0].Id), true));
                if (Btn("Start a NEW development profile"))
                {
                    account = "dev-" + Guid.NewGuid().ToString("N").Substring(0, 12); club = null; token = ""; lastPayload = null; selectedTarget = ""; page = "Club"; leaders = Array.Empty<LeaderView>();
                }
                Text("Ledger inspection is available through the authenticated local development tool.");
                if (Btn("Visual layer / MK preview")) page = "ArtSheet";
            }
        }
        void Capture(string path)
        {
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG()); Destroy(texture);
            Debug.Log("CLUB_CAPTURE " + path);
        }
        string EvidencePath(string name) => System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, name);
        IEnumerator CapturePage(string screen, string name)
        {
            page = screen; scroll = Vector2.zero; yield return new WaitForSecondsRealtime(.22f); yield return new WaitForEndOfFrame(); Capture(EvidencePath("club-" + name + ".png"));
        }
        IEnumerator VisualWalkthrough()
        {
            foreach (int size in new[] { 1, 4, 8, 16 })
            {
                account = "dev-visual-d-" + Guid.NewGuid().ToString("N").Substring(0, 10); token = ""; club = null;
                yield return Login(); if (failed) { Application.Quit(1); yield break; }
                yield return Send(Intent("Squad", number: size), true);
                string defender = club.Id;
                account = "dev-visual-a-" + Guid.NewGuid().ToString("N").Substring(0, 10); token = ""; club = null;
                yield return Login(); if (failed) { Application.Quit(1); yield break; }
                yield return Send(Intent("Squad", number: size), true);
                if (size == 1)
                {
                    yield return CapturePage("Club", "visual-club");
                    yield return CapturePage("Recruitment", "visual-recruitment");
                    yield return CapturePage("Roster", "visual-roster");
                    yield return CapturePage("Shop", "visual-shop");
                    yield return CapturePage("Equipment", "visual-equipment");
                    yield return CapturePage("ArtSheet", "visual-art-sheet");
                    yield return CapturePage("BB", "visual-ammunition");
                    mode = "Ranked"; yield return CapturePage("Opponents", "visual-ranked");
                }
                if (size == 8) yield return CapturePage("Roster", "visual-roster-squad");
                yield return Send(Intent("Attack", defender, "Practice"));
                if (failed || replayInput.Attacker.Fighters.Count != size || replayInput.Defender.Fighters.Count != size) { Debug.LogError("VISUAL_MATRIX_FAILED"); Application.Quit(1); yield break; }
                replayTime = replayResult.Events.Count > 0 ? replayResult.Events[0].TimeMs + 25 : 0;
                yield return null; yield return new WaitForEndOfFrame(); Capture(EvidencePath("club-battle-" + size + "v" + size + ".png"));
                replayTime = replayResult.SimulatedDurationMs + 1;
                yield return null; yield return CapturePage("Battle", "visual-result-" + size);
                yield return CapturePage("Recovery", "visual-recovery-" + size);
                Debug.Log("VISUAL_MATRIX size=" + size + " status=" + replayResult.Status + " digest=" + BattleWire.Digest(replayResult));
            }
            Debug.Log(smokeFailure ? "VISUAL_MATRIX_FAILED" : "VISUAL_MATRIX_PASSED");
            Application.Quit(smokeFailure ? 1 : 0);
        }
        IEnumerator ReconnectWalkthrough()
        {
            var prior = JsonUtility.FromJson<ClubView>(System.IO.File.ReadAllText(EvidencePath("club-smoke-state.json")));
            yield return Login();
            if (failed || club.Id != prior.Id || club.Version != prior.Version || club.Money != prior.Money || club.Credits != prior.Credits || club.History.Length != prior.History.Length || club.ShieldUntil != prior.ShieldUntil || club.RevengeTickets.Length != prior.RevengeTickets.Length || !club.BbStock.SequenceEqual(prior.BbStock))
            { Debug.LogError("CLUB_RECONNECT_FAILED"); Application.Quit(1); yield break; }
            yield return CapturePage("Club", "relaunch");
            Debug.Log("CLUB_RECONNECT_PASSED"); Application.Quit(0);
        }
        IEnumerator Walkthrough()
        {
            yield return Login(); if (failed) { Application.Quit(1); yield break; }
            yield return CapturePage("Club", "club");
            yield return CapturePage("Recruitment", "recruitment");
            yield return Send(Intent("Hire", club.Offers[0].Id, flag: true));
            yield return CapturePage("Supply", "supply");
            yield return Send(Intent("Buy", "Pistol-MK1"));
            yield return Send(Intent("Equip", club.Fighters[0].Id, club.Items[0].Id));
            yield return Send(Intent("Train", club.Fighters[0].Id, "Accuracy"));
            yield return Send(Intent("Heal", club.Fighters[0].Id));
            yield return Send(Intent("Refill", number: 0));
            page = "Roster"; yield return new WaitForSecondsRealtime(.22f); yield return new WaitForEndOfFrame();
            Capture(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, "club-roster.png"));
            yield return Send(Intent("Attack", rivals[0].Id, "Practice"));
            replayTime = 200000; yield return null; yield return new WaitForEndOfFrame();
            Capture(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, "club-result.png"));
            yield return Send(Intent("Shield", number: 8));
            yield return Send(Intent("Revenge", rivals[0].Id), true);
            yield return CapturePage("Opponents", "revenge");
            yield return CapturePage("History", "history");
            yield return CapturePage("Status", "status");
            System.IO.File.WriteAllText(EvidencePath("club-smoke-state.json"), JsonUtility.ToJson(club));
            yield return new WaitForSecondsRealtime(1);
            if (smokeFailure || replayResult == null || club.History.Length == 0 || !System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, "club-result.png"))) { Debug.LogError("CLUB_UI_SMOKE_FAILED"); Application.Quit(1); }
            else { Debug.Log("CLUB_UI_SMOKE_PASSED account=" + account + " matches=" + club.History.Length); Application.Quit(0); }
        }
    }
}
