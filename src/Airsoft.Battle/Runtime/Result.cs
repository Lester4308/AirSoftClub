using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Airsoft.Battle
{
    public enum Side { Attacker, Defender }
    public enum MatchOutcome { AttackerWin, DefenderWin, Draw }
    public enum ResultStatus { Completed, TechnicalFailure }
    public enum EndReason { Elimination, SimultaneousElimination, NoProgress, DurationLimit, TechnicalEventGuard }
    public sealed class BattleEvent
    {
        public int TimeMs { get; }
        public Side ActorSide { get; }
        public string ActorId { get; }
        public string TargetId { get; }
        public bool Hit { get; }
        public Fixed Damage { get; }
        public BattleEvent(int timeMs, Side actorSide, string actorId, string targetId, bool hit, Fixed damage)
        { TimeMs = timeMs; ActorSide = actorSide; ActorId = actorId; TargetId = targetId; Hit = hit; Damage = damage; }
    }
    public sealed class FighterResult
    {
        public string Id { get; }
        public Fixed MaxHp { get; }
        public Fixed FinalHp { get; }
        public bool Eliminated => FinalHp.Raw == 0;
        internal FighterResult(string id, Fixed maxHp, Fixed hp) { Id = id; MaxHp = maxHp; FinalHp = hp; }
    }
    public sealed class TeamResult
    {
        public ReadOnlyCollection<FighterResult> Fighters { get; }
        public ReadOnlyCollection<string> EliminatedFighters { get; }
        public int BbConsumed { get; }
        public int BbRemaining { get; }
        internal TeamResult(IEnumerable<FighterResult> fighters, int consumed, int remaining)
        {
            Fighters = Array.AsReadOnly(fighters.ToArray());
            EliminatedFighters = Array.AsReadOnly(Fighters.Where(f => f.Eliminated).Select(f => f.Id).ToArray());
            BbConsumed = consumed; BbRemaining = remaining;
        }
    }
    public sealed class MatchResult
    {
        public ResultStatus Status { get; }
        public MatchOutcome? Outcome { get; }
        public EndReason Reason { get; }
        public string Diagnostic { get; }
        public int SimulatedDurationMs { get; }
        public TeamResult Attacker { get; }
        public TeamResult Defender { get; }
        public ReadOnlyCollection<BattleEvent> Events { get; }
        public ulong Seed { get; }
        public RulesetVersion RulesetVersion { get; }
        internal MatchResult(ResultStatus status, MatchOutcome? outcome, EndReason reason, string diagnostic,
            int time, TeamResult attacker, TeamResult defender, IEnumerable<BattleEvent> events,
            ulong seed, RulesetVersion version)
        {
            Status = status; Outcome = outcome; Reason = reason; Diagnostic = diagnostic;
            SimulatedDurationMs = time; Attacker = attacker; Defender = defender;
            Events = Array.AsReadOnly(events.ToArray()); Seed = seed; RulesetVersion = version;
        }
        // Versioned length-prefixed invariant serialization for repeat/replay diagnostics.
        public byte[] ToCanonicalBytes()
        {
            var b = new StringBuilder();
            void Add(string s) { b.Append(s.Length.ToString(CultureInfo.InvariantCulture)).Append(':').Append(s); }
            void Number(long n) { Add(n.ToString(CultureInfo.InvariantCulture)); }
            Add("battle-result-v1"); Number((int)Status); Number(Outcome.HasValue ? (int)Outcome.Value : -1);
            Number((int)Reason); Add(Diagnostic); Number(SimulatedDurationMs);
            Add(Seed.ToString(CultureInfo.InvariantCulture)); Add(RulesetVersion.Value);
            foreach (var team in new[] { Attacker, Defender })
            {
                Number(team.BbConsumed); Number(team.BbRemaining); Number(team.Fighters.Count);
                foreach (var fighter in team.Fighters) { Add(fighter.Id); Number(fighter.MaxHp.Raw); Number(fighter.FinalHp.Raw); }
            }
            Number(Events.Count);
            foreach (var e in Events)
            { Number(e.TimeMs); Number((int)e.ActorSide); Add(e.ActorId); Add(e.TargetId); Number(e.Hit ? 1 : 0); Number(e.Damage.Raw); }
            return Encoding.UTF8.GetBytes(b.ToString());
        }
    }
}
