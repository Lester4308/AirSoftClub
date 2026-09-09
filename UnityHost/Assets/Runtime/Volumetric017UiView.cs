using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    // Cohesive volumetric 2D sprite preview. It intentionally falls back to
    // Male017 for equipment combinations until matching production layers exist.
    internal sealed class Volumetric017UiView
    {
        const string BasePath = "Art/Volumetric017/Sprites/";
        static Sprite inspect, roster, combatNear, combatMass;

        readonly RectTransform root;
        readonly Image image;
        readonly Vector2 displaySize;

        Volumetric017UiView(RectTransform parent, string name, Vector2 size)
        {
            EnsureLoaded();
            displaySize = size;
            root = MakeRect(name, parent);
            root.sizeDelta = size;
            root.anchorMin = root.anchorMax = new Vector2(0, 1);
            root.pivot = new Vector2(0, 1);

            image = MakeRect("CohesiveSprite", root).gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        public RectTransform Root => root;

        public static Volumetric017UiView Create(RectTransform parent, string name, Vector2 displaySize)
            => new Volumetric017UiView(parent, name, displaySize);

        public void Apply(bool facingRight, bool alive)
        {
            image.sprite = SelectSprite(displaySize);
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = image.rectTransform.offsetMax = Vector2.zero;
            image.color = alive ? Color.white : new Color(.47f, .52f, .51f, .58f);
            root.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }

        static Sprite SelectSprite(Vector2 size)
        {
            float height = size.y;
            if (height >= 1000f) return inspect;
            if (height >= 520f) return roster;
            if (height >= 260f) return combatNear;
            return combatMass;
        }

        static void EnsureLoaded()
        {
            if (inspect != null) return;
            inspect = Need("male-inspect");
            roster = Need("male-roster");
            combatNear = Need("male-combat-near");
            combatMass = Need("male-combat-mass");
        }

        static Sprite Need(string name)
        {
            var sprite = Resources.Load<Sprite>(BasePath + name);
            if (sprite == null) throw new InvalidOperationException("Missing volumetric-017 UI sprite: " + name);
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
