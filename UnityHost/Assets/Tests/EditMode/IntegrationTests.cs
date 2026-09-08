using System;
using System.Linq;
using Airsoft.Battle;
using AirsoftClub.Unity;
using NUnit.Framework;

public sealed class IntegrationTests
{
    [Test] public void GoldenBytesAndDigest() { BattleHost.VerifyGolden(); }
    [Test] public void SerializationRoundtrips() { BattleHost.VerifyWire(); }
    [Test] public void FixedAndRngVectors() { BattleHost.VerifyNumeric(); }
    [Test] public void ReadinessBoundary()
    {
        Assert.IsFalse(Readiness.IsReady(Fixed.FromInt(9), Fixed.FromInt(100)));
        Assert.IsTrue(Readiness.IsReady(Fixed.FromInt(10), Fixed.FromInt(100)));
    }
    [TestCase(null, 0u, "Steam AppID is not configured.")]
    [TestCase("", 0u, "Steam AppID is not configured.")]
    [TestCase("0", 0u, "Steam AppID is not configured.")]
    [TestCase("480", 480u, "Steam sample AppID 480 is not accepted for this game.")]
    [TestCase("123", 456u, "Steam AppID mismatch.")]
    public void SteamAppIdValidationRejectsUnsafeConfiguration(string configured, uint running, string expected)
    {
        Assert.IsFalse(SteamIdentityAdapter.TryValidateAppId(configured, running, out string error));
        Assert.AreEqual(expected, error);
    }
    [Test] public void SteamAppIdValidationAcceptsConfiguredAndMatchingRuntime()
    {
        Assert.IsTrue(SteamIdentityAdapter.TryValidateAppId("123", 0, out string beforeInitialization));
        Assert.IsNull(beforeInitialization);
        Assert.IsTrue(SteamIdentityAdapter.TryValidateAppId("123", 123, out string afterInitialization));
        Assert.IsNull(afterInitialization);
    }
    [Test] public void Repeat100()
    {
        for (int i = 0; i < 100; i++) BattleHost.VerifyGolden();
    }
    [Test] public void DifferentSeedsValidAndVariable()
    {
        var digests = Enumerable.Range(0, 20).Select(i =>
        {
            var config = BattleHost.Sized(16, (ulong)i);
            var result = new BattleEngine().Run(config); BattleHost.Validate(config, result);
            return string.Join(",", result.Events.Select(e => e.Hit.ToString()));
        }).Distinct().Count();
        Assert.Greater(digests, 1);
    }
    [Test] public void Batch300AndPerformance() { BattleHost.BatchAndPerformance(); }
    [Test] public void ValidationErrorsPropagate() { Assert.Throws<System.IO.EndOfStreamException>(() => new BattleHost().Execute(new byte[0])); }
    [Test] public void TechnicalFailureSurvivesWire()
    {
        var c = BattleWire.ReadConfig(BattleHost.GoldenInput);
        var input = new MatchConfig(c.Attacker, c.Defender, c.Seed, new BattleRules(technicalEventLimit: 1));
        var result = BattleWire.ReadResult(BattleWire.WriteResult(new BattleEngine().Run(input)));
        Assert.AreEqual(ResultStatus.TechnicalFailure, result.Status);
        Assert.IsNull(result.Outcome); Assert.IsNotEmpty(result.Diagnostic);
    }
}
