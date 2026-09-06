using Airsoft.Battle;

namespace Airsoft.Club;

// Server-side prototype config. Approved percentages are not catalog tuning knobs.
public sealed record EconomyConfig(string Version = "economy-003-v1", long BaseReward = 100,
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
    public static long Power(TeamSnapshot team) => Math.Max(1, team.Fighters.Sum(f =>
        checked(100 + (f.Accuracy.Raw + f.Endurance.Raw + f.Agility.Raw + f.StartingHp.Raw +
        f.Loadout.Protection.Raw + (f.Weapon?.Damage.Raw ?? 0)) / Fixed.Scale)) * team.BbTier.DamageMultiplier.Raw / Fixed.Scale);
}
public sealed record LedgerEntry(string Operation, string Reason, long MoneyDelta, long CreditsDelta);
public sealed class Wallet
{
    public long Money { get; set; }
    public long Credits { get; set; }
    public List<LedgerEntry> Entries { get; set; } = new();
    // Caller persists the aggregate atomically; this method performs no partial mutation on failure.
    public bool Apply(string operation, string reason, long money, long credits)
    {
        if (string.IsNullOrWhiteSpace(operation) || string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Ledger identity required");
        var entry = new LedgerEntry(operation, reason, money, credits);
        var prior = Entries.SingleOrDefault(e => e.Operation == operation);
        if (prior != null)
        {
            if (prior != entry) throw new InvalidOperationException("Idempotency payload conflict");
            return false;
        }
        long nextMoney = checked(Money + money), nextCredits = checked(Credits + credits);
        if (nextMoney < 0 || nextCredits < 0) throw new InvalidOperationException("Insufficient balance");
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
