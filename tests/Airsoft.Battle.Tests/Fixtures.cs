using System;
using System.Linq;
using Airsoft.Battle;

namespace Airsoft.Battle.Tests;

// PROTOTYPE TEST VALUES — NOT FINAL BALANCE. Not a production item catalog.
internal static class Fixtures
{
    public static Fixed F(int n) => Fixed.FromInt(n);
    public static readonly WeaponSnapshot[] Weapons =
    {
        new("Basic Pistol", WeaponFamily.Pistol, F(18), Fixed.Zero, F(2), 1000, 1),
        new("Basic SMG", WeaponFamily.Smg, F(9), Fixed.Ratio(2,100), F(3), 700, 3),
        new("Basic AR", WeaponFamily.AssaultRifle, F(14), Fixed.Ratio(4,100), F(6), 900, 3),
        new("Basic Shotgun", WeaponFamily.Shotgun, F(7), Fixed.Zero, F(1), 1500, 6),
        new("Basic DMR", WeaponFamily.Dmr, F(32), Fixed.Ratio(8,100), F(12), 1500, 1),
        new("Basic Sniper", WeaponFamily.SniperRifle, F(55), Fixed.Ratio(12,100), F(20), 2200, 1)
    };
    public static readonly ArmorLoadout[] Armors =
    {
        ArmorLoadout.None, new(F(10), Fixed.Zero, F(0)),
        new(F(25), Fixed.Zero, F(2)), new(F(50), Fixed.Zero, F(5))
    };
    public static readonly BbTierDefinition[] Bbs =
    {
        new("Basic", Fixed.One, F(0)), new("Improved", Fixed.Ratio(103,100), F(0)),
        new("Advanced", Fixed.Ratio(105,100), F(0)), new("High-End", Fixed.Ratio(110,100), F(0)),
        new("Premium", Fixed.Ratio(115,100), F(0))
    };
    public static FighterSnapshot Fighter(string id = "f", int accuracy = 10, int endurance = 10,
        int agility = 10, int hp = 100, WeaponSnapshot? weapon = null, bool weaponless = false,
        ArmorLoadout? armor = null) =>
        new(id, F(accuracy), F(endurance), F(agility), F(hp),
            weaponless ? null : weapon ?? Weapons[0], armor ?? Armors[0]);
    public static TeamSnapshot Team(int count = 1, int ammo = 200, int variant = 0) =>
        new(Enumerable.Range(0, count).Select(i => Fighter("fighter-" + i.ToString("D2"),
            weapon: Weapons[(i + variant) % Weapons.Length], armor: Armors[(i + variant) % Armors.Length])),
            Bbs[variant % Bbs.Length], ammo);
    public static TeamSnapshot Single(FighterSnapshot fighter, int ammo = 200, BbTierDefinition? bb = null) =>
        new(new[] { fighter }, bb ?? Bbs[0], ammo);
    public static MatchConfig Match(TeamSnapshot? a = null, TeamSnapshot? b = null, ulong seed = 1, BattleRules? rules = null) =>
        new(a ?? Team(), b ?? Team(), seed, rules ?? new BattleRules());
}
