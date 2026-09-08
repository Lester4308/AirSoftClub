using System.Collections;
using AirsoftClub.Unity;
using NUnit.Framework;
using UnityEngine;
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