using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Airsoft.Battle;
using UnityEngine;
using static AirsoftClub.Unity.BetaTheme;

namespace AirsoftClub.Unity
{
    public static class BetaArena
    {
        public static void Draw(Rect r,MatchConfig input,MatchResult result,float time,IDictionary<string,long> hp,AppearanceView[] appearance,Action<Rect,string> tooltip)
        {
            Warehouse(r);
            int rows=Math.Max(1,(Math.Max(input.Attacker.Fighters.Count,input.Defender.Fighters.Count)+3)/4);
            float lane=(r.height-40)/rows;
            var points=new Dictionary<string,Vector2>();var muzzles=new Dictionary<string,Vector2>();
            var recent=result.Events.Where(e=>e.TimeMs<=time&&e.TimeMs>time-180).ToArray();
            void SideDraw(TeamSnapshot team,bool left)
            {
                for(int n=0;n<team.Fighters.Count;n++)
                {
                    var f=team.Fighters[n];int row=n/4,col=n%4;
                    float height=Math.Min(244,lane-13),width=height*.67f;
                    float x=left?r.x+29+col*r.width*.102f:r.xMax-29-width-col*r.width*.102f;
                    float y=r.y+23+row*lane+lane-height-4;string key=(left?"A":"D")+f.Id;
                    var look=appearance.FirstOrDefault(a=>a.Id==f.Id&&a.Side==(left?"A":"D"));
                    long value=hp.TryGetValue(key,out var h)?h:f.StartingHp.Raw;
                    bool firing=recent.Any(e=>e.ActorId==f.Id&&e.ActorSide==(left?Side.Attacker:Side.Defender));
                    bool hit=recent.Any(e=>e.TargetId==f.Id&&e.ActorSide!=(left?Side.Attacker:Side.Defender)&&e.Damage.Raw>0);
                    var body=new Rect(x+(firing?(left?-1.4f:1.4f):0),y,width,height);
                    TacticalArt.Fighter(body,f.Id,left,f.Weapon?.Id,look?.Head==true,look?.Rig==true,look?.Camo==true,value>0,hit?.9f:0);
                    Box(new Rect(x+width*.38f,y+height*.34f,Math.Max(3,width*.08f),Math.Max(2,height*.025f)),left?Blue:Orange);
                    Bar(new Rect(x+width*.19f,y+height-4,width*.68f,4),value/(float)Formulas.MaxHp(f.Endurance,input.Rules).Raw,left?Blue:Orange);
                    points[key]=new Vector2(x+width*.50f,y+height*.43f);
                    muzzles[key]=TacticalArt.Muzzle(body,left,f.Weapon?.Id);
                    tooltip(body,(left?"ATTACK":"DEFENSE")+"  /  "+f.Id.Substring(0,Math.Min(8,f.Id.Length))+"\nHP "+(value/10000f).ToString("0.0")+" • "+(f.Weapon?.Family.ToString()??"Unarmed")+"\nACC "+f.Accuracy+" / END "+f.Endurance+" / AGI "+f.Agility);
                    if(value==0)Label(new Rect(x,y+height*.43f,width,25),"OUT",Micro,Orange);
                }
            }
            for(int n=0;n<rows;n++)
            {
                float y=r.y+24+n*lane;Box(new Rect(r.x+8,y,r.width-16,lane-3),new Color(.07f,.10f,.10f,n%2==0?.20f:.40f));
                Box(new Rect(r.x+15,y+lane-1,r.width-30,1),new Color(.64f,.67f,.53f,.28f));
                Crate(new Rect(r.center.x-49,y+lane*.47f,45,lane*.37f));Crate(new Rect(r.center.x+25,y+lane*.29f,37,lane*.37f));
                Label(new Rect(r.center.x-20,y+4,46,20),"0"+(n+1),Micro,Gold);
            }
            Label(new Rect(r.x+19,r.y+3,470,23),"ATTACK  /  WEST",Small,Blue);
            Label(new Rect(r.xMax-190,r.y+3,182,23),"DEFENSE  /  EAST",Small,Orange);
            SideDraw(input.Attacker,true);SideDraw(input.Defender,false);
            var shots=result.Events.Where(e=>e.TimeMs<=time&&e.TimeMs>time-190).GroupBy(e=>e.ActorSide+e.ActorId).Select(g=>g.Last()).Take(24);
            foreach(var e in shots)
            {
                string a=(e.ActorSide==Side.Attacker?"A":"D")+e.ActorId,b=(e.ActorSide==Side.Attacker?"D":"A")+e.TargetId;
                if(!muzzles.TryGetValue(a,out var from)||!points.TryGetValue(b,out var to))continue;
                Color color=BbColor(BbIndex(e.ActorSide==Side.Attacker?input.Attacker.BbTier:input.Defender.BbTier));
                float elapsed=time-e.TimeMs,phase=Mathf.Clamp01(elapsed/190f);
                var point=Vector2.Lerp(from,to,phase);
                for(int n=0;n<4;n++){var trail=Vector2.Lerp(from,to,Mathf.Max(0,phase-n*.006f));Box(new Rect(trail.x,trail.y,2+n*.35f,2+n*.35f),color);}
                if(elapsed<85)
                {
                    float dir=e.ActorSide==Side.Attacker?1:-1;
                    Box(new Rect(from.x+dir*2,from.y-2,10,4),Gold);Box(new Rect(from.x+dir*4,from.y-5,3,10),new Color(1,.93f,.66f));
                }
            }
            foreach(var group in result.Events.Where(e=>e.Damage.Raw>0&&e.TimeMs<=time&&e.TimeMs>time-550).GroupBy(e=>(e.ActorSide==Side.Attacker?"D":"A")+e.TargetId).Take(12))
            {
                if(!points.TryGetValue(group.Key,out var p))continue;
                float age=time-group.Max(e=>e.TimeMs);Label(new Rect(p.x-21,p.y-28-age*.027f,100,30),"-"+(group.Sum(e=>e.Damage.Raw)/10000f).ToString("0.#"),Body,new Color(1,.77f,.54f,1-age/650));
            }
        }
    }
    public sealed partial class ClubClient
    {
        void ResetPlayback()
        {
            hp.Clear();foreach(var f in replayInput.Attacker.Fighters)hp["A"+f.Id]=f.StartingHp.Raw;
            foreach(var f in replayInput.Defender.Fighters)hp["D"+f.Id]=f.StartingHp.Raw;
            eventIndex=0;replayTime=0;page="Battle";
        }
        void BetaBattle()
        {
            if(replayResult==null)return;
            if(replayTime>=replayResult.SimulatedDurationMs){BetaResult();return;}
            Label(new Rect(226,128,800,38),"COMPOUND 01  /  BATTLE REPLAY",Heading);
            Label(new Rect(1200,132,370,31),(Math.Min(replayTime,replayResult.SimulatedDurationMs)/1000f).ToString("0.0")+" s",Body,Gold);
            int a=replayResult.Events.Count(e=>e.ActorSide==Side.Attacker&&e.TimeMs<=replayTime),d=replayResult.Events.Count(e=>e.ActorSide==Side.Defender&&e.TimeMs<=replayTime);
            WalletChip(new Rect(226,180,655,66),"ATTACK / "+club.BbCatalog[BbIndex(replayInput.Attacker.BbTier)].Name,"BB "+(replayInput.Attacker.BbBudget-a)+" remaining   •   "+a+" fired",BbColor(BbIndex(replayInput.Attacker.BbTier)));
            WalletChip(new Rect(899,180,671,66),"DEFENSE / "+club.BbCatalog[BbIndex(replayInput.Defender.BbTier)].Name,"BB "+(replayInput.Defender.BbBudget-d)+" remaining   •   "+d+" fired",BbColor(BbIndex(replayInput.Defender.BbTier)));
            BetaArena.Draw(new Rect(226,264,1344,533),replayInput,replayResult,replayTime,hp,appearance,Tip);
            for(int n=0;n<5;n++){Box(new Rect(237+n*170,819,6,6),BbColor(n));Label(new Rect(251+n*170,809,157,29),club.BbCatalog[n].Name,Small,BbColor(n));}
            if(Click(new Rect(1196,808,374,34),"SKIP TO SAVED RESULT"))replayTime=replayResult.SimulatedDurationMs+1;
        }
        void BetaResult()
        {
            var h=club.History.FirstOrDefault(x=>x.Id==savedMatch?.Id);
            bool attacking=h==null||h.Attacker==club.Id,draw=replayResult.Outcome==MatchOutcome.Draw;
            bool won=attacking?replayResult.Outcome==MatchOutcome.AttackerWin:replayResult.Outcome==MatchOutcome.DefenderWin;
            Warehouse(content);Box(content,new Color(.02f,.04f,.055f,.60f));
            Label(new Rect(270,175,1250,40),"AFTER ACTION  /  "+replayResult.Reason.ToString().ToUpperInvariant(),Small,Gold);
            Label(new Rect(267,226,1220,78),draw?"DRAW":won?"VICTORY":"DEFEAT",new GUIStyle(Large){fontSize=68},draw?Blue:won?Gold:Orange);
            Label(new Rect(273,326,1150,40),"The result is saved. Your next challenge is waiting.",Body);
            long[] values={savedMatch?.RewardMoney??0,savedMatch?.RewardClubXp??0,savedMatch?.RewardFighterXp??0,savedMatch?.RatingDelta??0};
            string[] names={"MONEY","CLUB XP","FIGHTER XP / EACH","RATING"};
            for(int n=0;n<4;n++)
            {
                var r=new Rect(271+n*310,408,290,143);PanelBox(r);Label(new Rect(r.x+22,r.y+19,247,25),names[n],Small);
                Label(new Rect(r.x+22,r.y+60+10*(1-Mathf.Clamp01((replayTime-replayResult.SimulatedDurationMs)/400f)),247,57),(n==3&&savedMatch?.RatingKnown!=true?"UNKNOWN":values[n].ToString("+0;-0;0")),Large,n==3?Blue:Gold);
            }
            Label(new Rect(274,597,1190,36),(replayResult.SimulatedDurationMs/1000f).ToString("0.0")+" seconds  •  BB fired "+replayResult.Attacker.BbConsumed+" / "+replayResult.Defender.BbConsumed+"  •  "+replayInput.Attacker.Fighters.Count+" vs "+replayInput.Defender.Fighters.Count+" fighters",Body);
            if(Click(new Rect(273,690,292,56),"NEW BATTLE",true))page="Opponents";
            if(Click(new Rect(584,690,280,56),"BACK TO CLUB"))page="Club";
            if(Click(new Rect(883,690,280,56),"REPLAY / REVIEW"))ResetPlayback();
            if(Click(new Rect(1182,690,280,56),"HISTORY"))page="History";
            Label(new Rect(274,785,1190,35),"Rewards and rating are the server's settled amounts. Playback never changes them.",Small);
        }
        IEnumerator BbWalkthrough()
        {
            string defenderAccount="dev-bb-d-"+Guid.NewGuid().ToString("N").Substring(0,10),attackerAccount="dev-bb-a-"+Guid.NewGuid().ToString("N").Substring(0,10);
            string defender="";
            for(int tier=0;tier<5;tier++)
            {
                account=defenderAccount;token="";club=null;yield return Login();
                if(failed){Application.Quit(1);yield break;}
                if(tier==0)yield return Send(Intent("Squad",number:1),true);
                if(club.BbStock[tier]<club.Capacity)yield return Send(Intent("Refill",number:tier));yield return Send(Intent("BbTier",number:tier));defender=club.Id;
                account=attackerAccount;token="";club=null;yield return Login();
                if(failed){Application.Quit(1);yield break;}
                if(tier==0)yield return Send(Intent("Squad",number:1),true);else yield return Send(Intent("Recovery"),true);
                if(club.BbStock[tier]<club.Capacity)yield return Send(Intent("Refill",number:tier));yield return Send(Intent("BbTier",number:tier));
                yield return Send(Intent("Attack",defender,"Practice"));
                if(failed||replayInput==null||BbIndex(replayInput.Attacker.BbTier)!=tier||BbIndex(replayInput.Defender.BbTier)!=tier){Debug.LogError("BB_VISUAL_FAILED");Application.Quit(1);yield break;}
                replayTime=replayResult.Events[0].TimeMs+65;yield return null;yield return new WaitForEndOfFrame();Capture(EvidencePath("club-bb-tier-"+tier+".png"));
                Debug.Log("BB_VISUAL tier="+tier+" status="+replayResult.Status);
            }
            Debug.Log(smokeFailure?"BB_VISUAL_FAILED":"BB_VISUAL_PASSED");Application.Quit(smokeFailure?1:0);
        }
    }
}
