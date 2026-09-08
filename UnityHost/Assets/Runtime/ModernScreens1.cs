using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        // Base content region (right of the 220px nav, below the 64px header)
        const float CX = 250f, CY = 150f, CW = 1310f, CH = 740f;

        void RenderArtModern(RectTransform root)
        {
            var c = PanelRect("ArtPreview", root, CX, CY, CW, CH);
            c.gameObject.AddComponent<Image>().color = ModernStyle.PanelAlt;
            c.GetComponent<Image>().raycastTarget = false;
            MLabel("VISUAL LAYER / MK REVIEW", c, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            var labRect = (RectTransform)c.GetChild(c.childCount - 1);
            labRect.sizeDelta = new Vector2(500, 40); labRect.anchoredPosition = new Vector2(24, -24);
            string[] fam = { "Pistol", "Smg", "AssaultRifle", "Shotgun", "Dmr", "SniperRifle" };
            for (int n = 0; n < fam.Length; n++)
            {
                MLabel(fam[n], c, 15, ModernStyle.Ink, TextAnchor.MiddleLeft);
                var t = (RectTransform)c.GetChild(c.childCount - 1);
                t.sizeDelta = new Vector2(200, 24); t.anchoredPosition = new Vector2(24, -90 - n * 80);
            }
        }

        // ---- Hub (Club) ----
        void RenderHubModern(RectTransform root)
        {
            // Hero banner
            var hero = PanelRect("Hero", root, CX, CY, CW, 220);
            var heroBg = hero.gameObject.AddComponent<Image>();
            heroBg.sprite = ModernStyle.WhiteSprite(); heroBg.color = ModernStyle.Panel; heroBg.raycastTarget = false;

            MLabel("HEADQUARTERS", hero, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)hero.GetChild(0)).sizeDelta = new Vector2(400, 30); ((RectTransform)hero.GetChild(0)).anchoredPosition = new Vector2(30, -24);

            MLabel(club == null ? "" : "YOUR CLUB, YOUR NEXT BATTLE.", hero, 30, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)hero.GetChild(1)).sizeDelta = new Vector2(700, 60); ((RectTransform)hero.GetChild(1)).anchoredPosition = new Vector2(30, -70);

            MLabel(club == null ? "" : club.Fighters.Length + " fighters  ·  " + club.Fighters.Count(f => f.Ready) + " ready", hero, 15, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)hero.GetChild(2)).sizeDelta = new Vector2(400, 30); ((RectTransform)hero.GetChild(2)).anchoredPosition = new Vector2(30, -170);

            MButton(club == null || club.Fighters.Length == 0 ? "CHOOSE FIRST RECRUIT" : "FIND A BATTLE", hero, 700, -150, 260, 52, () => page = club == null || club.Fighters.Length == 0 ? "Recruitment" : "Opponents", ModernStyle.Gold);

            // Stat cards
            string[] names = { "TEAM READINESS", "CLUB PROGRESSION", "SUPPLY STATUS" };
            string[] vals = {
                club == null ? "-" : club.Fighters.Count(f => f.Ready) + " / " + club.Fighters.Length + " READY",
                club == null ? "-" : (club.Xp % 1000) + " / 1,000 XP",
                club == null || club.BbStock == null ? "-" : (club.BbStock[club.ActiveBbTier]) + " / " + club.Capacity + " BB" };
            for (int n = 0; n < 3; n++)
            {
                var card = PanelRect("Stat" + n, root, CX + n * 440, CY + 340, 420, 200);
                card.gameObject.AddComponent<Image>().color = ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                MLabel(names[n], card, 15, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(0)).sizeDelta = new Vector2(380, 30); ((RectTransform)card.GetChild(0)).anchoredPosition = new Vector2(20, -20);
                MLabel(vals[n], card, 24, ModernStyle.Gold, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(1)).sizeDelta = new Vector2(380, 60); ((RectTransform)card.GetChild(1)).anchoredPosition = new Vector2(20, -80);
            }

            // Daily / protection
            var left = PanelRect("Daily", root, CX, CY + 560, 640, 150);
            left.gameObject.AddComponent<Image>().color = ModernStyle.Card;
            left.GetComponent<Image>().raycastTarget = false;
            MLabel("REWARDS & PROGRESS", left, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)left.GetChild(0)).sizeDelta = new Vector2(600, 28); ((RectTransform)left.GetChild(0)).anchoredPosition = new Vector2(20, -20);
            var streak = "Daily streak " + (club == null ? 0 : club.Streak) + "/7";
            MLabel(streak, left, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)left.GetChild(1)).sizeDelta = new Vector2(600, 24); ((RectTransform)left.GetChild(1)).anchoredPosition = new Vector2(20, -50);
            MButton("CLAIM DAILY", left, 20, -84, 150, 44, () => StartCoroutine(Send(Intent("Daily"))), ModernStyle.Gold);
            MButton("CLAIM CREDITS", left, 190, -84, 160, 44, () => StartCoroutine(Send(Intent("Progression"))), ModernStyle.Blue);

            var right = PanelRect("Protection", root, CX + 670, CY + 560, 640, 150);
            right.gameObject.AddComponent<Image>().color = ModernStyle.Card;
            right.GetComponent<Image>().raycastTarget = false;
            MLabel("CLUB PROTECTION", right, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)right.GetChild(0)).sizeDelta = new Vector2(600, 28); ((RectTransform)right.GetChild(0)).anchoredPosition = new Vector2(20, -20);
            MLabel(club != null && club.ShieldUntil > club.ServerNow ? "SHIELD ACTIVE" : "No active shield", right, 14, club != null && club.ShieldUntil > club.ServerNow ? ModernStyle.Blue : ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)right.GetChild(1)).sizeDelta = new Vector2(600, 24); ((RectTransform)right.GetChild(1)).anchoredPosition = new Vector2(20, -50);
            if (club != null) for (int n = 0; n < club.Shields.Length && n < 4; n++)
            {
                var idx = n;
                var sh = club.Shields[n];
                MButton(sh.Hours + "h  ·  " + sh.Credits + " C", right, 20 + n * 155, -84, 140, 44, () => StartCoroutine(Send(Intent("Shield", number: sh.Hours))), ModernStyle.Neutral);
            }
        }

        // ---- Roster ----
        void RenderRosterModern(RectTransform root)
        {
            MLabel("YOUR FIGHTERS", root, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(500, 36); ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, MY - 60);
            if (club == null) { return; }

            if (club.Fighters.Length == 0)
            {
                MButton("RECRUIT YOUR FIRST FIGHTER", root, CX, CY + 300, 300, 48, () => page = "Recruitment", ModernStyle.Gold);
                return;
            }

            // Card list (left)
            var list = PanelRect("RosterList", root, CX, CY, 640, CH);
            list.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            list.GetComponent<Image>().raycastTarget = false;
            for (int n = 0; n < club.Fighters.Length; n++)
            {
                var f = club.Fighters[n];
                var card = PanelRect("Fighter" + n, list, 15, -20 - n * 120, 610, 108);
                card.gameObject.AddComponent<Image>().color = f.Id == selectedFighter ? ModernStyle.Neutral : ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                // name + level
                MLabel(f.Name, card, 17, ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(0)).sizeDelta = new Vector2(260, 30); ((RectTransform)card.GetChild(0)).anchoredPosition = new Vector2(20, -15);
                MLabel("LV " + f.Level + "  ·  " + (f.Ready ? "READY" : "RECOVERING"), card, 13, f.Ready ? ModernStyle.Blue : ModernStyle.Orange, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(1)).sizeDelta = new Vector2(240, 24); ((RectTransform)card.GetChild(1)).anchoredPosition = new Vector2(20, -48);
                MLabel("ACC " + f.Accuracy + "   END " + f.Endurance + "   AGI " + f.Agility, card, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)card.GetChild(2)).sizeDelta = new Vector2(300, 24); ((RectTransform)card.GetChild(2)).anchoredPosition = new Vector2(20, -80);

                MButton("SELECT", card, 455, -30, 120, 44, () => selectedFighter = f.Id, ModernStyle.Gold);
            }

            // Detail (right)
            var fIdx = club.Fighters.FirstOrDefault(x => x.Id == selectedFighter);
            if (fIdx == null && club.Fighters.Length > 0) { selectedFighter = club.Fighters[0].Id; }
            fIdx = club.Fighters.FirstOrDefault(x => x.Id == selectedFighter);
            if (fIdx == null) return;

            var det = PanelRect("Detail", root, CX + 680, CY, 630, CH);
            det.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            det.GetComponent<Image>().raycastTarget = false;

            MLabel(fIdx.Name.ToUpperInvariant(), det, 24, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(0)).sizeDelta = new Vector2(500, 40); ((RectTransform)det.GetChild(0)).anchoredPosition = new Vector2(24, -24);

            MLabel("LEVEL " + fIdx.Level + "   ·   " + (fIdx.Hp / 10000f).ToString("0.0") + " / " + (fIdx.MaxHp / 10000f).ToString("0.0") + " HP", det, 16, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(1)).sizeDelta = new Vector2(500, 30); ((RectTransform)det.GetChild(1)).anchoredPosition = new Vector2(24, -70);

            // Stats + training buttons
            string[] statNames = { "ACCURACY", "ENDURANCE", "AGILITY" };
            int[] statVals = { fIdx.Accuracy, fIdx.Endurance, fIdx.Agility };
            for (int n = 0; n < 3; n++)
            {
                var sn = statNames[n];
                var row = PanelRect("Stat", det, 24, -120 - n * 60, 560, 52);
                row.gameObject.AddComponent<Image>().color = ModernStyle.Bg2;
                row.GetComponent<Image>().raycastTarget = false;
                MLabel(sn + "   " + statVals[n], row, 16, ModernStyle.Ink, TextAnchor.MiddleLeft);
                ((RectTransform)row.GetChild(0)).sizeDelta = new Vector2(300, 40); ((RectTransform)row.GetChild(0)).anchoredPosition = new Vector2(14, 0);
                MButton("+1", row, 460, 0, 70, 34, () => StartCoroutine(Send(Intent("Train", fIdx.Id, sn))), ModernStyle.Blue);
            }

            // Heal
            MButton("HEAL / " + fIdx.HealMoney + " MONEY", det, 24, -330, 260, 48, () => StartCoroutine(Send(Intent("Heal", fIdx.Id))), ModernStyle.Gold);

            // Recovery text
            MLabel("Full recovery: " + Math.Ceiling(fIdx.RecoveryRemainingMs / 60000d) + " min free", det, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)det.GetChild(2)).sizeDelta = new Vector2(500, 26); ((RectTransform)det.GetChild(2)).anchoredPosition = new Vector2(24, -390);

            // Gear slots (compact read)
            string[] slots = { "Weapon", "Camouflage", "HeadProtection", "LoadBearingArmor" };
            for (int n = 0; n < 4; n++)
            {
                var eq = fIdx.Equipment.FirstOrDefault(e => e.Slot == slots[n]);
                var gear = PanelRect("Gear", det, 24 + n % 2 * 290, -430 - n / 2 * 60, 270, 52);
                gear.gameObject.AddComponent<Image>().color = ModernStyle.Card;
                gear.GetComponent<Image>().raycastTarget = false;
                MLabel((eq?.Definition ?? "Empty slot"), gear, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)gear.GetChild(0)).sizeDelta = new Vector2(250, 40); ((RectTransform)gear.GetChild(0)).anchoredPosition = new Vector2(12, 0);
            }

            MButton("VISIT SHOP", det, 24, -560, 260, 48, () => { page = "Shop"; shopCategory = "Weapons"; }, ModernStyle.Blue);
        }
    }
}