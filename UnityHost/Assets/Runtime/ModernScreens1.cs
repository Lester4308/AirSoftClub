using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        // Base content region (right of the 220px nav, below the 64px header).
        // Top-left: content starts at x=250 (left), y=90 (from top of canvas).
        const float CX = 250f, CY = 90f, CW = 1310f, CH = 740f;

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
            // Hero banner (top of content area)
            var hero = PanelRect("Hero", root, CX, CY, CW, 220);
            var heroBg = hero.gameObject.AddComponent<Image>();
            heroBg.sprite = ModernStyle.WhiteSprite(); heroBg.color = ModernStyle.Panel; heroBg.raycastTarget = false;

            MLabel("HEADQUARTERS", hero, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            var hl = (RectTransform)hero.GetChild(hero.childCount - 1);
            hl.sizeDelta = new Vector2(400, 30); hl.anchoredPosition = new Vector2(30, -24);

            MLabel(club == null ? "" : "YOUR CLUB, YOUR NEXT BATTLE.", hero, 30, ModernStyle.Ink, TextAnchor.MiddleLeft);
            var tl = (RectTransform)hero.GetChild(hero.childCount - 1);
            tl.sizeDelta = new Vector2(700, 60); tl.anchoredPosition = new Vector2(30, -70);

            MLabel(club == null ? "" : club.Fighters.Length + " fighters  ·  " + club.Fighters.Count(f => f.Ready) + " ready" + (club.History != null && club.History.Length == 0 ? "  ·  FIRST BATTLE READY" : ""), hero, 15, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var fl = (RectTransform)hero.GetChild(hero.childCount - 1);
            fl.sizeDelta = new Vector2(400, 30); fl.anchoredPosition = new Vector2(30, -170);

            MButton(club == null || club.Fighters.Length == 0 ? "CHOOSE FIRST RECRUIT" : "FIND A BATTLE", hero, 700, 160, 260, 52, () => page = club == null || club.Fighters.Length == 0 ? "Recruitment" : "Opponents", ModernStyle.Gold);

            // Stat cards row (below hero)
            string[] names = { "TEAM READINESS", "CLUB PROGRESSION", "SUPPLY STATUS" };
            string[] vals = {
                club == null ? "-" : club.Fighters.Count(f => f.Ready) + " / " + club.Fighters.Length + " READY",
                club == null ? "-" : (club.Xp % 1000) + " / 1,000 XP",
                club == null || club.BbStock == null || club.BbStock.Length == 0 ? "-" : (club.BbStock[Mathf.Clamp(club.ActiveBbTier, 0, club.BbStock.Length - 1)]) + " / " + club.Capacity + " BB" };
            for (int n = 0; n < 3; n++)
            {
                var card = PanelRect("Stat" + n, root, CX + n * 442, CY + 250, 426, 190);
                card.gameObject.AddComponent<Image>().color = ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                MLabel(names[n], card, 15, ModernStyle.Muted, TextAnchor.MiddleLeft);
                var c1 = (RectTransform)card.GetChild(card.childCount - 1);
                c1.sizeDelta = new Vector2(380, 30); c1.anchoredPosition = new Vector2(20, -20);
                MLabel(vals[n], card, 24, ModernStyle.Gold, TextAnchor.MiddleLeft);
                var c2 = (RectTransform)card.GetChild(card.childCount - 1);
                c2.sizeDelta = new Vector2(380, 60); c2.anchoredPosition = new Vector2(20, -70);
            }

            // Daily / protection row (bottom)
            var left = PanelRect("Daily", root, CX, CY + 470, 640, 160);
            left.gameObject.AddComponent<Image>().color = ModernStyle.Card;
            left.GetComponent<Image>().raycastTarget = false;
            MLabel("REWARDS & PROGRESS", left, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            var l1 = (RectTransform)left.GetChild(left.childCount - 1);
            l1.sizeDelta = new Vector2(600, 28); l1.anchoredPosition = new Vector2(20, -20);
            bool claimedToday = club != null && club.LastDay == club.ServerNow / 86400000L;
            var streak = claimedToday ? "Claimed today · next reset 00:00 UTC" : "Daily streak " + (club == null ? 0 : club.Streak) + "/7 · resets 00:00 UTC";
            MLabel(streak, left, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var l2 = (RectTransform)left.GetChild(left.childCount - 1);
            l2.sizeDelta = new Vector2(600, 24); l2.anchoredPosition = new Vector2(20, -50);
            if (!claimedToday) MButton("CLAIM DAILY", left, 20, 84, 150, 44, () => StartCoroutine(Send(Intent("Daily"))), ModernStyle.Gold);
            MButton("CLAIM CREDITS", left, 190, 84, 160, 44, () => StartCoroutine(Send(Intent("Progression"))), ModernStyle.Blue);
            MButton("CONVERT 1 CREDIT → " + club.ConvertRate + " MONEY", left, 370, 84, 220, 44,
                () => ConfirmCredits(Intent("Convert", number: 1), 1, "CREDITS CONVERSION", "1 CREDIT → " + club.ConvertRate + " MONEY. Conversion cannot be reversed."), ModernStyle.Neutral);

            var right = PanelRect("Protection", root, CX + 670, CY + 470, 640, 160);
            right.gameObject.AddComponent<Image>().color = ModernStyle.Card;
            right.GetComponent<Image>().raycastTarget = false;
            MLabel("CLUB PROTECTION", right, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            var r1 = (RectTransform)right.GetChild(right.childCount - 1);
            r1.sizeDelta = new Vector2(600, 28); r1.anchoredPosition = new Vector2(20, -20);
            string shieldState = club != null && club.ShieldUntil > club.ServerNow
                ? "SHIELD ACTIVE · Ranked or rated Revenge removes it"
                : "No active shield · blocks new incoming attacks";
            MLabel(shieldState, right, 14, club != null && club.ShieldUntil > club.ServerNow ? ModernStyle.Blue : ModernStyle.Muted, TextAnchor.MiddleLeft);
            var r2 = (RectTransform)right.GetChild(right.childCount - 1);
            r2.sizeDelta = new Vector2(600, 24); r2.anchoredPosition = new Vector2(20, -50);
            if (club != null) for (int n = 0; n < club.Shields.Length && n < 4; n++)
            {
                var sh = club.Shields[n];
                MButton(sh.Hours + "h  ·  " + sh.Credits + " C", right, 20 + n * 155, 84, 140, 44,
                    () => ConfirmCredits(Intent("Shield", number: sh.Hours), sh.Credits, "ACTIVATE SHIELD", sh.Hours + "h protection. Ranked or rated Revenge removes it."), ModernStyle.Neutral);
            }
        }

        // ---- Roster ----
        void RenderRosterModern(RectTransform root)
        {
            MLabel("YOUR FIGHTERS", root, 22, ModernStyle.Gold, TextAnchor.MiddleLeft);
            var tt = (RectTransform)root.GetChild(root.childCount - 1);
            tt.sizeDelta = new Vector2(500, 36); tt.anchoredPosition = new Vector2(CX, -(CY + 8));
            if (club == null) { return; }

            if (club.Fighters.Length == 0)
            {
                MButton("RECRUIT YOUR FIRST FIGHTER", root, CX, CY + 300, 300, 48, () => page = "Recruitment", ModernStyle.Gold);
                return;
            }

            // Card list (left)
            var list = PanelRect("RosterList", root, CX, CY + 60, 640, CH - 60);
            list.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            list.GetComponent<Image>().raycastTarget = false;
            for (int n = 0; n < club.Fighters.Length; n++)
            {
                var f = club.Fighters[n];
                var card = PanelRect("Fighter" + n, list, 15, 12 + n * 120, 610, 108);
                card.gameObject.AddComponent<Image>().color = f.Id == selectedFighter ? ModernStyle.Neutral : ModernStyle.Card;
                card.GetComponent<Image>().raycastTarget = false;
                var thumbnail = Volumetric017UiView.Create(card, "Portrait", new Vector2(68, 96));
                thumbnail.Root.anchoredPosition = new Vector2(12, -6);
                thumbnail.Apply(true, f.Hp > 0, n % 2 == 1);
                MLabel(f.Name, card, 17, ModernStyle.Ink, TextAnchor.MiddleLeft);
                var c0 = (RectTransform)card.GetChild(card.childCount - 1);
                c0.sizeDelta = new Vector2(260, 30); c0.anchoredPosition = new Vector2(92, -18);
                MLabel("LV " + f.Level + "  ·  " + (f.Ready ? "READY" : "RECOVERING"), card, 13, f.Ready ? ModernStyle.Blue : ModernStyle.Orange, TextAnchor.MiddleLeft);
                var c1 = (RectTransform)card.GetChild(card.childCount - 1);
                c1.sizeDelta = new Vector2(300, 24); c1.anchoredPosition = new Vector2(92, -52);
                MLabel("ACC " + f.Accuracy + "   END " + f.Endurance + "   AGI " + f.Agility, card, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                var c2 = (RectTransform)card.GetChild(card.childCount - 1);
                c2.sizeDelta = new Vector2(330, 24); c2.anchoredPosition = new Vector2(92, -84);

                MButton("SELECT", card, 455, 56, 120, 44, () => selectedFighter = f.Id, ModernStyle.Gold);
            }

            // Detail (right)
            var fIdx = club.Fighters.FirstOrDefault(x => x.Id == selectedFighter);
            if (fIdx == null && club.Fighters.Length > 0) { selectedFighter = club.Fighters[0].Id; }
            fIdx = club.Fighters.FirstOrDefault(x => x.Id == selectedFighter);
            if (fIdx == null) return;

            var det = PanelRect("Detail", root, CX + 680, CY + 60, 630, CH - 60);
            det.gameObject.AddComponent<Image>().color = ModernStyle.Panel;
            det.GetComponent<Image>().raycastTarget = false;

            var portrait = Volumetric017UiView.Create(det, "SelectedFighter", new Vector2(190, 285));
            portrait.Root.anchoredPosition = new Vector2(410, -16);
            portrait.Apply(true, fIdx.Hp > 0, Mathf.Abs(selectedFighter.GetHashCode()) % 2 == 1);

            MLabel(fIdx.Name.ToUpperInvariant(), det, 24, ModernStyle.Gold, TextAnchor.MiddleLeft);
            var d0 = (RectTransform)det.GetChild(det.childCount - 1);
            d0.sizeDelta = new Vector2(500, 40); d0.anchoredPosition = new Vector2(24, -24);

            MLabel("LEVEL " + fIdx.Level + "   ·   " + (fIdx.Hp / 10000f).ToString("0.0") + " / " + (fIdx.MaxHp / 10000f).ToString("0.0") + " HP", det, 16, ModernStyle.Ink, TextAnchor.MiddleLeft);
            var d1 = (RectTransform)det.GetChild(det.childCount - 1);
            d1.sizeDelta = new Vector2(500, 30); d1.anchoredPosition = new Vector2(24, -70);

            // Stats + training buttons
            string[] statNames = { "ACCURACY", "ENDURANCE", "AGILITY" };
            int[] statVals = { fIdx.Accuracy, fIdx.Endurance, fIdx.Agility };
            for (int nf = 0; nf < 3; nf++)
            {
                var sn = statNames[nf];
                var row = PanelRect("Stat", det, 24, 110 + nf * 62, 560, 52);
                row.gameObject.AddComponent<Image>().color = ModernStyle.Bg2;
                row.GetComponent<Image>().raycastTarget = false;
                MLabel(sn + "   " + statVals[nf], row, 16, ModernStyle.Ink, TextAnchor.MiddleLeft);
                var sl = (RectTransform)row.GetChild(row.childCount - 1);
                sl.sizeDelta = new Vector2(300, 40); sl.anchoredPosition = new Vector2(14, -24);
                MButton("+1", row, 460, 44, 70, 34, () => StartCoroutine(Send(Intent("Train", fIdx.Id, sn))), ModernStyle.Blue);
            }

            // Heal
            MButton("HEAL / " + fIdx.HealMoney + " MONEY", det, 24, 300, 260, 48, () => StartCoroutine(Send(Intent("Heal", fIdx.Id))), ModernStyle.Gold);

            // Recovery text
            MLabel("Full recovery: " + Math.Ceiling(fIdx.RecoveryRemainingMs / 60000d) + " min free", det, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
            var rc = (RectTransform)det.GetChild(det.childCount - 1);
            rc.sizeDelta = new Vector2(500, 26); rc.anchoredPosition = new Vector2(24, -370);

            // Gear slots (compact read)
            string[] slots = { "Weapon", "Camouflage", "HeadProtection", "LoadBearingArmor" };
            for (int ng = 0; ng < 4; ng++)
            {
                var eq = fIdx.Equipment.FirstOrDefault(e => e.Slot == slots[ng]);
                var gear = PanelRect("Gear", det, 24 + ng % 2 * 290, 410 + ng / 2 * 62, 270, 52);
                gear.gameObject.AddComponent<Image>().color = ModernStyle.Card;
                gear.GetComponent<Image>().raycastTarget = false;
                MLabel((eq?.Definition ?? "Empty slot"), gear, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                var gl = (RectTransform)gear.GetChild(gear.childCount - 1);
                gl.sizeDelta = new Vector2(250, 40); gl.anchoredPosition = new Vector2(12, -24);
            }

            // Equipped weapon preview (volumetric family sprite per catalog id)
            var equippedWeapon = fIdx.Equipment.FirstOrDefault(e => e.Slot == "Weapon");
            if (equippedWeapon != null)
            {
                // Bigger weapon showcase beside the portrait when a weapon is equipped.
                var wp = VolumetricWeapon017UiView.Create(det, "WeaponShowcase", new Vector2(120, 70));
                wp.Root.anchoredPosition = new Vector2(390, -300);
                wp.Apply(equippedWeapon.Item ?? equippedWeapon.Definition);
            }

            MButton("VISIT SHOP", det, 24, 550, 260, 48, () => { page = "Shop"; shopCategory = "Weapons"; }, ModernStyle.Blue);
        }
    }
}