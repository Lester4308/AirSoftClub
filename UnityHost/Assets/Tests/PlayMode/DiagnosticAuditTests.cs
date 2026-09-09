using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Airsoft.Battle;
using AirsoftClub.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public sealed class DiagnosticAuditTests
{
    static void Set(object target, string field, object value) => target.GetType()
        .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    [UnityTest]
    public IEnumerator AuditInventorySettingsAndResources()
    {
        var failures = new List<string>();
        var fixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        fixture.Fighters = new[] { new FighterView { Id = "audit-fighter", Name = "Audit", Hp = 1000000,
            MaxHp = 1000000, Level = 1, Accuracy = 10, Endurance = 10, Agility = 10,
            Ready = true, Equipment = Array.Empty<EquipmentView>() } };
        fixture.Items = new[] { new ItemView { Id = "audit-item", Definition = "Pistol-MK1", Slot = "Weapon", Equipped = false } };
        var host = new GameObject("DiagnosticAudit");
        var client = host.AddComponent<ClubClient>();
        Set(client, "club", fixture);
        Set(client, "page", "Roster");
        Set(client, "selectedFighter", "audit-fighter");
        Set(client, "inventorySlot", "Weapon");
        client.SetModernForTest(true);
        yield return null;
        yield return null;
        var buttons = client.Modern.GetComponentsInChildren<Button>();
        foreach (var label in new[] { "CLOSE", "EQUIP" })
        {
            var b = buttons.FirstOrDefault(x => x.GetComponentInChildren<Text>()?.text == label);
            bool ok = b != null && b.IsInteractable();
            Debug.Log("DIAGNOSTIC inventory_" + label + "=" + ok);
            if (!ok) failures.Add("Inventory " + label + " is disabled or absent");
        }
        foreach (var name in new[] { "male-inspect", "male-roster", "male-combat-near", "male-combat-mass",
            "female-inspect", "female-roster", "female-combat-near", "female-combat-mass" })
            if (Resources.Load<Sprite>("Art/Volumetric017/Sprites/" + name) == null) failures.Add("Missing fighter " + name);
        foreach (var family in new[] { "pistol", "smg", "assault-rifle", "pump-shotgun", "dmr", "sniper-rifle" })
            foreach (var suffix in new[] { "card", "silhouette" })
                if (Resources.Load<Sprite>("Art/Volumetric017/Weapons/" + family + "-" + suffix) == null)
                    failures.Add("Missing weapon " + family + "-" + suffix);
        Debug.Log("DIAGNOSTIC sprite_missing=" + failures.Count(x => x.StartsWith("Missing")));
        Set(client, "inventorySlot", "");
        Set(client, "page", "Settings");
        yield return null;
        yield return null;
        bool isHub = client.Modern.GetComponentsInChildren<Text>().Any(t => t.text == "HEADQUARTERS");
        Debug.Log("DIAGNOSTIC settings_renders_hub=" + isHub);
        if (isHub) failures.Add("Settings renders Club hub");
        Object.Destroy(client.Modern.gameObject);
        Object.Destroy(host);
        yield return null;
        Assert.IsEmpty(failures, string.Join("; ", failures));
    }

    [UnityTest]
    public IEnumerator InventoryClose_UsesPointerEvent_AndClosesOverlay()
    {
        var fixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        fixture.Fighters = new[] { new FighterView { Id = "audit-fighter", Name = "Audit", Hp = 1000000,
            MaxHp = 1000000, Level = 1, Accuracy = 10, Endurance = 10, Agility = 10,
            Ready = true, Equipment = Array.Empty<EquipmentView>() } };
        fixture.Items = new[] { new ItemView { Id = "audit-item", Definition = "Pistol-MK1", Slot = "Weapon", Equipped = false } };
        var host = new GameObject("PointerInventoryAudit");
        var client = host.AddComponent<ClubClient>();
        Set(client, "club", fixture);
        Set(client, "page", "Roster");
        Set(client, "selectedFighter", "audit-fighter");
        Set(client, "inventorySlot", "Weapon");
        client.SetModernForTest(true);
        yield return null;
        yield return null;

        var close = client.Modern.GetComponentsInChildren<Button>()
            .FirstOrDefault(x => x.GetComponentInChildren<Text>()?.text == "CLOSE");
        Assert.IsNotNull(close);
        Assert.IsTrue(close.IsInteractable());
        var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
        ExecuteEvents.Execute(close.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(close.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(close.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        yield return null;

        var slot = (string)client.GetType().GetField("inventorySlot", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
        Assert.AreEqual(string.Empty, slot);
        Object.Destroy(client.Modern.gameObject);
        Object.Destroy(host);
    }

    [UnityTest]
    public IEnumerator ResultButton_SurvivesLongPressAcrossFrames_AndNavigates()
    {
        Screen.SetResolution(1600, 900, false);
        yield return null;
        var fixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        fixture.History = Array.Empty<HistoryView>();
        var host = new GameObject("ResultInteractionAudit");
        var client = host.AddComponent<ClubClient>();
        var input = BattleWire.ReadConfig(BattleHost.GoldenInput);
        var result = BattleWire.ReadResult(BattleHost.GoldenExpected);
        Set(client, "club", fixture);
        Set(client, "page", "Battle");
        Set(client, "savedMatch", new MatchView { Id = "audit-match", Status = "Completed", RatingKnown = true });
        Set(client, "replayInput", input);
        Set(client, "replayResult", result);
        Set(client, "appearance", Array.Empty<AppearanceView>());
        Invoke(client, "ResetPlayback");
        Set(client, "replayTime", result.SimulatedDurationMs + 1f);
        client.SetModernForTest(true);
        yield return null;
        yield return null;

        var button = FindButton(client, "BACK TO CLUB");
        Assert.IsNotNull(button);
        Assert.IsTrue(button.IsInteractable());
        int instanceId = button.GetInstanceID();
        int childCount = client.Modern.Root.childCount;
        var pointer = PointerAtCenter(button);
        AssertRaycastHits(pointer, button.gameObject);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        yield return null;
        yield return null;
        yield return null;
        Assert.AreEqual(instanceId, FindButton(client, "BACK TO CLUB").GetInstanceID(),
            "The result button was rebuilt while the pointer was held.");
        Assert.AreEqual(childCount, client.Modern.Root.childCount,
            "The completed result hierarchy was rebuilt while idle.");
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        yield return null;
        Assert.AreEqual("Club", Get<string>(client, "page"));
        Cleanup(client, host);
    }

    [UnityTest]
    public IEnumerator ResultButtons_RaycastAndNavigate_ForVictoryDefeatAndDraw()
    {
        Screen.SetResolution(1600, 900, false);
        yield return null;
        foreach (var outcome in new[] { MatchOutcome.AttackerWin, MatchOutcome.DefenderWin, MatchOutcome.Draw })
        {
            foreach (var action in new[] { "NEW BATTLE", "BACK TO CLUB", "REPLAY", "HISTORY" })
            {
                var fixture = (ClubView)typeof(ModernUiSmokeTests)
                    .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
                fixture.History = Array.Empty<HistoryView>();
                var host = new GameObject("ResultMatrixAudit");
                var client = host.AddComponent<ClubClient>();
                var input = BattleWire.ReadConfig(BattleHost.GoldenInput);
                var canonical = BattleWire.ReadResult(BattleHost.GoldenExpected);
                var result = ResultWithOutcome(canonical, outcome);
                Set(client, "club", fixture);
                Set(client, "page", "Battle");
                Set(client, "savedMatch", new MatchView { Id = "audit-match", Status = "Completed", RatingKnown = true });
                Set(client, "replayInput", input);
                Set(client, "replayResult", result);
                Set(client, "appearance", Array.Empty<AppearanceView>());
                Invoke(client, "ResetPlayback");
                Set(client, "replayTime", result.SimulatedDurationMs + 1f);
                client.SetModernForTest(true);
                yield return null;
                yield return null;
                var button = FindButton(client, action);
                Assert.IsNotNull(button, outcome + " / " + action);
                Assert.IsTrue(button.IsInteractable(), outcome + " / " + action);
                yield return null;
                Canvas.ForceUpdateCanvases();
                button = FindButton(client, action);
                var pointer = PointerAtCenter(button);
                AssertRaycastHits(pointer, button.gameObject, outcome + " / " + action);
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
                yield return null;
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                yield return null;
                string expected = action == "NEW BATTLE" ? "Opponents" : action == "BACK TO CLUB" ? "Club" : action == "HISTORY" ? "History" : "Battle";
                Assert.AreEqual(expected, Get<string>(client, "page"), outcome + " / " + action);
                if (action == "REPLAY") Assert.Less(Get<float>(client, "replayTime"), result.SimulatedDurationMs);
                Cleanup(client, host);
                yield return null;
            }
        }
    }

    // Verifies the result verdict label text matches the outcome and attacker/defender side.
    [UnityTest]
    public IEnumerator ResultVerdictLabel_MatchesOutcomeAndSide()
    {
        Screen.SetResolution(1600, 900, false);
        yield return null;
        var fixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        fixture.History = Array.Empty<HistoryView>();
        var input = BattleWire.ReadConfig(BattleHost.GoldenInput);
        var canonical = BattleWire.ReadResult(BattleHost.GoldenExpected);

        // Attacker side (no history → defaults to attacking)
        foreach (var kv in new[] {
            (MatchOutcome.AttackerWin, "VICTORY"),
            (MatchOutcome.DefenderWin, "DEFEAT"),
            (MatchOutcome.Draw, "DRAW") })
        {
            var host = new GameObject("VerdictAudit");
            var client = host.AddComponent<ClubClient>();
            var result = ResultWithOutcome(canonical, kv.Item1);
            Set(client, "club", fixture);
            Set(client, "page", "Battle");
            Set(client, "savedMatch", new MatchView { Id = "audit-match", Status = "Completed", RatingKnown = true });
            Set(client, "replayInput", input);
            Set(client, "replayResult", result);
            Set(client, "appearance", Array.Empty<AppearanceView>());
            Invoke(client, "ResetPlayback");
            Set(client, "replayTime", result.SimulatedDurationMs + 1f);
            client.SetModernForTest(true);
            yield return null;
            yield return null;
            var texts = client.Modern.GetComponentsInChildren<Text>(true).Select(t => t.text).ToArray();
            Assert.IsTrue(texts.Contains(kv.Item2),
                "Attacker side " + kv.Item1 + " did not render verdict '" + kv.Item2 + "'");
            Cleanup(client, host);
            yield return null;
        }

        // Defender side: history entry has Attacker != club.Id
        var defHost = new GameObject("DefenderVerdict");
        var defClient = defHost.AddComponent<ClubClient>();
        var defFixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        defFixture.Id = "my-club";
        defFixture.History = new[] { new HistoryView { Id = "def-match", Attacker = "other-club", Defender = "my-club", Outcome = "DefenderWin" } };
        var defResult = ResultWithOutcome(canonical, MatchOutcome.DefenderWin);
        Set(defClient, "club", defFixture);
        Set(defClient, "page", "Battle");
        Set(defClient, "savedMatch", new MatchView { Id = "def-match", Status = "Completed", RatingKnown = true });
        Set(defClient, "replayInput", input);
        Set(defClient, "replayResult", defResult);
        Set(defClient, "appearance", Array.Empty<AppearanceView>());
        Invoke(defClient, "ResetPlayback");
        Set(defClient, "replayTime", defResult.SimulatedDurationMs + 1f);
        defClient.SetModernForTest(true);
        yield return null;
        yield return null;
        var defTexts = defClient.Modern.GetComponentsInChildren<Text>(true).Select(t => t.text).ToArray();
        Assert.IsTrue(defTexts.Contains("VICTORY"),
            "Defender won (DefenderWin) but verdict was not 'VICTORY'");
        Cleanup(defClient, defHost);
    }

    // Proves the modal shade blocks raycast to background buttons: a pointer aimed
    // at a background nav button does NOT hit it when a shade Image covers it.
    [UnityTest]
    public IEnumerator ModalShadeBlocksRaycastToBackgroundButtons()
    {
        var host = new GameObject("ShadeRaycastAudit");
        var client = host.AddComponent<ClubClient>();
        try
        {
            Set(client, "club", (ClubView)typeof(ModernUiSmokeTests)
                .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null));
            Set(client, "page", "Club");
            client.SetModernForTest(true);
            yield return null;
            yield return null;

            // Open Credits modal
            var convert = ButtonByText("CONVERT 1 CREDIT → 100 MONEY");
            Assert.IsNotNull(convert, "Credits action not found.");
            ClickPointer(convert);
            yield return null;
            yield return null;

            // The shade should cover the full screen
            var shade = Object.FindObjectsByType<Image>(FindObjectsSortMode.None)
                .FirstOrDefault(i => i.gameObject.name == "CreditsConfirmationShade");
            Assert.IsNotNull(shade, "Modal shade Image was not created.");
            Assert.IsTrue(shade.raycastTarget, "Shade should block raycasts.");

            // Raycast at the nav button position — should hit the shade, NOT the nav button
            var nav = ButtonByText("Roster");
            Assert.IsNotNull(nav, "Background nav button not found.");
            Assert.IsFalse(nav.IsInteractable(), "Background nav should be non-interactable while modal is open.");
            var pointer = PointerAtCenter(nav);
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            Assert.IsNotEmpty(hits, "Raycast under modal produced no hits.");
            // UGUI delivers events to the TOPMOST hit only. The shade is drawn last (top),
            // so the first hit must be the shade/panel — NOT the nav button beneath it.
            bool topIsNav = hits[0].gameObject == nav.gameObject || hits[0].gameObject.transform.IsChildOf(nav.transform);
            Assert.IsFalse(topIsNav, "Topmost raycast hit was the background nav button, not the modal shade.");
            bool topIsShade = hits[0].gameObject == shade.gameObject || hits[0].gameObject.transform.IsChildOf(shade.transform);
            Assert.IsTrue(topIsShade, "Topmost raycast hit under the modal was not the shade.");
        }
        finally
        {
            Cleanup(client, host);
        }
        yield return null;
    }

    static Button ButtonByText(string text) => Object.FindObjectsByType<Button>(FindObjectsSortMode.None)
        .FirstOrDefault(b => b.GetComponentInChildren<Text>()?.text == text);

    static void ClickPointer(Button button)
    {
        Assert.IsNotNull(EventSystem.current, "Pointer interaction requires an EventSystem.");
        var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }

    static MatchResult ResultWithOutcome(MatchResult source, MatchOutcome outcome)
    {
        var ctor = typeof(MatchResult).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
        return (MatchResult)ctor.Invoke(new object[] { ResultStatus.Completed, (MatchOutcome?)outcome, source.Reason, "",
            source.SimulatedDurationMs, source.Attacker, source.Defender, source.Events, source.Seed, source.RulesetVersion });
    }

    static Button FindButton(ClubClient client, string label) => client.Modern.GetComponentsInChildren<Button>(true)
        .FirstOrDefault(x => x.GetComponentInChildren<Text>(true)?.text == label);

    static PointerEventData PointerAtCenter(Button button)
    {
        var camera = button.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay
            ? null : button.GetComponentInParent<Canvas>().worldCamera;
        var position = RectTransformUtility.WorldToScreenPoint(camera, ((RectTransform)button.transform).TransformPoint(((RectTransform)button.transform).rect.center));
        return new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left, position = position };
    }

    static void AssertRaycastHits(PointerEventData pointer, GameObject target, string context = null)
    {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hits);
        Assert.IsTrue(hits.Any(h => h.gameObject == target || h.gameObject.transform.IsChildOf(target.transform)),
            (context == null ? "" : context + ": ") + "EventSystem.RaycastAll did not hit " + target.name +
            ". pointer=" + pointer.position + " screen=" + Screen.width + "x" + Screen.height +
            " target=" + ((RectTransform)target.transform).rect + " world=" + ((RectTransform)target.transform).position +
            ". Hits: " + string.Join(", ", hits.Select(h => h.gameObject.name)));
    }

    static void Invoke(object target, string method) => target.GetType()
        .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);

    static T Get<T>(object target, string field) => (T)target.GetType()
        .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);

    static void Cleanup(ClubClient client, GameObject host)
    {
        if (client.Modern != null) Object.Destroy(client.Modern.gameObject);
        Object.Destroy(host);
    }

    [Test]
    public void StableAppearance_UsesPersistedIdentityAcrossScreens()
    {
        Assert.IsTrue(Volumetric017UiView.IsFemaleAppearance("female-standard", "candidate-1"));
        Assert.IsFalse(Volumetric017UiView.IsFemaleAppearance("male-standard", "candidate-1"));
        Assert.AreEqual(Volumetric017UiView.IsFemale("legacy-1"), Volumetric017UiView.IsFemaleAppearance("", "legacy-1"));
    }
}
