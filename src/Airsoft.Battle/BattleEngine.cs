using System;
using System.Collections.Generic;
using System.Linq;

namespace Airsoft.Battle
{
    public sealed class BattleEngine
    {
        private readonly Func<ulong, IRandomSource> randomFactory;
        public BattleEngine(Func<ulong, IRandomSource>? randomFactory = null)
        { this.randomFactory = randomFactory ?? (seed => new SeededRandom(seed)); }

        private sealed class Actor
        {
            public FighterSnapshot Snapshot { get; }
            public Fixed Hp;
            public readonly Fixed MaxHp;
            public int NextMs;
            public readonly int Interval;
            public Actor(FighterSnapshot snapshot, BattleRules rules)
            {
                Snapshot = snapshot; Hp = snapshot.StartingHp; MaxHp = Formulas.MaxHp(snapshot.Endurance, rules);
                Interval = snapshot.Weapon == null ? 0 : Formulas.IntervalMs(snapshot, rules);
                NextMs = Interval;
            }
        }
        private sealed class Team
        {
            public readonly TeamSnapshot Snapshot;
            public readonly Actor[] Actors;
            public int Ammo;
            public Team(TeamSnapshot snapshot, BattleRules rules)
            { Snapshot = snapshot; Actors = snapshot.Fighters.Select(f => new Actor(f, rules)).ToArray(); Ammo = snapshot.BbBudget; }
            public bool Alive => Actors.Any(a => a.Hp.Raw > 0);
            public bool CanAttack => Ammo > 0 && Actors.Any(a => a.Hp.Raw > 0 && a.Snapshot.Weapon != null);
            public TeamResult Result() => new TeamResult(
                Actors.Select(a => new FighterResult(a.Snapshot.Id, a.MaxHp, a.Hp)), Snapshot.BbBudget - Ammo, Ammo);
        }

        public MatchResult Run(MatchConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            var rng = randomFactory(config.Seed) ?? throw new InvalidOperationException("Null random source.");
            var rules = config.Rules;
            var teams = new[] { new Team(config.Attacker, rules), new Team(config.Defender, rules) };
            var events = new List<BattleEvent>();
            int time = 0;
            MatchResult Finish(MatchOutcome? outcome, EndReason reason, bool failure = false) =>
                new MatchResult(failure ? ResultStatus.TechnicalFailure : ResultStatus.Completed, outcome, reason,
                    failure ? "TECHNICAL_EVENT_GUARD: projectile event limit exceeded; no gameplay outcome." : "",
                    time, teams[0].Result(), teams[1].Result(), events, config.Seed, rules.Version);

            while (true)
            {
                bool aliveA = teams[0].Alive, aliveB = teams[1].Alive;
                if (!aliveA && !aliveB) return Finish(MatchOutcome.Draw, EndReason.SimultaneousElimination);
                if (!aliveA) return Finish(MatchOutcome.DefenderWin, EndReason.Elimination);
                if (!aliveB) return Finish(MatchOutcome.AttackerWin, EndReason.Elimination);
                if (!teams[0].CanAttack && !teams[1].CanAttack) return Finish(MatchOutcome.Draw, EndReason.NoProgress);
                int next = int.MaxValue;
                foreach (var team in teams)
                    if (team.Ammo > 0)
                        foreach (var actor in team.Actors)
                            if (actor.Hp.Raw > 0 && actor.Snapshot.Weapon != null) next = Math.Min(next, actor.NextMs);
                // Exclusive deadline: events at or beyond the cap are not fired.
                if (next >= rules.MaxDurationMs) { time = rules.MaxDurationMs; return Finish(MatchOutcome.Draw, EndReason.DurationLimit); }
                time = next;

                // Reserve the entire batch's projectile count before modifying ammo or HP.
                int batchCount = 0;
                foreach (var team in teams)
                {
                    int remaining = team.Ammo;
                    foreach (var actor in team.Actors)
                        if (actor.Hp.Raw > 0 && actor.Snapshot.Weapon != null && actor.NextMs == time)
                        {
                            int count = Math.Min(remaining, actor.Snapshot.Weapon.Projectiles);
                            remaining -= count; batchCount += count;
                        }
                }
                if (batchCount > rules.TechnicalEventLimit - events.Count)
                    return Finish(null, EndReason.TechnicalEventGuard, true);

                var damage = new[] { new long[teams[0].Actors.Length], new long[teams[1].Actors.Length] };
                // Stable side then ordinal fighter-ID order allocates shared ammo and RNG draws.
                // All actors/targets alive at batch start remain eligible until damage commits.
                for (int side = 0; side < teams.Length; side++)
                {
                    var team = teams[side];
                    var other = teams[1 - side];
                    var targets = Enumerable.Range(0, other.Actors.Length).Where(i => other.Actors[i].Hp.Raw > 0).ToArray();
                    foreach (var actor in team.Actors)
                    {
                        var weapon = actor.Snapshot.Weapon;
                        if (actor.Hp.Raw <= 0 || weapon == null || actor.NextMs != time || team.Ammo == 0) continue;
                        int count = Math.Min(team.Ammo, weapon.Projectiles); // Partial burst/pellet volley.
                        team.Ammo -= count;
                        int targetIndex = targets[Draw(rng, targets.Length)];
                        var target = other.Actors[targetIndex];
                        Fixed chance = Formulas.HitChance(actor.Snapshot, target.Snapshot, rules);
                        for (int projectile = 0; projectile < count; projectile++)
                        {
                            bool hit = Draw(rng, (int)Fixed.Scale) < chance.Raw;
                            Fixed dealt = hit ? Formulas.Damage(weapon, team.Snapshot.BbTier, target.Snapshot.Loadout, rules) : Fixed.Zero;
                            damage[1 - side][targetIndex] = checked(damage[1 - side][targetIndex] + dealt.Raw);
                            events.Add(new BattleEvent(time, (Side)side, actor.Snapshot.Id, target.Snapshot.Id, hit, dealt));
                        }
                        actor.NextMs = checked(time + actor.Interval);
                    }
                }
                for (int side = 0; side < teams.Length; side++)
                    for (int i = 0; i < teams[side].Actors.Length; i++)
                    {
                        var actor = teams[side].Actors[i];
                        actor.Hp = Fixed.FromRaw(Math.Max(0, actor.Hp.Raw - damage[side][i]));
                    }
            }
        }
        private static int Draw(IRandomSource rng, int max)
        {
            int value = rng.NextInt(max);
            if (value < 0 || value >= max) throw new InvalidOperationException("Injected RNG violated range contract.");
            return value;
        }
    }
}
