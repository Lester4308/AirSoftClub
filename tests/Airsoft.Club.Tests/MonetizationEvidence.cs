using Airsoft.Battle;
using Airsoft.Club;
using System.Text.Json;
internal static partial class Program
{
    static ClubState MonetizedBuild(int level, int category)
    {
        var candidates = Catalog.Items.Where(i => i.Slot == Slot.Weapon && (i.Level <= level || category >= 2 && i.Credits == 0 && i.Level <= level + EarlyAccess.Depth(level)) && (category == 3 || i.Credits == 0) && (category > 0 || i.Mk == 1));
        var weapon = candidates.OrderByDescending(i => (decimal)i.Damage * i.Projectiles / i.Interval * (i.Mk == 3 ? 1.035m : i.Mk == 2 ? 1.02m : 1)).First();
        var s = Clubs.Create("simulation", 0); s.Xp = (level - 1) * 1000; s.ActiveBbTier = category == 0 ? 0 : category == 1 ? 3 : category == 2 ? 1 : 4;
        int weight = Math.Min(3, level + (category >= 2 ? EarlyAccess.Depth(level) : 0));
        for (int n = 0; n < 4; n++)
        {
            var f = new Fighter { Id = "f" + n, Accuracy = 10 + level, Endurance = 10 + level, Agility = 10 + level }; f.Hp = f.MaxHp; s.Fighters.Add(f);
            foreach (var slot in Enum.GetValues<Slot>()) { string id = n + slot.ToString(); s.Items[id] = slot == Slot.Weapon ? weapon.Id : slot + "-" + weight; f.Equipment[slot] = id; }
        }
        s.BbStock[s.ActiveBbTier] = s.Capacity; return s;
    }
    static int MonetizationEvidence()
    {
        var rows = new List<object>(); int count = 0;
        for (int level = 1; level <= 10; level++) for (int category = 0; category < 4; category++)
        {
            var s = MonetizedBuild(level, category); var t = Clubs.Snapshot(s, false, 0); var baseline = Clubs.Snapshot(MonetizedBuild(level, 1), true, 0);
            int wins = 0, draws = 0; long damage = 0, hp = 0, bb = 0, money = 0, xp = 0, healing = 0;
            for (ulong seed = 0; seed < 100; seed++)
            {
                var result = new BattleEngine().Run(new MatchConfig(t, baseline, seed, new BattleRules())); Check(result.Status == ResultStatus.Completed); count++;
                if (result.Outcome == MatchOutcome.AttackerWin) wins++; if (result.Outcome == MatchOutcome.Draw) draws++;
                damage += result.Events.Where(e => e.ActorSide == Side.Attacker).Sum(e => e.Damage.Raw); hp += result.Attacker.Fighters.Sum(f => f.FinalHp.Raw); bb += result.Attacker.BbConsumed;
                healing += result.Attacker.Fighters.Sum(f => (f.MaxHp.Raw - f.FinalHp.Raw + 9999) / 10000);
                var reward = Rewards.Calculate(result.Outcome!.Value, level, Rewards.Power(t), Rewards.Power(baseline), 0, false, new()); money += reward.Money; xp += reward.ClubXp;
            }
            decimal ammoMoney = bb / 100m * AlphaConfig.BbMoney(s.ActiveBbTier) / 500;
            rows.Add(new { Level = level, Build = new[] { "Normal", "Optimized normal", "Moderate premium", "Max premium" }[category], Weapon = t.Fighters[0].Weapon!.Id, BbTier = s.ActiveBbTier, EffectiveTeamPower = Rewards.Power(t), RelativePower = Rewards.Power(t) / (decimal)Rewards.Power(baseline), Wins = wins, Draws = draws, Battles = 100, MeanDamage = damage / 1000000m, MeanRemainingHp = hp / 1000000m, MeanBb = bb / 100m, MeanBbMoneyCost = ammoMoney, MeanBbCreditsCost = bb / 100m * AlphaConfig.BbCredits(s.ActiveBbTier) / 500, MeanHealMoney = healing / 100m, MeanRewardMoney = money / 100m, MeanClubXp = xp / 100m, EstimatedBattlesPerLevel = xp == 0 ? 0 : 100000m / xp, NetMoneyAfterFullHealAndBb = money / 100m - healing / 100m - ammoMoney, UnlockCreditCostPerItem = t.Fighters[0].Weapon == null || Catalog.Get(t.Fighters[0].Weapon!.Id).Level <= level ? 0 : EarlyAccess.Price(Catalog.Get(t.Fighters[0].Weapon!.Id).Level - level) });
        }
        Directory.CreateDirectory("Artifacts"); File.WriteAllText("Artifacts/monetization-014.json", JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"MONETIZATION MATRIX levels=10 builds=4 battles={count} invariantFailures=0 rows={rows.Count}"); return 0;
    }
}
