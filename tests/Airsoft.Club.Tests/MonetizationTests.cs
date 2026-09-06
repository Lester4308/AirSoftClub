using Airsoft.Battle;
using Airsoft.Club;
internal static partial class Program
{
    static void MonetizationTests()
    {
        Test("progressive early access boundaries, price depth and permanent entitlement", () =>
        {
            int[] depths = { 0, 1, 2, 2, 3, 3, 3, 3, 3, 3 };
            for (int level = 1; level <= 10; level++)
            {
                var s = Starter(); s.Xp = (level - 1) * 1000; Check(EarlyAccess.Depth(level) == depths[level - 1]);
                foreach (var item in Catalog.Items)
                    Check(EarlyAccess.CanUnlock(s, item) == (item.Credits == 0 && item.Level > level && item.Level <= level + depths[level - 1]));
            }
            var club = Starter(); Reject(() => Retention.EarlyUnlock(club, "Smg-MK1", "lv1"));
            club.Xp = 2000; Retention.EarlyUnlock(club, "Dmr-MK1", "depth2"); Check(club.Wallet.Credits == 8 && club.Items.Count == 0);
            Reject(() => Retention.EarlyUnlock(club, "Dmr-MK1", "duplicate"));
            Check(EarlyAccess.Price(1) == 1 && EarlyAccess.Price(2) == 2 && EarlyAccess.Price(3) == 4);
        });
        Test("legacy early weapon is capped by throughput and releases naturally without repurchase", () =>
        {
            var s = Starter(); var f = s.Fighters[0]; s.Unlocks.Add("AssaultRifle-MK2");
            Clubs.Equip(s, f.Id, Clubs.Buy(s, "AssaultRifle-MK2", "legacy"));
            var early = Clubs.Snapshot(s, true, 0).Fighters[0]; var native = EarlyAccess.Native(Catalog.Get("AssaultRifle-MK2"));
            Check(early.Weapon!.Damage.Raw < native.Damage.Raw);
            var baseline = new FighterSnapshot(f.Id, early.Accuracy, early.Endurance, early.Agility, early.StartingHp, EarlyAccess.Native(Catalog.Get("Pistol-MK2")), early.Loadout);
            decimal ratio = (decimal)early.Weapon.Damage.Raw * early.Weapon.Projectiles / Formulas.IntervalMs(early, new BattleRules()) / ((decimal)baseline.Weapon!.Damage.Raw * baseline.Weapon.Projectiles / Formulas.IntervalMs(baseline, new BattleRules()));
            Check(ratio <= 1.25m);
            int items = s.Items.Count; long money = s.Wallet.Money; s.Xp = 2000;
            Check(Clubs.Snapshot(s, true, 0).Fighters[0].Weapon!.Damage.Raw == native.Damage.Raw && s.Items.Count == items && s.Wallet.Money == money && s.Unlocks.Contains("AssaultRifle-MK2"));
        });
        Test("all current early weapon depths bound throughput at varied fighter tempo", () =>
        {
            foreach (int level in Enumerable.Range(2, 9)) foreach (int agility in new[] { 1, 20, 100 })
            {
                var s = Starter(); s.Xp = (level - 1) * 1000;
                foreach (var item in Catalog.Items.Where(i => i.Slot == Slot.Weapon && EarlyAccess.CanUnlock(s, i)))
                {
                    var f = new FighterSnapshot("cap", Fixed.FromInt(20), Fixed.FromInt(20), Fixed.FromInt(agility), Fixed.FromInt(100), EarlyAccess.Native(item), new ArmorLoadout(Fixed.Zero, Fixed.Zero, Fixed.Zero));
                    var capped = EarlyAccess.Cap(s, f);
                    decimal Dps(FighterSnapshot x) => (decimal)x.Weapon!.Damage.Raw * x.Weapon.Projectiles / Formulas.IntervalMs(x, new());
                    decimal baseline = Catalog.Items.Where(i => i.Slot == Slot.Weapon && i.Credits == 0 && i.Level <= level).Max(i => Dps(new FighterSnapshot(f.Id, f.Accuracy, f.Endurance, f.Agility, f.StartingHp, EarlyAccess.Native(i), f.Loadout)));
                    Check(Dps(capped) <= baseline * 1.25m && capped.Weapon!.Damage.Raw > 0);
                }
            }
        });
        Test("early armor effective HP bound and natural release", () =>
        {
            var s = Starter(); var item = Catalog.Get("HeadProtection-3"); int capped = EarlyAccess.Protection(s, item);
            Check((50m + capped) / (50 + Catalog.Get("HeadProtection-1").Protection) <= 1.25m);
            s.Xp = 2000; Check(EarlyAccess.Protection(s, item) == item.Protection);
        });
    }
}
