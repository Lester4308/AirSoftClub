using UnityEngine;
using UnityEngine.UI;

namespace AirsoftClub.Unity
{
    // UGUI design system for the modern client. Pure construction — no IMGUI.
    // Palette intentionally distinct from the old BetaTheme: deep tactical graphite,
    // warm gold accents, restrained blue and olived tan.
    public static class ModernStyle
    {
        // Palette
        public static readonly Color Bg         = Hex("0C1216");
        public static readonly Color Bg2        = Hex("111A21");
        public static readonly Color Panel      = Hex("151F27");
        public static readonly Color PanelAlt   = Hex("1B2A33");
        public static readonly Color Card       = Hex("223140");
        public static readonly Color CardHover  = Hex("2A3D4E");
        public static readonly Color Edge       = Hex("33465A");
        public static readonly Color Ink        = Hex("EDF1EC");
        public static readonly Color Muted      = Hex("93A3A6");
        public static readonly Color Gold       = Hex("D9B36C");
        public static readonly Color Blue       = Hex("6FAFC4");
        public static readonly Color Orange     = Hex("D08A3E");
        public static readonly Color Danger     = Hex("C85A4D");
        public static readonly Color Neutral    = Hex("4A5A63");
        public static readonly Color Stripe     = Hex("6A6C56");
        public static readonly Color Shadow     = new Color(0, 0, 0, 0.45f);

        public static readonly Color PressDark  = new Color(0, 0, 0, 0.25f);

        public static Color Hex(string s) { ColorUtility.TryParseHtmlString("#" + s, out var c); return c; }

        // Shared white sprite for Image.color tinting (sliced-9 by UI Image automatically).
        static Texture2D _white;
        static Sprite _whiteSprite;
        public static Sprite WhiteSprite()
        {
            if (_whiteSprite != null) return _whiteSprite;
            _white = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.DontSave };
            _white.SetPixel(0, 0, Color.white); _white.Apply();
            _whiteSprite = Sprite.Create(_white, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f);
            _whiteSprite.hideFlags = HideFlags.DontSave;
            return _whiteSprite;
        }

        // Font fallback: use the built-in legacy Arial (always available in players).
        static Font _font;
        public static Font Font()
        {
            if (_font == null) _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return _font;
        }

        // ---- Element factories (Canvas-space, rect fill under parent) ----
        static RectTransform Make(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static RectTransform AddStretch(string name, Transform parent, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            var r = Make(name, parent);
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(left, bottom);
            r.offsetMax = new Vector2(-right, -top);
            return r;
        }

        public static RectTransform AddRect(string name, Transform parent, Vector2 pos, Vector2 size, Vector2 anchor)
        {
            var r = Make(name, parent);
            r.anchorMin = anchor; r.anchorMax = anchor;
            r.sizeDelta = size;
            r.anchoredPosition = pos;
            return r;
        }

        public static Image AddImage(string name, Transform parent)
        {
            var r = Make(name, parent);
            var img = r.gameObject.AddComponent<Image>();
            img.sprite = WhiteSprite();
            img.color = Color.white;
            return img;
        }

        public static Text AddText(string name, Transform parent, string content, int size, Color color, TextAnchor align = TextAnchor.MiddleCenter)
        {
            var r = Make(name, parent);
            var t = r.gameObject.AddComponent<Text>();
            t.font = Font();
            t.fontSize = size;
            t.fontStyle = FontStyle.Normal;
            t.color = color;
            t.alignment = align;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.text = content;
            t.raycastTarget = false;
            return t;
        }

        // A styled panel with rounded-ish look (sliced border), subtle inner shading.
        public static Image MakePanel(Transform parent, Color fill, Color edge, int edgeW = 2)
        {
            var img = AddImage("Panel", parent);
            img.color = fill;
            // Underline edge accent
            var line = AddImage("Edge", img.transform);
            line.color = edge;
            var lr = (RectTransform)line.transform;
            lr.anchorMin = Vector2.zero; lr.anchorMax = new Vector2(1, 0.006f);
            lr.offsetMin = Vector2.zero; lr.offsetMax = Vector2.zero;
            return img;
        }

        public static Button Button(Transform parent, string label, int fontSize = 15, Color? fill = null, Color? text = null)
        {
            var r = Make("Button", parent);
            var img = r.gameObject.AddComponent<Image>();
            var target = fill ?? Card;
            img.color = target;
            img.sprite = WhiteSprite();
            var btn = r.gameObject.AddComponent<Button>();
            btn.targetGraphic = img;
            var txt = AddText("Label", r, label, fontSize, text ?? Ink);
            var tr = (RectTransform)txt.transform;
            tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
            return btn;
        }
    }
}