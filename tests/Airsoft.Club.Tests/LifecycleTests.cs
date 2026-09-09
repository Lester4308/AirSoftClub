using Airsoft.Club;
using Airsoft.Battle;
internal static partial class Program
{
    static ClubState Starter()
    { var s = Clubs.Create(Guid.NewGuid().ToString("N"), 0); Clubs.Hire(s, s.Offers[0].Id, 1, true, 0, "free"); return s; }
    static void LifecycleTests()
    {
        Test("appearance identity persists from recruit offer to fighter", () =>
        {
            var s = Clubs.Create(Guid.NewGuid().ToString("N"), 0);
            var offer = s.Offers[0];
            Check(offer.AppearanceId is "male-017" or "female-017");
            Clubs.Hire(s, offer.Id, 1, true, 0, "appearance");
            Check(s.Fighters.Single().AppearanceId == offer.AppearanceId);
            Check(Clubs.StableAppearance("legacy") == Clubs.StableAppearance("legacy"));
        });
        Test("auto Basic opt-in level threshold guard and no premium spend", () =>
        {
            var s = Starter(); s.BbStock[0] = 0; Check(!Clubs.AutoRefill(s, "off")); s.AutoBuyBasic = true; Check(!Clubs.AutoRefill(s, "locked"));
            s.Xp = 2000; Check(Clubs.AutoRefill(s, "refill") && s.BbStock[0] == 500 && s.Wallet.Money == 950 && s.Wallet.Credits == 10);
            s.BbStock[0] = 0; s.Wallet.Apply("poor", "fixture", -801, 0); Check(!Clubs.AutoRefill(s, "guard"));
            s.Wallet.Apply("cash", "fixture", 500, 0); s.ActiveBbTier = 4; Check(!Clubs.AutoRefill(s, "premium") && s.Wallet.Credits == 10);
        });
        Test("permanent free choice, consumed offer, roster16", () =>
        {
            var shifted = Clubs.Create("shifted", 0); Clubs.Hire(shifted, shifted.Offers[0].Id, 1, false, 0, "paid-first");
            Reject(() => Clubs.Hire(shifted, "1-3", 1, true, 0, "shifted-free"));
            var s = Starter(); Reject(() => Clubs.Hire(s, s.Offers[0].Id, 1, true, 0, "again"));
            s.Wallet.Apply("fixture", "fixture", 100000, 0);
            while (s.Fighters.Count < 16) { if (s.Offers.Count == 0) Clubs.Refresh(s, 0, 3, true, "refresh" + s.OfferVersion); Clubs.Hire(s, s.Offers[0].Id, s.OfferVersion, false, 0, Guid.NewGuid().ToString()); }
            Reject(() => Clubs.Hire(s, s.Offers[0].Id, s.OfferVersion, false, 0, "17"));
            Check(s.Fighters.Count == 16 && s.Wallet.Credits == 10);
        });
        Test("refresh 3599/3600 and nine/ten, duplicate completion", () =>
        {
            var s = Starter(); Reject(() => Clubs.Refresh(s, 3599999, 2));
            Clubs.Refresh(s, 3600000, 2); Check(s.OfferVersion == 2 && s.CompletedSinceRefresh == 0);
            for (int i = 0; i < 9; i++) Clubs.Completed(s, "m" + i);
            Reject(() => Clubs.Refresh(s, 3600001, 3)); Clubs.Completed(s, "m8"); Check(s.CompletedSinceRefresh == 9);
            Clubs.Completed(s, "m9"); Clubs.Refresh(s, 3600001, 3); Check(s.CompletedSinceRefresh == 0 && s.OffersAt == 3600001);
            Clubs.Completed(s, "new"); Clubs.Refresh(s, 3600002, 4, true, "paid"); Check(s.CompletedSinceRefresh == 0 && s.OffersAt == 3600002);
            Reject(() => Clubs.Hire(s, "1-1", 1, false, 3600002, "stale"));
        });
        Test("dismiss original price, gear returned, last defender retained", () =>
        {
            var s = Starter(); var first = s.Fighters[0]; var item = Clubs.Buy(s, "Pistol-MK1", "buy"); Clubs.Equip(s, first.Id, item);
            Reject(() => Clubs.Dismiss(s, first.Id, "last"));
            var other = Clubs.Hire(s, s.Offers[0].Id, 1, false, 0, "hire"); long money = s.Wallet.Money;
            Clubs.Dismiss(s, first.Id, "dismiss"); Check(s.Wallet.Money == money && s.Items.ContainsKey(item) && first.Equipment.Count == 0);
            Reject(() => Clubs.Dismiss(s, first.Id, "dismiss")); Clubs.Equip(s, other.Id, item);
        });
        Test("readiness exact and fractional recovery split invariant", () =>
        {
            var f = new Fighter { Endurance = 10, Hp = 99999 }; Check(!f.Ready); f.Hp = 100000; Check(f.Ready);
            var g = new Fighter { Endurance = 10, Hp = 100000 };
            for (int t = 1; t <= 1000; t++) f.Recover(t); g.Recover(1000);
            Check(f.Hp == g.Hp && f.RecoveryRemainder == g.RecoveryRemainder);
            Reject(() => f.Recover(999)); f.Recover(long.MaxValue); Check(f.Hp == f.MaxHp && f.RecoveryRemainder == 0);
        });
        Test("Endurance materializes old recovery without free HP", () =>
        {
            var s = Starter(); var f = s.Fighters[0]; f.Endurance = 10; f.Hp = 800000;
            Clubs.Train(s, f.Id, "Endurance", 60000, "train"); Check(f.Hp == 810000 && f.MaxHp == 1050000);
            f.Recover(120000); Check(f.Hp == 820500);
            f.Accuracy = f.TrainingCap; Reject(() => Clubs.Train(s, f.Id, "Accuracy", 120000, "cap"));
        });
        Test("automatic ready and weaponless/full defense isolation", () =>
        {
            var s = Starter(); var f = s.Fighters[0]; f.Hp = 0; s.BbStock[0] = 0;
            var defense = Clubs.Snapshot(s, true, 0); Check(defense.Fighters.Count == 1 && defense.BbBudget == s.Capacity && f.Hp == 0 && s.BbStock[0] == 0);
            Reject(() => Clubs.Snapshot(s, false, 0)); f.Hp = f.MaxHp;
            Check(Clubs.Snapshot(s, false, 0).Fighters[0].Weapon == null);
        });
        Test("shared BB, emergency boundaries, no premium automatic spend", () =>
        {
            var s = Starter(); s.Wallet.Apply("poor", "fixture", -1000, 0); s.BbStock[0] = 150;
            Reject(() => Clubs.Emergency(s, 0)); s.BbStock[0] = 149; Clubs.Emergency(s, 0); Check(s.BbStock[0] == 649);
            s.BbStock[0] = 0; Reject(() => Clubs.Emergency(s, 10799999)); Clubs.Emergency(s, 10800000);
            Check(s.Wallet.Credits == 10); s.BbStock[4] = 999; Clubs.Refill(s, 4, "premium"); Check(s.BbStock[4] == 1000 && s.Wallet.Credits == 9 && s.ActiveBbTier == 0);
            Reject(() => Clubs.Refill(s, 4, "full"));
        });
        Test("pending offense blocks management and duplicate equip rejected", () =>
        {
            var s = Starter(); var id = Clubs.Buy(s, "Pistol-MK1", "buy"); Clubs.Equip(s, s.Fighters[0].Id, id);
            Reject(() => Clubs.Equip(s, s.Fighters[0].Id, id)); s.PendingMatch = "pending";
            Reject(() => Clubs.Heal(s, s.Fighters[0].Id, 0, "heal")); Reject(() => Clubs.Refill(s, 0, "refill"));
        });
    }
}
