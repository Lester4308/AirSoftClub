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
        // Simulate the runtime: a ClubClient in modern mode.
        var host = new GameObject("Host");
        var client = host.AddComponent<ClubClient>();
        client.SetModernForTest(true); // internal hook sets useModern + fires ControllerBoot

        yield return null;

        var canvas = Object.FindObjectOfType<Canvas>();
        Assert.IsNotNull(canvas, "UGUI Canvas was not created by the modern bootstrap.");
        Assert.IsNotNull(EventSystem.current, "EventSystem was not created.");

        // The controller should have produced some rendered UI (login screen).
        var texts = Object.FindObjectsOfType<Text>();
        bool any = false;
        foreach (var t in texts) if (t.raycastTarget == false && t.text.Length > 0) { any = true; break; }
        Assert.IsTrue(any, "Modern UI rendered no visible text elements.");

        Object.Destroy(host);
        var canvasObj = canvas.gameObject;
        Object.Destroy(canvasObj);
    }
}