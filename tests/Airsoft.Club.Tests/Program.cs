using Airsoft.Club;
using Airsoft.Battle;

internal static partial class Program
{
    static int passed, failed;
    static void Test(string name, Action body)
    {
        try { body(); passed++; Console.WriteLine("PASS " + name); }
        catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + " " + e); }
    }
    static void Check(bool value) { if (!value) throw new Exception("Invariant failed"); }
    static void Reject(Action body) { try { body(); } catch (InvalidOperationException) { return; } catch (ArgumentException) { return; } throw new Exception("Expected rejection"); }
    static int Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "--economy") return EconomyEvidence();
        if (args.Length == 1 && args[0] == "--monetization") return MonetizationEvidence();
        if (args.Length == 1 && args[0] == "--early-access") return EarlyAccessEvidence();
        if (args.Length == 1 && args[0] == "--balance") return BalanceEvidence();
        if (args.Length == 1 && args[0] == "--final-balance") return FinalBalanceEvidence();
        EconomyTests(); LifecycleTests(); PvpTests(); RetentionTests(); AlphaTests(); MonetizationTests();
        Console.WriteLine($"CLUB SUMMARY passed={passed} failed={failed}");
        return failed == 0 ? 0 : 1;
    }
    static void EconomyTests()
    {
        var c = new EconomyConfig();
        Test("approved outcome table", () =>
        {
            Check(Rewards.Calculate(MatchOutcome.AttackerWin, 1, 100, 100, 0, false, c) == new Reward(100, 100, 100));
            Check(Rewards.Calculate(MatchOutcome.Draw, 1, 100, 100, 0, false, c) == new Reward(25, 35, 50));
            Check(Rewards.Calculate(MatchOutcome.DefenderWin, 1, 100, 100, 0, false, c) == new Reward(0, 10, 25));
        });
        Test("friend sequence and exhausted loss/draw", () =>
        {
            long[] expected = { 100, 50, 25, 0, 0 };
            for (int n = 0; n < 5; n++) Check(Rewards.Calculate(MatchOutcome.AttackerWin, 1, 100, 100, n, true, c).Money == expected[n]);
            foreach (var outcome in Enum.GetValues<MatchOutcome>()) Check(Rewards.Calculate(outcome, 1, 100, 100, 3, true, c) == new Reward(0, 0, 0));
        });
        Test("bounded power and single floor", () =>
        {
            Check(Rewards.Calculate(MatchOutcome.AttackerWin, 1, 10000, 1, 0, false, c).Money == 10);
            Check(Rewards.Calculate(MatchOutcome.AttackerWin, 1, 1, 10000, 0, false, c).Money == 150);
            Check(Rewards.Calculate(MatchOutcome.Draw, 1, 3, 2, 1, true, c).Money == 8);
        });
        Test("starter exactly once, conversion retry and conflicting key", () =>
        {
            var w = new Wallet(); w.Starter(c); w.Starter(c); Check(w.Credits == 10 && w.Money == 1000);
            w.Convert("one", 2, c); w.Convert("one", 2, c); Check(w.Credits == 8 && w.Money == 1200);
            Reject(() => w.Convert("one", 3, c)); Check(w.Credits == 8);
        });
        Test("no reverse conversion and insufficient balance atomic", () =>
        {
            var w = new Wallet(); Reject(() => w.Convert("x", -1, c)); Reject(() => w.Convert("x", 1, c)); Check(w.Entries.Count == 0);
            w.Apply("cash", "fixture", 10, 0); Reject(() => w.Apply("over", "buy", -11, 0)); Check(w.Money == 10 && w.Entries.Count == 1);
        });
        Test("eliminated participant gets XP, no outsiders", () =>
        {
            var input = BattleWire.ReadConfig(File.ReadAllBytes("src/Airsoft.Battle/Runtime/Resources/golden-input.bytes"));
            var result = new BattleEngine().Run(input); var xp = Rewards.Participants(result.Attacker, 100);
            Check(xp.Count == input.Attacker.Fighters.Count && xp.Values.All(v => v == 100) && !xp.ContainsKey("outsider"));
        });
    }
}
