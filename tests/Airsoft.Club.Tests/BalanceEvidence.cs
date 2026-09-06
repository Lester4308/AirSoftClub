using Airsoft.Battle;
using Airsoft.Club;
using System.Text.Json;
internal static partial class Program
{
    static TeamSnapshot BalanceTeam(int size, WeaponFamily family, int armor, bool premium)
    {
        var s = Clubs.Create("balance", 0); s.Fighters.Clear(); s.ActiveBbTier = premium ? 4 : 0;
        for (int n = 0; n < size; n++)
        {
            var f = new Fighter { Id = n.ToString("D2"), Accuracy = 10, Endurance = 10, Agility = 10, Hp = 1000000 }; s.Fighters.Add(f);
            foreach (var slot in Enum.GetValues<Slot>())
            {
                string id = n + "-" + slot; s.Items[id] = slot == Slot.Weapon ? family + "-MK" + (premium ? 3 : 1) : slot + "-" + armor; f.Equipment[slot] = id;
            }
        }
        return Clubs.Snapshot(s, true, 0);
    }
    static int BalanceEvidence()
    {
        // Predeclared metric: matched full-build expected DPS × effective HP ratio; no win-rate ceiling claim.
        var rules = new BattleRules(); decimal maxRatio = 0; int battles = 0, premiumWins = 0, draws = 0; var rows = new List<object>();
        foreach (var family in Enum.GetValues<WeaponFamily>()) foreach (int armor in new[] { 1, 2, 3 }) foreach (int size in new[] { 1, 8, 16 })
        {
            var normal = BalanceTeam(size, family, armor, false); var paid = BalanceTeam(size, family, armor, true);
            var a = normal.Fighters[0]; var p = paid.Fighters[0];
            decimal baseline = Formulas.Damage(a.Weapon!, normal.BbTier, a.Loadout, rules).Raw;
            decimal boosted = Formulas.Damage(p.Weapon!, paid.BbTier, a.Loadout, rules).Raw;
            // Same stats/armor/tempo/projectiles/hit and HP, so remaining factors cancel exactly.
            decimal ratio = boosted / baseline; maxRatio = Math.Max(maxRatio, ratio); Check(ratio <= 1.20m);
            int wins = 0, localDraws = 0;
            for (ulong seed = 0; seed < 50; seed++)
                foreach (bool swap in new[] { false, true })
                {
                    var config = new MatchConfig(swap ? normal : paid, swap ? paid : normal, seed, rules); var result = new BattleEngine().Run(config);
                    Check(result.Status == ResultStatus.Completed && result.Attacker.BbRemaining >= 0 && result.Defender.BbRemaining >= 0);
                    battles++; if (result.Outcome == MatchOutcome.Draw) { draws++; localDraws++; }
                    else if (result.Outcome == (swap ? MatchOutcome.DefenderWin : MatchOutcome.AttackerWin)) { wins++; premiumWins++; }
                }
            rows.Add(new { Family = family.ToString(), Armor = armor, Size = size, Ratio = ratio, PremiumWins = wins, Draws = localDraws, Battles = 100 });
        }
        long ammo = 0; for (ulong seed = 0; seed < 1000; seed++)
        {
            var starter = Starter(); starter.Fighters[0].Id = "00"; Clubs.Equip(starter, "00", Clubs.Buy(starter, "Pistol-MK1", "weapon")); var a = Clubs.Snapshot(starter, true, 0); var r = new BattleEngine().Run(new MatchConfig(a, a, seed, rules)); ammo += r.Attacker.BbConsumed;
        }
        var output = new
        {
            Metric = "matched full-build expected DPS x effective HP ratio",
            MaxRatio = maxRatio,
            Battles = battles,
            PremiumWins = premiumWins,
            Draws = draws,
            StarterAmmoMean = ammo / 1000m,
            CurrentStarterBattles = Clubs.Create("starter", 0).BbStock[0] / (ammo / 1000m),
            Rows = rows
        };
        Directory.CreateDirectory("Artifacts"); File.WriteAllText("Artifacts/balance-011.json", JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(JsonSerializer.Serialize(new { maxRatio, battles, premiumWins, draws, starterAmmoMean = ammo / 1000m })); return 0;
    }
}
