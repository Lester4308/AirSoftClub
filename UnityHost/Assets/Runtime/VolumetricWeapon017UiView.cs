using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    internal sealed class VolumetricWeapon017UiView
    {
        const string BasePath = "Art/Volumetric017/Weapons/";
        static readonly string[] Families = { "pistol", "smg", "assault-rifle", "pump-shotgun", "dmr", "sniper-rifle" };
        // Explicit map from catalog definition prefix (e.g. "AssaultRifle-MK2") to Families index.
        // Never silently falls back to pistol — returns -1 for truly unknown ids.
        static readonly (string prefix, int index)[] FamilyMap =
        {
            ("Pistol", 0), ("Smg", 1), ("AssaultRifle", 2), ("Shotgun", 3), ("Dmr", 4), ("SniperRifle", 5),
        };
        static Sprite[] cards, silhouettes;
        readonly RectTransform root;
        readonly Image image;

        VolumetricWeapon017UiView(RectTransform parent, string name, Vector2 size, bool silhouette = false)
        {
            silhouetteMode = silhouette;
            EnsureLoaded();
            root = MakeRect(name, parent);
            root.sizeDelta = size;
            root.anchorMin = root.anchorMax = new Vector2(0, 1);
            root.pivot = new Vector2(0, 1);
            image = MakeRect("WeaponSprite", root).gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = image.rectTransform.offsetMax = Vector2.zero;
        }

        readonly bool silhouetteMode;
        public RectTransform Root => root;

        public static VolumetricWeapon017UiView Create(RectTransform parent, string name, Vector2 size)
            => new VolumetricWeapon017UiView(parent, name, size);

        // Battle-field compact read: the family silhouette (not the full card art).
        public static VolumetricWeapon017UiView CreateSilhouette(RectTransform parent, string name, Vector2 size)
            => new VolumetricWeapon017UiView(parent, name, size, silhouette: true);

        // Precisely maps the equipped catalog id (e.g. "AssaultRifle-MK2") to the
        // matching volumetric family sprite. Returns false if the id is unrecognized.
        public bool Apply(string weaponId)
        {
            int index = FamilyIndex(weaponId);
            if (index < 0) return false;
            image.sprite = silhouetteMode ? silhouettes[index] : cards[index];
            return true;
        }

        // WeaponFamily ordinal matches Families order (Pistol..SniperRifle).
        internal void ApplyFamily(int ordinal)
        {
            if (ordinal < 0 || ordinal >= Families.Length) return;
            image.sprite = silhouetteMode ? silhouettes[ordinal] : cards[ordinal];
        }

        static int FamilyIndex(string weaponId)
        {
            if (weaponId == null) return -1;
            for (int i = 0; i < FamilyMap.Length; i++)
                if (weaponId.StartsWith(FamilyMap[i].prefix, StringComparison.Ordinal))
                    return FamilyMap[i].index;
            return -1; // unknown — caller decides fallback
        }

        static void EnsureLoaded()
        {
            if (cards != null) return;
            cards = new Sprite[Families.Length];
            silhouettes = new Sprite[Families.Length];
            for (int i = 0; i < Families.Length; i++)
            {
                cards[i] = Need(Families[i] + "-card");
                silhouettes[i] = Need(Families[i] + "-silhouette");
            }
        }

        static Sprite Need(string name)
        {
            var sprite = Resources.Load<Sprite>(BasePath + name);
            if (sprite == null) throw new InvalidOperationException("Missing volumetric weapon sprite: " + name);
            return sprite;
        }

        static RectTransform MakeRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }
    }
}
