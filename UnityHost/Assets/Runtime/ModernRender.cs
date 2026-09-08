using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    // Modern UGUI screens — partial of ClubClient so it can read state and call coroutines.
    public sealed partial class ClubClient
    {
        // Canvas-space utilities (built under root via ModernStyle).
        RectTransform MRoot;
        const float MX = 1600f, MY = 920f;

        // Invoked every frame by ModernUiController.
        public void RenderModernUI(RectTransform root)
        {
            MRoot = root;
            Rect bg;
            var bgImg = root.GetComponentInChildren<Image>();
            // Background is the only non-cleared child (children persist via root)
            // We rebuild all dynamic content under a container that is cleared each frame.
            ClearModern(root);
            DrawBackground(root);
            if (club == null) { RenderLoginModern(root); return; }
            RenderChrome(root);
            switch (page)
            {
                case "Club": RenderHubModern(root); break;
                case "Recruitment": RenderRecruitmentModern(root); break;
                case "Roster":
                case "Training":
                case "Equipment":
                case "Recovery": RenderRosterModern(root); break;
                case "Shop":
                case "Supply":
                case "BB": RenderShopModern(root); break;
                case "Opponents": RenderOpponentsModern(root); break;
                case "Battle": RenderBattleModern(root); break;
                case "History": RenderHistoryModern(root); break;
                case "Status": RenderStatusModern(root); break;
                case "ArtSheet": RenderArtModern(root); break;
                default: RenderHubModern(root); break;
            }
        }

        void ClearModern(RectTransform root)
        {
            // Keep index 0 (background), destroy the rest.
            for (int i = root.childCount - 1; i >= 1; i--)
                UnityEngine.Object.Destroy(root.GetChild(i).gameObject);
        }

        void DrawBackground(RectTransform root)
        {
            var bg = new GameObject("Stage");
            bg.transform.SetParent(root, false);
            var img = bg.AddComponent<Image>();
            img.color = ModernStyle.Bg;
            img.raycastTarget = false;
            var r = (RectTransform)bg.transform;
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
            // subtle vignette band at top
            var band = new GameObject("TopBand");
            band.transform.SetParent(bg.transform, false);
            var bImg = band.AddComponent<Image>();
            bImg.color = new Color(0, 0, 0, 0.16f);
            bImg.raycastTarget = false;
            var br = (RectTransform)band.transform;
            br.anchorMin = Vector2.zero; br.anchorMax = new Vector2(1, 1);
            br.sizeDelta = new Vector2(0, 200);
            br.anchoredPosition = new Vector2(0, 0);
            br.pivot = new Vector2(0.5f, 1f);
        }

        // ---- Shared helpers ----
        RectTransform PanelRect(string name, Transform parent, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform));
            r.transform.SetParent(parent, false);
            var rt = (RectTransform)r.transform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.zero;
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
            return rt;
        }

        Image PanelBg(Transform parent, Color c)
        {
            var img = PanelRect("Panel", parent, 0, 0, 0, 0).gameObject;
            var i = img.AddComponent<Image>();
            i.sprite = ModernStyle.WhiteSprite();
            i.color = c;
            i.raycastTarget = false;
            return i;
        }

        Text MLabel(string text, Transform parent, int size = 20, Color? color = null, TextAnchor align = TextAnchor.MiddleLeft)
        {
            var go = new GameObject("T", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.font = ModernStyle.Font();
            t.fontSize = size;
            t.color = color ?? ModernStyle.Ink;
            t.alignment = align;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.text = text;
            t.raycastTarget = false;
            return t;
        }

        Button MButton(string label, Transform parent, float x, float y, float w, float h, Action onClick, Color? fill = null)
        {
            var rt = PanelRect("Btn", parent, x, y, w, h);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = ModernStyle.WhiteSprite();
            img.color = fill ?? ModernStyle.Card;
            var btn = rt.gameObject.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick());
            // label
            var tgo = new GameObject("Label", typeof(RectTransform));
            tgo.transform.SetParent(rt, false);
            var trt = (RectTransform)tgo.transform;
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            var t = tgo.AddComponent<Text>();
            t.font = ModernStyle.Font(); t.fontSize = 16; t.fontStyle = FontStyle.Bold;
            t.color = ModernStyle.Ink; t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow; t.verticalOverflow = VerticalWrapMode.Overflow;
            t.text = label; t.raycastTarget = false;
            return btn;
        }

        // ---- Login ----
        void RenderLoginModern(RectTransform root)
        {
            var panel = PanelRect("Login", root, 560, 330, 480, 280);
            var bgImg = panel.gameObject.AddComponent<Image>();
            bgImg.sprite = ModernStyle.WhiteSprite(); bgImg.color = ModernStyle.Panel;
            bgImg.raycastTarget = false;
            var border = PanelRect("Border", panel, 0, -280, 480, 2);
            border.gameObject.AddComponent<Image>().color = ModernStyle.Gold;
            border.GetComponent<Image>().raycastTarget = false;

            var title = MLabel("AIRSOFT CLUB", panel, 30, ModernStyle.Gold, TextAnchor.MiddleCenter);
            ((RectTransform)title.transform).anchoredPosition = new Vector2(0, 0);
            ((RectTransform)title.transform).sizeDelta = new Vector2(440, 60);

            var sub = MLabel("Build your tactical club.", panel, 16, ModernStyle.Muted, TextAnchor.MiddleCenter);
            var subR = (RectTransform)sub.transform;
            subR.sizeDelta = new Vector2(440, 30); subR.anchoredPosition = new Vector2(0, -66);

            // Account input
            var inpR = PanelRect("Input", panel, 0, -110, 420, 48);
            inpR.anchorMin = new Vector2(0.5f, 0.5f); inpR.anchorMax = new Vector2(0.5f, 0.5f);
            var inpBg = inpR.gameObject.AddComponent<Image>();
            inpBg.sprite = ModernStyle.WhiteSprite(); inpBg.color = ModernStyle.Bg2; inpBg.raycastTarget = true;
            var inp = inpR.gameObject.AddComponent<InputField>();
            inp.textComponent = MLabel(account, inpR, 18, ModernStyle.Ink, TextAnchor.MiddleLeft);
            var inpText = (RectTransform)inp.textComponent.transform;
            inpText.sizeDelta = new Vector2(400, 40); inpText.anchoredPosition = new Vector2(0, 0);
            inp.text = account; inp.characterLimit = 44;
            inp.onValueChanged.AddListener(v => account = v);

            MButton("ENTER CLUB", panel, 0, -180, 420, 50, () => StartCoroutine(Login()), ModernStyle.Gold).transform.localScale = Vector3.one;
            var s = MLabel(status, panel, 13, ModernStyle.Orange, TextAnchor.MiddleCenter);
            ((RectTransform)s.transform).sizeDelta = new Vector2(440, 30);
            ((RectTransform)s.transform).anchoredPosition = new Vector2(0, -245);
        }

        void RenderChrome(RectTransform root)
        {
            // Nav sidebar
            var nav = PanelRect("Nav", root, 0, 0, 220, 920);
            nav.anchorMin = Vector2.zero; nav.anchorMax = new Vector2(0, 1);
            nav.gameObject.AddComponent<Image>().color = ModernStyle.Bg2;
            nav.GetComponent<Image>().raycastTarget = false;
            var navB = PanelRect("Edge", nav, 0, 0, 220, 920);
            navB.gameObject.AddComponent<Image>().color = ModernStyle.Edge;

            var logo = MLabel("AIRSOFT\nCLUB", nav, 26, ModernStyle.Gold, TextAnchor.MiddleCenter);
            ((RectTransform)logo.transform).sizeDelta = new Vector2(200, 90);
            ((RectTransform)logo.transform).anchoredPosition = new Vector2(0, 400);

            string[] tabs = { "Club", "Roster", "Recruitment", "Training", "Shop", "BB", "Opponents", "History", "Settings" };
            float ty = 320;
            foreach (var t in tabs)
            {
                var idx = t;
                var b = MButton(idx, nav, 15, ty, 190, 42, () => { page = idx; scroll = Vector2.zero; inventorySlot = ""; },
                    page == idx ? ModernStyle.Neutral : ModernStyle.Card);
                ((RectTransform)b.transform).anchoredPosition = new Vector2(0, ty);
                ty -= 50;
            }

            // Header resources
            var head = PanelRect("Header", root, 220, 0, 1380, 64);
            head.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            head.GetComponent<Image>().raycastTarget = false;
            MLabel(club != null ? club.Name : "", head, 24, ModernStyle.Ink, TextAnchor.MiddleLeft);
            var nameR = (RectTransform)head.transform.GetChild(head.childCount - 1);
            nameR.sizeDelta = new Vector2(300, 60); nameR.anchoredPosition = new Vector2(30, 0);

            MLabel("Lv " + (club?.Level.ToString() ?? "") + "  ·  " + (club?.Rating.ToString() ?? ""), head, 15, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var lvR = (RectTransform)head.transform.GetChild(head.childCount - 1);
            lvR.sizeDelta = new Vector2(300, 60); lvR.anchoredPosition = new Vector2(360, 0);

            WalletModern(head, "MONEY", club?.Money.ToString("N0") ?? "", ModernStyle.Gold, new Vector2(980, 0));
            WalletModern(head, "CREDITS", club?.Credits.ToString("N0") ?? "", ModernStyle.Blue, new Vector2(1170, 0));
        }

        void WalletModern(Transform parent, string name, string amount, Color c, Vector2 pos)
        {
            var w = PanelRect(name, parent, pos.x, pos.y, 170, 44);
            w.gameObject.AddComponent<Image>().color = ModernStyle.Bg2;
            w.GetComponent<Image>().raycastTarget = false;
            var l = MLabel(name, w, 11, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var lR = (RectTransform)l.transform; lR.sizeDelta = new Vector2(160, 20); lR.anchoredPosition = new Vector2(8, 10);
            var v = MLabel(amount, w, 16, c, TextAnchor.MiddleLeft);
            var vR = (RectTransform)v.transform; vR.sizeDelta = new Vector2(160, 24); vR.anchoredPosition = new Vector2(8, -10);
        }
    }
}