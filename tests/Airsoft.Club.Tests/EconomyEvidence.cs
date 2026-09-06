using Airsoft.Battle;
using Airsoft.Club;
using System.Text.Json;
internal static partial class Program
{
    static int EconomyEvidence()
    {
        long totalRewards = 0, totalHealing = 0, totalRefills = 0, totalWait = 0; int count = 0, emergencies = 0; long minimumMoney = long.MaxValue;
        for (int account = 0; account < 100; account++)
        {
            var s = Starter(); var f = s.Fighters[0]; f.Id = "fighter"; Clubs.Equip(s, f.Id, Clubs.Buy(s, "Pistol-MK1", "weapon"));
            var defense = Clubs.Snapshot(s, true, 0); long now = 0;
            for (int battle = 0; battle < 20; battle++)
            {
                long heal = (f.MaxHp - f.Hp + 9999) / 10000;
                if (heal > 0 && s.Wallet.Money >= heal) { Clubs.Heal(s, f.Id, now, "heal" + battle); totalHealing += heal; }
                else if (heal > 0) { now += 6000000; totalWait += 6000000; f.Recover(now); }
                if (s.BbStock[0] < 20)
                {
                    if (s.Wallet.Money >= 50) { Clubs.Refill(s, 0, "refill" + battle); totalRefills += 50; }
                    else { long next = Math.Max(now, s.EmergencyAt + 10800000); totalWait += next - now; now = next; Clubs.Emergency(s, now); emergencies++; }
                }
                var offense = Clubs.Snapshot(s, false, now); var result = new BattleEngine().Run(new MatchConfig(offense, defense, (ulong)(account * 20 + battle), new BattleRules()));
                Check(result.Status == ResultStatus.Completed);
                var reward = Rewards.Calculate(result.Outcome!.Value, 1, Rewards.Power(offense), Rewards.Power(defense), 0, false, new());
                s.Wallet.Apply("battle" + battle, "fixture-settle", reward.Money, 0); totalRewards += reward.Money;
                f.Hp = result.Attacker.Fighters[0].FinalHp.Raw; f.RecoveryAt = now; f.RecoveryRemainder = 0; f.Xp += reward.FighterXp; s.Xp += reward.ClubXp;
                s.BbStock[0] -= result.Attacker.BbConsumed; Clubs.Completed(s, "m" + battle); minimumMoney = Math.Min(minimumMoney, s.Wallet.Money); count++;
                Check(s.Wallet.Credits == 10 && s.Wallet.Money >= 0 && s.BbStock[0] >= 0);
            }
        }
        var output = new { Accounts = 100, Battles = count, TotalRewards = totalRewards, TotalHealing = totalHealing, TotalRefills = totalRefills, FreeRecoveryWaitMinutes = totalWait / 60000, Emergencies = emergencies, MinimumMoney = minimumMoney, PremiumSpent = 0 };
        Directory.CreateDirectory("Artifacts"); File.WriteAllText("Artifacts/economy-011.json", JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true })); Console.WriteLine(JsonSerializer.Serialize(output)); return 0;
    }
}
