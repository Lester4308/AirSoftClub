using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirsoftClub.Unity
{
    // Replaceable 2D layers. No transforms, collision, animation events or combat authority.
    public static class TacticalArt
    {
        static readonly Dictionary<string, Texture2D> Layers = new Dictionary<string, Texture2D>();
        sealed class Canvas
        {
            readonly int w, h; readonly Color32[] pixels;
            public Canvas(int width, int height) { w = width; h = height; pixels = new Color32[w * h]; }
            public void Poly(string hex, params int[] xy)
            {
                ColorUtility.TryParseHtmlString("#" + hex, out var c);
                int minX = w, maxX = 0, minY = h, maxY = 0;
                for (int n = 0; n < xy.Length; n += 2) { minX = Math.Min(minX, xy[n]); maxX = Math.Max(maxX, xy[n]); minY = Math.Min(minY, xy[n+1]); maxY = Math.Max(maxY, xy[n+1]); }
                for (int y = Math.Max(0,minY); y < Math.Min(h,maxY); y++) for (int x = Math.Max(0,minX); x < Math.Min(w,maxX); x++)
                {
                    bool inside = false; int j = xy.Length - 2;
                    for (int i = 0; i < xy.Length; i += 2) { if ((xy[i+1] > y) != (xy[j+1] > y) && x < (float)(xy[j]-xy[i]) * (y-xy[i+1]) / (xy[j+1]-xy[i+1]) + xy[i]) inside = !inside; j = i; }
                    if (inside) pixels[(h - y - 1) * w + x] = c;
                }
            }
            public void Box(string c, int x, int y, int width, int height) => Poly(c,x,y,x+width,y,x+width,y+height,x,y+height);
            public Texture2D Finish() { var t = new Texture2D(w,h,TextureFormat.RGBA32,false) { filterMode = FilterMode.Bilinear, hideFlags = HideFlags.DontSave }; t.SetPixels32(pixels); t.Apply(); return t; }
        }
        static Texture2D Layer(string key, Action<Canvas> paint, int w = 256, int h = 384)
        { if (!Layers.TryGetValue(key, out var t)) { var c = new Canvas(w,h); paint(c); Layers[key] = t = c.Finish(); } return t; }
        static Texture2D Body(bool female, bool camo) => Layer("body" + female + camo, c =>
        {
            string mid = camo ? "73795D" : "747A78", light = camo ? "919579" : "919A94", dark = camo ? "464F40" : "4A5354";
            // Tactical stance with separated legs, stable baseline boots.
            c.Poly("272F2F",84,339,111,338,113,359,76,365,65,359,68,348);
            c.Poly("3B4240",139,337,166,334,184,353,182,363,141,362);
            c.Poly("606452",82,207,125,205,126,257,115,339,82,343,77,303);
            c.Poly(mid,125,209,161,204,160,264,174,337,139,343,125,277);
            c.Poly(light,85,210,112,216,103,272,82,306);
            c.Poly(dark,107,267,123,254,115,339,101,340);
            c.Poly(dark,146,261,160,259,174,337,159,340);
            c.Box("3C443A",80,278,29,22); c.Poly("7E8268",82,278,109,278,104,287,81,291);
            c.Box("424A3C",141,276,23,22);
            int waist = female ? 151 : 162;
            c.Poly(mid,93,96,145,96,172,115,184,160,waist,220,88,220,74,160,73,119);
            c.Poly(light,94,98,118,102,110,159,83,179,73,131);
            c.Poly(dark,145,98,171,115,177,162,waist,220,137,212,140,150);
            c.Poly("3C443A",91,207,waist,207,waist,221,88,221);
            if (camo)
            {
                c.Poly("353F32",77,129,94,127,109,147,95,157,76,150);
                c.Poly("B0A185",104,170,130,165,139,186,124,199,97,188);
                c.Poly("434C39",144,116,162,119,165,146,148,151,137,130);
                c.Poly("9B9170",85,230,106,225,113,248,98,266,83,257);
                c.Poly("353F32",138,236,155,232,156,255,140,267,129,254);
                c.Poly("A29A7B",144,309,164,306,171,329,154,336);
            }
            // Arms hold the gun away from the chest, leaving its complete profile readable.
            c.Poly(mid,77,116,94,121,101,166,138,180,128,197,79,179,66,144);
            c.Poly(light,71,137,86,139,92,169,82,172);
            c.Poly(dark,167,119,184,125,188,166,159,190,145,178,167,156);
            c.Poly("333C39",122,177,140,174,149,185,137,201,125,196);
            c.Poly("858575",145,169,159,168,167,180,159,191,143,185);
            // Faceted protected head; slight 3/4 orientation.
            c.Poly("A3967B",94,39,135,27,157,41,161,73,150,96,120,105,94,88,85,62);
            c.Poly("C0AD8C",98,42,125,35,137,58,125,77,99,70);
            c.Poly("756F5B",139,39,156,43,161,73,149,94,138,82);
            if (female) c.Poly("34312B",92,44,91,90,77,108,77,82,82,48);
        });
        static Texture2D Face => Layer("face", c =>
        {
            c.Poly("272F30",92,58,133,52,158,59,158,75,139,82,98,76);
            c.Poly("617C7E",99,60,130,57,145,61,137,70,100,69);
            c.Poly("AAC0B8",104,60,130,57,119,61,103,63);
            c.Poly("424943",99,76,140,76,153,87,141,105,117,107,95,93);
            c.Poly("60695B",103,78,125,82,128,103,113,101,99,91);
            for (int n=0;n<4;n++) c.Box("27332F",107+n*7,86,3,8);
        });
        static Texture2D Head => Layer("helmet", c =>
        {
            c.Poly("353E34",83,53,84,30,105,15,140,17,160,34,165,54,148,61,94,59);
            c.Poly("81846B",86,32,108,20,138,22,151,37,133,45,89,43);
            c.Poly("666E57",88,44,135,45,154,37,161,52,145,58,90,54);
            c.Box("28342E",84,45,11,23); c.Box("9B9876",148,42,12,8);
            c.Box("A4A58B",111,19,17,6);
        });
        static Texture2D Rig => Layer("rig", c =>
        {
            c.Poly("30382F",99,101,112,100,116,122,142,121,142,101,154,105,166,194,153,213,95,207,87,131);
            c.Poly("555E48",110,119,149,120,158,162,143,201,100,197,96,136);
            c.Poly("6E7456",105,123,143,124,147,135,107,137);
            for (int n=0;n<3;n++) { c.Box("2A352C",101+n*17,150,15,40); c.Box("878469",102+n*17,150,13,32); c.Box("B0A17D",103+n*17,151,11,6); }
            c.Box("303A30",96,199,54,8); c.Box("BBA37D",124,201,12,7);
        });
        static Texture2D Weapon(string family, int mk) => Layer("weapon" + family + mk, c =>
        {
            bool pistol=family.Contains("Pistol"), sniper=family.Contains("Sniper"), dmr=family.Contains("Dmr"), smg=family.Contains("Smg"), shotgun=family.Contains("Shotgun");
            string metal=mk==3?"676D68":mk==2?"727D70":"444D4E", trim=mk==3?"C6A264":mk==2?"98AAA0":"6A7472";
            if (pistol)
            {
                c.Poly("222C2D",124,38,318,38,331,51,325,65,218,65,206,102,167,99,171,65,125,61);
                c.Box(metal,133,39,182,17); c.Box(trim,250,42,39,5);
                c.Poly("57635B",178,64,211,66,199,95,172,92);
                c.Box("222B2C",156,31,9,8); c.Box("222B2C",304,31,8,8);
                if(mk>=2) { c.Box("293637",209,23,38,15); c.Box(trim,214,25,25,5); }
                if(mk==3) { c.Box("293436",326,44,62,17); c.Box(trim,344,44,11,17); }
            }
            else
            {
                int end = smg ? 354 : sniper || dmr ? 453 : 418;
                c.Poly("232E2E",27,39,109,48,130,39,268,39,282,47,end,47,end,60,279,62,258,74,169,72,137,65,108,63,35,87,23,80);
                c.Poly(metal,118,43,259,43,272,53,257,65,141,64,112,58);
                c.Box(metal,267,44,end-305,22); c.Box("273333",end-38,49,52,9);
                c.Poly("202B2B",165,64,198,64,186,109,158,100);
                c.Poly("667062",203,65,236,66,247,100,230,112,212,104);
                c.Poly("222C2D",32,43,86,48,99,60,43,79,36,72,69,59,33,57);
                for(int n=0;n<5;n++) c.Box("202C2D",278+n*13,48,7,9);
                c.Box(trim,127,45,128,4);
                if(shotgun) { c.Box("7B7563",287,43,65,28); c.Box("313C38",307,45,4,25); }
                if(mk>=2 || sniper || dmr) { c.Box("263233",204,23,61,17); c.Box("8DA29A",209,25,11,12); c.Box("242E2E",221,37,14,9); }
                if(mk==3)
                {
                    c.Box(trim,132,53,31,6); c.Box(trim,241,47,14,12);
                    c.Box("273333",end+6,44,44,19); c.Box(trim,end+14,44,8,19);
                    c.Box("273333",293,68,15,29); c.Box("909078",291,68,20,7);
                    c.Box("6C786C",305,31,38,11);
                }
            }
        },512,128);
        public static void DrawWeapon(Rect r, string id)
        { if(!string.IsNullOrEmpty(id)) GUI.DrawTexture(r,Weapon(id,id.Contains("MK3")?3:id.Contains("MK2")?2:1),ScaleMode.ScaleToFit); }
        static void DrawLayer(Rect r,Texture2D texture,bool right)
        { GUI.DrawTextureWithTexCoords(r,texture,right?new Rect(0,0,1,1):new Rect(1,0,-1,1)); }
        public static void Fighter(Rect r,string id,bool right,string weapon,bool head,bool rig,bool camo,bool alive=true,float hit=0,bool idle=true)
        {
            bool female = !string.IsNullOrEmpty(id) && id[id.Length-1]%2==0;
            if(idle && alive) r.y += Mathf.Sin(Time.unscaledTime*1.7f+(id?.Length??0)) * r.height*.004f;
            var old=GUI.color; GUI.color=alive?Color.white:new Color(.47f,.52f,.51f,.58f);
            DrawLayer(r,Body(female,camo),right); DrawLayer(r,Face,right);
            if(head)DrawLayer(r,Head,right); if(rig)DrawLayer(r,Rig,right);
            if(!string.IsNullOrEmpty(weapon))
            {
                var gun = new Rect(right?r.x+r.width*.32f:r.x-r.width*.15f,r.y+r.height*.365f,r.width*.86f,r.height*.22f);
                DrawLayer(gun,Weapon(weapon,weapon.Contains("MK3")?3:weapon.Contains("MK2")?2:1),right);
            }
            if(hit>0) { GUI.color=new Color(1,.65f,.46f,Mathf.Clamp01(hit)*.32f); DrawLayer(r,Body(female,camo),right); }
            GUI.color=old;
        }
    }
}
