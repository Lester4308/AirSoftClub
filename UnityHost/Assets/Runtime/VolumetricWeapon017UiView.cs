using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    internal sealed class VolumetricWeapon017UiView
    {
        const string BasePath = "Art/Volumetric017/Weapons/";
        static readonly string[] Families = { "pistol", "smg", "assault-rifle", "pump-shotgun", "dmr", "sniper-rifle" };
        static Sprite[] cards;
        readonly RectTransform root;
        readonly Image image;

        VolumetricWeapon017UiView(RectTransform parent, string name, Vector2 size)
        {
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

        public RectTransform Root => root;

        public static VolumetricWeapon017UiView Create(RectTransform parent, string name, Vector2 size)
            => new VolumetricWeapon017UiView(parent, name, size);

        public void Apply(string stableId)
        {
            int hash = 17;
            if (!string.IsNullOrEmpty(stableId))
                for (int i = 0; i < stableId.Length; i++) hash = unchecked(hash * 31 + stableId[i]);
            int index = Math.Abs(hash % cards.Length);
            image.sprite = cards[index];
        }

        static void EnsureLoaded()
        {
            if (cards != null) return;
            cards = new Sprite[Families.Length];
            for (int i = 0; i < Families.Length; i++)
                cards[i] = Need(Families[i] + "-card");
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
