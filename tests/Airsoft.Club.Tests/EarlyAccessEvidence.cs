using Airsoft.Battle;
using Airsoft.Club;
using System.Text.Json;
internal static partial class Program
{
    static int EarlyAccessEvidence()
    {
        var rows = new List<object>(); decimal maximum = 0;
        for (int level = 1; level <= 6; level++)
        {
            (decimal Score, string Build) Best(bool paid)
            {
                decimal best = 0; string name = "";
                foreach (var weapon in Catalog.Items.Where(i => i.Slot == Slot.Weapon && (i.Level <= level || paid && i.Credits == 0 && i.Level <= level + AlphaConfig.EarlyLevels) && (paid || i.Credits == 0)))
                    for (int weight = 1; weight <= Math.Min(3, level + (paid ? AlphaConfig.EarlyLevels : 0)); weight++)
                    {
                        var s = Clubs.Create("metric", 0); s.ActiveBbTier = paid ? 4 : 3;
                        var f = new Fighter { Id = "f", Accuracy = 10, Endurance = 10, Agility = 10, Hp = 1000000 }; s.Fighters.Add(f);
                        foreach (var slot in Enum.GetValues<Slot>()) { string id = slot.ToString(); s.Items[id] = slot == Slot.Weapon ? weapon.Id : slot + "-" + weight; f.Equipment[slot] = id; }
                        var team = Clubs.Snapshot(s, true, 0); var actor = team.Fighters[0]; var rules = new BattleRules();
                        var target = new FighterSnapshot("ref", Fixed.FromInt(10), Fixed.FromInt(10), Fixed.FromInt(10), Fixed.FromInt(100), actor.Weapon, new ArmorLoadout(Fixed.Zero, Fixed.Zero, Fixed.Zero));
                        decimal dps = (decimal)Formulas.Damage(actor.Weapon!, team.BbTier, target.Loadout, rules).Raw * actor.Weapon!.Projectiles / Formulas.IntervalMs(actor, rules) * Formulas.HitChance(actor, target, rules).Raw;
                        decimal defenseFactor = (decimal)Formulas.Damage(actor.Weapon, team.BbTier, target.Loadout, rules).Raw / Formulas.Damage(actor.Weapon, team.BbTier, actor.Loadout, rules).Raw / Formulas.HitChance(target, actor, rules).Raw;
                        decimal score = dps * defenseFactor;
                        if (score > best) { best = score; name = weapon.Id + "/armor" + weight + "/bb" + s.ActiveBbTier; }
                    }
                return (best, name);
            }
            var normal = Best(false); var paid = Best(true); decimal ratio = paid.Score / normal.Score; maximum = Math.Max(maximum, ratio);
            rows.Add(new { Level = level, Normal = normal.Build, Paid = paid.Build, Ratio = ratio, AboveTarget = ratio > 1.20m });
        }
        var result = new { Metric = "Best available sustained DPS x relative effective HP at equal base stats; includes early-access catalog paths and High-End soft BB", MaxRatio = maximum, TuningRequired = maximum > 1.20m, Rows = rows };
        Directory.CreateDirectory("Artifacts"); File.WriteAllText("Artifacts/early-access-013.json", JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(JsonSerializer.Serialize(result)); return 0; // Evidence, not approval of final balance.
    }
}
