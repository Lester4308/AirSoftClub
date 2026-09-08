using Airsoft.Battle;

namespace Airsoft.Club;

// Server-side prototype config. Approved percentages are not catalog tuning knobs.
public sealed record EconomyConfig(string Version = "economy-013-v1", long BaseReward = 100,
    long RewardPerOpponentLevel = 10, long StarterMoney = 1000, int CreditsToMoney = 100);
public sealed record Reward(long Money, long ClubXp, long FighterXp);
public static class Rewards
{
    public static Reward Calculate(MatchOutcome outcome, int opponentLevel, long acceptedAttackerPower,
        long acceptedDefenderPower, int previousFriendWins, bool friendWindow, EconomyConfig config)
    {
        if (!Enum.IsDefined(outcome) || opponentLevel < 1 || acceptedAttackerPower < 1 || acceptedDefenderPower < 1 || previousFriendWins < 0)
            throw new ArgumentException("Invalid accepted reward inputs");
        if (config.BaseReward < 0 || config.RewardPerOpponentLevel < 0) throw new ArgumentException("Invalid reward config");
        // Basis points; one final floor per reward, no floating point or survivor weighting.
        long power = Math.Clamp(checked(acceptedDefenderPower * 10000) / acceptedAttackerPower, 1000, 15000);
        int repeat = !friendWindow ? 100 : previousFriendWins switch { 0 => 100, 1 => 50, 2 => 25, _ => 0 };
        var percentages = outcome switch
        {
            MatchOutcome.AttackerWin => new Reward(100, 100, 100),
            MatchOutcome.Draw => new Reward(25, 35, 50),
            _ => new Reward(0, 10, 25)
        };
        long basis = checked(config.BaseReward + (opponentLevel - 1) * config.RewardPerOpponentLevel);
        long Amount(long percent) => checked(basis * power * percent * repeat) / 100000000;
        return new Reward(Amount(percentages.Money), Amount(percentages.ClubXp), Amount(percentages.FighterXp));
    }
    public static Dictionary<string, long> Participants(TeamResult result, long xp) =>
        result.Fighters.ToDictionary(f => f.Id, _ => xp, StringComparer.Ordinal);
    public static long Power(TeamSnapshot team)
    {
        // Coarse sustained threat plus survivability; never exposed as an exact player-facing number.
        var rules = new BattleRules();
        long budget = Math.Min(team.BbBudget, 1000);
        return Math.Max(1, team.Fighters.Sum(f =>
        {
            long survival = f.StartingHp.Raw / Fixed.Scale + f.Loadout.Protection.Raw / Fixed.Scale + Formulas.Evasion(f, rules).Raw / 100;
            if (f.Weapon == null || budget == 0) return Math.Max(1, survival / 4);
            long damage = Formulas.Damage(f.Weapon, team.BbTier, new ArmorLoadout(Fixed.Zero, Fixed.Zero, Fixed.Zero), rules).Raw / 100;
            long threat = damage * f.Weapon.Projectiles * 1000 / Formulas.IntervalMs(f, rules);
            threat = threat * (50 + f.Accuracy.Raw / Fixed.Scale) / 100;
            return Math.Max(1, survival + threat * Math.Min(100, budget) / 100);
        }));
    }
    public static string Category(long own, long rival)
    {
        long ratio = checked(rival * 100) / Math.Max(1, own);
        return ratio < 60 ? "Very Weak" : ratio < 85 ? "Weak" : ratio <= 120 ? "Balanced" : ratio <= 165 ? "Strong" : "Very Strong";
    }
}
public sealed record LedgerEntry(string Operation, string Reason, long MoneyDelta, long CreditsDelta);
public sealed class Wallet
{
    public long Money { get; set; }
    public long Credits { get; set; }
    public List<LedgerEntry> Entries { get; set; } = new();
    public LedgerEntry? Find(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation)) throw new ArgumentException("Ledger identity required");
        return Entries.SingleOrDefault(e => e.Operation == operation);
    }
    // Validates an exact retry without mutating the wallet. Aggregate commands use this
    // before state-based eligibility checks so a completed purchase can be retried safely.
    public bool IsReplay(string operation, string reason, long money, long credits)
    {
        if (string.IsNullOrWhiteSpace(operation) || string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Ledger identity required");
        var entry = new LedgerEntry(operation, reason, money, credits);
        var prior = Entries.SingleOrDefault(e => e.Operation == operation);
        if (prior == null) return false;
        if (prior != entry) throw new InvalidOperationException("Idempotency payload conflict");
        return true;
    }
    // Caller persists the aggregate atomically; this method performs no partial mutation on failure.
    public bool Apply(string operation, string reason, long money, long credits)
    {
        if (IsReplay(operation, reason, money, credits)) return false;
        var entry = new LedgerEntry(operation, reason, money, credits);
        long nextMoney = checked(Money + money), nextCredits = checked(Credits + credits);
        if (nextMoney < 0 || nextCredits < 0) throw new InvalidOperationException(nextMoney < 0 ? "Insufficient Money" : "Insufficient Credits");
        Entries.Add(entry); Money = nextMoney; Credits = nextCredits;
        return true;
    }
    public void Starter(EconomyConfig c) => Apply("starter", "starter-v1", c.StarterMoney, 10);
    public void Convert(string operation, int credits, EconomyConfig c)
    {
        if (credits <= 0 || c.CreditsToMoney <= 0) throw new ArgumentOutOfRangeException(nameof(credits));
        Apply(operation, "convert:" + c.Version, checked((long)credits * c.CreditsToMoney), -credits);
    }
}
