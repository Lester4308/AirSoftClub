using Airsoft.Club;
internal static partial class Program
{
    static void AlphaTests()
    {
        Test("alpha gradual weapon unlock and access-only progressive depth", () =>
        {
            var s = Starter(); Reject(() => Clubs.Buy(s, "SniperRifle-MK1", "locked"));
            Reject(() => Retention.EarlyUnlock(s, "SniperRifle-MK1", "too-far"));
            Reject(() => Retention.EarlyUnlock(s, "Shotgun-MK3", "premium"));
            long money = s.Wallet.Money; s.Xp = 2000; Retention.EarlyUnlock(s, "Shotgun-MK1", "access");
            Check(s.Items.Count == 0 && s.Wallet.Money == money && s.Wallet.Credits == 9);
            Clubs.Buy(s, "Shotgun-MK1", "buy"); Check(s.Items.Count == 1 && s.Wallet.Money == money - AlphaConfig.Mk1Money);
        });
        Test("partial heal exact charge and recovery remainder preserved", () =>
        {
            var s = Starter(); var f = s.Fighters[0]; f.Hp = 100001; f.RecoveryRemainder = 5;
            Clubs.HealAmount(s, f.Id, 10, 0, "partial"); Check(f.Hp == 200001 && s.Wallet.Money == 990 && f.RecoveryRemainder == 5);
            Clubs.Heal(s, f.Id, 0, "full"); Check(f.Hp == f.MaxHp && f.RecoveryRemainder == 0);
            Reject(() => Clubs.HealAmount(s, f.Id, -1, 0, "negative"));
        });
        Test("seventh UTC daily grants configured Credits once", () =>
        {
            var s = Starter(); for (int day = 0; day < 7; day++) Retention.Daily(s, day * Pvp.Day);
            Check(s.Retention.Streak == 7 && s.Wallet.Credits == 10 + AlphaConfig.SeventhDayCredits);
            Reject(() => Retention.Daily(s, 7 * Pvp.Day - 1)); Retention.Daily(s, 7 * Pvp.Day);
            Check(s.Retention.Streak == 1 && s.Wallet.Credits == 11);
        });
        Test("relative power reflects weapon and ammunition without exposing exact UI score", () =>
        {
            var s = Starter(); long bare = Rewards.Power(Clubs.Snapshot(s, false, 0));
            Clubs.Equip(s, s.Fighters[0].Id, Clubs.Buy(s, "Pistol-MK1", "gear"));
            long armed = Rewards.Power(Clubs.Snapshot(s, false, 0)); Check(armed > bare);
            s.BbStock[0] = 0; Check(Rewards.Power(Clubs.Snapshot(s, false, 0)) < armed);
            Check(Rewards.Category(100, 100) == "Balanced" && Rewards.Category(100, 200) == "Very Strong");
        });
        Test("recruit quality scales with club level and paid resale returns original cost", () =>
        {
            var low = Starter(); var high = Starter(); high.Xp = 5000;
            Clubs.Refresh(low, 3600000, 13); Clubs.Refresh(high, 3600000, 13);
            Check(high.Offers.Zip(low.Offers).All(p => p.First.Accuracy > p.Second.Accuracy && p.First.Price > p.Second.Price));
            var f = Clubs.Hire(low, low.Offers[0].Id, low.OfferVersion, false, 3600000, "paid");
            long before = low.Wallet.Money; f.Xp = 10000; Clubs.Dismiss(low, f.Id, "resale");
            Check(low.Wallet.Money == before + f.InitialPrice * AlphaConfig.ResalePercent / 100 && low.FreeRecruitClaimed);
        });
    }
}
