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
        public string Id, Name, PendingMatch, CatalogVersion;
        public long Version, Money, Credits, Xp, ShieldUntil, ServerNow, OffersAt;
        public int Level, Rating, Capacity, ActiveBbTier, OfferVersion, CompletedSinceRefresh;
        public bool FreeRecruitClaimed;
        public int[] BbStock;
        public FighterView[] Fighters;
        public OfferView[] Offers;
        public ItemView[] Items;
        public CatalogView[] Catalog;
        public HistoryView[] History;
    }
    [Serializable] public class FighterView { public string Id, Name; public int Accuracy, Endurance, Agility, Level, TrainingCap; public long Hp, MaxHp, Xp; public bool Ready; public EquipmentView[] Equipment; }
    [Serializable] public class EquipmentView { public string Slot, Item, Definition; }
    [Serializable] public class OfferView { public string Id, Name; public int Accuracy, Endurance, Agility; public long Price; }
    [Serializable] public class ItemView { public string Id, Definition, Slot; public bool Equipped; }
    [Serializable] public class CatalogView { public string Id, Slot; public long Money, Credits; public int Mk, Level; }
    [Serializable] public class HistoryView { public string Id, Status, Mode, Attacker, Defender; }
    [Serializable] public class RivalView { public string Id, Name, Category; public int Level, Rating, Fighters; public bool Protected; }
    [Serializable] public class RivalsView { public RivalView[] Opponents; }
    [Serializable] public class LoginView { public string Token; }
    [Serializable] public class LoginRequest { public string Account; }
    [Serializable] public class CommandRequest
    {
        public string Key, Type, Target = "", Value = "", CatalogVersion = "catalog-004-v1";
        public long Version;
        public int Number, OfferVersion;
        public bool Flag;
    }
    [Serializable] public class MatchView { public string Id, MatchId, Status, Input, Result, Digest; }
    public sealed class ClubClient : MonoBehaviour
    {
        const string Endpoint = "http://127.0.0.1:5080";
        string account, token = "", page = "Club", status = "Connect to your local development server.", selectedFighter = "", lastPayload, selectedTarget = "";
        string mode = "Practice";
        ClubView club;
        RivalView[] rivals = Array.Empty<RivalView>();
        bool busy, smoke, failed, smokeFailure;
        Vector2 scroll;
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
            var go = new GameObject("Club development client"); DontDestroyOnLoad(go); go.AddComponent<ClubClient>();
        }
        void Start()
        {
            Application.targetFrameRate = 60; Application.runInBackground = true;
            smoke = Environment.GetCommandLineArgs().Contains("--club-ui-smoke");
            account = PlayerPrefs.GetString("club-development-account", "dev-" + Guid.NewGuid().ToString("N").Substring(0, 12));
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
                    status = request.responseCode == 401 ? "Session expired. Reconnect to continue." : request.downloadHandler.text.Length > 0 ? request.downloadHandler.text : "Server unavailable. Start tools/run-development.ps1, then retry.";
                    Debug.Log("CLUB_REQUEST_FAILED code=" + request.responseCode);
                    if (request.responseCode == 409 || request.responseCode == 400) lastPayload = null;
                }
                else { failed = false; success(request.downloadHandler.text); }
            }
        }
        IEnumerator Login()
        {
            busy = true;
            yield return Request("/dev/login", JsonUtility.ToJson(new LoginRequest { Account = account }), text => token = JsonUtility.FromJson<LoginView>(text).Token);
            if (!failed) { PlayerPrefs.SetString("club-development-account", account); PlayerPrefs.Save(); yield return Refresh(); }
            busy = false;
        }
        IEnumerator Refresh()
        {
            yield return Request("/api/club", null, text => { club = JsonUtility.FromJson<ClubView>(text); if (selectedFighter == "" && club.Fighters.Length > 0) selectedFighter = club.Fighters[0].Id; });
            if (!failed) yield return Request("/api/opponents", null, text => rivals = JsonUtility.FromJson<RivalsView>(text).Opponents);
        }
        CommandRequest Intent(string type, string target = "", string value = "", int number = 0, bool flag = false) => new CommandRequest
        { Key = Guid.NewGuid().ToString("N"), Version = club.Version, Type = type, Target = target, Value = value, Number = number, Flag = flag, OfferVersion = club.OfferVersion, CatalogVersion = club.CatalogVersion };
        IEnumerator Send(CommandRequest command)
        {
            busy = true; lastPayload = JsonUtility.ToJson(command);
            string match = "";
            yield return Request("/api/command", lastPayload, text => { if (command.Type == "Attack") match = JsonUtility.FromJson<MatchView>(text).MatchId; status = "Saved by server."; lastPayload = null; });
            if (!failed) yield return Refresh();
            if (match.Length > 0) yield return LoadMatch(match);
            busy = false;
        }
        IEnumerator Retry()
        {
            busy = true;
            if (lastPayload != null) yield return Request("/api/command", lastPayload, text => { lastPayload = null; status = "Recovered server receipt."; });
            yield return Refresh();
            if (!failed && club.PendingMatch != null && club.PendingMatch.Length > 0) yield return LoadMatch(club.PendingMatch);
            busy = false;
        }
        IEnumerator LoadMatch(string id)
        {
            busy = true; MatchView view = null;
            for (int n = 0; n < 40; n++)
            {
                yield return Request("/api/match/" + id, null, text => view = JsonUtility.FromJson<MatchView>(text));
                if (failed || view.Status != "Pending") break;
                status = "Battle accepted. Waiting for authoritative settlement..."; yield return new WaitForSecondsRealtime(0.5f);
            }
            if (view != null && view.Status == "Completed")
            {
                replayInput = BattleWire.ReadConfig(Convert.FromBase64String(view.Input)); replayResult = BattleWire.ReadResult(Convert.FromBase64String(view.Result));
                hp.Clear(); foreach (var f in replayInput.Attacker.Fighters) hp["A" + f.Id] = f.StartingHp.Raw;
                foreach (var f in replayInput.Defender.Fighters) hp["D" + f.Id] = f.StartingHp.Raw;
                replayTime = 0; eventIndex = 0; page = "Battle"; status = "Server result saved. Playback speed and skip only affect presentation.";
            }
            else if (view != null) status = "Match " + view.Status + ". Use refresh to recover pending work.";
            yield return Refresh(); busy = false;
        }
        void Update()
        {
            if (page != "Battle" || replayResult == null) return;
            replayTime += Time.unscaledDeltaTime * 1000;
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
        void Styles()
        {
            if (title != null) return;
            title = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, normal = { textColor = new Color(.96f, .74f, .36f) } };
            label = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true, normal = { textColor = new Color(.87f, .91f, .94f) } };
            small = new GUIStyle(label) { fontSize = 13 };
            button = new GUIStyle(GUI.skin.button) { fontSize = 15, fixedHeight = 34, margin = new RectOffset(4, 4, 4, 4) };
        }
        bool Btn(string text, params GUILayoutOption[] options) => GUILayout.Button(text, button, options);
        void Text(string value) => GUILayout.Label(value, label);
        void OnGUI()
        {
            Styles(); GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 0, new Color(.055f, .085f, .11f), 0, 0);
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 800f); GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            GUILayout.BeginArea(new Rect(28, 20, 1224, 760));
            GUILayout.Label("AIRSOFT / CLUB", title); GUILayout.Label("LOCAL DEVELOPMENT • Placeholder interface • Server-authoritative • No live Steam or payments", small);
            GUILayout.Space(12);
            GUI.enabled = !busy;
            if (club == null)
            {
                Text("Build your club. Equip your fighters. Challenge a rival."); GUILayout.Space(24);
                Text("Development account"); GUI.SetNextControlName("account"); account = GUILayout.TextField(account, 44, GUILayout.Width(480), GUILayout.Height(32));
                if (Btn("Connect", GUILayout.Width(220))) StartCoroutine(Login());
            }
            else
            {
                Text($"{club.Name}  •  Level {club.Level}  /  XP {club.Xp}  •  Rating {club.Rating}     Money {club.Money}     Credits {club.Credits}");
                GUILayout.BeginHorizontal(); foreach (string tab in new[] { "Club", "Roster", "Recruitment", "Supply", "Opponents", "History" }) if (Btn(tab)) { page = tab; scroll = Vector2.zero; }
                if (Btn("Refresh")) StartCoroutine(Retry()); if (Btn("Reconnect")) StartCoroutine(Login()); GUILayout.EndHorizontal();
                GUILayout.Space(10); scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(555));
                if (page == "Club") Hub(); else if (page == "Roster") Roster(); else if (page == "Recruitment") Recruitment();
                else if (page == "Supply") Supply(); else if (page == "Opponents") Opponents(); else if (page == "History") History(); else if (page == "Battle") Battle();
                GUILayout.EndScrollView();
            }
            GUI.enabled = true; GUILayout.Space(8); GUILayout.Label((busy ? "Working... " : "") + status, small);
            if (!busy && lastPayload != null && Btn("Retry the same operation safely", GUILayout.Width(330))) StartCoroutine(Retry());
            GUILayout.EndArea();
        }
        void Hub()
        {
            GUILayout.Label("YOUR NEXT MOVE", title);
            Text(club.Fighters.Length == 0 ? "Choose one of three free recruits to found your club." : "All ready fighters join automatically. Select a fighter only to manage their equipment.");
            Text($"Roster {club.Fighters.Length}/16  •  Ready {club.Fighters.Count(f => f.Ready)}  •  Shared BB tier {club.ActiveBbTier}: {club.BbStock[club.ActiveBbTier]}/{club.Capacity}");
            Text("Accuracy / Endurance / Agility. Battle XP unlocks training caps; Money buys training. Offensive wounds persist; defense begins at full HP.");
            if (club.PendingMatch != null && club.PendingMatch.Length > 0 && Btn("Recover pending battle")) StartCoroutine(LoadMatch(club.PendingMatch));
            if (!club.FreeRecruitClaimed && Btn("Choose starter recruit")) page = "Recruitment";
            if (Btn("Manage fighters")) page = "Roster"; if (Btn("Find an opponent")) page = "Opponents";
            GUILayout.Space(14); Text("Free recovery: approximately 1% Max HP per minute. Emergency Basic BB is available when stock and Money are low.");
            if (Btn("Claim emergency Basic")) StartCoroutine(Send(Intent("Emergency")));
            if (Btn("Convert 1 Credit → 100 Money")) StartCoroutine(Send(Intent("Convert", number: 1)));
            Text("Protection blocks new incoming attacks. Own Ranked cancels it; Friend/Practice keeps it.");
            GUILayout.BeginHorizontal(); foreach (int hours in new[] { 8, 24, 72, 168 }) if (Btn("Shield " + hours + "h")) StartCoroutine(Send(Intent("Shield", number: hours))); GUILayout.EndHorizontal();
        }
        void Roster()
        {
            if (club.Fighters.Length == 0) Text("No fighters yet. Open Recruitment.");
            foreach (var f in club.Fighters)
            {
                GUILayout.BeginVertical(GUI.skin.box); Text($"{f.Name} • Level {f.Level} • HP {f.Hp / 10000f:0.0}/{f.MaxHp / 10000f:0.0} • {(f.Ready ? "READY" : "RECOVERING")} • XP {f.Xp}");
                Text($"Accuracy {f.Accuracy} / Endurance {f.Endurance} / Agility {f.Agility} • Training cap {f.TrainingCap}");
                GUILayout.BeginHorizontal(); foreach (string stat in new[] { "Accuracy", "Endurance", "Agility" }) if (Btn(stat + " +1 / 25 Money")) StartCoroutine(Send(Intent("Train", f.Id, stat)));
                if (Btn("Heal / Money")) StartCoroutine(Send(Intent("Heal", f.Id))); if (Btn("Manage gear")) selectedFighter = f.Id; GUILayout.EndHorizontal();
                Text(f.Equipment.Length == 0 ? "Weaponless — participates as a target, cannot shoot." : string.Join(" • ", f.Equipment.Select(e => e.Definition)));
                if (selectedFighter == f.Id)
                {
                    foreach (var e in f.Equipment) if (Btn("Unequip " + e.Definition)) StartCoroutine(Send(Intent("Unequip", f.Id, e.Slot)));
                    foreach (var i in club.Items.Where(i => !i.Equipped)) if (Btn("Equip " + i.Definition)) StartCoroutine(Send(Intent("Equip", f.Id, i.Id)));
                    if (club.Fighters.Length > 1 && Btn("Dismiss / return gear + 30% initial hire price")) StartCoroutine(Send(Intent("Dismiss", f.Id)));
                }
                GUILayout.EndVertical();
            }
        }
        void Recruitment()
        {
            Text($"Pool #{club.OfferVersion} • {club.CompletedSinceRefresh}/10 completed battles • Free refresh after 1h OR 10 battles");
            GUILayout.BeginHorizontal(); if (Btn("Free refresh")) StartCoroutine(Send(Intent("Refresh"))); if (Btn("Refresh / 50 Money")) StartCoroutine(Send(Intent("Refresh", flag: true))); GUILayout.EndHorizontal();
            for (int n = 0; n < club.Offers.Length; n++)
            {
                var o = club.Offers[n]; bool free = !club.FreeRecruitClaimed && n < 3;
                GUILayout.BeginHorizontal(GUI.skin.box); Text($"{o.Name}     ACC {o.Accuracy}  END {o.Endurance}  AGI {o.Agility}");
                if (Btn(free ? "Choose FREE" : "Hire / " + o.Price + " Money", GUILayout.Width(250))) StartCoroutine(Send(Intent("Hire", o.Id, flag: free))); GUILayout.EndHorizontal();
            }
        }
        void Supply()
        {
            Text("Shared ammunition • one active tier for the whole club");
            for (int n = 0; n < 5; n++)
            {
                GUILayout.BeginHorizontal(); Text($"Tier {n}  {club.BbStock[n]}/{club.Capacity}");
                if (Btn(club.ActiveBbTier == n ? "ACTIVE" : "Select", GUILayout.Width(160))) StartCoroutine(Send(Intent("BbTier", number: n)));
                if (Btn(n == 4 ? "+500 / 1 Credit" : "+500 / " + (50 + n * 20) + " Money", GUILayout.Width(250))) StartCoroutine(Send(Intent("Refill", number: n))); GUILayout.EndHorizontal();
            }
            Text("Equipment catalog — Base = MK1. Buy here, equip in Roster. Prototype prices.");
            foreach (var i in club.Catalog)
            {
                GUILayout.BeginHorizontal(GUI.skin.box); Text(i.Id + " • " + i.Slot + " • Level " + i.Level);
                if (Btn(i.Credits > 0 ? "Buy / " + i.Credits + " Credits" : "Buy / " + i.Money + " Money", GUILayout.Width(250))) StartCoroutine(Send(Intent("Buy", i.Id))); GUILayout.EndHorizontal();
            }
        }
        void Opponents()
        {
            Text("Development rivals • category shown instead of exact internal power. Friend here is a local policy test, not a Steam friend claim.");
            mode = new[] { "Practice", "Ranked", "Friend" }[GUILayout.SelectionGrid(Array.IndexOf(new[] { "Practice", "Ranked", "Friend" }, mode), new[] { "Practice", "Ranked", "Friend" }, 3, button)];
            if (mode == "Ranked" && club.ShieldUntil > club.ServerNow) Text("CONFIRMATION: starting Ranked will cancel your active shield.");
            foreach (var r in rivals)
            {
                GUILayout.BeginHorizontal(GUI.skin.box); Text($"{r.Name}  •  Level {r.Level}  •  {r.Fighters} fighters  •  Rating {r.Rating} {(r.Protected ? "PROTECTED" : "")}");
                if (Btn("Preview", GUILayout.Width(180))) selectedTarget = r.Id; GUILayout.EndHorizontal();
            }
            if (selectedTarget.Length > 0)
            {
                Text($"Ready to challenge: {mode}. Your {club.Fighters.Count(f => f.Ready)} ready fighters automatically participate. Server checks health, ammo and exposure at acceptance.");
                if (Btn("Confirm & start battle")) StartCoroutine(Send(Intent("Attack", selectedTarget, mode)));
            }
            Text("Revenge tickets are recorded; live settlement is gated by the pending counterparty rating policy.");
        }
        void History()
        {
            foreach (var h in club.History) { GUILayout.BeginHorizontal(GUI.skin.box); Text(h.Mode + " • " + h.Status + " • " + h.Id.Substring(0, 8)); if (Btn("Open", GUILayout.Width(160))) StartCoroutine(LoadMatch(h.Id)); GUILayout.EndHorizontal(); }
        }
        void Battle()
        {
            if (replayResult == null) return;
            Text($"{Math.Min(replayTime, replayResult.SimulatedDurationMs) / 1000:0.0}s  •  BB used {replayResult.Attacker.BbConsumed} / {replayResult.Defender.BbConsumed}");
            if (Btn("Skip to saved result")) replayTime = replayResult.SimulatedDurationMs + 1;
            GUILayout.BeginHorizontal(); Team(replayInput.Attacker, "A", "YOUR CLUB"); Team(replayInput.Defender, "D", "DEFENSE"); GUILayout.EndHorizontal();
            if (replayTime >= replayResult.SimulatedDurationMs) { GUILayout.Label(replayResult.Outcome.ToString(), title); Text("Settlement saved • " + replayResult.Reason); }
        }
        void Team(TeamSnapshot team, string side, string heading)
        {
            GUILayout.BeginVertical(GUILayout.Width(570)); Text(heading);
            foreach (var f in team.Fighters)
            {
                long value = hp[side + f.Id]; float ratio = value / (float)Formulas.MaxHp(f.Endurance, replayInput.Rules).Raw;
                Rect rect = GUILayoutUtility.GetRect(560, 25); GUI.color = new Color(.18f, .25f, .28f); GUI.DrawTexture(rect, Texture2D.whiteTexture);
                GUI.color = side == "A" ? new Color(.2f, .6f, .55f) : new Color(.65f, .36f, .2f); GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * ratio, rect.height), Texture2D.whiteTexture); GUI.color = Color.white;
                GUI.Label(rect, "  " + f.Id.Substring(0, Math.Min(8, f.Id.Length)) + "   " + (value == 0 ? "ELIMINATED" : (value / 10000f).ToString("0.0") + " HP") + (f.Weapon == null ? " • weaponless" : " • " + f.Weapon.Family), small);
            }
            GUILayout.EndVertical();
        }
        void Capture(string path)
        {
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG()); Destroy(texture);
            Debug.Log("CLUB_CAPTURE " + path);
        }
        IEnumerator Walkthrough()
        {
            yield return Login(); if (failed) { Application.Quit(1); yield break; }
            yield return Send(Intent("Hire", club.Offers[0].Id, flag: true));
            yield return Send(Intent("Buy", "Pistol-MK1"));
            yield return Send(Intent("Equip", club.Fighters[0].Id, club.Items[0].Id));
            yield return Send(Intent("Train", club.Fighters[0].Id, "Accuracy"));
            yield return Send(Intent("Heal", club.Fighters[0].Id));
            yield return Send(Intent("Refill", number: 0));
            page = "Roster"; yield return new WaitForEndOfFrame();
            Capture(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, "club-roster.png"));
            yield return Send(Intent("Attack", rivals[0].Id, "Practice"));
            replayTime = 200000; yield return null; yield return new WaitForEndOfFrame();
            Capture(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, "club-result.png"));
            yield return new WaitForSecondsRealtime(1);
            if (smokeFailure || replayResult == null || club.History.Length == 0 || !System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).Parent.FullName, "club-result.png"))) { Debug.LogError("CLUB_UI_SMOKE_FAILED"); Application.Quit(1); }
            else { Debug.Log("CLUB_UI_SMOKE_PASSED account=" + account + " matches=" + club.History.Length); Application.Quit(0); }
        }
    }
}
