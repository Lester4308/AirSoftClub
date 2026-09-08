using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        Vector2 recruitmentScroll;
        Vector2 opponentsScroll;
        Vector2 historyScroll;
        Vector2 ammoScroll;

        RectTransform VerticalScrollArea(string name, RectTransform root, float x, float y, float width, float height, float contentHeight, Vector2 position, Action<Vector2> savePosition)
        {
            var viewport = PanelRect(name, root, x, y, width, height);
            var background = viewport.gameObject.AddComponent<Image>();
            background.color = ModernStyle.Panel;
            background.raycastTarget = true;
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = PanelRect("Content", viewport, 0, 0, width, Mathf.Max(height, contentHeight));
            float maxOffset = Mathf.Max(0f, content.sizeDelta.y - height);
            content.anchoredPosition = new Vector2(0f, Mathf.Clamp(position.y, 0f, maxOffset));

            var scrollRect = viewport.gameObject.AddComponent<ScrollRect>();
            scrollRect.content = content;
            scrollRect.viewport = viewport;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 35f;
            scrollRect.onValueChanged.AddListener(_ => savePosition(new Vector2(0f, Mathf.Clamp(content.anchoredPosition.y, 0f, maxOffset))));
            return content;
        }

        // ---- Recruitment ----
        void RenderRecruitmentModern(RectTransform root)
        {
            MLabel("RECRUITMENT", root, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(500, 36);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -CY);

            if (club == null) return;
            MLabel("Pool " + club.OfferVersion + "  ·  " + club.CompletedSinceRefresh + "/10 battles  ·  refresh in " + Math.Max(0, (club.RefreshAvailableAt - club.ServerNow) / 60000) + " min", root, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(700, 30);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -(CY + 38));

            // Compact two-column candidate grid leaves room for the detail panel.
            int offerRows = Mathf.CeilToInt(club.Offers.Length / 2f);
            var offers = VerticalScrollArea("Offers", root, CX, CY + 80, 515, CH - 80, offerRows * 145 - 15, recruitmentScroll, value => recruitmentScroll = value);
            for (int n = 0; n < club.Offers.Length; n++)
            {
                var o = club.Offers[n];
                var card = PanelRect("Offer" + n, offers, n % 2 * 265, n / 2 * 145, 250, 130);
                card.gameObject.AddComponent<Image>().color = o.Id == selectedOffer ? ModernStyle.Neutral : ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                var recruit = Male017UiView.Create(card, "Candidate", new Vector2(68, 108));
                recruit.Root.anchoredPosition = new Vector2(8, -8);
                recruit.Apply(false, false, false, false, true, true);
                MLabel(o.Name, card, 17, ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(card.childCount - 1)).sizeDelta = new Vector2(155, 28); ((RectTransform)card.GetChild(card.childCount - 1)).anchoredPosition = new Vector2(82, -12);
                MLabel("A " + o.Accuracy + "   E " + o.Endurance + "   G " + o.Agility, card, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(card.childCount - 1)).sizeDelta = new Vector2(155, 40); ((RectTransform)card.GetChild(card.childCount - 1)).anchoredPosition = new Vector2(82, -45);
                MButton("INSPECT", card, 14, 82, 120, 38, () => selectedOffer = o.Id, ModernStyle.Blue);
            }

            // Detailed selected candidate + hire
            var choice = club.Offers.FirstOrDefault(o => o.Id == selectedOffer) ?? club.Offers.FirstOrDefault(o => o.Id.EndsWith("-0"));
            if (choice == null) return;
            selectedOffer = choice.Id;

            var det = PanelRect("Detail", root, CX + 550, CY + 80, 760, 330);
            det.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            det.GetComponent<Image>().raycastTarget = false;

            var candidatePortrait = Male017UiView.Create(det, "SelectedCandidate", new Vector2(190, 285));
            candidatePortrait.Root.anchoredPosition = new Vector2(540, -20);
            candidatePortrait.Apply(false, false, false, false, true, true);

            MLabel("CHOOSE A FIGHTER", det, 18, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(det.childCount - 1)).sizeDelta = new Vector2(500, 34); ((RectTransform)det.GetChild(det.childCount - 1)).anchoredPosition = new Vector2(24, -24);

            MLabel(choice.Name.ToUpperInvariant(), det, 22, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(det.childCount - 1)).sizeDelta = new Vector2(500, 40); ((RectTransform)det.GetChild(det.childCount - 1)).anchoredPosition = new Vector2(24, -70);

            MLabel("ACC " + choice.Accuracy + "    END " + choice.Endurance + "    AGI " + choice.Agility, det, 16, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(det.childCount - 1)).sizeDelta = new Vector2(500, 34); ((RectTransform)det.GetChild(det.childCount - 1)).anchoredPosition = new Vector2(24, -110);

            // Compare to current selected fighter
            var baseline = club.Fighters.FirstOrDefault(f => f.Id == selectedFighter);
            if (baseline != null)
            {
                var cmp = "vs " + baseline.Name + ":  " + (choice.Accuracy - baseline.Accuracy).ToString("+0;-0;0") + " / " + (choice.Endurance - baseline.Endurance).ToString("+0;-0;0") + " / " + (choice.Agility - baseline.Agility).ToString("+0;-0;0");
                MLabel(cmp, det, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)det.GetChild(det.childCount - 1)).sizeDelta = new Vector2(500, 30); ((RectTransform)det.GetChild(det.childCount - 1)).anchoredPosition = new Vector2(24, -150);
            }

            bool free = !club.FreeRecruitClaimed && (choice.Id.EndsWith("-0") || choice.Id.EndsWith("-1") || choice.Id.EndsWith("-2"));
            MButton(free ? "CHOOSE FREE RECRUIT" : "HIRE / " + choice.Price + " MONEY", det, 24, 220, 560, 52, () => StartCoroutine(Send(Intent("Hire", choice.Id, flag: free))), ModernStyle.Gold);
            MLabel("One permanent starter choice. All ready fighters join battles.", det, 12, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(det.childCount - 1)).sizeDelta = new Vector2(560, 26);
            ((RectTransform)det.GetChild(det.childCount - 1)).anchoredPosition = new Vector2(24, -285);
        }

        // ---- Shop ----
        void RenderShopModern(RectTransform root)
        {
            MLabel(page == "BB" ? "AMMUNITION SUPPLY" : "CLUB ARMORY", root, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(500, 36);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -CY);
            if (club == null) return;

            if (page == "BB") { RenderAmmoModern(root); return; }

            // Category list (left)
            string[] cats = { "Weapons", "Head Protection", "Armor", "Camouflage" };
            for (int n = 0; n < cats.Length; n++)
            {
                var cat = cats[n];
                var cbtn = MButton(cat, root, CX, CY + 60 + n * 55, 180, 44, () => { shopCategory = cat; catalogScroll = Vector2.zero; }, shopCategory == cat ? ModernStyle.Neutral : ModernStyle.Card);
                ((RectTransform)cbtn.transform).anchoredPosition = new Vector2(CX, -(CY + 60 + n * 55));
            }

            // Scrollable grid of catalog entries.
            var entries = club.Catalog
                .Where(i => shopCategory == "Weapons" ? i.Slot == "Weapon" : shopCategory == "Head Protection" ? i.Slot == "HeadProtection" : shopCategory == "Armor" ? i.Slot == "LoadBearingArmor" : i.Slot == "Camouflage")
                .ToArray();
            int cols = 4;
            int rows = Mathf.CeilToInt(entries.Length / (float)cols);
            var catalog = VerticalScrollArea("Catalog", root, CX + 220, CY + 60, CW - 220, CH - 60, rows * 135 - 10, catalogScroll, value => catalogScroll = value);
            for (int n = 0; n < entries.Length; n++)
            {
                var i = entries[n];
                var card = PanelRect("Item" + n, catalog, n % cols * 270, n / cols * 135, 255, 125);
                card.gameObject.AddComponent<Image>().color = ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                bool camoPreview = i.Slot == "Camouflage";
                var itemPreview = Male017UiView.Create(card, "Preview", new Vector2(56, 92));
                itemPreview.Root.anchoredPosition = new Vector2(6, -8);
                itemPreview.Apply(camoPreview, i.Slot == "HeadProtection", i.Slot == "LoadBearingArmor",
                    i.Slot == "Weapon", true, true);
                MLabel(i.Id, card, 15, i.Credits > 0 ? ModernStyle.Gold : ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(card.childCount - 1)).sizeDelta = new Vector2(180, 24); ((RectTransform)card.GetChild(card.childCount - 1)).anchoredPosition = new Vector2(68, -10);
                MLabel("LV " + i.Level + "  ·  " + (i.Credits > 0 ? i.Credits + " Credits" : i.Money + " Money"), card, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(card.childCount - 1)).sizeDelta = new Vector2(180, 22); ((RectTransform)card.GetChild(card.childCount - 1)).anchoredPosition = new Vector2(68, -36);
                MLabel(i.Slot == "Weapon" ? ("DMG " + i.Damage + " · int " + i.Interval + "ms") : ("PROT " + i.Protection + " · AGI-PEN " + i.AgilityPenalty), card, 12, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(card.childCount - 1)).sizeDelta = new Vector2(180, 22); ((RectTransform)card.GetChild(card.childCount - 1)).anchoredPosition = new Vector2(68, -60);
                if (i.EarlyAllowed)
                {
                    MButton("UNLOCK EARLY / " + i.EarlyPrice + " CREDITS", card, 14, 86, 225, 32,
                        () => ConfirmCredits(Intent("EarlyUnlock", i.Id), i.EarlyPrice,
                            "EARLY ACCESS", "Unlock access to " + i.Id + ". The item is still purchased separately with Money."), ModernStyle.Gold);
                }
                else
                {
                    MButton("BUY", card, 14, 86, 105, 32, () =>
                    {
                        if (i.Credits > 0)
                            ConfirmCredits(Intent("Buy", i.Id), (int)i.Credits, "CREDITS PURCHASE", "Buy " + i.Id + " using Credits.");
                        else StartCoroutine(Send(Intent("Buy", i.Id)));
                    }, i.Credits > 0 ? ModernStyle.Gold : ModernStyle.Blue);
                }
            }
        }

        void RenderAmmoModern(RectTransform root)
        {
            MLabel("SHARED BB RESERVE", root, 18, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(500, 34);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -(CY + 45));
            if (club == null || club.BbCatalog == null) return;

            var ammo = VerticalScrollArea("Ammo", root, CX, CY + 90, 810, CH - 90, club.BbCatalog.Length * 110 - 14, ammoScroll, value => ammoScroll = value);
            for (int n = 0; n < club.BbCatalog.Length; n++)
            {
                var b = club.BbCatalog[n];
                int tier = n;
                var card = PanelRect("Ball" + n, ammo, 0, n * 110, 810, 96);
                card.gameObject.AddComponent<Image>().color = n == club.ActiveBbTier ? ModernStyle.Neutral : ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                MLabel(b.Name + (club.ActiveBbTier == n ? "  / ACTIVE" : ""), card, 16, n == club.ActiveBbTier ? ModernStyle.Gold : ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(0)).sizeDelta = new Vector2(400, 34); ((RectTransform)card.GetChild(0)).anchoredPosition = new Vector2(20, -20);
                MLabel((club.BbStock[n]) + " / " + club.Capacity + " BB", card, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(1)).sizeDelta = new Vector2(300, 26); ((RectTransform)card.GetChild(1)).anchoredPosition = new Vector2(20, -60);
                MButton("SELECT TIER", card, 400, 20, 170, 44, () => StartCoroutine(Send(Intent("BbTier", number: tier))), n == club.ActiveBbTier ? ModernStyle.Neutral : ModernStyle.Blue);
                MButton("REFILL / " + (b.Money > 0 ? b.Money + " MONEY" : b.Credits + " CREDITS"), card, 590, 20, 200, 44, () => StartCoroutine(Send(Intent("Refill", number: tier))), ModernStyle.Gold);
            }
        }

        // ---- Opponents ----
        void RenderOpponentsModern(RectTransform root)
        {
            MLabel("CHOOSE YOUR NEXT CHALLENGE", root, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(700, 36);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -CY);

            if (club == null) return;
            // Mode tabs
            string[] modes = { "Practice", "Ranked", "Friend" };
            for (int n = 0; n < modes.Length; n++)
            {
                var m = modes[n];
                MButton(m.ToUpperInvariant(), root, CX + n * 130, CY + 45, 120, 40, () => mode = m, mode == m ? ModernStyle.Neutral : ModernStyle.Card);
                ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX + n * 130, -(CY + 45));
            }

            if (rivals == null || rivals.Length == 0)
            {
                MLabel("No valid opponents at the moment.", root, 16, ModernStyle.Muted, TextAnchor.MiddleCenter);
                var rt = (RectTransform)root.GetChild(root.childCount - 1);
                rt.sizeDelta = new Vector2(600, 40); rt.anchoredPosition = new Vector2(CX + 300, -(CY + 220));
                return;
            }

            // Opponent list (left)
            var list = VerticalScrollArea("Rivals", root, CX, CY + 100, 700, CH - 100, 24 + rivals.Length * 124, opponentsScroll, value => opponentsScroll = value);
            for (int n = 0; n < rivals.Length; n++)
            {
                var r = rivals[n];
                var card = PanelRect("Rival" + n, list, 15, 12 + n * 124, 660, 112);
                card.gameObject.AddComponent<Image>().color = r.Id == selectedTarget ? ModernStyle.Neutral : ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                MLabel(r.Name, card, 17, ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(0)).sizeDelta = new Vector2(350, 30); ((RectTransform)card.GetChild(0)).anchoredPosition = new Vector2(18, -16);
                MLabel("Lv " + r.Level + "  ·  " + r.Fighters + " fighters  ·  " + r.Category, card, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(1)).sizeDelta = new Vector2(350, 26); ((RectTransform)card.GetChild(1)).anchoredPosition = new Vector2(18, -50);
                var ct = r.Category.ToUpperInvariant();
                MLabel(ct, card, 13, WebStyleCatColor(r), TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(2)).sizeDelta = new Vector2(200, 26); ((RectTransform)card.GetChild(2)).anchoredPosition = new Vector2(400, -16);
                MButton("SELECT", card, 500, 30, 120, 46, () => selectedTarget = r.Id, ModernStyle.Gold);
            }

            // Detail (right)
            var det = PanelRect("OppDetail", root, CX + 720, CY + 100, 590, 420);
            det.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            det.GetComponent<Image>().raycastTarget = false;
            var chosen = rivals.FirstOrDefault(r => r.Id == selectedTarget) ?? (rivals.Length > 0 ? rivals[0] : null);
            if (chosen == null) { if (rivals.Length > 0) selectedTarget = rivals[0].Id; return; }
            selectedTarget = chosen.Id;

            MLabel(chosen.Name.ToUpperInvariant(), det, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(0)).sizeDelta = new Vector2(500, 40); ((RectTransform)det.GetChild(0)).anchoredPosition = new Vector2(24, -24);
            MLabel(chosen.Category + " MATCHUP  /  " + chosen.Fighters + " fighters", det, 16, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(1)).sizeDelta = new Vector2(500, 30); ((RectTransform)det.GetChild(1)).anchoredPosition = new Vector2(24, -80);
            MLabel(mode + "  ·  Rating " + chosen.Rating + "  ·  " + (chosen.Protected ? "Protected" : "Open"), det, 15, chosen.Protected ? ModernStyle.Orange : ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(2)).sizeDelta = new Vector2(500, 30); ((RectTransform)det.GetChild(2)).anchoredPosition = new Vector2(24, -120);

            MLabel("Full-strength win: " + chosen.PreviewWinMoney + " Money  ·  " + chosen.PreviewWinClubXp + " Club XP", det, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(3)).sizeDelta = new Vector2(520, 26); ((RectTransform)det.GetChild(3)).anchoredPosition = new Vector2(24, -160);

            if (!chosen.Protected)
                MButton("CONFIRM " + mode.ToUpperInvariant() + " BATTLE", det, 24, 220, 520, 54, () => StartCoroutine(Send(Intent("Attack", chosen.Id, mode))), ModernStyle.Gold);

            var ticket = (club.RevengeTickets ?? Array.Empty<RevengeView>()).FirstOrDefault(t => t.Target == chosen.Id);
            if (ticket != null)
                MButton("REVENGE · " + ticket.Attempts + "/3", det, 24, 290, 520, 46, () => { var c = Intent("Attack", ticket.Target, "Revenge"); c.Ticket = ticket.Origin; StartCoroutine(Send(c)); }, ModernStyle.Orange);
        }

        Color WebStyleCatColor(RivalView r)
        {
            return r.Category == null || r.Category.Contains("Weak") ? ModernStyle.Blue : r.Category.Contains("Strong") ? ModernStyle.Orange : ModernStyle.Muted;
        }

        // ---- History ----
        void RenderHistoryModern(RectTransform root)
        {
            MLabel("BATTLE HISTORY", root, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(500, 36);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -CY);
            if (club == null || club.History == null) return;

            if (club.History.Length == 0)
            {
                MLabel("No battles yet. Go pick a fight.", root, 16, ModernStyle.Muted, TextAnchor.MiddleCenter);
                var rt = (RectTransform)root.GetChild(root.childCount - 1);
                rt.sizeDelta = new Vector2(600, 40); rt.anchoredPosition = new Vector2(CX + 300, -(CY + 220));
                return;
            }
            int historyRows = Mathf.CeilToInt(club.History.Length / 2f);
            var history = VerticalScrollArea("History", root, CX, CY + 60, CW, CH - 60, historyRows * 66 - 8, historyScroll, value => historyScroll = value);
            for (int n = 0; n < club.History.Length; n++)
            {
                var h = club.History[n];
                var card = PanelRect("Rec" + n, history, n % 2 * 660, n / 2 * 66, 640, 58);
                card.gameObject.AddComponent<Image>().color = ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                var side = (h.Attacker == club.Id ? "ATTACK" : "DEFENSE") + "  /  " + h.Mode.ToUpperInvariant();
                MLabel(side, card, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(0)).sizeDelta = new Vector2(170, 28); ((RectTransform)card.GetChild(0)).anchoredPosition = new Vector2(14, -14);
                MLabel(h.Outcome + "  ·  Money +" + h.Money + "  ·  Rating " + (h.RatingKnown ? h.RatingDelta.ToString("+0;-0;0") : "unknown"), card, 13, ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(1)).sizeDelta = new Vector2(310, 26); ((RectTransform)card.GetChild(1)).anchoredPosition = new Vector2(180, -14);
                MButton("REVIEW", card, 500, 8, 120, 42, () => StartCoroutine(LoadMatch(h.Id)), ModernStyle.Blue);
            }
        }

        // ---- Status ----
        void RenderStatusModern(RectTransform root)
        {
            var c = PanelRect("Status", root, CX, CY, CW, CH);
            c.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            c.GetComponent<Image>().raycastTarget = false;
            MLabel("SERVER & PROFILE STATUS", c, 20, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)c.GetChild(0)).sizeDelta = new Vector2(500, 34); ((RectTransform)c.GetChild(0)).anchoredPosition = new Vector2(24, -24);
            MLabel("Version " + (club?.Version.ToString() ?? "-") + "  ·  " + (club?.ConfigVersion ?? "-"), c, 15, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)c.GetChild(1)).sizeDelta = new Vector2(900, 30); ((RectTransform)c.GetChild(1)).anchoredPosition = new Vector2(24, -60);
            MLabel(club != null && club.DefensePublished ? "Defense published: full HP / virtual BB" : "Recruit a fighter to publish defense.", c, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)c.GetChild(2)).sizeDelta = new Vector2(900, 30); ((RectTransform)c.GetChild(2)).anchoredPosition = new Vector2(24, -96);
        }
    }
}