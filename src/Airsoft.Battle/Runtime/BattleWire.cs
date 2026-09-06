using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Airsoft.Battle
{
    // Explicit little-endian schema v1. No reflection, object hashes or runtime serializer.
    public static class BattleWire
    {
        private const int MaxBytes = 64 * 1024 * 1024;
        public static byte[] WriteConfig(MatchConfig config) => Write(1, w =>
        {
            w.Write(config.Seed); WriteRules(w, config.Rules);
            WriteTeam(w, config.Attacker); WriteTeam(w, config.Defender);
        });
        public static MatchConfig ReadConfig(byte[] bytes) => Read(bytes, 1, r =>
        {
            ulong seed = r.ReadUInt64(); var rules = ReadRules(r);
            return new MatchConfig(ReadTeam(r), ReadTeam(r), seed, rules);
        });
        public static byte[] WriteTeam(TeamSnapshot team) => Write(2, w => WriteTeam(w, team));
        public static TeamSnapshot ReadTeam(byte[] bytes) => Read(bytes, 2, ReadTeam);
        public static byte[] WriteResult(MatchResult result) => Write(3, w =>
        {
            w.Write((int)result.Status); w.Write(result.Outcome.HasValue ? (int)result.Outcome.Value : -1);
            w.Write((int)result.Reason); Text(w, result.Diagnostic); w.Write(result.SimulatedDurationMs);
            w.Write(result.Seed); Text(w, result.RulesetVersion.Value);
            WriteTeamResult(w, result.Attacker); WriteTeamResult(w, result.Defender);
            w.Write(result.Events.Count);
            foreach (var e in result.Events)
            { w.Write(e.TimeMs); w.Write((int)e.ActorSide); Text(w, e.ActorId); Text(w, e.TargetId); w.Write(e.Hit); w.Write(e.Damage.Raw); }
        });
        public static MatchResult ReadResult(byte[] bytes) => Read(bytes, 3, r =>
        {
            var status = EnumValue<ResultStatus>(r.ReadInt32());
            int outcomeCode = r.ReadInt32();
            MatchOutcome? outcome = outcomeCode == -1 ? (MatchOutcome?)null : EnumValue<MatchOutcome>(outcomeCode);
            var reason = EnumValue<EndReason>(r.ReadInt32()); string diagnostic = Text(r);
            int time = Count(r, 0, 3_600_000); ulong seed = r.ReadUInt64(); var version = new RulesetVersion(Text(r));
            var a = ReadTeamResult(r); var b = ReadTeamResult(r);
            int count = Count(r, 0, 1_000_000); var events = new BattleEvent[count];
            for (int i = 0; i < count; i++)
            {
                int at = Count(r, 0, time); var side = EnumValue<Side>(r.ReadInt32());
                string actor = Text(r), target = Text(r); bool hit = Bool(r); var damage = Fixed.FromRaw(r.ReadInt64());
                if (damage.Raw < 0 || (!hit && damage.Raw != 0) || (hit && damage.Raw == 0)) throw new InvalidDataException("Invalid event damage.");
                if (i > 0 && at < events[i - 1].TimeMs) throw new InvalidDataException("Unordered events.");
                var actors = side == Side.Attacker ? a : b; var targets = side == Side.Attacker ? b : a;
                if (!actors.Fighters.Any(f => f.Id == actor) || !targets.Fighters.Any(f => f.Id == target)) throw new InvalidDataException("Unknown event actor/target.");
                events[i] = new BattleEvent(at, side, actor, target, hit, damage);
            }
            if (events.Count(e => e.ActorSide == Side.Attacker) != a.BbConsumed ||
                events.Count(e => e.ActorSide == Side.Defender) != b.BbConsumed) throw new InvalidDataException("Ammo/event mismatch.");
            if (status == ResultStatus.Completed && (!outcome.HasValue || reason == EndReason.TechnicalEventGuard))
                throw new InvalidDataException("Invalid completed status.");
            if (status == ResultStatus.TechnicalFailure && (outcome.HasValue || reason != EndReason.TechnicalEventGuard || diagnostic.Length == 0))
                throw new InvalidDataException("Invalid failure status.");
            return new MatchResult(status, outcome, reason, diagnostic, time, a, b, events, seed, version);
        });
        public static string Digest(MatchResult result)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(result.ToCanonicalBytes())).Replace("-", "").ToLowerInvariant();
        }
        private static byte[] Write(int kind, Action<BinaryWriter> body)
        {
            using (var stream = new MemoryStream())
            {
                using (var w = new BinaryWriter(stream, Encoding.UTF8, true))
                { w.Write(0x41434257); w.Write(1); w.Write(kind); body(w); }
                if (stream.Length > MaxBytes) throw new InvalidDataException("Wire size limit.");
                return stream.ToArray();
            }
        }
        private static T Read<T>(byte[] bytes, int kind, Func<BinaryReader, T> body)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length > MaxBytes) throw new InvalidDataException("Wire size limit.");
            using (var stream = new MemoryStream(bytes, false))
            using (var r = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (r.ReadInt32() != 0x41434257 || r.ReadInt32() != 1 || r.ReadInt32() != kind)
                    throw new InvalidDataException("Unsupported wire header/version/kind.");
                T result = body(r);
                if (stream.Position != stream.Length) throw new InvalidDataException("Trailing bytes.");
                return result;
            }
        }
        private static void Text(BinaryWriter w, string value)
        { var bytes = new UTF8Encoding(false, true).GetBytes(value); if (bytes.Length > 4096) throw new InvalidDataException("String too long."); w.Write(bytes.Length); w.Write(bytes); }
        private static string Text(BinaryReader r)
        { int n = Count(r, 0, 4096); var bytes = r.ReadBytes(n); if (bytes.Length != n) throw new EndOfStreamException(); return new UTF8Encoding(false, true).GetString(bytes); }
        private static int Count(BinaryReader r, int min, int max)
        { int n = r.ReadInt32(); if (n < min || n > max) throw new InvalidDataException("Invalid count/value."); return n; }
        private static bool Bool(BinaryReader r)
        { byte b = r.ReadByte(); if (b > 1) throw new InvalidDataException("Invalid Boolean."); return b == 1; }
        private static T EnumValue<T>(int code) where T : struct
        { if (!Enum.IsDefined(typeof(T), code)) throw new InvalidDataException("Invalid enum."); return (T)Enum.ToObject(typeof(T), code); }
        private static void WriteRules(BinaryWriter w, BattleRules x)
        {
            Text(w, x.Version.Value); w.Write(x.MaxDurationMs); w.Write(x.TechnicalEventLimit); w.Write(x.MinIntervalMs);
            foreach (var v in new[] { x.BaseHp, x.HpPerEndurance, x.BaseHit, x.AccuracyCoefficient, x.EvasionCoefficient,
                x.TempoCoefficient, x.MinHit, x.MaxHit, x.ArmorScale, x.MaxMitigation }) w.Write(v.Raw);
        }
        private static BattleRules ReadRules(BinaryReader r) => new BattleRules(Text(r), r.ReadInt32(), r.ReadInt32(), r.ReadInt32(),
            Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()),
            Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()),
            Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()));
        private static void WriteTeam(BinaryWriter w, TeamSnapshot team)
        {
            Text(w, team.BbTier.Id); w.Write(team.BbTier.DamageMultiplier.Raw); w.Write(team.BbTier.Penetration.Raw);
            w.Write(team.BbBudget); w.Write(team.Fighters.Count);
            foreach (var f in team.Fighters)
            {
                Text(w, f.Id); w.Write(f.Accuracy.Raw); w.Write(f.Endurance.Raw); w.Write(f.Agility.Raw); w.Write(f.StartingHp.Raw);
                w.Write(f.Loadout.Protection.Raw); w.Write(f.Loadout.EvasionBonus.Raw); w.Write(f.Loadout.AgilityPenalty.Raw);
                w.Write(f.Weapon != null);
                if (f.Weapon != null)
                {
                    var x = f.Weapon; Text(w, x.Id); w.Write((int)x.Family); w.Write(x.Damage.Raw);
                    w.Write(x.AccuracyContribution.Raw); w.Write(x.Penetration.Raw); w.Write(x.IntervalMs); w.Write(x.Projectiles);
                }
            }
        }
        private static TeamSnapshot ReadTeam(BinaryReader r)
        {
            var bb = new BbTierDefinition(Text(r), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()));
            int budget = r.ReadInt32(); int count = Count(r, 1, 16); var fighters = new FighterSnapshot[count];
            for (int i = 0; i < count; i++)
            {
                string id = Text(r); var accuracy = Fixed.FromRaw(r.ReadInt64()); var endurance = Fixed.FromRaw(r.ReadInt64());
                var agility = Fixed.FromRaw(r.ReadInt64()); var hp = Fixed.FromRaw(r.ReadInt64());
                var armor = new ArmorLoadout(Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()));
                WeaponSnapshot? weapon = null;
                if (Bool(r)) weapon = new WeaponSnapshot(Text(r), EnumValue<WeaponFamily>(r.ReadInt32()),
                    Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), Fixed.FromRaw(r.ReadInt64()), r.ReadInt32(), r.ReadInt32());
                fighters[i] = new FighterSnapshot(id, accuracy, endurance, agility, hp, weapon, armor);
            }
            return new TeamSnapshot(fighters, bb, budget);
        }
        private static void WriteTeamResult(BinaryWriter w, TeamResult team)
        {
            w.Write(team.BbConsumed); w.Write(team.BbRemaining); w.Write(team.Fighters.Count);
            foreach (var f in team.Fighters) { Text(w, f.Id); w.Write(f.MaxHp.Raw); w.Write(f.FinalHp.Raw); }
        }
        private static TeamResult ReadTeamResult(BinaryReader r)
        {
            int used = Count(r, 0, 1_000_000), left = Count(r, 0, 1_000_000), count = Count(r, 1, 16);
            if (used + left > 1_000_000) throw new InvalidDataException("Invalid budget.");
            var fighters = new FighterResult[count];
            for (int i = 0; i < count; i++)
            {
                string id = Require.Id(Text(r), "fighterId"); var max = Require.Value(Fixed.FromRaw(r.ReadInt64()), 1_000_100, "maxHp", true);
                var hp = Fixed.FromRaw(r.ReadInt64());
                if (hp.Raw < 0 || hp.Raw > max.Raw || (i > 0 && StringComparer.Ordinal.Compare(fighters[i - 1].Id, id) >= 0))
                    throw new InvalidDataException("Invalid HP/order.");
                fighters[i] = new FighterResult(id, max, hp);
            }
            return new TeamResult(fighters, used, left);
        }
    }
}
