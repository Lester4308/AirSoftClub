using System;
using System.Diagnostics;
using System.Linq;
using Airsoft.Battle;
using UnityEngine;

namespace AirsoftClub.Unity
{
    public sealed class BattleHost
    {
        public MatchResult Execute(byte[] input) => new BattleEngine().Run(BattleWire.ReadConfig(input));
        public static byte[] GoldenInput => Resources.Load<TextAsset>("golden-input").bytes;
        public static byte[] GoldenExpected => Resources.Load<TextAsset>("golden-result").bytes;
        public static string GoldenDigest => Resources.Load<TextAsset>("golden-digest").text.Trim();
        public static void Check(bool condition, string message)
        { if (!condition) throw new InvalidOperationException(message); }

        public static MatchResult VerifyGolden()
        {
            var result = new BattleHost().Execute(GoldenInput);
            Check(BattleWire.Digest(result) == GoldenDigest, "Golden digest mismatch");
            Check(BattleWire.WriteResult(result).SequenceEqual(GoldenExpected), "Golden byte mismatch");
            return result;
        }
        public static void VerifyWire()
        {
            var config = BattleWire.ReadConfig(GoldenInput);
            Check(BattleWire.WriteConfig(config).SequenceEqual(GoldenInput), "Config roundtrip");
            foreach (var team in new[] { config.Attacker, config.Defender })
            {
                var wire = BattleWire.WriteTeam(team);
                Check(wire.SequenceEqual(BattleWire.WriteTeam(BattleWire.ReadTeam(wire))), "Team roundtrip");
            }
            var expected = BattleWire.ReadResult(GoldenExpected);
            Check(BattleWire.WriteResult(expected).SequenceEqual(GoldenExpected), "Result roundtrip");
        }
        public static void VerifyNumeric()
        {
            Check(Fixed.Ratio(-1, 3).Raw == -3333, "Signed truncation");
            Check((Fixed.FromRaw(-3) * Fixed.Ratio(1, 2)).Raw == -1, "Multiply");
            Check((Fixed.FromInt(-1) / Fixed.FromInt(3)).Raw == -3333, "Divide");
            Check(Fixed.Clamp(Fixed.FromInt(-1), Fixed.Zero, Fixed.One).Raw == 0, "Clamp");
            bool overflow = false;
            try { Fixed.FromInt(long.MaxValue); } catch (OverflowException) { overflow = true; }
            Check(overflow, "Overflow");
            var rng = new SeededRandom(0);
            Check(rng.NextUInt32() == 0xe220a839 && rng.NextUInt32() == 0x6e789e6a && rng.NextUInt32() == 0x06c45d18, "SplitMix64 vectors");
        }
        public static MatchConfig Sized(int size, ulong seed)
        {
            var source = BattleWire.ReadConfig(GoldenInput);
            var template = source.Attacker.Fighters[0];
            var fighters = Enumerable.Range(0, size).Select(i => new FighterSnapshot("f-" + i.ToString("D2"),
                template.Accuracy, template.Endurance, template.Agility, template.StartingHp, template.Weapon, template.Loadout)).ToArray();
            var team = new TeamSnapshot(fighters, source.Attacker.BbTier, 500);
            return new MatchConfig(team, team, seed, source.Rules);
        }
        public static void Validate(MatchConfig config, MatchResult result)
        {
            Check(result.Status == ResultStatus.Completed, "Technical failure");
            Check(result.SimulatedDurationMs <= config.Rules.MaxDurationMs, "Duration");
            foreach (var team in new[] { result.Attacker, result.Defender })
            {
                Check(team.BbRemaining >= 0 && team.BbConsumed >= 0, "Ammo");
                Check(team.Fighters.All(f => f.FinalHp.Raw >= 0 && f.FinalHp.Raw <= f.MaxHp.Raw), "HP");
            }
            Check(result.Attacker.BbConsumed == result.Events.Count(e => e.ActorSide == Side.Attacker), "Attacker projectiles");
            Check(result.Defender.BbConsumed == result.Events.Count(e => e.ActorSide == Side.Defender), "Defender projectiles");
        }
        public static void BatchAndPerformance()
        {
            var engine = new BattleEngine();
            foreach (int size in new[] { 1, 8, 16 })
            {
                var configs = Enumerable.Range(0, 100).Select(i => Sized(size, (ulong)i)).ToArray();
                engine.Run(configs[0]); // Warm-up; preparation excluded.
                long memoryBefore = GC.GetTotalMemory(false);
                var timer = Stopwatch.StartNew();
                foreach (var config in configs) Validate(config, engine.Run(config));
                timer.Stop();
                UnityEngine.Debug.Log("PERF size=" + size + " battles=100 elapsedMs=" +
                    timer.Elapsed.TotalMilliseconds.ToString("F3", System.Globalization.CultureInfo.InvariantCulture) +
                    " heapDeltaBytes=" + (GC.GetTotalMemory(false) - memoryBefore));
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void PlayerSmoke()
        {
            if (!Environment.GetCommandLineArgs().Contains("--airsoft-smoke")) return;
            try
            {
                VerifyGolden(); VerifyWire(); VerifyNumeric(); BatchAndPerformance();
                UnityEngine.Debug.Log("AIRSOFT_PLAYER_PASSED digest=" + GoldenDigest);
                Application.Quit(0);
            }
            catch (Exception exception) { UnityEngine.Debug.LogException(exception); Application.Quit(1); }
        }
    }
}
