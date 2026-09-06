using System;
using System.Collections.Generic;
using System.Linq;
using Airsoft.Battle;
using UnityEngine;

namespace AirsoftClub.Unity
{
    // Presentation only: reusable geometric silhouettes, never simulation or collision authority.
    public static class VisualPrototype
    {
        static readonly Color Teal = new Color(.23f, .72f, .66f), Rust = new Color(.91f, .48f, .27f);
        static void Box(Rect r, Color c) { var old = GUI.color; GUI.color = c; GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = old; }
        static void Line(Vector2 a, Vector2 b, Color c, float width)
        {
            int steps = Math.Max(1, (int)(Vector2.Distance(a, b) / 2));
            for (int n = 0; n <= steps; n++) { var p = Vector2.Lerp(a, b, n / (float)steps); Box(new Rect(p.x, p.y, width, width), c); }
        }
        public static void Fighter(Rect r, string identity, bool right, string weapon, bool head, bool rig, bool camo, int mk, bool alive, float health)
        {
            float u = r.height / 100f, x = r.x + r.width * .5f, y = r.y;
            Color suit = camo ? new Color(.39f, .46f, .30f) : right ? Teal : Rust;
            if (!alive) suit = new Color(.26f, .29f, .30f);
            bool female = identity.Length > 0 && identity[identity.Length - 1] % 2 == 0;
            Box(new Rect(x - 24*u, y + 87*u, 53*u, 5*u), new Color(0,0,0,.24f));
            Box(new Rect(x - 12*u, y + 64*u, 11*u, 26*u), suit); Box(new Rect(x + 5*u, y + 62*u, 11*u, 28*u), suit);
            Box(new Rect(x - 17*u, y + 32*u, female ? 31*u : 36*u, 36*u), suit);
            if (camo) { Box(new Rect(x - 13*u,y + 48*u, 12*u, 7*u), new Color(.22f,.29f,.20f)); Box(new Rect(x + 4*u,y + 38*u, 8*u, 10*u), new Color(.60f,.55f,.34f)); }
            if (female) Box(new Rect(x - 20*u, y + 9*u, 12*u, 28*u), new Color(.22f,.16f,.11f));
            // Oversized 3/4 head; deliberately shared male/female base, equipment provides identity.
            Box(new Rect(x - 16*u, y + 7*u, 34*u, 27*u), alive ? new Color(.76f,.60f,.44f) : suit);
            if (head) Box(new Rect(x - 20*u, y + 3*u, 41*u, 13*u), new Color(.29f,.36f,.29f));
            Box(new Rect(x - 15*u, y + 17*u, 34*u, 7*u), new Color(.08f,.15f,.17f));
            if (rig) { Box(new Rect(x - 15*u, y + 34*u, 30*u, 26*u), new Color(.20f,.23f,.19f)); for (int n=0;n<3;n++) Box(new Rect(x - 12*u+n*9*u,y+46*u,7*u,10*u),new Color(.57f,.52f,.36f)); }
            if (!string.IsNullOrEmpty(weapon))
            {
                float length = weapon.Contains("Pistol") ? 23 : weapon.Contains("Sniper") || weapon.Contains("Dmr") ? 57 : 44;
                Box(new Rect(right ? x : x-length*u, y+39*u, length*u, 7*u),new Color(.10f,.14f,.16f));
                Box(new Rect(x - 2*u,y+41*u,8*u,14*u),new Color(.18f,.20f,.20f));
                Box(new Rect(x - 8*u,y+39*u,18*u,7*u),suit);
                Box(new Rect(x - 2*u,y+36*u,7*u,3*u),mk==3 ? new Color(.91f,.68f,.25f) : mk==2 ? new Color(.65f,.74f,.78f) : suit);
            }
            Box(new Rect(r.x + 8, r.yMax - 3, r.width - 16, 4),new Color(.17f,.22f,.23f));
            Box(new Rect(r.x + 8, r.yMax - 3, (r.width - 16)*Mathf.Clamp01(health), 4),right ? Teal : Rust);
            if (!alive) Line(new Vector2(x-14*u,y+28*u),new Vector2(x+20*u,y+63*u),new Color(.9f,.5f,.4f,.8f),3);
        }
        public static void Arena(Rect r, MatchConfig input, MatchResult result, float time, IDictionary<string,long> hp, GUIStyle text, AppearanceView[] appearance)
        {
            Box(r,new Color(.10f,.15f,.17f)); var positions = new Dictionary<string,Vector2>();
            int rows = Math.Max(1,(Math.Max(input.Attacker.Fighters.Count,input.Defender.Fighters.Count)+3)/4);
            float lane = (r.height-40)/rows;
            for(int n=0;n<rows;n++)
            {
                float y=r.y+22+n*lane; Box(new Rect(r.x+10,y,r.width-20,lane-6),new Color(.16f,.21f,.21f));
                Box(new Rect(r.center.x-45,y+lane*.36f,32,lane*.37f),new Color(.29f,.32f,.28f));
                Box(new Rect(r.center.x+15,y+lane*.13f,40,lane*.36f),new Color(.34f,.36f,.30f));
            }
            GUI.Label(new Rect(r.x+16,r.y+2,250,22),"ATTACK  /  WEST",text); GUI.Label(new Rect(r.xMax-260,r.y+2,250,22),"DEFENSE  /  EAST",text);
            void Draw(TeamSnapshot team,bool left)
            {
                for(int n=0;n<team.Fighters.Count;n++)
                {
                    var f=team.Fighters[n];int row=n/4,col=n%4;float size=Math.Min(110,lane-15);
                    float x=left ? r.x+30+col*(r.width*.40f/4) : r.xMax-30-size-col*(r.width*.40f/4);
                    float y=r.y+25+row*lane+(lane-size)*.5f; string key=(left?"A":"D")+f.Id;long value=hp.ContainsKey(key)?hp[key]:f.StartingHp.Raw;
                    positions[key]=new Vector2(x+size/2,y+size*.4f);
                    var look = appearance.FirstOrDefault(a => a.Id == f.Id && a.Side == (left ? "A" : "D"));
                    Fighter(new Rect(x,y,size,size),f.Id,left,f.Weapon?.Id,look?.Head == true,look?.Rig == true,look?.Camo == true,f.Weapon?.Id.Contains("MK3")==true?3:f.Weapon?.Id.Contains("MK2")==true?2:1,value>0,value/(float)Formulas.MaxHp(f.Endurance,input.Rules).Raw);
                }
            }
            Draw(input.Attacker,true);Draw(input.Defender,false);
            foreach(var e in result.Events.Where(e=>e.TimeMs<=time && e.TimeMs>time-140).GroupBy(e=>e.ActorSide+e.ActorId).Select(g=>g.Last()).Take(8))
            {
                string a=(e.ActorSide==Side.Attacker?"A":"D")+e.ActorId,b=(e.ActorSide==Side.Attacker?"D":"A")+e.TargetId;
                if(positions.ContainsKey(a)&&positions.ContainsKey(b)) Line(positions[a],positions[b],new Color(1,.80f,.36f,.65f),1.5f);
            }
        }
    }
}
