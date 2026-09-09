using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AirsoftClub.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public sealed class DiagnosticAuditTests
{
    static void Set(object target, string field, object value) => target.GetType()
        .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    [UnityTest]
    public IEnumerator AuditInventorySettingsAndResources()
    {
        var failures = new List<string>();
        var fixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        fixture.Fighters = new[] { new FighterView { Id = "audit-fighter", Name = "Audit", Hp = 1000000,
            MaxHp = 1000000, Level = 1, Accuracy = 10, Endurance = 10, Agility = 10,
            Ready = true, Equipment = Array.Empty<EquipmentView>() } };
        fixture.Items = new[] { new ItemView { Id = "audit-item", Definition = "Pistol-MK1", Slot = "Weapon", Equipped = false } };
        var host = new GameObject("DiagnosticAudit");
        var client = host.AddComponent<ClubClient>();
        Set(client, "club", fixture);
        Set(client, "page", "Roster");
        Set(client, "selectedFighter", "audit-fighter");
        Set(client, "inventorySlot", "Weapon");
        client.SetModernForTest(true);
        yield return null;
        yield return null;
        var buttons = client.Modern.GetComponentsInChildren<Button>();
        foreach (var label in new[] { "CLOSE", "EQUIP" })
        {
            var b = buttons.FirstOrDefault(x => x.GetComponentInChildren<Text>()?.text == label);
            bool ok = b != null && b.IsInteractable();
            Debug.Log("DIAGNOSTIC inventory_" + label + "=" + ok);
            if (!ok) failures.Add("Inventory " + label + " is disabled or absent");
        }
        foreach (var name in new[] { "male-inspect", "male-roster", "male-combat-near", "male-combat-mass",
            "female-inspect", "female-roster", "female-combat-near", "female-combat-mass" })
            if (Resources.Load<Sprite>("Art/Volumetric017/Sprites/" + name) == null) failures.Add("Missing fighter " + name);
        foreach (var family in new[] { "pistol", "smg", "assault-rifle", "pump-shotgun", "dmr", "sniper-rifle" })
            foreach (var suffix in new[] { "card", "silhouette" })
                if (Resources.Load<Sprite>("Art/Volumetric017/Weapons/" + family + "-" + suffix) == null)
                    failures.Add("Missing weapon " + family + "-" + suffix);
        Debug.Log("DIAGNOSTIC sprite_missing=" + failures.Count(x => x.StartsWith("Missing")));
        Set(client, "inventorySlot", "");
        Set(client, "page", "Settings");
        yield return null;
        yield return null;
        bool isHub = client.Modern.GetComponentsInChildren<Text>().Any(t => t.text == "HEADQUARTERS");
        Debug.Log("DIAGNOSTIC settings_renders_hub=" + isHub);
        if (isHub) failures.Add("Settings renders Club hub");
        Object.Destroy(client.Modern.gameObject);
        Object.Destroy(host);
        yield return null;
        Assert.IsEmpty(failures, string.Join("; ", failures));
    }

    [UnityTest]
    public IEnumerator InventoryClose_UsesPointerEvent_AndClosesOverlay()
    {
        var fixture = (ClubView)typeof(ModernUiSmokeTests)
            .GetMethod("ClubFixture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        fixture.Fighters = new[] { new FighterView { Id = "audit-fighter", Name = "Audit", Hp = 1000000,
            MaxHp = 1000000, Level = 1, Accuracy = 10, Endurance = 10, Agility = 10,
            Ready = true, Equipment = Array.Empty<EquipmentView>() } };
        fixture.Items = new[] { new ItemView { Id = "audit-item", Definition = "Pistol-MK1", Slot = "Weapon", Equipped = false } };
        var host = new GameObject("PointerInventoryAudit");
        var client = host.AddComponent<ClubClient>();
        Set(client, "club", fixture);
        Set(client, "page", "Roster");
        Set(client, "selectedFighter", "audit-fighter");
        Set(client, "inventorySlot", "Weapon");
        client.SetModernForTest(true);
        yield return null;
        yield return null;

        var close = client.Modern.GetComponentsInChildren<Button>()
            .FirstOrDefault(x => x.GetComponentInChildren<Text>()?.text == "CLOSE");
        Assert.IsNotNull(close);
        Assert.IsTrue(close.IsInteractable());
        var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
        ExecuteEvents.Execute(close.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(close.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(close.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        yield return null;

        var slot = (string)client.GetType().GetField("inventorySlot", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
        Assert.AreEqual(string.Empty, slot);
        Object.Destroy(client.Modern.gameObject);
        Object.Destroy(host);
    }

    [Test]
    public void StableAppearance_UsesPersistedIdentityAcrossScreens()
    {
        Assert.IsTrue(Volumetric017UiView.IsFemaleAppearance("female-standard", "candidate-1"));
        Assert.IsFalse(Volumetric017UiView.IsFemaleAppearance("male-standard", "candidate-1"));
        Assert.AreEqual(Volumetric017UiView.IsFemale("legacy-1"), Volumetric017UiView.IsFemaleAppearance("", "legacy-1"));
    }
}
