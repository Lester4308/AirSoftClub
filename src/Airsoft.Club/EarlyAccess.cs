using Airsoft.Battle;
namespace Airsoft.Club;

public static class EarlyAccess
{
    public static int Depth(int level) => level < 2 ? 0 : level == 2 ? 1 : level < 5 ? 2 : 3;
    public static int Price(int depth) => depth is >= 1 and <= 3 ? AlphaConfig.EarlyCredits * (1 << (depth - 1)) : throw new InvalidOperationException("Invalid early-access depth");
    public static bool CanUnlock(ClubState s, ItemDefinition i) => i.Credits == 0 && i.Level > s.Level && i.Level <= s.Level + Depth(s.Level) && !s.Unlocks.Contains(i.Id);
    public static WeaponSnapshot Native(ItemDefinition i) => new(i.Id, i.Family, Fixed.FromInt(i.Damage) * Fixed.Ratio(i.Mk == 3 ? AlphaConfig.Mk3Permille : i.Mk == 2 ? AlphaConfig.Mk2Permille : 1000, 1000), Fixed.Zero, Fixed.Zero, i.Interval, i.Projectiles);
    public static FighterSnapshot Cap(ClubState s, FighterSnapshot f)
    {
        var weapon = f.Weapon;
        if (weapon == null || Catalog.Get(weapon.Id).Level <= s.Level) return f;
        var rules = new BattleRules();
        FighterSnapshot With(WeaponSnapshot w) => new(f.Id, f.Accuracy, f.Endurance, f.Agility, f.StartingHp, w, f.Loadout);
        // Compare sustained contribution at the actual fighter's tempo, same armor, accuracy and BB.
        // Damage/projectiles/interval are all included; this is not a blanket +25% damage modifier.
        long best = Catalog.Items.Where(i => i.Slot == Slot.Weapon && i.Level <= s.Level && i.Credits == 0)
            .Max(i => { var w = Native(i); return w.Damage.Raw * w.Projectiles * 1000 / Formulas.IntervalMs(With(w), rules); });
        long maximum = best * AlphaConfig.EarlyItemCapPercent / 100 * Formulas.IntervalMs(f, rules) / (weapon.Projectiles * 1000L);
        return With(new WeaponSnapshot(weapon.Id, weapon.Family, Fixed.FromRaw(Math.Min(weapon.Damage.Raw, maximum)), weapon.AccuracyContribution, weapon.Penetration, weapon.IntervalMs, weapon.Projectiles));
    }
    public static int Protection(ClubState s, ItemDefinition item)
    {
        if (item.Level <= s.Level) return item.Protection;
        int baseline = Catalog.Items.Where(i => i.Slot == item.Slot && i.Level <= s.Level && i.Credits == 0).Max(i => i.Protection);
        long scale = new BattleRules().ArmorScale.Raw / Fixed.Scale;
        // (scale + protection) bounds relative effective HP; other equipped armor only lowers this ratio.
        return (int)Math.Min(item.Protection, (scale + baseline) * AlphaConfig.EarlyItemCapPercent / 100 - scale);
    }
}
