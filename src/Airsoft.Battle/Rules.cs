using System;

namespace Airsoft.Battle
{
    public sealed class BattleRules
    {
        public RulesetVersion Version { get; }
        public Fixed BaseHp { get; }
        public Fixed HpPerEndurance { get; }
        public Fixed BaseHit { get; }
        public Fixed AccuracyCoefficient { get; }
        public Fixed EvasionCoefficient { get; }
        public Fixed TempoCoefficient { get; }
        public Fixed MinHit { get; }
        public Fixed MaxHit { get; }
        public Fixed ArmorScale { get; }
        public Fixed MaxMitigation { get; }
        public int MinIntervalMs { get; }
        public int MaxDurationMs { get; }
        public int TechnicalEventLimit { get; }

        // PROTOTYPE CONFIG — NOT FINAL BALANCE.
        public BattleRules(string version = "prototype-001", int maxDurationMs = 120_000,
            int technicalEventLimit = 1_000_000, int minIntervalMs = 10,
            Fixed? baseHp = null, Fixed? hpPerEndurance = null, Fixed? baseHit = null,
            Fixed? accuracyCoefficient = null, Fixed? evasionCoefficient = null,
            Fixed? tempoCoefficient = null, Fixed? minHit = null, Fixed? maxHit = null,
            Fixed? armorScale = null, Fixed? maxMitigation = null)
        {
            Version = new RulesetVersion(version);
            BaseHp = Require.Value(baseHp ?? Fixed.FromInt(50), 100, nameof(baseHp), true);
            HpPerEndurance = Require.Value(hpPerEndurance ?? Fixed.FromInt(5), 100, nameof(hpPerEndurance), true);
            BaseHit = Require.Value(baseHit ?? Fixed.Ratio(50, 100), 1, nameof(baseHit));
            AccuracyCoefficient = Require.Value(accuracyCoefficient ?? Fixed.Ratio(1, 100), 1, nameof(accuracyCoefficient));
            EvasionCoefficient = Require.Value(evasionCoefficient ?? Fixed.Ratio(5, 1000), 1, nameof(evasionCoefficient));
            TempoCoefficient = Require.Value(tempoCoefficient ?? Fixed.Ratio(2, 100), 1, nameof(tempoCoefficient));
            MinHit = Require.Value(minHit ?? Fixed.Ratio(5, 100), 1, nameof(minHit), true);
            MaxHit = Require.Value(maxHit ?? Fixed.Ratio(95, 100), 1, nameof(maxHit), true);
            if (MinHit.Raw > MaxHit.Raw) throw new ArgumentException("Hit bounds inverted.");
            ArmorScale = Require.Value(armorScale ?? Fixed.FromInt(50), 10_000, nameof(armorScale), true);
            MaxMitigation = Require.Value(maxMitigation ?? Fixed.Ratio(80, 100), 1, nameof(maxMitigation));
            if (MaxMitigation.Raw >= Fixed.Scale) throw new ArgumentOutOfRangeException(nameof(maxMitigation));
            MinIntervalMs = Require.Range(minIntervalMs, 1, 60_000, nameof(minIntervalMs));
            MaxDurationMs = Require.Range(maxDurationMs, 1, 3_600_000, nameof(maxDurationMs));
            TechnicalEventLimit = Require.Range(technicalEventLimit, 1, 1_000_000, nameof(technicalEventLimit));
        }
    }
    public static class Formulas
    {
        public static Fixed MaxHp(Fixed endurance, BattleRules rules) =>
            rules.BaseHp + Require.Value(endurance, 10_000, nameof(endurance)) * rules.HpPerEndurance;
        public static Fixed EffectiveAgility(FighterSnapshot fighter) =>
            Fixed.Max(Fixed.Zero, fighter.Agility - fighter.Loadout.AgilityPenalty);
        public static Fixed Evasion(FighterSnapshot fighter, BattleRules rules) =>
            EffectiveAgility(fighter) * rules.EvasionCoefficient + fighter.Loadout.EvasionBonus;
        public static Fixed HitChance(FighterSnapshot actor, FighterSnapshot target, BattleRules rules) =>
            Fixed.Clamp(rules.BaseHit + actor.Accuracy * rules.AccuracyCoefficient +
                (actor.Weapon?.AccuracyContribution ?? Fixed.Zero) - Evasion(target, rules),
                rules.MinHit, rules.MaxHit);
        public static int IntervalMs(FighterSnapshot actor, BattleRules rules)
        {
            if (actor.Weapon == null) throw new ArgumentException("Weaponless fighter has no ranged schedule.");
            Fixed speed = Fixed.One + EffectiveAgility(actor) * rules.TempoCoefficient;
            long numerator = checked((long)actor.Weapon.IntervalMs * Fixed.Scale);
            // Ceiling prevents truncation to zero; explicit positive minimum guarantees time progress.
            return Math.Max(rules.MinIntervalMs, checked((int)((numerator + speed.Raw - 1) / speed.Raw)));
        }
        public static Fixed Damage(WeaponSnapshot weapon, BbTierDefinition bb, ArmorLoadout armor, BattleRules rules)
        {
            Fixed protection = Fixed.Max(Fixed.Zero, armor.Protection - weapon.Penetration - bb.Penetration);
            Fixed mitigation = Fixed.Min(rules.MaxMitigation, protection / (rules.ArmorScale + protection));
            Fixed raw = weapon.Damage * bb.DamageMultiplier;
            // One quantum minimum for a valid hit, including tiny configured damage.
            return Fixed.Max(Fixed.FromRaw(1), raw * (Fixed.One - mitigation));
        }
    }
}
