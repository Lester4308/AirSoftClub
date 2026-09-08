using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    // Static bootstrap: creates the UGUI/Canvas infrastructure and the controller.
    public static class ControllerBoot
    {
        public static void Create(ClubClient client)
        {
            if (!client.useModern) return;
            var root = new GameObject("Modern UI");
            UnityEngine.Object.DontDestroyOnLoad(root);
            var controller = root.AddComponent<ModernUiController>();
            client.Modern = controller;
            controller.Attach(client);
        }
    }

    // Thin UGUI/Canvas owner. It builds the Canvas + EventSystem once and, each frame,
    // delegates the actual screen composition to ClubClient.RenderModernUI (a partial
    // method on the client, so it has full access to state and coroutines).
    public sealed class ModernUiController : MonoBehaviour
    {
        ClubClient C;
        Canvas _canvas;
        RectTransform _root;
        int lastRenderFrame = -1;
        string lastSignature;

        public RectTransform Root => _root;

        void Awake()
        {
            if (EventSystem.current == null)
            {
                var es = new GameObject("EventSystem");
                UnityEngine.Object.DontDestroyOnLoad(es);
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }

        public void Attach(ClubClient c)
        {
            C = c;
            BuildCanvas();
        }

        public void Clear()
        {
            if (_root == null) return;
            for (int i = _root.childCount - 1; i >= 0; i--)
            {
                var child = _root.GetChild(i).gameObject;
                child.SetActive(false);
                UnityEngine.Object.Destroy(child);
            }
        }

        void BuildCanvas()
        {
            var go = new GameObject("Canvas");
            go.transform.SetParent(transform, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            scaler.matchWidthOrHeight = 0.5f; // balanced scaling for 16:9 and slightly taller desktop windows
            var r = (RectTransform)go.transform;
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            _canvas = canvas; _root = r;
            // Background
            var bg = new GameObject("Background");
            bg.transform.SetParent(_root, false);
            bg.AddComponent<Image>().color = ModernStyle.Bg;
            bg.GetComponent<Image>().raycastTarget = false;
            var br = (RectTransform)bg.transform;
            br.anchorMin = Vector2.zero; br.anchorMax = Vector2.one;
            br.offsetMin = Vector2.zero; br.offsetMax = Vector2.zero;
        }

        void Update()
        {
            if (C == null || lastRenderFrame == Time.frameCount) return;
            lastRenderFrame = Time.frameCount;
            string signature = C.ModernRenderSignature();
            if (signature == lastSignature) return;
            lastSignature = signature;
            Clear();
            C.RenderModernUI(_root);
            RestoreSelection();
        }

        void RestoreSelection()
        {
            if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject != null) return;
            var first = _root.GetComponentInChildren<Selectable>(true);
            if (first != null && first.IsInteractable()) EventSystem.current.SetSelectedGameObject(first.gameObject);
        }
    }
}