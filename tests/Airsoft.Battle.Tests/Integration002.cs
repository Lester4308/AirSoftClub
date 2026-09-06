using System;
using System.IO;
using System.Linq;
using Airsoft.Battle;

namespace Airsoft.Battle.Tests;

internal static class Integration002
{
    private const string FixturePath = "src/Airsoft.Battle/Runtime/Resources/";
    public static void Generate()
    {
        Directory.CreateDirectory(FixturePath);
        var config = Fixtures.Match(Fixtures.Team(3, 250, 1), Fixtures.Team(2, 300, 2), 0xFEDCBA9876543210UL);
        File.WriteAllBytes(FixturePath + "golden-input.bytes", BattleWire.WriteConfig(config));
        var result = new BattleEngine().Run(config);
        File.WriteAllBytes(FixturePath + "golden-result.bytes", BattleWire.WriteResult(result));
        File.WriteAllText(FixturePath + "golden-digest.txt", BattleWire.Digest(result));
        Console.WriteLine("GOLDEN " + BattleWire.Digest(result));
    }
    public static void Verify()
    {
        var input = File.ReadAllBytes(FixturePath + "golden-input.bytes");
        var config = BattleWire.ReadConfig(input);
        var expectedBytes = File.ReadAllBytes(FixturePath + "golden-result.bytes");
        var result = new BattleEngine().Run(config);
        Assert(input.SequenceEqual(BattleWire.WriteConfig(config)), "Config roundtrip");
        Assert(expectedBytes.SequenceEqual(BattleWire.WriteResult(result)), "Golden result bytes");
        Assert(result.ToCanonicalBytes().SequenceEqual(BattleWire.ReadResult(expectedBytes).ToCanonicalBytes()), "Result roundtrip");
        var teamBytes = BattleWire.WriteTeam(config.Attacker);
        Assert(teamBytes.SequenceEqual(BattleWire.WriteTeam(BattleWire.ReadTeam(teamBytes))), "Team roundtrip");
        Assert(BattleWire.Digest(result) == File.ReadAllText(FixturePath + "golden-digest.txt").Trim(), "Digest");
        var weaponless = Fixtures.Match(Fixtures.Single(Fixtures.Fighter(weaponless: true)), Fixtures.Team(ammo: 0));
        Assert(BattleWire.ReadConfig(BattleWire.WriteConfig(weaponless)).Attacker.Fighters[0].Weapon == null, "Nullable weapon");
        var failure = new BattleEngine().Run(Fixtures.Match(rules: new BattleRules(technicalEventLimit: 1)));
        var copy = BattleWire.ReadResult(BattleWire.WriteResult(failure));
        Assert(copy.Status == ResultStatus.TechnicalFailure && copy.Outcome == null && copy.Diagnostic.Length > 0, "Failure roundtrip");
        var corrupt = (byte[])input.Clone(); corrupt[4] = 2;
        Reject(() => BattleWire.ReadConfig(corrupt));
        Reject(() => BattleWire.ReadConfig(input.Concat(new byte[] { 0 }).ToArray()));
        Reject(() => BattleWire.ReadConfig(input.Take(input.Length - 1).ToArray()));
        Console.WriteLine("GOLDEN " + BattleWire.Digest(result));
    }
    private static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Reject(Action action)
    {
        try { action(); } catch (InvalidDataException) { return; } catch (EndOfStreamException) { return; }
        throw new Exception("Malformed wire accepted.");
    }
}
