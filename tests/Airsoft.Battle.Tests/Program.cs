using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Airsoft.Battle;
using static Airsoft.Battle.Tests.Fixtures;

namespace Airsoft.Battle.Tests;

internal static class Program
{
    private static int passed, failed;
    private static void Check(bool condition, string message = "Assertion failed")
    { if (!condition) throw new Exception(message); }
    private static void Equal<T>(T expected, T actual) { Check(Equals(expected, actual), $"Expected {expected}, actual {actual}"); }
    private static void Throws<T>(Action action) where T : Exception
    { try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
    private static void Test(string name, Action action)
    {
        try { action(); passed++; Console.WriteLine("PASS " + name); }
        catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + ": " + e); }
    }
    private sealed class ZeroRandom : IRandomSource
    { public uint NextUInt32() => 0; public int NextInt(int max) => 0; }
    private sealed class MissRandom : IRandomSource
    { public uint NextUInt32() => uint.MaxValue; public int NextInt(int max) => max - 1; }

    public static int Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "--simulate")
        {
            if (!int.TryParse(args[1], out int count) || count < 1 || count > 1_000_000) return 2;
            return Simulate(count);
        }
        if (args.Length != 0) { Console.Error.WriteLine("Usage: [--simulate count]"); return 2; }
        RunTests();
        Console.WriteLine($"TEST SUMMARY passed={passed} failed={failed}");
        return failed == 0 ? 0 : 1;
    }

    private static void RunTests()
    {
        Test("fixed ratio and signed truncation", () =>
        {
            Equal(3333L, Fixed.Ratio(1, 3).Raw); Equal(-3333L, Fixed.Ratio(-1, 3).Raw);
            Equal(1L, (Fixed.FromRaw(3) * Fixed.Ratio(1, 2)).Raw);
            Equal(-1L, (Fixed.FromRaw(-3) * Fixed.Ratio(1, 2)).Raw);
            Equal(6666L, (Fixed.Ratio(1, 3) * F(2)).Raw);
        });
        Test("fixed overflow and divide by zero", () =>
        {
            Throws<OverflowException>(() => Fixed.FromInt(long.MaxValue));
            Throws<OverflowException>(() => { _ = Fixed.FromRaw(long.MaxValue) + Fixed.One; });
            Throws<OverflowException>(() => { _ = Fixed.FromRaw(long.MaxValue) * F(2); });
            Throws<DivideByZeroException>(() => Fixed.Ratio(1, 0));
        });
        Test("RNG golden vectors seed zero", () =>
        {
            var rng = new SeededRandom(0);
            Equal(0xE220A839U, rng.NextUInt32()); Equal(0x6E789E6AU, rng.NextUInt32()); Equal(0x06C45D18U, rng.NextUInt32());
        });
        Test("RNG sequence repeat and range", () =>
        {
            var a = new SeededRandom(ulong.MaxValue); var b = new SeededRandom(ulong.MaxValue);
            foreach (int bound in new[] { 1, 3, 10_000, int.MaxValue })
                for (int i = 0; i < 1000; i++) { int value = a.NextInt(bound); Equal(value, b.NextInt(bound)); Check(value >= 0 && value < bound); }
            Throws<ArgumentOutOfRangeException>(() => a.NextInt(0));
        });
        var rules = new BattleRules();
        Test("Accuracy raises hit chance", () => Check(Formulas.HitChance(Fighter(accuracy: 30), Fighter(), rules).Raw > Formulas.HitChance(Fighter(accuracy: 5), Fighter(), rules).Raw));
        Test("Agility raises evasion", () => Check(Formulas.HitChance(Fighter(), Fighter(agility: 30), rules).Raw < Formulas.HitChance(Fighter(), Fighter(agility: 5), rules).Raw));
        Test("Agility shortens action interval", () => Check(Formulas.IntervalMs(Fighter(agility: 30), rules) < Formulas.IntervalMs(Fighter(agility: 5), rules)));
        Test("Equipment penalty affects tempo and evasion", () =>
        {
            var heavy = Fighter(armor: Armors[3]); var light = Fighter();
            Check(Formulas.IntervalMs(heavy, rules) > Formulas.IntervalMs(light, rules));
            Check(Formulas.Evasion(heavy, rules).Raw < Formulas.Evasion(light, rules).Raw);
        });
        Test("Endurance raises MaxHP", () => { Equal(F(100), Formulas.MaxHp(F(10), rules)); Equal(F(150), Formulas.MaxHp(F(20), rules)); });
        Test("Hit min five percent", () => Equal(Fixed.Ratio(5, 100), Formulas.HitChance(Fighter(accuracy: 0), Fighter(agility: 10000), rules)));
        Test("Hit max ninety-five percent", () => Equal(Fixed.Ratio(95, 100), Formulas.HitChance(Fighter(accuracy: 10000), Fighter(agility: 0), rules)));
        Test("Formula coefficients configurable", () =>
        {
            var r = new BattleRules(accuracyCoefficient: Fixed.Zero, evasionCoefficient: Fixed.Zero, tempoCoefficient: Fixed.Zero);
            Equal(Formulas.HitChance(Fighter(accuracy: 1), Fighter(), r), Formulas.HitChance(Fighter(accuracy: 100), Fighter(), r));
            Equal(1000, Formulas.IntervalMs(Fighter(agility: 100), r));
        });
        Test("Armor reduces damage", () => Check(Formulas.Damage(Weapons[0], Bbs[0], Armors[3], rules).Raw < Formulas.Damage(Weapons[0], Bbs[0], Armors[0], rules).Raw));
        Test("Penetration reduces mitigation", () => Check(Formulas.Damage(Weapons[0], new("penetrating", Fixed.One, F(30)), Armors[3], rules).Raw > Formulas.Damage(Weapons[0], Bbs[0], Armors[3], rules).Raw));
        Test("BB multiplier modifies damage", () => Equal(Fixed.Ratio(207, 10), Formulas.Damage(Weapons[0], Bbs[4], Armors[0], rules)));
        Test("Mitigation bounded and quantum positive", () =>
        {
            var armor = new ArmorLoadout(F(10000), Fixed.Zero, Fixed.Zero);
            var damage = Formulas.Damage(Weapons[0], Bbs[0], armor, rules);
            Equal(Fixed.Ratio(36, 10), damage);
            var tiny = new WeaponSnapshot("tiny", WeaponFamily.Pistol, Fixed.FromRaw(1), Fixed.Zero, Fixed.Zero, 1000, 1);
            Equal(1L, Formulas.Damage(tiny, Bbs[0], armor, rules).Raw);
            Throws<ArgumentOutOfRangeException>(() => new BattleRules(maxMitigation: Fixed.One));
        });
        Test("Readiness exact boundaries without latch", () =>
        {
            Check(!Readiness.IsReady(Fixed.Zero, F(100))); Check(!Readiness.IsReady(F(9), F(100)));
            Check(!Readiness.IsReady(Fixed.FromRaw(99999), F(100)));
            Check(Readiness.IsReady(F(10), F(100))); Check(Readiness.IsReady(F(100), F(100)));
            Check(!Readiness.IsReady(F(101), F(100)));
            Check(!Readiness.IsReady(Fixed.FromRaw(10), Fixed.FromRaw(101)));
            Check(Readiness.IsReady(Fixed.FromRaw(11), Fixed.FromRaw(101)));
        });
        Test("MaxHP upgrade leaves CurrentHP unchanged", () =>
        {
            var f = Fighter(endurance: 12, hp: 80);
            var config = Match(Single(f, 0), Team(ammo: 0));
            var result = new BattleEngine().Run(config);
            Equal(F(110), result.Attacker.Fighters[0].MaxHp); Equal(F(80), result.Attacker.Fighters[0].FinalHp);
        });
        Test("Empty and oversize teams rejected", () =>
        {
            Throws<ArgumentOutOfRangeException>(() => Team(0)); Throws<ArgumentOutOfRangeException>(() => Team(17));
            Throws<ArgumentOutOfRangeException>(() => Team(ammo: -1));
        });
        Test("Duplicate IDs and invalid health rejected", () =>
        {
            Throws<ArgumentException>(() => new TeamSnapshot(new[] { Fighter(), Fighter() }, Bbs[0], 20));
            Throws<ArgumentException>(() => Match(Single(Fighter(hp: 101))));
            Throws<ArgumentException>(() => Match(Single(Fighter(hp: 9))));
            Throws<ArgumentOutOfRangeException>(() => Fighter(hp: 0));
        });
        Test("Invalid rules and weapon rejected", () =>
        {
            Throws<ArgumentOutOfRangeException>(() => new BattleRules(maxDurationMs: 0));
            Throws<ArgumentOutOfRangeException>(() => new BattleRules(minIntervalMs: 0));
            Throws<ArgumentException>(() => new BattleRules(minHit: Fixed.One, maxHit: Fixed.Ratio(1, 2)));
            Throws<ArgumentOutOfRangeException>(() => new WeaponSnapshot("bad", WeaponFamily.Shotgun, F(1), Fixed.Zero, Fixed.Zero, 1000, 0));
        });
        Test("Snapshots copied sorted and read-only", () =>
        {
            var input = new[] { Fighter("z"), Fighter("a") }; var team = new TeamSnapshot(input, Bbs[0], 10);
            input[1] = Fighter("replaced"); Equal("a", team.Fighters[0].Id);
            Throws<NotSupportedException>(() => ((IList<FighterSnapshot>)team.Fighters)[0] = Fighter());
        });
        Test("Defense uses full captured capacity and leaves inputs unchanged", () =>
        {
            var defender = TeamSnapshot.DefenseAtCapacity(new[] { Fighter() }, Bbs[0], 500);
            var result = new BattleEngine().Run(Match(Single(Fighter(hp: 35)), defender));
            Equal(500, defender.BbBudget); Equal(F(100), defender.Fighters[0].StartingHp);
            Equal(500, result.Defender.BbRemaining + result.Defender.BbConsumed);
        });
        Test("Both weaponless is no-progress draw", () =>
        {
            var t = Single(Fighter(weaponless: true));
            var r = new BattleEngine().Run(Match(t, t)); Equal(EndReason.NoProgress, r.Reason); Equal(0, r.Events.Count);
        });
        Test("Weaponless target remains and cannot attack", () =>
        {
            var config = Match(Single(Fighter(weaponless: true)), Team());
            var r = new BattleEngine(_ => new ZeroRandom()).Run(config);
            Equal(MatchOutcome.DefenderWin, r.Outcome); Equal(0, r.Attacker.BbConsumed);
            Check(r.Events.All(e => e.ActorSide == Side.Defender)); Validate(config, r);
        });
        Test("Mixed weaponless roster", () =>
        {
            var t = new TeamSnapshot(new[] { Fighter("unarmed", weaponless: true), Fighter("armed") }, Bbs[0], 100);
            var c = Match(t, Team(2)); var r = new BattleEngine().Run(c);
            Equal(2, r.Attacker.Fighters.Count); Check(r.Events.All(e => e.ActorSide != Side.Attacker || e.ActorId != "unarmed")); Validate(c, r);
        });
        Test("One side no ammo still receives attacks", () =>
        {
            var c = Match(Team(ammo: 0), Team()); var r = new BattleEngine(_ => new ZeroRandom()).Run(c);
            Equal(MatchOutcome.DefenderWin, r.Outcome); Check(r.Events.Count > 0); Validate(c, r);
        });
        Test("Both no ammo is immediate draw", () =>
        {
            var r = new BattleEngine().Run(Match(Team(ammo: 0), Team(ammo: 0)));
            Equal(EndReason.NoProgress, r.Reason); Equal(0, r.SimulatedDurationMs);
        });
        Test("Partial burst consumes only remaining BB", () =>
        {
            var t = Single(Fighter(weapon: Weapons[1]), 2); var c = Match(t, Team(ammo: 0));
            var r = new BattleEngine(_ => new ZeroRandom()).Run(c);
            Equal(2, r.Attacker.BbConsumed); Equal(2, r.Events.Count); Equal(EndReason.NoProgress, r.Reason); Validate(c, r);
        });
        Test("Shotgun one BB per pellet", () =>
        {
            var c = Match(Single(Fighter(weapon: Weapons[3]), 6), Team(ammo: 0));
            var r = new BattleEngine(_ => new MissRandom()).Run(c);
            Equal(6, r.Events.Count); Equal(6, r.Attacker.BbConsumed); Validate(c, r);
        });
        Test("Same-time final elimination draws", () =>
        {
            var lethal = new WeaponSnapshot("lethal", WeaponFamily.Pistol, F(100), Fixed.Zero, Fixed.Zero, 1000, 1);
            var t = Single(Fighter(weapon: lethal)); var c = Match(t, t);
            var r = new BattleEngine(_ => new ZeroRandom()).Run(c);
            Equal(EndReason.SimultaneousElimination, r.Reason); Equal(MatchOutcome.Draw, r.Outcome);
            Equal(2, r.Events.Count); Equal(1, r.Attacker.EliminatedFighters.Count); Validate(c, r);
        });
        Test("Dead fighter never attacks at later time", () =>
        {
            var gun = new WeaponSnapshot("fast-lethal", WeaponFamily.Pistol, F(100), Fixed.Zero, Fixed.Zero, 100, 1);
            var c = Match(Single(Fighter(weapon: gun)), Team()); var r = new BattleEngine(_ => new ZeroRandom()).Run(c);
            Equal(1, r.Events.Count); Equal(Side.Attacker, r.Events[0].ActorSide); Validate(c, r);
        });
        Test("Tempo schedules faster fighter multiple times", () =>
        {
            var c = Match(Single(Fighter(agility: 100)), Single(Fighter(agility: 0)), rules: new BattleRules(maxDurationMs: 1100));
            var r = new BattleEngine(_ => new MissRandom()).Run(c);
            Check(r.Events.Count(e => e.ActorSide == Side.Attacker) > r.Events.Count(e => e.ActorSide == Side.Defender));
            Validate(c, r);
        });
        Test("Time cap is exclusive, no action cap draw", () =>
        {
            int interval = Formulas.IntervalMs(Fighter(), rules);
            var r = new BattleEngine().Run(Match(rules: new BattleRules(maxDurationMs: interval)));
            Equal(EndReason.DurationLimit, r.Reason); Equal(interval, r.SimulatedDurationMs); Equal(0, r.Events.Count);
        });
        Test("Technical guard explicit failure, batch atomic", () =>
        {
            var c = Match(rules: new BattleRules(technicalEventLimit: 1));
            var r = new BattleEngine().Run(c);
            Equal(ResultStatus.TechnicalFailure, r.Status); Check(r.Outcome == null);
            Equal(EndReason.TechnicalEventGuard, r.Reason); Check(r.Diagnostic.Length > 0);
            Equal(0, r.Attacker.BbConsumed); Equal(0, r.Defender.BbConsumed); Equal(0, r.Events.Count);
        });
        Test("100 identical full authoritative results", () =>
        {
            var c = Match(Team(16), Team(16), 87654321); var engine = new BattleEngine();
            var expected = engine.Run(c).ToCanonicalBytes();
            for (int i = 0; i < 100; i++) Check(expected.SequenceEqual(engine.Run(c).ToCanonicalBytes()));
        });
        Test("Different seeds vary event sequence safely", () =>
        {
            var logs = new HashSet<string>();
            for (ulong seed = 0; seed < 100; seed++) { var c = Match(Team(3), Team(3), seed); var r = new BattleEngine().Run(c); Validate(c, r); logs.Add(Convert.ToBase64String(r.ToCanonicalBytes())); }
            Check(logs.Count > 1);
            var x = new BattleEngine().Run(Match(seed: 1)); var y = new BattleEngine().Run(Match(seed: 2));
            Check(!x.Events.Select(e => (e.Hit, e.TargetId)).SequenceEqual(y.Events.Select(e => (e.Hit, e.TargetId))));
        });
        Test("Collection permutation and culture do not change output", () =>
        {
            var team = Team(8); var reversed = new TeamSnapshot(team.Fighters.Reverse(), team.BbTier, team.BbBudget);
            var expected = new BattleEngine().Run(Match(team, team)).ToCanonicalBytes();
            var prior = CultureInfo.CurrentCulture;
            try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA"); Check(expected.SequenceEqual(new BattleEngine().Run(Match(reversed, reversed)).ToCanonicalBytes())); }
            finally { CultureInfo.CurrentCulture = prior; }
        });
        foreach (var sizes in new[] { (1, 1), (1, 2), (2, 1), (3, 3), (8, 8), (16, 16) })
            Test($"Required roster {sizes.Item1}v{sizes.Item2}", () =>
            {
                for (ulong seed = 0; seed < 20; seed++) { var c = Match(Team(sizes.Item1), Team(sizes.Item2), seed); Validate(c, new BattleEngine().Run(c)); }
            });
        Test("All 256 size pairs terminate safely", () =>
        {
            for (int a = 1; a <= 16; a++) for (int b = 1; b <= 16; b++) { var c = Match(Team(a), Team(b), (ulong)(a * 100 + b)); Validate(c, new BattleEngine().Run(c)); }
        });
    }

    // Independent batch replay checks valid/live actors and targets, HP, elimination and ammo conservation.
    private static void Validate(MatchConfig c, MatchResult r)
    {
        Equal(ResultStatus.Completed, r.Status); Check(r.Outcome.HasValue);
        Check(r.SimulatedDurationMs >= 0 && r.SimulatedDurationMs <= c.Rules.MaxDurationMs);
        var inputs = new[] { c.Attacker, c.Defender }; var outputs = new[] { r.Attacker, r.Defender };
        var hp = inputs.Select(t => t.Fighters.ToDictionary(f => f.Id, f => f.StartingHp.Raw)).ToArray();
        var seenDead = new HashSet<(int, string)>();
        int previousTime = -1;
        foreach (var group in r.Events.GroupBy(e => e.TimeMs))
        {
            Check(group.Key > previousTime && group.Key < c.Rules.MaxDurationMs); previousTime = group.Key;
            var pending = new Dictionary<(int, string), long>();
            foreach (var e in group)
            {
                int side = (int)e.ActorSide, targetSide = 1 - side;
                Check(hp[side].ContainsKey(e.ActorId) && hp[side][e.ActorId] > 0);
                Check(hp[targetSide].ContainsKey(e.TargetId) && hp[targetSide][e.TargetId] > 0);
                Check(!seenDead.Contains((side, e.ActorId)));
                Check(inputs[side].Fighters.Single(f => f.Id == e.ActorId).Weapon != null);
                Check(e.Hit ? e.Damage.Raw > 0 : e.Damage.Raw == 0);
                var key = (targetSide, e.TargetId);
                pending[key] = pending.GetValueOrDefault(key) + e.Damage.Raw;
            }
            foreach (var entry in pending)
            {
                hp[entry.Key.Item1][entry.Key.Item2] = Math.Max(0, hp[entry.Key.Item1][entry.Key.Item2] - entry.Value);
                if (hp[entry.Key.Item1][entry.Key.Item2] == 0) seenDead.Add(entry.Key);
            }
        }
        for (int side = 0; side < 2; side++)
        {
            var result = outputs[side];
            Equal(inputs[side].BbBudget, result.BbConsumed + result.BbRemaining);
            Equal(r.Events.Count(e => (int)e.ActorSide == side), result.BbConsumed);
            Check(result.BbRemaining >= 0);
            foreach (var f in result.Fighters) { Check(f.FinalHp.Raw >= 0 && f.FinalHp.Raw <= f.MaxHp.Raw); Equal(hp[side][f.Id], f.FinalHp.Raw); }
        }
    }

    private static int Simulate(int count)
    {
        int[] wins = new int[3]; int failures = 0; long totalDuration = 0, ammoA = 0, ammoB = 0;
        var durations = new int[count]; var reasons = new Dictionary<EndReason, int>();
        var engine = new BattleEngine();
        var scenarios = new[] { (1, 1), (1, 2), (2, 1), (3, 3), (8, 8), (16, 16) };
        for (int i = 0; i < count; i++)
        {
            var pair = scenarios[i % scenarios.Length];
            var c = Match(Team(pair.Item1, 500, i % 5), Team(pair.Item2, 500, (i / 5) % 5), (ulong)i);
            var r = engine.Run(c);
            try { Validate(c, r); } catch (Exception e) { failures++; Console.Error.WriteLine($"seed={i}: {e.Message}"); }
            if (r.Outcome.HasValue) wins[(int)r.Outcome.Value]++;
            durations[i] = r.SimulatedDurationMs; totalDuration += durations[i];
            ammoA += r.Attacker.BbConsumed; ammoB += r.Defender.BbConsumed;
            reasons[r.Reason] = reasons.GetValueOrDefault(r.Reason) + 1;
        }
        Array.Sort(durations);
        Console.WriteLine("PROTOTYPE TEST VALUES — NOT FINAL BALANCE");
        Console.WriteLine($"battles={count} completed={wins.Sum()} invariantFailures={failures}");
        Console.WriteLine($"attackerWins={wins[0]} defenderWins={wins[1]} draws={wins[2]}");
        Console.WriteLine(FormattableString.Invariant($"terminationPercent={100m * wins.Sum() / count:F2} drawPercent={100m * wins[2] / count:F2}"));
        Console.WriteLine(FormattableString.Invariant($"durationMs min={durations[0]} mean={(decimal)totalDuration / count:F2} p50={durations[count / 2]} p95={durations[Math.Min(count - 1, count * 95 / 100)]} max={durations[count - 1]}"));
        Console.WriteLine(FormattableString.Invariant($"bbConsumed meanAttacker={(decimal)ammoA / count:F2} meanDefender={(decimal)ammoB / count:F2}"));
        foreach (var reason in reasons.OrderBy(k => k.Key)) Console.WriteLine($"reason.{reason.Key}={reason.Value}");
        return failures == 0 && wins.Sum() == count ? 0 : 1;
    }
}
