using System;
using System.Linq;
using Airsoft.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        // ---- Battle ----
        void RenderBattleModern(RectTransform root)
        {
            if (replayResult == null) { RenderStatusModern(root); return; }
            // If the replay already finished, show the result; otherwise the live arena.
            if (replayTime >= replayResult.SimulatedDurationMs) { RenderResultModern(root); return; }

            MLabel("COMPOUND 01  /  BATTLE REPLAY", root, 20, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(600, 32);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX, -CY);

            var tSec = (Math.Min(replayTime, replayResult.SimulatedDurationMs) / 1000f).ToString("0.0") + " s";
            MLabel(tSec, root, 22, ModernStyle.Orange, TextAnchor.MiddleCenter);
            ((RectTransform)root.GetChild(root.childCount - 1)).sizeDelta = new Vector2(200, 40);
            ((RectTransform)root.GetChild(root.childCount - 1)).anchoredPosition = new Vector2(CX + CW - 200, -CY);

            // BB summary
            int afired = replayResult.Events.Count(e => e.ActorSide == Side.Attacker && e.TimeMs <= replayTime);
            int dfired = replayResult.Events.Count(e => e.ActorSide == Side.Defender && e.TimeMs <= replayTime);
            int at = BetaTheme.BbIndex(replayInput.Attacker.BbTier);
            int dt = BetaTheme.BbIndex(replayInput.Defender.BbTier);
            string ak = club.BbCatalog != null && club.BbCatalog.Length > at ? club.BbCatalog[at].Name : "BB";
            string dk = club.BbCatalog != null && club.BbCatalog.Length > dt ? club.BbCatalog[dt].Name : "BB";

            var ab = PanelRect("AtkBB", root, CX, CY + 50, 620, 60);
            ab.gameObject.AddComponent<Image>().color = ModernStyle.Panel; ab.GetComponent<Image>().raycastTarget = false;
            MLabel("ATTACK / " + ak, ab, 14, ModernStyle.Blue, TextAnchor.MiddleLeft);
            ((RectTransform)ab.GetChild(0)).sizeDelta = new Vector2(200, 28); ((RectTransform)ab.GetChild(0)).anchoredPosition = new Vector2(16, -6);
            MLabel((replayInput.Attacker.BbBudget - afired) + " BB remaining  ·  " + afired + " fired", ab, 14, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)ab.GetChild(1)).sizeDelta = new Vector2(400, 28); ((RectTransform)ab.GetChild(1)).anchoredPosition = new Vector2(16, -30);

            var db = PanelRect("DefBB", root, CX + 690, CY + 50, 620, 60);
            db.gameObject.AddComponent<Image>().color = ModernStyle.Panel; db.GetComponent<Image>().raycastTarget = false;
            MLabel("DEFENSE / " + dk, db, 14, ModernStyle.Orange, TextAnchor.MiddleLeft);
            ((RectTransform)db.GetChild(0)).sizeDelta = new Vector2(200, 28); ((RectTransform)db.GetChild(0)).anchoredPosition = new Vector2(16, -6);
            MLabel((replayInput.Defender.BbBudget - dfired) + " BB remaining  ·  " + dfired + " fired", db, 14, ModernStyle.Ink, TextAnchor.MiddleLeft);
            ((RectTransform)db.GetChild(1)).sizeDelta = new Vector2(400, 28); ((RectTransform)db.GetChild(1)).anchoredPosition = new Vector2(16, -30);

            // Arena
            var arena = PanelRect("Arena", root, CX, CY + 130, CW, 500);
            arena.gameObject.AddComponent<Image>().color = ModernStyle.PanelAlt; arena.GetComponent<Image>().raycastTarget = false;
            DrawModernArena(arena, replayInput, replayResult, replayTime, appearance);

            // BB legend + skip
            for (int n = 0; n < 5; n++)
            {
                var c = BetaTheme.BbColor(n);
                var chip = PanelRect("Legend" + n, root, CX + n * 200, CY + 650, 190, 44);
                chip.gameObject.AddComponent<Image>().color = new Color(c.r, c.g, c.b, 0.28f); chip.GetComponent<Image>().raycastTarget = false;
                MLabel(club != null && club.BbCatalog != null && club.BbCatalog.Length > n ? club.BbCatalog[n].Name : "BB", chip, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)chip.GetChild(0)).sizeDelta = new Vector2(170, 36); ((RectTransform)chip.GetChild(0)).anchoredPosition = new Vector2(12, -4);
            }
            MButton("SKIP TO RESULT", root, CX + 1070, CY + 650, 240, 44, () => { replayTime = replayResult.SimulatedDurationMs + 1; }, ModernStyle.Blue);
        }

        // Canvas arena: draws lanes, team labels, fighter silhouettes + HP bars.
        // Projectile dots are presentation-only event indicators, not spatial simulation.
        void DrawModernArena(RectTransform parent, MatchConfig input, MatchResult result, float time, AppearanceView[] appearance)
        {
            int rows = Math.Max(1, (Math.Max(input.Attacker.Fighters.Count, input.Defender.Fighters.Count) + 3) / 4);
            float lane = parent.rect.height / rows;
            bool dense = rows >= 3;

            var w = parent.rect.width; var h = parent.rect.height;
            // lane separators + ground
            for (int n = 0; n < rows; n++)
            {
                var laneRow = PanelRect("Lane" + n, parent, 0, n * lane, w, lane);
                laneRow.gameObject.AddComponent<Image>().color = new Color(0.11f, 0.16f, 0.17f, 0.5f);
                laneRow.GetComponent<Image>().raycastTarget = false;
            }

            // Draw fighters (left=attacker, right=defender)
            void DrawSide(TeamSnapshot team, bool left)
            {
                for (int i = 0; i < team.Fighters.Count; i++)
                {
                    var f = team.Fighters[i];
                    int row = i / 4, col = i % 4;
                    float fw = 120f, fh = Mathf.Min(200f, lane - (dense ? 28f : 50f));
                    float x = left ? 30 + col * 130 : w - 30 - fw - col * 130;
                    float y = row * lane + (dense ? 20f : 30f);
                    if (y + fh > h) continue;
                    var body = PanelRect("Fig" + (left ? "A" : "D") + i, parent, x, y, fw, fh);
                    var bodyImage = body.gameObject.AddComponent<Image>();
                    Color teamColor = left ? ModernStyle.Blue : ModernStyle.Orange;
                    bodyImage.color = new Color(teamColor.r, teamColor.g, teamColor.b, 0.12f);
                    bodyImage.raycastTarget = false;

                    string key = (left ? "A" : "D") + f.Id;
                    long hpV = hp != null && hp.TryGetValue(key, out var val) ? val : f.StartingHp.Raw;
                    var fighterArt = Volumetric017UiView.Create(body, "FighterArt", new Vector2(fw, fh));
                    fighterArt.Root.anchoredPosition = Vector2.zero;
                    fighterArt.Apply(left, hpV > 0);

                    // Overlay the volumetric family weapon as a compact grounded silhouette
                    // at the fighter's hands line (not drifting above), read per fighter.
                    if (f.Weapon != null)
                    {
                        var w = VolumetricWeapon017UiView.CreateSilhouette(body, "WeaponOverlay", new Vector2(dense ? 36 : 48, dense ? 22 : 28));
                        w.Root.anchoredPosition = new Vector2(fw * 0.55f, fh * 0.42f);
                        w.ApplyFamily((int)f.Weapon.Family);
                    }

                    // name plate
                    MLabel(f.Id.Substring(0, Math.Min(5, f.Id.Length)), body, dense ? 12 : 11, ModernStyle.Ink, TextAnchor.MiddleCenter);
                    var nameT = (RectTransform)body.GetChild(body.childCount - 1);
                    nameT.sizeDelta = new Vector2(fw, dense ? 18 : 20); nameT.anchoredPosition = new Vector2(0, dense ? -2 : -12);

                    // HP remains anchored at the fighter's feet in every density.
                    float frac = (float)hpV / 10000f / ((float)Formulas.MaxHp(f.Endurance, input.Rules).Raw / 10000f);
                    frac = Mathf.Clamp01(frac);
                    float hpHeight = dense ? 10f : 8f;
                    var hpBar = PanelRect("HP", body, 8, fh - hpHeight - 4, fw - 16, hpHeight);
                    hpBar.gameObject.AddComponent<Image>().color = new Color(0.03f, 0.05f, 0.06f, 0.88f);
                    hpBar.GetComponent<Image>().raycastTarget = false;
                    var hpFill = PanelRect("Fill", hpBar, 0, 0, (fw - 16) * frac, hpHeight);
                    hpFill.gameObject.AddComponent<Image>().color = left ? ModernStyle.Blue : ModernStyle.Orange;
                    hpFill.GetComponent<Image>().raycastTarget = false;

                    // A solid side marker survives when character detail collapses at 16v16.
                    var teamMark = PanelRect("TeamMark", body, left ? 0 : fw - 4, 0, 4, fh);
                    teamMark.gameObject.AddComponent<Image>().color = teamColor;
                    teamMark.GetComponent<Image>().raycastTarget = false;
                }
            }
            DrawSide(input.Attacker, true);
            DrawSide(input.Defender, false);

            // Team labels
            MLabel("ATTACK  /  WEST", parent, 13, ModernStyle.Blue, TextAnchor.MiddleLeft);
            var awl = (RectTransform)parent.GetChild(parent.childCount - 1);
            awl.sizeDelta = new Vector2(200, 24); awl.anchoredPosition = new Vector2(20, -6);
            MLabel("DEFENSE  /  EAST", parent, 13, ModernStyle.Orange, TextAnchor.MiddleRight);
            var dwl = (RectTransform)parent.GetChild(parent.childCount - 1);
            dwl.sizeDelta = new Vector2(200, 24); dwl.anchoredPosition = new Vector2(w - 220, -6);

            // ~ simplistic BB trails (animated dots drawn over recent events)
            var recent = result.Events.Where(e => e.TimeMs <= time && e.TimeMs > time - 260 && e.Damage.Raw > 0).ToArray();

            // BB trail dots (cosmetic presentation only — never affects settlement)
            foreach (var e in recent)
            {
                Color col = BetaTheme.BbColor(BetaTheme.BbIndex(e.ActorSide == Side.Attacker ? input.Attacker.BbTier : input.Defender.BbTier));
                float age = time - e.TimeMs;
                float phase = Mathf.Clamp01(age / 260f);
                float trailX = e.ActorSide == Side.Attacker
                    ? 30f + (w - 74f) * phase
                    : w - 44f - (w - 74f) * phase;
                int targetIndex = 0;
                var targetTeam = e.ActorSide == Side.Attacker ? input.Defender : input.Attacker;
                for (int i = 0; i < targetTeam.Fighters.Count; i++)
                {
                    if (string.Equals(targetTeam.Fighters[i].Id, e.TargetId, StringComparison.Ordinal))
                    {
                        targetIndex = i;
                        break;
                    }
                }
                int targetRow = targetIndex / 4;
                float targetHeight = Mathf.Min(200f, lane - (dense ? 28f : 50f));
                float trailY = targetRow * lane + (dense ? 20f : 30f) + targetHeight * 0.5f - 7f;
                var dot = PanelRect("Trail", parent, trailX, trailY, 14, 14);
                dot.gameObject.AddComponent<Image>().color = new Color(col.r, col.g, col.b, 0.85f);
                dot.GetComponent<Image>().raycastTarget = false;
            }
        }

        // ---- Result ----
        void RenderResultModern(RectTransform root)
        {
            if (replayResult == null || savedMatch == null) { RenderStatusModern(root); return; }
            var h = club != null && club.History != null ? club.History.FirstOrDefault(x => x.Id == savedMatch.Id) : null;
            bool attacking = h == null || h.Attacker == club.Id;
            bool isDraw = replayResult.Outcome == MatchOutcome.Draw;
            bool won = attacking ? replayResult.Outcome == MatchOutcome.AttackerWin : replayResult.Outcome == MatchOutcome.DefenderWin;

            Color outcomeColor = isDraw ? ModernStyle.Blue : (won ? ModernStyle.Gold : ModernStyle.Orange);
            string verdict = isDraw ? "DRAW" : (won ? "VICTORY" : "DEFEAT");

            var card = PanelRect("Result", root, CX, CY, CW, 520);
            card.gameObject.AddComponent<Image>().color = ModernStyle.Panel; card.GetComponent<Image>().raycastTarget = false;

            MLabel("AFTER ACTION  /  " + replayResult.Reason.ToString().ToUpperInvariant(), card, 15, ModernStyle.Gold, TextAnchor.MiddleLeft);
            ((RectTransform)card.GetChild(0)).sizeDelta = new Vector2(900, 28); ((RectTransform)card.GetChild(0)).anchoredPosition = new Vector2(30, -24);

            MLabel(verdict, card, 72, outcomeColor, TextAnchor.MiddleCenter);
            var v = (RectTransform)card.GetChild(1);
            v.sizeDelta = new Vector2(900, 100); v.anchoredPosition = new Vector2(205, -65);

            MLabel("The result is saved. Your next challenge is waiting.", card, 16, ModernStyle.Muted, TextAnchor.MiddleCenter);
            var sub = (RectTransform)card.GetChild(2);
            sub.sizeDelta = new Vector2(900, 30); sub.anchoredPosition = new Vector2(205, -165);

            // Reward cards
            long[] vals = { savedMatch.RewardMoney, savedMatch.RewardClubXp, savedMatch.RewardFighterXp, savedMatch.RatingKnown ? savedMatch.RatingDelta : 0 };
            string[] names = { "MONEY", "CLUB XP", "FIGHTER XP", "RATING" };
            for (int n = 0; n < 4; n++)
            {
                var rc = PanelRect("Reward" + n, card, 30 + n * 315, 230, 285, 120);
                rc.gameObject.AddComponent<Image>().color = ModernStyle.Card; rc.GetComponent<Image>().raycastTarget = false;
                MLabel(names[n], rc, 13, ModernStyle.Muted, TextAnchor.MiddleLeft);
                ((RectTransform)rc.GetChild(0)).sizeDelta = new Vector2(265, 24); ((RectTransform)rc.GetChild(0)).anchoredPosition = new Vector2(10, -14);
                MLabel((n == 3 && !savedMatch.RatingKnown) ? "UNKNOWN" : vals[n].ToString("+0;-0;0"), rc, 22, n == 3 ? ModernStyle.Blue : ModernStyle.Gold, TextAnchor.MiddleLeft);
                ((RectTransform)rc.GetChild(1)).sizeDelta = new Vector2(265, 50); ((RectTransform)rc.GetChild(1)).anchoredPosition = new Vector2(10, -50);
            }

            MLabel((replayResult.SimulatedDurationMs / 1000f).ToString("0.0") + " s  ·  BB " + replayResult.Attacker.BbConsumed + " / " + replayResult.Defender.BbConsumed + "  ·  " + replayInput.Attacker.Fighters.Count + " vs " + replayInput.Defender.Fighters.Count, card, 14, ModernStyle.Muted, TextAnchor.MiddleLeft);
            ((RectTransform)card.GetChild(card.childCount - 1)).sizeDelta = new Vector2(900, 28); ((RectTransform)card.GetChild(card.childCount - 1)).anchoredPosition = new Vector2(30, -390);

            MButton("NEW BATTLE", root, CX, CY + 550, 300, 50, () => page = "Opponents", ModernStyle.Gold);
            MButton("BACK TO CLUB", root, CX + 330, CY + 550, 300, 50, () => page = "Club", ModernStyle.Blue);
            MButton("REPLAY", root, CX + 660, CY + 550, 300, 50, () => ResetPlayback(), ModernStyle.Neutral);
            MButton("HISTORY", root, CX + 990, CY + 550, 300, 50, () => page = "History", ModernStyle.Neutral);
        }
    }
}
