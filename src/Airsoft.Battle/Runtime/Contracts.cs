using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Airsoft.Battle
{
    public enum WeaponFamily { Pistol, Smg, AssaultRifle, Shotgun, Dmr, SniperRifle }
    public sealed class RulesetVersion
    {
        public string Value { get; }
        public RulesetVersion(string value) { Value = Require.Id(value, nameof(value)); }
    }
    public sealed class WeaponSnapshot
    {
        public string Id { get; }
        public WeaponFamily Family { get; }
        public Fixed Damage { get; }
        public Fixed AccuracyContribution { get; }
        public Fixed Penetration { get; }
        public int IntervalMs { get; }
        public int Projectiles { get; }
        public WeaponSnapshot(string id, WeaponFamily family, Fixed damage, Fixed accuracyContribution,
            Fixed penetration, int intervalMs, int projectiles)
        {
            Id = Require.Id(id, nameof(id));
            if (!Enum.IsDefined(typeof(WeaponFamily), family)) throw new ArgumentOutOfRangeException(nameof(family));
            Family = family;
            Damage = Require.Value(damage, 10_000, nameof(damage), true);
            AccuracyContribution = Require.Value(accuracyContribution, 1, nameof(accuracyContribution));
            Penetration = Require.Value(penetration, 10_000, nameof(penetration));
            IntervalMs = Require.Range(intervalMs, 1, 60_000, nameof(intervalMs));
            Projectiles = Require.Range(projectiles, 1, 64, nameof(projectiles));
        }
    }
    // Prepared combat values from camouflage/head/body slots; no inventory entities.
    public sealed class ArmorLoadout
    {
        public Fixed Protection { get; }
        public Fixed EvasionBonus { get; }
        public Fixed AgilityPenalty { get; }
        public ArmorLoadout(Fixed protection, Fixed evasionBonus, Fixed agilityPenalty)
        {
            Protection = Require.Value(protection, 10_000, nameof(protection));
            EvasionBonus = Require.Value(evasionBonus, 1, nameof(evasionBonus));
            AgilityPenalty = Require.Value(agilityPenalty, 10_000, nameof(agilityPenalty));
        }
        public static ArmorLoadout None => new ArmorLoadout(Fixed.Zero, Fixed.Zero, Fixed.Zero);
    }
    public sealed class BbTierDefinition
    {
        public string Id { get; }
        public Fixed DamageMultiplier { get; }
        public Fixed Penetration { get; }
        public BbTierDefinition(string id, Fixed damageMultiplier, Fixed penetration)
        {
            Id = Require.Id(id, nameof(id));
            DamageMultiplier = Require.Value(damageMultiplier, 2, nameof(damageMultiplier), true);
            Penetration = Require.Value(penetration, 10_000, nameof(penetration));
        }
    }
    public sealed class FighterSnapshot
    {
        public string Id { get; }
        public Fixed Accuracy { get; }
        public Fixed Endurance { get; }
        public Fixed Agility { get; }
        public Fixed StartingHp { get; }
        public WeaponSnapshot? Weapon { get; }
        public ArmorLoadout Loadout { get; }
        public FighterSnapshot(string id, Fixed accuracy, Fixed endurance, Fixed agility, Fixed startingHp,
            WeaponSnapshot? weapon, ArmorLoadout loadout)
        {
            Id = Require.Id(id, nameof(id));
            Accuracy = Require.Value(accuracy, 10_000, nameof(accuracy));
            Endurance = Require.Value(endurance, 10_000, nameof(endurance));
            Agility = Require.Value(agility, 10_000, nameof(agility));
            StartingHp = Require.Value(startingHp, 1_000_100, nameof(startingHp), true);
            Weapon = weapon;
            Loadout = loadout ?? throw new ArgumentNullException(nameof(loadout));
        }
    }
    public sealed class TeamSnapshot
    {
        public ReadOnlyCollection<FighterSnapshot> Fighters { get; }
        public BbTierDefinition BbTier { get; }
        public int BbBudget { get; }
        public TeamSnapshot(IEnumerable<FighterSnapshot> fighters, BbTierDefinition bbTier, int bbBudget)
        {
            if (fighters == null) throw new ArgumentNullException(nameof(fighters));
            var copy = fighters.Take(17).ToArray();
            Require.Range(copy.Length, 1, 16, nameof(fighters));
            if (copy.Any(f => f == null)) throw new ArgumentException("Null fighter.", nameof(fighters));
            if (copy.Select(f => f.Id).Distinct(StringComparer.Ordinal).Count() != copy.Length)
                throw new ArgumentException("Duplicate fighter IDs.", nameof(fighters));
            Fighters = Array.AsReadOnly(copy.OrderBy(f => f.Id, StringComparer.Ordinal).ToArray());
            BbTier = bbTier ?? throw new ArgumentNullException(nameof(bbTier));
            BbBudget = Require.Range(bbBudget, 0, 1_000_000, nameof(bbBudget));
        }
        // Caller supplies the captured club capacity, not a live inventory lookup.
        public static TeamSnapshot DefenseAtCapacity(IEnumerable<FighterSnapshot> fighters,
            BbTierDefinition tier, int capturedCapacity) => new TeamSnapshot(fighters, tier, capturedCapacity);
    }
    public sealed class MatchConfig
    {
        public TeamSnapshot Attacker { get; }
        public TeamSnapshot Defender { get; }
        public ulong Seed { get; }
        public BattleRules Rules { get; }
        public MatchConfig(TeamSnapshot attacker, TeamSnapshot defender, ulong seed, BattleRules rules)
        {
            Attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
            Defender = defender ?? throw new ArgumentNullException(nameof(defender));
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            Seed = seed;
            foreach (var fighter in Attacker.Fighters.Concat(Defender.Fighters))
                if (!Readiness.IsReady(fighter.StartingHp, Formulas.MaxHp(fighter.Endurance, rules)))
                    throw new ArgumentException("Snapshots must contain ready fighters with HP <= MaxHP.");
        }
    }
    public static class Readiness
    {
        public static bool IsReady(Fixed currentHp, Fixed maxHp)
        {
            Require.Value(maxHp, 1_000_100, nameof(maxHp), true);
            // Exact 10% comparison: no division truncation and no injury latch.
            return currentHp.Raw > 0 && currentHp.Raw <= maxHp.Raw &&
                checked(currentHp.Raw * 10) >= maxHp.Raw;
        }
    }
}
