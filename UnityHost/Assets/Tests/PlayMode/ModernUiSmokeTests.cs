using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using AirsoftClub.Unity;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class ModernUiSmokeTests
{
    // Verifies the UGUI/Canvas bootstrap constructs a real Canvas + EventSystem and
    // the modern controller drives a render (login screen with no club state).
    [UnityTest] public IEnumerator ModernCanvasBootstrapProducesHierarchy()
    {
        var host = new GameObject("Host");
        var client = host.AddComponent<ClubClient>();
        client.SetModernForTest(true);

        yield return null;

        var canvas = Object.FindFirstObjectByType<Canvas>();
        Assert.IsNotNull(canvas, "UGUI Canvas was not created by the modern bootstrap.");
        Assert.IsNotNull(EventSystem.current, "EventSystem was not created.");

        var texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
        bool any = false;
        foreach (var t in texts) if (!t.raycastTarget && t.text.Length > 0) { any = true; break; }
        Assert.IsTrue(any, "Modern UI rendered no visible text elements.");

        Object.Destroy(host);
        Object.Destroy(canvas.gameObject);
    }

    // Regression guard for the top-left coordinate system: rendered text and the
    // button label must stay inside their parent rectangles instead of drifting.
    [UnityTest] public IEnumerator ModernLoginChildrenStayInsideTheirBoxes()
    {
        var host = new GameObject("LayoutHost");
        var client = host.AddComponent<ClubClient>();
        client.SetModernForTest(true);
        yield return null;

        var login = GameObject.Find("Login").GetComponent<RectTransform>();
        Assert.IsNotNull(login);
        foreach (var text in login.GetComponentsInChildren<Text>(true))
            Assert.IsTrue(IsInside(text.rectTransform, login), text.text + " escaped the Login box");

        foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            var label = button.GetComponentInChildren<Text>();
            Assert.IsNotNull(label, button.name + " has no label");
            Assert.IsTrue(IsInside(label.rectTransform, button.GetComponent<RectTransform>()), label.text + " escaped its button");
        }

        Object.Destroy(host);
        Object.Destroy(login.GetComponentInParent<Canvas>().gameObject);
    }

    [UnityTest] public IEnumerator CreditsActionsStageAnExplicitConfirmation()
    {
        var host = new GameObject("CommerceHost");
        var client = host.AddComponent<ClubClient>();
        SetField(client, "club", ClubFixture());
        SetField(client, "page", "Shop");
        client.SetModernForTest(true);
        yield return null;

        var early = ButtonWithText("UNLOCK EARLY / 2 CREDITS");
        Assert.IsNotNull(early, "Early access was not exposed separately from the Money item purchase.");
        PointerClick(early);
        yield return null;

        Assert.IsNotNull(ButtonWithText("CONFIRM 2 CREDITS"), "A Credits action spent without an explicit confirmation step.");
        Assert.IsFalse((bool)GetField(client, "busy"), "Staging a confirmation must not contact the server.");
        Assert.IsTrue(AllText().Any(t => t.Contains("item is still purchased separately with Money")));
        DestroyClient(host, client);
    }

    [UnityTest] public IEnumerator HubExplainsRetentionConversionAndShieldConsequences()
    {
        var host = new GameObject("RetentionHost");
        var client = host.AddComponent<ClubClient>();
        var fixture = ClubFixture();
        fixture.LastDay = fixture.ServerNow / 86400000;
        fixture.ShieldUntil = fixture.ServerNow + 3600000;
        SetField(client, "club", fixture);
        SetField(client, "page", "Club");
        client.SetModernForTest(true);
        yield return null;

        var text = string.Join("\n", AllText());
        StringAssert.Contains("Claimed today", text);
        StringAssert.Contains("00:00 UTC", text);
        StringAssert.Contains("1 CREDIT → 100 MONEY", text);
        StringAssert.Contains("FIRST BATTLE", text);
        StringAssert.Contains("Ranked or rated Revenge removes it", text);
        Assert.IsNotNull(ButtonWithText("CONVERT 1 CREDIT → 100 MONEY"));
        DestroyClient(host, client);
    }

    [UnityTest] public IEnumerator TransactionFailureIsVisibleAndRetryable()
    {
        var host = new GameObject("FailureHost");
        var client = host.AddComponent<ClubClient>();
        SetField(client, "club", ClubFixture());
        SetField(client, "failed", true);
        SetField(client, "status", "Stale catalog quote");
        SetField(client, "lastPayload", "{}");
        client.SetModernForTest(true);
        yield return null;

        StringAssert.Contains("Stale catalog quote", string.Join("\n", AllText()));
        Assert.IsNotNull(ButtonWithText("RETRY / REFRESH"));
        DestroyClient(host, client);
    }

    static ClubView ClubFixture() => new ClubView
    {
        Id = "dev-test", Name = "Test Club", Level = 2, Credits = 10, Money = 500,
        ConvertRate = 100, ServerNow = 10 * 86400000L + 1000, LastDay = 8,
        Fighters = Array.Empty<FighterView>(), Offers = Array.Empty<OfferView>(), Items = Array.Empty<ItemView>(),
        History = Array.Empty<HistoryView>(), RevengeTickets = Array.Empty<RevengeView>(),
        BbCatalog = Array.Empty<BbView>(), BbStock = Array.Empty<int>(), Shields = new[] { new ShieldView { Hours = 8, Credits = 2 } },
        CatalogVersion = "test", Catalog = new[] { new CatalogView { Id = "Pistol-MK2", Slot = "Weapon", Level = 3, Money = 200, EarlyAllowed = true, EarlyPrice = 2 } }
    };

    static string[] AllText() => Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Select(t => t.text).ToArray();

    static Button ButtonWithText(string text) => Object.FindObjectsByType<Button>(FindObjectsSortMode.None)
        .FirstOrDefault(b => b.GetComponentInChildren<Text>()?.text == text);

    static void PointerClick(Button button)
    {
        Assert.IsNotNull(EventSystem.current, "Pointer interaction requires an EventSystem.");
        Assert.IsTrue(button.IsInteractable(), button.GetComponentInChildren<Text>()?.text + " is not interactable");
        var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }

    static void SetField(object target, string name, object value) => target.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    static object GetField(object target, string name) => target.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);

    static void DestroyClient(GameObject host, ClubClient client)
    {
        if (client.Modern != null) Object.Destroy(client.Modern.gameObject);
        Object.Destroy(host);
    }

    [UnityTest] public IEnumerator ModernUiKeepsSelectionWithoutIdleRebuild()
    {
        var host = new GameObject("FocusHost");
        var client = host.AddComponent<ClubClient>();
        client.SetModernForTest(true);
        yield return null;
        var canvas = Object.FindFirstObjectByType<Canvas>();
        var button = Object.FindFirstObjectByType<Button>();
        Assert.IsNotNull(button);
        yield return null; // allow EventSystem's automatic first selection to settle
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        int childCount = canvas.transform.childCount;
        string signature = client.ModernRenderSignature();
        yield return null;
        Assert.AreSame(button.gameObject, EventSystem.current.currentSelectedGameObject, "Idle UI update lost keyboard focus.");
        Assert.AreEqual(signature, client.ModernRenderSignature(), "Visible state changed during the idle focus check.");
        Assert.AreEqual(childCount, canvas.transform.childCount, "Idle UI update rebuilt the Canvas hierarchy.");
        DestroyClient(host, client);
    }

    [UnityTest] public IEnumerator InputFocusAndTextSurviveIdleFrames()
    {
        var host = new GameObject("InputFocusHost");
        var client = host.AddComponent<ClubClient>();
        client.SetModernForTest(true);
        yield return null;
        var input = Object.FindFirstObjectByType<InputField>();
        input.Select();
        input.text = "dev-focus-proof";
        EventSystem.current.SetSelectedGameObject(input.gameObject);
        yield return null;
        yield return null;
        Assert.AreSame(input, Object.FindFirstObjectByType<InputField>());
        Assert.AreEqual("dev-focus-proof", input.text);
        Assert.AreSame(input.gameObject, EventSystem.current.currentSelectedGameObject);
        DestroyClient(host, client);
    }

    [UnityTest] public IEnumerator CreditsModalBlocksBackgroundButLeavesModalActionsUsable()
    {
        var host = new GameObject("ModalGateHost");
        var client = host.AddComponent<ClubClient>();
        SetField(client, "club", ClubFixture());
        SetField(client, "page", "Club");
        client.SetModernForTest(true);
        yield return null;
        PointerClick(ButtonWithText("CONVERT 1 CREDIT → 100 MONEY"));
        yield return null;
        yield return null;
        var confirm = ButtonWithText("CONFIRM 1 CREDITS");
        var cancel = ButtonWithText("CANCEL");
        var nav = ButtonWithText("Roster");
        Assert.IsTrue(confirm.IsInteractable());
        Assert.IsTrue(cancel.IsInteractable());
        Assert.IsFalse(nav.IsInteractable(), "Background navigation remained usable below a modal.");
        var pointer = PointerAtCenter(confirm);
        var hits = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hits);
        Assert.IsTrue(hits.Any(h => h.gameObject == confirm.gameObject || h.gameObject.transform.IsChildOf(confirm.transform)));
        DestroyClient(host, client);
    }

    static PointerEventData PointerAtCenter(Button button)
    {
        Canvas.ForceUpdateCanvases();
        var rt = (RectTransform)button.transform;
        var canvas = button.GetComponentInParent<Canvas>();
        return new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left,
            position = RectTransformUtility.WorldToScreenPoint(canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                rt.TransformPoint(rt.rect.center))
        };
    }

    [UnityTest] public IEnumerator TrainingButtonsStayInsideRowsAtSupportedReferenceResolutions()
    {
        foreach (var size in new[] { new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080) })
        {
            var host = new GameObject("TrainingGeometry" + size.x);
            var client = host.AddComponent<ClubClient>();
            var fixture = ClubFixture();
            fixture.Fighters = new[] { new FighterView { Id = "fighter-1", Name = "Fighter", Accuracy = 2, Endurance = 2, Agility = 2, TrainingCap = 10, MaxHp = 100000, Hp = 100000, Equipment = Array.Empty<EquipmentView>(), Ready = true } };
            SetField(client, "club", fixture);
            SetField(client, "selectedFighter", "fighter-1");
            SetField(client, "page", "Training");
            client.SetModernForTest(true);
            yield return null;
            Canvas.ForceUpdateCanvases();
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = size.x / 1600f;
            Canvas.ForceUpdateCanvases();
            foreach (var row in Object.FindObjectsByType<RectTransform>(FindObjectsSortMode.None).Where(r => r.name == "Stat"))
            {
                var button = row.GetComponentInChildren<Button>(true);
                Assert.IsNotNull(button);
                Assert.IsTrue(IsInside(button.GetComponent<RectTransform>(), row), size + " training button escaped its stat row");
                Assert.IsTrue(button.IsInteractable(), size + " training button unexpectedly disabled");
                Assert.IsFalse(button.GetComponentsInParent<CanvasGroup>(true).Any(g => !g.interactable || !g.blocksRaycasts), size + " training button blocked by CanvasGroup");
            }
            DestroyClient(host, client);
            yield return null;
        }
    }

    static bool IsInside(RectTransform child, RectTransform parent)
    {
        var corners = new Vector3[4];
        child.GetWorldCorners(corners);
        var parentCorners = new Vector3[4];
        parent.GetWorldCorners(parentCorners);
        const float tolerance = 1f;
        for (int i = 0; i < 4; i++)
        {
            if (corners[i].x < parentCorners[0].x - tolerance || corners[i].x > parentCorners[2].x + tolerance ||
                corners[i].y < parentCorners[0].y - tolerance || corners[i].y > parentCorners[2].y + tolerance)
                return false;
        }
        return true;
    }
}