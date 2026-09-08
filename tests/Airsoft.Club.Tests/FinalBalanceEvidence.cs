using Airsoft.Battle;
using Airsoft.Club;
using System.Text.Json;
internal static partial class Program
{
    static TeamSnapshot PacingTeam(int size, bool defense)
    {
        var s = Clubs.Create("pacing-" + size, 0); s.Fighters.Clear();
        for (int n = 0; n < size; n++)
        {
            var f = new Fighter { Id = "f" + n, Accuracy = 10, Endurance = 10, Agility = 10 };
            f.Hp = f.MaxHp; s.Fighters.Add(f);
            foreach (var slot in Enum.GetValues<Slot>())
            {
                string owned = n + "-" + slot;
                s.Items[owned] = slot == Slot.Weapon ? "Pistol-MK1" : slot + "-1";
                f.Equipment[slot] = owned;
            }
        }
        s.BbStock[0] = s.Capacity;
        return Clubs.Snapshot(s, defense, 0);
    }

    static int FinalBalanceEvidence()
    {
        var rules = new BattleRules();
        var starter = PacingTeam(1, false); long starterBb = 0, recoveryToFullMs = 0, recoveryToReadyMs = 0;
        for (ulong seed = 0; seed < 1000; seed++)
        {
            var result = new BattleEngine().Run(new MatchConfig(starter, PacingTeam(1, true), seed, rules));
            Check(result.Status == ResultStatus.Completed);
            starterBb += result.Attacker.BbConsumed;
            var fighter = result.Attacker.Fighters[0]; long missing = fighter.MaxHp.Raw - fighter.FinalHp.Raw;
            recoveryToFullMs += (missing * AlphaConfig.FullRecoveryMs + fighter.MaxHp.Raw - 1) / fighter.MaxHp.Raw;
            long readyHp = (fighter.MaxHp.Raw + 9) / 10;
            long readyMissing = Math.Max(0, readyHp - fighter.FinalHp.Raw);
            recoveryToReadyMs += (readyMissing * AlphaConfig.FullRecoveryMs + fighter.MaxHp.Raw - 1) / fighter.MaxHp.Raw;
        }
        decimal meanStarterBb = starterBb / 1000m;
        decimal starterBattles = AlphaConfig.StarterBb / meanStarterBb;
        decimal refillBattles = AlphaConfig.BbRefill / meanStarterBb;
        Check(starterBattles >= 5m && starterBattles <= 7m);

        var rosterRows = new List<object>(); int asymmetricBattles = 0;
        foreach (int attackerSize in new[] { 1, 2, 4, 8, 16 })
            foreach (int defenderSize in new[] { 1, 2, 4, 8, 16 })
            {
                var attacker = PacingTeam(attackerSize, false); var defender = PacingTeam(defenderSize, true);
                int wins = 0, draws = 0; long attackerBb = 0, defenderBb = 0;
                for (ulong seed = 0; seed < 100; seed++)
                {
                    var result = new BattleEngine().Run(new MatchConfig(attacker, defender, seed, rules));
                    Check(result.Status == ResultStatus.Completed && result.Attacker.BbRemaining >= 0 && result.Defender.BbRemaining >= 0);
                    if (result.Outcome == MatchOutcome.AttackerWin) wins++;
                    if (result.Outcome == MatchOutcome.Draw) draws++;
                    attackerBb += result.Attacker.BbConsumed; defenderBb += result.Defender.BbConsumed; asymmetricBattles++;
                }
                rosterRows.Add(new { AttackerSize = attackerSize, DefenderSize = defenderSize, AttackerWins = wins, Draws = draws, Battles = 100, MeanAttackerBb = attackerBb / 100m, MeanDefenderBb = defenderBb / 100m });
            }

        var output = new
        {
            PolicyVersion = AlphaConfig.Version,
            Starter = new { Simulations = 1000, MeanBb = meanStarterBb, BattlesFromStarter60 = starterBattles, BattlesPer500Refill = refillBattles },
            Recovery = new { FullFromZeroMinutes = AlphaConfig.FullRecoveryMs / 60000m, ReadyFromZeroMinutes = AlphaConfig.FullRecoveryMs / 600000m, MeanPostBattleToFullMinutes = recoveryToFullMs / 60000000m, MeanPostBattleToReadyMinutes = recoveryToReadyMs / 60000000m },
            Refill = new { Amount = AlphaConfig.BbRefill, BasicMoney = AlphaConfig.BbMoney(0), EmergencyCooldownMinutes = AlphaConfig.EmergencyCooldownMs / 60000m, AutoBuyLevel = AlphaConfig.AutoBuyLevel, AutoBuyThreshold = AlphaConfig.AutoBuyThreshold, SpendGuard = AlphaConfig.AutoBuyMinimumMoney },
            AsymmetricRosterBattles = asymmetricBattles,
            RosterRows = rosterRows
        };
        Directory.CreateDirectory("Artifacts");
        File.WriteAllText("Artifacts/final-balance-012.json", JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"FINAL BALANCE starterBattles={starterBattles:F2} refillBattles={refillBattles:F2} meanRecoveryMinutes={recoveryToFullMs / 60000000m:F2} asymmetricBattles={asymmetricBattles}");
        return 0;
    }
}
