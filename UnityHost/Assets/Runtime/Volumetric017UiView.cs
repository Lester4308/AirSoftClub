using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    // Cohesive volumetric 2D sprite preview. It intentionally falls back to
    // Male017 for equipment combinations until matching production layers exist.
    public sealed class Volumetric017UiView
    {
        const string BasePath = "Art/Volumetric017/Sprites/";
        static Sprite maleInspect, maleRoster, maleCombatNear, maleCombatMass;
        static Sprite femaleInspect, femaleRoster, femaleCombatNear, femaleCombatMass;

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

        // Stable, deterministic sex derived from the identity so every screen
        // shows the same fighter even for legacy API data without AppearanceId.
        public static bool IsFemale(string id) => (StableHash(id) & 1) == 1;

        public static bool IsFemaleAppearance(string appearanceId, string fallbackIdentity)
        {
            if (!string.IsNullOrEmpty(appearanceId)) return appearanceId.StartsWith("female", StringComparison.OrdinalIgnoreCase);
            return IsFemale(fallbackIdentity);
        }

        static int StableHash(string value)
        {
            unchecked
            {
                uint hash = 2166136261;
                foreach (char c in value ?? "") hash = (hash ^ c) * 16777619;
                return (int)(hash & 0x7fffffff);
            }
        }

        public static Volumetric017UiView Create(RectTransform parent, string name, Vector2 displaySize)
            => new Volumetric017UiView(parent, name, displaySize);

        public void Apply(bool facingRight, bool alive, bool female = false)
        {
            image.sprite = SelectSprite(displaySize, female);
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = image.rectTransform.offsetMax = Vector2.zero;
            image.color = alive ? Color.white : new Color(.47f, .52f, .51f, .58f);
            root.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }

        static Sprite SelectSprite(Vector2 size, bool female)
        {
            float height = size.y;
            if (height >= 1000f) return female ? femaleInspect : maleInspect;
            if (height >= 520f) return female ? femaleRoster : maleRoster;
            if (height >= 260f) return female ? femaleCombatNear : maleCombatNear;
            return female ? femaleCombatMass : maleCombatMass;
        }

        static void EnsureLoaded()
        {
            if (maleInspect != null) return;
            maleInspect = Need("male-inspect");
            maleRoster = Need("male-roster");
            maleCombatNear = Need("male-combat-near");
            maleCombatMass = Need("male-combat-mass");
            femaleInspect = Need("female-inspect");
            femaleRoster = Need("female-roster");
            femaleCombatNear = Need("female-combat-near");
            femaleCombatMass = Need("female-combat-mass");
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
