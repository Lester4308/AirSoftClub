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
        CommandRequest pendingCreditsCommand;
        int pendingCreditsCost;
        string pendingCreditsTitle = "", pendingCreditsDetail = "";
        const float MX = 1600f, MY = 900f;

        // Rebuild only when visible state changes. This preserves keyboard focus and
        // prevents recreating the complete Canvas hierarchy every frame.
        public string ModernRenderSignature()
        {
            int replayFrame = page == "Battle" && replayResult != null ? Mathf.FloorToInt(replayTime / 33f) : 0;
            return string.Join("|", page, club?.Version.ToString() ?? "login", selectedFighter, selectedOffer,
                selectedItem, selectedTarget, shopCategory, mode, busy, failed, status, pendingCreditsCommand?.Key,
                replayFrame, inventorySlot, revengeOverlay, account);
        }

        // Invoked by ModernUiController when visible state changes.
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
            RenderTransactionStateModern(root);
            RenderCreditsConfirmationModern(root);
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
        // Canvas is width-locked at 1600x920 reference. We use a top-down layout:
        // y coordinates are distances from the TOP of the parent. PanelRect places a
        // rect anchored to the top-left of its parent at (xFromLeft, yFromTop).
        RectTransform PanelRect(string name, Transform parent, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform));
            r.transform.SetParent(parent, false);
            var rt = (RectTransform)r.transform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);         // top-left pivot
            rt.anchoredPosition = new Vector2(x, -y); // x from left, y from top
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
            // Top-left anchored: position via anchoredPosition (x, -y) + sizeDelta.
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
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

        // Fully-fills its parent top-left label (for simple titles/stretches).
        Text MLabelFill(string text, Transform parent, int size = 20, Color? color = null, TextAnchor align = TextAnchor.MiddleLeft)
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
            btn.interactable = ActionsEnabled;
            btn.onClick.AddListener(() => onClick());
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(.78f, .78f, .78f, 1f);
            colors.disabledColor = new Color(.42f, .42f, .42f, .65f);
            colors.fadeDuration = .08f;
            btn.colors = colors;
            // label
            var tgo = new GameObject("Label", typeof(RectTransform));
            tgo.transform.SetParent(rt, false);
            var trt = (RectTransform)tgo.transform;
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.pivot = new Vector2(0.5f, 0.5f);
            trt.offsetMin = new Vector2(12, 4); trt.offsetMax = new Vector2(-12, -4);
            var t = tgo.AddComponent<Text>();
            t.font = ModernStyle.Font(); t.fontSize = 16; t.fontStyle = FontStyle.Bold;
            t.color = ModernStyle.Ink; t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow; t.verticalOverflow = VerticalWrapMode.Overflow;
            t.text = label; t.raycastTarget = false;
            return btn;
        }

        void ConfirmCredits(CommandRequest command, int credits, string title, string detail)
        {
            pendingCreditsCommand = command;
            pendingCreditsCost = credits;
            pendingCreditsTitle = title;
            pendingCreditsDetail = detail;
        }

        void RenderCreditsConfirmationModern(RectTransform root)
        {
            if (pendingCreditsCommand == null) return;
            var shade = PanelRect("CreditsConfirmationShade", root, 0, 0, MX, MY);
            shade.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, .72f);
            var panel = PanelRect("CreditsConfirmation", shade, 500, 285, 600, 300);
            panel.gameObject.AddComponent<Image>().color = ModernStyle.PanelAlt;
            MLabel(pendingCreditsTitle, panel, 22, ModernStyle.Gold, TextAnchor.MiddleCenter);
            ((RectTransform)panel.GetChild(0)).sizeDelta = new Vector2(552, 50); ((RectTransform)panel.GetChild(0)).anchoredPosition = new Vector2(24, -28);
            MLabel(pendingCreditsDetail + "\n\nBalance is authoritative and will refresh from the server.", panel, 15, ModernStyle.Ink, TextAnchor.MiddleCenter);
            ((RectTransform)panel.GetChild(1)).sizeDelta = new Vector2(552, 110); ((RectTransform)panel.GetChild(1)).anchoredPosition = new Vector2(24, -88);
            MButton("CONFIRM " + pendingCreditsCost + " CREDITS", panel, 35, 220, 250, 52, () => { var command = pendingCreditsCommand; pendingCreditsCommand = null; StartCoroutine(Send(command)); }, ModernStyle.Gold);
            MButton("CANCEL", panel, 315, 220, 250, 52, () => pendingCreditsCommand = null, ModernStyle.Neutral);
        }

        void RenderTransactionStateModern(RectTransform root)
        {
            if (!busy && !failed) return;
            var bar = PanelRect("TransactionState", root, CX, 844, CW, 54);
            bar.gameObject.AddComponent<Image>().color = failed ? new Color(.42f, .12f, .08f, .98f) : ModernStyle.PanelAlt;
            MLabel(busy ? "SERVER TRANSACTION IN PROGRESS — waiting for authoritative receipt" : status, bar, 14, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)bar.GetChild(0)).sizeDelta = new Vector2(980, 46); ((RectTransform)bar.GetChild(0)).anchoredPosition = new Vector2(16, -4);
            if (failed) MButton(lastPayload == null ? "REFRESH STATE" : "RETRY / REFRESH", bar, 1050, 7, 240, 40, () => StartCoroutine(Retry()), ModernStyle.Orange);
        }

        // ---- Login ----
                void RenderLoginModern(RectTransform root)
                {
                    var panel = PanelRect("Login", root, 560, 320, 480, 280);
                    panel.anchorMin = new Vector2(0.5f, 1); panel.anchorMax = new Vector2(0.5f, 1); panel.pivot = new Vector2(0.5f, 0.5f);
                    panel.anchoredPosition = new Vector2(0, -480);
                    var bgImg = panel.gameObject.AddComponent<Image>();
                    bgImg.sprite = ModernStyle.WhiteSprite(); bgImg.color = ModernStyle.Panel;
                    bgImg.raycastTarget = false;

                    var title = MLabel("AIRSOFT CLUB", panel, 30, ModernStyle.Gold, TextAnchor.MiddleCenter);
                    var titleR = (RectTransform)title.transform;
                    titleR.anchorMin = new Vector2(0, 1); titleR.anchorMax = new Vector2(0, 1); titleR.pivot = new Vector2(0.5f, 0.5f);
                    titleR.sizeDelta = new Vector2(440, 60); titleR.anchoredPosition = new Vector2(240, -45);

                    var sub = MLabel("Build your tactical club.", panel, 16, ModernStyle.Muted, TextAnchor.MiddleCenter);
                    var subR = (RectTransform)sub.transform;
                    subR.anchorMin = new Vector2(0, 1); subR.anchorMax = new Vector2(0, 1); subR.pivot = new Vector2(0.5f, 0.5f);
                    subR.sizeDelta = new Vector2(440, 30); subR.anchoredPosition = new Vector2(240, -95);

                    // Account input
                    var inpR = PanelRect("Input", panel, 240, -130, 420, 48);
                    inpR.anchorMin = new Vector2(0.5f, 0.5f); inpR.anchorMax = new Vector2(0.5f, 0.5f); inpR.pivot = new Vector2(0.5f, 0.5f);
                    inpR.anchoredPosition = new Vector2(0, -10);
                    var inpBg = inpR.gameObject.AddComponent<Image>();
                    inpBg.sprite = ModernStyle.WhiteSprite(); inpBg.color = ModernStyle.Bg2; inpBg.raycastTarget = true;
                    var inp = inpR.gameObject.AddComponent<InputField>();
                    inp.textComponent = MLabel(account, inpR, 18, ModernStyle.Ink, TextAnchor.MiddleLeft);
                    var inpText = (RectTransform)inp.textComponent.transform;
                    inpText.sizeDelta = new Vector2(400, 40); inpText.anchoredPosition = new Vector2(0, 0);

                    MButton("ENTER CLUB", panel, 30, 175, 420, 50, () => StartCoroutine(Login()), ModernStyle.Gold);
                    var s = MLabel(status, panel, 13, ModernStyle.Orange, TextAnchor.MiddleCenter);
                    var sR = (RectTransform)s.transform;
                    sR.anchorMin = new Vector2(0, 1); sR.anchorMax = new Vector2(0, 1); sR.pivot = new Vector2(0.5f, 0.5f);
                    sR.sizeDelta = new Vector2(440, 30); sR.anchoredPosition = new Vector2(240, -240);
                }

        void RenderChrome(RectTransform root)
        {
            // Nav sidebar (left, full height)
            var nav = PanelRect("Nav", root, 0, 0, 220, MY); // anchored top-left
            nav.anchorMin = new Vector2(0, 0); nav.anchorMax = new Vector2(0, 1);
            nav.sizeDelta = new Vector2(220, 0); nav.anchoredPosition = new Vector2(0, 0);
            nav.gameObject.AddComponent<Image>().color = ModernStyle.Bg2;
            nav.GetComponent<Image>().raycastTarget = false;

            var logo = MLabel("AIRSOFT\nCLUB", nav, 26, ModernStyle.Gold, TextAnchor.MiddleCenter);
            var logoR = (RectTransform)logo.transform;
            logoR.anchorMin = new Vector2(0, 1); logoR.anchorMax = new Vector2(0, 1); logoR.pivot = new Vector2(0.5f, 1);
            logoR.sizeDelta = new Vector2(200, 90); logoR.anchoredPosition = new Vector2(110, -20);

            string[] tabs = { "Club", "Roster", "Recruitment", "Training", "Shop", "BB", "Opponents", "History", "Settings" };
            float ty = 130;
            foreach (var t in tabs)
            {
                var idx = t;
                var b = MButton(idx, nav, 15, ty, 190, 42, () => { page = idx; scroll = Vector2.zero; inventorySlot = ""; },
                    page == idx ? ModernStyle.Neutral : ModernStyle.Card);
                ((RectTransform)b.transform).anchoredPosition = new Vector2(15, -(ty + 42));
                ty += 52;
            }

            // Header resources (top bar, left of nav)
            var head = PanelRect("Header", root, 220, 12, 1380, 64);
            head.anchorMin = new Vector2(0, 1); head.anchorMax = new Vector2(1, 1);
            head.sizeDelta = new Vector2(-220, 64); head.anchoredPosition = new Vector2(220, -12);
            head.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            head.GetComponent<Image>().raycastTarget = false;
            MLabel(club != null ? club.Name : "", head, 24, ModernStyle.Ink, TextAnchor.MiddleLeft);
            var nameR = (RectTransform)head.GetChild(head.childCount - 1);
            nameR.sizeDelta = new Vector2(300, 60); nameR.anchoredPosition = new Vector2(30, -32);

            MLabel("Lv " + (club?.Level.ToString() ?? "") + "  ·  " + (club?.Rating.ToString() ?? ""), head, 15, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var lvR = (RectTransform)head.GetChild(head.childCount - 1);
            lvR.sizeDelta = new Vector2(300, 60); lvR.anchoredPosition = new Vector2(360, -32);

            WalletModern(head, "MONEY", club?.Money.ToString("N0") ?? "", ModernStyle.Gold, new Vector2(980, 32));
            WalletModern(head, "CREDITS", club?.Credits.ToString("N0") ?? "", ModernStyle.Blue, new Vector2(1170, 32));
        }

        void WalletModern(Transform parent, string name, string amount, Color c, Vector2 pos)
        {
            var w = PanelRect(name, parent, pos.x, pos.y, 170, 44);
            w.anchorMin = new Vector2(0, 1); w.anchorMax = new Vector2(0, 1); w.pivot = new Vector2(0, 0.5f);
            w.anchoredPosition = new Vector2(pos.x, -pos.y);
            w.gameObject.AddComponent<Image>().color = ModernStyle.Bg2;
            w.GetComponent<Image>().raycastTarget = false;
            var l = MLabel(name, w, 11, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var lR = (RectTransform)l.transform; lR.sizeDelta = new Vector2(160, 20); lR.anchoredPosition = new Vector2(8, -8);
            var v = MLabel(amount, w, 16, c, TextAnchor.MiddleLeft);
            var vR = (RectTransform)v.transform; vR.sizeDelta = new Vector2(160, 24); vR.anchoredPosition = new Vector2(8, -24);
        }
    }
}