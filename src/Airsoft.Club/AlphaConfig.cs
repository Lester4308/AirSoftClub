namespace Airsoft.Club;

// Versioned server-side prototype values. Product invariants remain fixed; tuning is not final balance.
public static class AlphaConfig
{
    public const string Version = "alpha-014-v1";
    public const int MaxRoster = 16, StarterBb = 60, FighterXpPerLevel = 100, ClubXpPerLevel = 1000;
    public const int TrainingBaseCap = 15, TrainingCapPerLevel = 5, TrainingMoney = 25;
    public const int CapacityBase = 1000, CapacityPerLevel = 500, CapacityMaximum = 1000000;
    public const long FullRecoveryMs = 6000000, RecruitRefreshMs = 3600000, EmergencyCooldownMs = 10800000;
    public const int RecruitCount = 7, RecruitMatches = 10, RecruitRefreshMoney = 50, RecruitStatBase = 5, RecruitVariance = 10, RecruitPricePerStat = 5, ResalePercent = 30;
    public const int BbRefill = 500, EmergencyPercent = 15, AutoBuyLevel = 3, AutoBuyThreshold = 100, AutoBuyMinimumMoney = 150;
    public const int Mk1Money = 100, Mk2Money = 600, Mk3Credits = 6, Mk2Permille = 1020, Mk3Permille = 1035;
    public const int EarlyLevels = 3, EarlyItemCapPercent = 125, EarlyCredits = 1, DailyMoney = 100, DailyStep = 10, SeventhDayCredits = 1;
    public const int FirstBattleCredits = 2, LevelCredits = 1, CreditLevelLimit = 10;
    public const int RatingBase = 10, RatingDivisor = 100, RatingMin = 5, RatingMax = 20, IncomingCap = 4;
    public const long BattleLeaseMs = 120000;
    public static readonly IReadOnlyList<string> BbNames = Array.AsReadOnly(new[] { "Basic", "Improved", "Advanced", "High-End", "Premium" });
    public static readonly IReadOnlyList<int> BbPercent = Array.AsReadOnly(new[] { 100, 103, 105, 110, 115 });
    public static int BbMoney(int tier) => tier == 4 ? 0 : 50 + tier * 20;
    public static int BbCredits(int tier) => tier == 4 ? 1 : 0;
    public static int ShieldCredits(int hours) => hours switch { 8 => 1, 24 => 2, 72 => 4, 168 => 7, _ => throw new InvalidOperationException("Unknown shield") };
}
