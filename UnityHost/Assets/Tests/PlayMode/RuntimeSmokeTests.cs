using System.Collections;
using AirsoftClub.Unity;
using NUnit.Framework;
using UnityEngine.TestTools;

public sealed class RuntimeSmokeTests
{
    [UnityTest] public IEnumerator GoldenRuntimeSmoke()
    {
        yield return null;
        Assert.IsNotNull(BattleHost.VerifyGolden());
        BattleHost.VerifyWire(); BattleHost.VerifyNumeric();
    }
}
