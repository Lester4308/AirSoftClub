using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    // Presentation-only compositor for the male-017 prototype pack.
    // The pack has provenance records but no final-art approval.
    internal sealed class Male017UiView
    {
        const float CanvasWidth = 512f, CanvasHeight = 768f;
        static readonly string BasePath = "Art/Male017/Sprites/";
        static Male017SpriteCatalog catalog;

        readonly RectTransform root;
        readonly Image rear, body, head, vest, rifle, front;

        Male017UiView(RectTransform parent, string name, Vector2 displaySize)
        {
            if (catalog == null) catalog = Male017SpriteCatalog.Load();
            root = MakeRect(name, parent);
            root.sizeDelta = displaySize;
            root.anchorMin = root.anchorMax = new Vector2(0, 1);
            root.pivot = new Vector2(0, 1);

            float fit = Mathf.Min(displaySize.x / CanvasWidth, displaySize.y / CanvasHeight);
            var composition = MakeRect("Composition", root);
            composition.anchorMin = composition.anchorMax = new Vector2(0, 1);
            composition.pivot = new Vector2(0, 1);
            composition.sizeDelta = new Vector2(CanvasWidth, CanvasHeight);
            composition.localScale = Vector3.one * fit;
            composition.anchoredPosition = new Vector2((displaySize.x - CanvasWidth * fit) * 0.5f, 0);

            rear = MakeImage("SupportArm", composition);
            body = MakeImage("Body", composition);
            head = MakeImage("Head", composition);
            vest = MakeImage("Vest", composition);
            rifle = MakeImage("Weapon", composition);
            front = MakeImage("ShootingArm", composition);
        }

        public RectTransform Root => root;

        public static Male017UiView Create(RectTransform parent, string name, Vector2 displaySize)
            => new Male017UiView(parent, name, displaySize);

        public void Apply(bool camouflage, bool headProtection, bool armor, bool weaponVisible, bool facingRight, bool alive, float recoilPixels = 0f)
        {
            rear.sprite = camouflage ? catalog.ArmLeftCamo : catalog.ArmLeftOlive;
            body.sprite = camouflage ? catalog.BodyCamo : catalog.BodyOlive;
            head.sprite = headProtection ? catalog.HeadEquipped : catalog.HeadBare;
            vest.sprite = catalog.Vest;
            rifle.sprite = catalog.Rifle;
            front.sprite = camouflage ? catalog.ArmFrontCamo : catalog.ArmFrontOlive;

            SetStatic(body, new Vector2(128, 183), 1.25f);
            SetStatic(head, headProtection ? new Vector2(173, 69) : new Vector2(155, 69), headProtection ? 0.52f : 0.49f);
            SetStatic(vest, new Vector2(169, 217), 0.63f);
            SetStatic(rifle, new Vector2(175 - recoilPixels, 250), 0.80f);

            ConfigureArm(rear,
                camouflage ? new Vector2(220, 180) : new Vector2(235, 130),
                camouflage ? new Vector2(1100, 430) : new Vector2(1180, 380),
                new Vector2(340, 280), new Vector2(422 - recoilPixels, 320));
            ConfigureArm(front, new Vector2(48, 35), new Vector2(252, 164),
                new Vector2(154, 249), new Vector2(271 - recoilPixels, 354));

            vest.enabled = armor;
            rifle.enabled = weaponVisible;
            Color tint = alive ? Color.white : new Color(.47f, .52f, .51f, .58f);
            rear.color = body.color = head.color = vest.color = rifle.color = front.color = tint;
            root.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }

        static void SetStatic(Image image, Vector2 topLeft, float scale)
        {
            image.SetNativeSize();
            var rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(topLeft.x, -topLeft.y);
            rt.localScale = Vector3.one * scale;
            rt.localRotation = Quaternion.identity;
        }

        static void ConfigureArm(Image image, Vector2 sourceShoulder, Vector2 sourceGrip, Vector2 targetShoulder, Vector2 targetGrip)
        {
            image.SetNativeSize();
            Vector2 u = sourceGrip - sourceShoulder;
            Vector2 v = targetGrip - targetShoulder;
            float denominator = Vector2.Dot(u, u);
            float co = Vector2.Dot(u, v) / denominator;
            float si = (u.x * v.y - u.y * v.x) / denominator;
            Vector2 origin = new Vector2(
                targetShoulder.x - co * sourceShoulder.x + si * sourceShoulder.y,
                targetShoulder.y - si * sourceShoulder.x - co * sourceShoulder.y);
            var rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(origin.x, -origin.y);
            rt.localScale = Vector3.one * Mathf.Sqrt(co * co + si * si);
            rt.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(si, co) * Mathf.Rad2Deg);
        }

        static RectTransform MakeRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        static Image MakeImage(string name, Transform parent)
        {
            var rt = MakeRect(name, parent);
            var image = rt.gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            return image;
        }

        sealed class Male017SpriteCatalog
        {
            public Sprite ArmLeftOlive, ArmLeftCamo, BodyOlive, BodyCamo, HeadBare, HeadEquipped, Vest, Rifle, ArmFrontOlive, ArmFrontCamo;

            public static Male017SpriteCatalog Load()
            {
                return new Male017SpriteCatalog
                {
                    ArmLeftOlive = Need("arm-left-olive"), ArmLeftCamo = Need("arm-left-camo"),
                    BodyOlive = Need("body-olive"), BodyCamo = Need("body-camo"),
                    HeadBare = Need("head-bare"), HeadEquipped = Need("head-equipped"),
                    Vest = Need("vest"), Rifle = Need("rifle"),
                    ArmFrontOlive = Need("arm-front-olive"), ArmFrontCamo = Need("arm-front-camo")
                };
            }

            static Sprite Need(string name)
            {
                var sprite = Resources.Load<Sprite>(BasePath + name);
                if (sprite == null) throw new InvalidOperationException("Missing male-017 UI sprite: " + name);
                return sprite;
            }
        }
    }
}