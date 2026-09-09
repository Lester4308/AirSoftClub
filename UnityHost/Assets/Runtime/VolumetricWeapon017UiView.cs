using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    internal sealed class VolumetricWeapon017UiView
    {
        const string BasePath = "Art/Volumetric017/Weapons/";
        static readonly string[] Families = { "pistol", "smg", "assault-rifle", "pump-shotgun", "dmr", "sniper-rifle" };
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

        // Precisely maps the equipped catalog id (e.g. "AssaultRifle-MK2") to the
        // matching volumetric family sprite. Falls back to Pistol for unknown ids.
        public void Apply(string weaponId)
        {
            int index = FamilyIndex(weaponId);
            image.sprite = silhouetteMode ? silhouettes[index] : cards[index];
        }

        // WeaponFamily ordinal matches Families order (Pistol..SniperRifle).
        internal void ApplyFamily(int ordinal)
        {
            int index = ordinal >= 0 && ordinal < Families.Length ? ordinal : 0;
            image.sprite = silhouetteMode ? silhouettes[index] : cards[index];
        }

        static int FamilyIndex(string weaponId)
        {
            for (int i = 0; i < Families.Length; i++)
                if (weaponId != null && weaponId.IndexOf(Families[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return i;
            return 0; // pistol fallback
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
