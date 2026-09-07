using System;
using UnityEngine;

namespace AirsoftClub.Unity
{
    public static class BetaTheme
    {
        public static readonly Color Background=Hex("0D141B"), Panel=Hex("18232C"), Card=Hex("202E36"), Edge=Hex("34444A"), Gold=Hex("D4AF70"), Blue=Hex("70ACC3"), Orange=Hex("D38B4A"), Ink=Hex("EBEFEA"), Muted=Hex("91A19F");
        public static GUIStyle Heading, Body, Small, Micro, Large, Button, ActionButton, SelectedButton;
        static Texture2D Solid(Color c) { var t=new Texture2D(1,1){hideFlags=HideFlags.DontSave};t.SetPixel(0,0,c);t.Apply();return t; }
        public static Color Hex(string s) { ColorUtility.TryParseHtmlString("#"+s,out var c);return c; }
        public static void Init()
        {
            if(Heading!=null)return;
            Heading=new GUIStyle(GUI.skin.label){fontSize=25,fontStyle=FontStyle.Bold,normal={textColor=Ink}};
            Body=new GUIStyle(GUI.skin.label){fontSize=16,wordWrap=true,normal={textColor=Ink}};
            Small=new GUIStyle(Body){fontSize=13,normal={textColor=Muted}};
            Micro=new GUIStyle(Small){fontSize=11}; Large=new GUIStyle(Heading){fontSize=44,normal={textColor=Gold}};
            Button=new GUIStyle(GUI.skin.button){fontSize=14,fontStyle=FontStyle.Bold,padding=new RectOffset(14,14,6,6),normal={background=Solid(Card),textColor=Ink},hover={background=Solid(Hex("354A54")),textColor=Color.white},active={background=Solid(Hex("53666B")),textColor=Gold},border=new RectOffset(0,0,0,0)};
            ActionButton=new GUIStyle(Button){normal={background=Solid(Orange),textColor=Background},hover={background=Solid(Gold),textColor=Background}};
            SelectedButton=new GUIStyle(Button){normal={background=Solid(Hex("2A3D47")),textColor=Ink}};
        }
        public static void Box(Rect r,Color c){var old=GUI.color;GUI.color=c;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;}
        public static void PanelBox(Rect r){Box(r,Panel);Box(new Rect(r.x,r.y,r.width,1),Edge);}
        public static void Label(Rect r,string text,GUIStyle style=null,Color? color=null)
        { var s=style??Body;var old=s.normal.textColor;if(color.HasValue)s.normal.textColor=color.Value;GUI.Label(r,text,s);s.normal.textColor=old; }
        public static bool Click(Rect r,string text,bool primary=false,bool selected=false)
        {
            bool clicked=GUI.Button(r,text,primary?ActionButton:selected?SelectedButton:Button);
            if(selected)Box(new Rect(r.x,r.y,3,r.height),Gold);return clicked;
        }
        public static void Bar(Rect r,float fraction,Color c)
        {Box(r,Hex("0C161D"));Box(new Rect(r.x,r.y,r.width*Mathf.Clamp01(fraction),r.height),c);}
        public static int BbIndex(Airsoft.Battle.BbTierDefinition tier) => tier.Id.StartsWith("bb-") && int.TryParse(tier.Id.Substring(3), out int n) ? Mathf.Clamp(n,0,4) : 0;
        public static Color BbColor(int tier) => new[]{Hex("EFF4EA"),Hex("73D780"),Hex("6EB3FF"),Hex("ED756A"),Hex("C98DFA")}[Mathf.Clamp(tier,0,4)];
        public static void Warehouse(Rect r,bool yard=false)
        {
            Box(r,Hex(yard?"293C44":"25333A"));
            // Shallow side-view industrial planes; no collision geometry.
            Box(new Rect(r.x,r.y+r.height*.57f,r.width,r.height*.43f),Hex("303A39"));
            for(int n=0;n<8;n++)
            {
                float x=r.x+n*r.width/8;
                Box(new Rect(x,r.y,5,r.height*.61f),Hex("151F27"));
                Box(new Rect(x+14,r.y+26,r.width/10,r.height*.20f),Hex(yard?"465653":"3F565C"));
                Box(new Rect(x+14,r.y+30,r.width/10,3),Hex("647773"));
            }
            Box(new Rect(r.x,r.y+r.height*.33f,r.width,11),Hex("19252C"));
            Box(new Rect(r.x,r.y+r.height*.56f,r.width,6),Hex("6A6C56"));
            for(int n=0;n<5;n++)Box(new Rect(r.x,r.y+r.height*(.63f+n*.08f),r.width,1),Hex("46524D"));
            for(int n=0;n<3;n++)Crate(new Rect(r.x+r.width*(.23f+n*.25f),r.y+r.height*.44f,r.width*.13f,r.height*.20f));
            Box(r,new Color(.02f,.04f,.06f,.27f));
        }
        public static void Crate(Rect r)
        {
            Box(new Rect(r.x+8,r.y+7,r.width,r.height),new Color(0,0,0,.22f));Box(r,Hex("676B54"));
            Box(new Rect(r.x,r.y,r.width,7),Hex("979174"));Box(new Rect(r.x+r.width*.82f,r.y,r.width*.18f,r.height),Hex("4A5347"));
            Box(new Rect(r.x+10,r.y+10,r.width*.68f,r.height-17),Hex("59654F"));
            Box(new Rect(r.x+8,r.y+r.height*.6f,r.width*.7f,4),Hex("85856B"));
        }
    }
}
