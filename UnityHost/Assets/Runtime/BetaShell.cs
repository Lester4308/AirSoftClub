using System;
using System.Linq;
using UnityEngine;
using static AirsoftClub.Unity.BetaTheme;

namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        readonly Rect content = new Rect(226,128,1344,716);
        Vector2 rosterScroll, catalogScroll, inventoryScroll;
        bool ActionsEnabled => !busy && inventorySlot.Length==0 && !revengeOverlay;
        bool revengeOverlay;
        string displayedPage=""; float pageChangedAt;
        string selectedOffer="", selectedItem="", shopCategory="Weapons", inventorySlot="", hoverText="";
        void OnGUI()
        {
            if (useModern) return; // UGUI/Canvas controller renders instead of legacy IMGUI
            Init(); title=Heading; label=Body; small=Small; button=new GUIStyle(Button){fixedHeight=34};
            float scale=Mathf.Min(Screen.width/1600f,Screen.height/900f);
            GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1600*scale)/2,(Screen.height-900*scale)/2,0),Quaternion.identity,new Vector3(scale,scale,1));
            Box(new Rect(0,0,1600,900),Background); hoverText="";
            if(displayedPage!=page){displayedPage=page;pageChangedAt=Time.unscaledTime;}
            PanelBox(new Rect(0,0,200,900)); Label(new Rect(25,25,160,65),"AIRSOFT\nCLUB",Heading,Gold);
            Label(new Rect(27,94,155,25),"TACTICAL CLUB / BETA",Micro);
            string[] tabs={"Club","Roster","Recruitment","Training","Equipment","Shop","BB","Recovery","Opponents","History","Settings"};
            GUI.enabled=inventorySlot.Length==0&&!revengeOverlay;
            for(int n=0;n<tabs.Length;n++) if(Click(new Rect(16,151+n*49,169,40),tabs[n],false,page==tabs[n])) {page=tabs[n];scroll=Vector2.zero;inventorySlot="";}
            GUI.enabled=true;
            Label(new Rect(25,802,158,65),"LOCAL DEVELOPMENT\nSteam sandbox pending",Micro);
            if(club==null) { LoginScreen(); return; }
            Label(new Rect(229,26,670,39),club.Name,Heading);
            Label(new Rect(230,67,540,28),"CLUB LEVEL "+club.Level+"   /   RATING "+club.Rating+"   /   "+club.Fighters.Count(f=>f.Ready)+" READY",Small);
            WalletChip(new Rect(900,28,196,60),"MONEY",club.Money.ToString("N0"),Gold);
            WalletChip(new Rect(1108,28,174,60),"CREDITS",club.Credits.ToString("N0"),Blue);
            WalletChip(new Rect(1294,28,276,60),"ACTIVE BB",club.BbCatalog[club.ActiveBbTier].Name+"  "+club.BbStock[club.ActiveBbTier],BbColor(club.ActiveBbTier));
            Bar(new Rect(230,102,1340,3),(club.Xp%1000)/1000f,Gold);
            GUI.enabled=!busy&&inventorySlot.Length==0&&!revengeOverlay;
            if(page=="Club")BetaHub();
            else if(new[]{"Roster","Training","Equipment","Recovery"}.Contains(page))BetaRoster();
            else if(page=="Recruitment")BetaRecruitment();
            else if(page=="Shop"||page=="Supply"||page=="BB")BetaShop();
            else if(page=="Opponents")BetaOpponents();
            else if(page=="Battle")BetaBattle();
            else if(page=="ArtSheet")BetaArtSheet();
            else if(page=="History")BetaHistory();
            else { PanelBox(content);GUILayout.BeginArea(new Rect(content.x+24,content.y+18,content.width-48,content.height-36));scroll=GUILayout.BeginScrollView(scroll);StatusPage();GUILayout.EndScrollView();GUILayout.EndArea(); }
            if(page!="Battle")Box(content,new Color(Background.r,Background.g,Background.b,Mathf.Clamp01(1-(Time.unscaledTime-pageChangedAt)/.16f)));
            GUI.enabled=true;
            Label(new Rect(228,864,1050,28),(busy?"SAVING  /  ":"")+status,Small);
            if(ActionsEnabled && Click(new Rect(1300,856,128,31),"Refresh"))StartCoroutine(Retry());
            if(ActionsEnabled && Click(new Rect(1440,856,130,31),"Reconnect"))StartCoroutine(steamSession?SteamLogin():Login());
            if(!busy && lastPayload!=null && Click(new Rect(1110,813,440,34),"Retry saved operation"))StartCoroutine(Retry());
            if(inventorySlot.Length>0)InventoryOverlay();
            if(revengeOverlay)RevengeOverlay();
            GUI.enabled=true;
            if(hoverText.Length>0 && inventorySlot.Length==0 && !revengeOverlay)
            {
                var mouse=Event.current.mousePosition;var tip=new Rect(Mathf.Min(mouse.x+20,1270),Mathf.Min(mouse.y+20,740),290,94);
                Box(tip,Hex("0B141B"));Box(new Rect(tip.x,tip.y,3,tip.height),Gold);Label(new Rect(tip.x+13,tip.y+10,265,80),hoverText,Small,Ink);
            }
        }
        void WalletChip(Rect r,string name,string amount,Color c){PanelBox(r);Label(new Rect(r.x+14,r.y+7,r.width-20,18),name,Micro);Label(new Rect(r.x+14,r.y+25,r.width-20,30),amount,Body,c);}
        void Tip(Rect r,string text){if(r.Contains(Event.current.mousePosition))hoverText=text;}
        void LoginScreen()
        {
            Warehouse(new Rect(225,128,1345,716));Box(new Rect(285,238,552,454),new Color(.04f,.07f,.09f,.94f));
            Label(new Rect(323,278,485,65),"BUILD YOUR CLUB.",Large);
            Label(new Rect(326,353,457,78),"Recruit a squad. Invest in your gear.\nTake your place on the field.",Body);
            Label(new Rect(326,457,420,25),"YOUR LOCAL ACCOUNT",Small);
            account=GUI.TextField(new Rect(326,493,420,36),account,44);
            if(Click(new Rect(326,555,420,48),"ENTER CLUB",true))StartCoroutine(Login());
            if(Click(new Rect(326,620,420,38),"Steam sandbox sign-in"))StartCoroutine(SteamLogin());
            Label(new Rect(325,697,450,70),status,Small);
        }
        void BetaHub()
        {
            var hero=new Rect(226,128,1344,330);Warehouse(hero);
            Box(new Rect(226,128,670,330),new Color(.025f,.05f,.07f,.72f));
            Label(new Rect(257,162,600,28),"HEADQUARTERS  /  TRAINING COMPOUND",Small,Gold);
            Label(new Rect(254,204,650,70),"YOUR NEXT CHAPTER.",Large);
            Label(new Rect(257,278,560,57),club.Fighters.Length==0?"Your club begins with one recruit. Choose a fighter and build from there.":"The team is yours. Train with purpose, equip for the field and choose your next rival.",Body);
            if(Click(new Rect(259,365,228,49),club.Fighters.Length==0?"CHOOSE FIRST RECRUIT":"FIND A BATTLE",true))page=club.Fighters.Length==0?"Recruitment":"Opponents";
            if(Click(new Rect(505,365,198,49),"MANAGE ROSTER"))page="Roster";
            Label(new Rect(1155,373,370,47),"COMPOUND 01\nINDUSTRIAL TRAINING",Small);
            string[] names={"TEAM READINESS","CLUB PROGRESSION","SUPPLY STATUS"};
            string[] values={club.Fighters.Count(f=>f.Ready)+" / "+club.Fighters.Length+" READY",club.Xp%1000+" / 1,000 XP",club.BbStock[club.ActiveBbTier]+" / "+club.Capacity+" BB"};
            for(int n=0;n<3;n++){var r=new Rect(226+n*452,478,440,108);PanelBox(r);Label(new Rect(r.x+20,r.y+15,400,23),names[n],Small);Label(new Rect(r.x+20,r.y+45,400,44),values[n],Heading,n==0?Blue:Gold);}
            var left=new Rect(226,606,665,238);PanelBox(left);
            Label(new Rect(246,625,615,36),"REWARDS & PROGRESS",Heading);
            Label(new Rect(247,674,610,25),"Daily streak "+club.Streak+"/7  •  Day 7 includes a Credit",Small);
            if(Click(new Rect(247,714,292,39),"Claim daily reward",true))StartCoroutine(Send(Intent("Daily")));
            if(Click(new Rect(553,714,312,39),"Claim earned Credits"))StartCoroutine(Send(Intent("Progression")));
            if(Click(new Rect(247,770,292,37),"Emergency Basic BB"))StartCoroutine(Send(Intent("Emergency")));
            if(Click(new Rect(553,770,312,37),"1 Credit → "+club.ConvertRate+" Money"))StartCoroutine(Send(Intent("Convert",number:1)));
            var right=new Rect(911,606,659,238);PanelBox(right);
            Label(new Rect(932,625,600,36),"CLUB PROTECTION",Heading);
            Label(new Rect(932,673,610,44),club.ShieldUntil>club.ServerNow?"Shield active • "+Math.Ceiling((club.ShieldUntil-club.ServerNow)/60000d)+" min remaining":"Protection from new incoming attacks. Starting Ranked cancels it.",Small);
            for(int n=0;n<club.Shields.Length;n++){var sh=club.Shields[n];if(Click(new Rect(932+n*153,735,143,42),sh.Hours+"h / "+sh.Credits+" C"))StartCoroutine(Send(Intent("Shield",number:sh.Hours)));}
            if(!string.IsNullOrEmpty(club.PendingMatch)&&Click(new Rect(932,789,610,32),"Recover pending battle"))StartCoroutine(LoadMatch(club.PendingMatch));
        }
        void BetaHistory()
        {
            Label(new Rect(226,128,1300,40),"BATTLE HISTORY",Heading);
            scroll=GUI.BeginScrollView(new Rect(226,182,1344,662),scroll,new Rect(0,0,1320,Math.Max(660,club.History.Length*100)));
            for(int n=0;n<club.History.Length;n++){var h=club.History[n];var r=new Rect(0,n*100,1314,88);PanelBox(r);
                Label(new Rect(22,r.y+12,450,29),(h.Attacker==club.Id?"ATTACK":"DEFENSE")+"  /  "+h.Mode.ToUpperInvariant(),Body,Gold);
                Label(new Rect(22,r.y+46,780,30),h.Status+" • "+h.Outcome+" • Money +"+h.Money+" • Rating "+(h.RatingKnown?h.RatingDelta.ToString("+0;-0;0"):"unknown"),Small);
                if(Click(new Rect(1100,r.y+23,185,42),"Review battle"))StartCoroutine(LoadMatch(h.Id));
            }GUI.EndScrollView();
        }
        void BetaOpponents()
        {
            if(!rivals.Any(r=>r.Id==selectedTarget)&&rivals.Length>0)selectedTarget=rivals[0].Id;
            if(Click(new Rect(226,770,660,45),"REVENGE TICKETS / "+(club.RevengeTickets?.Length??0)))revengeOverlay=true;
            Label(new Rect(226,128,690,35),"CHOOSE YOUR NEXT CHALLENGE",Heading);
            string[] modes={"Practice","Ranked","Friend"};
            for(int n=0;n<3;n++)if(Click(new Rect(226+n*224,179,211,40),modes[n],false,mode==modes[n]))mode=modes[n];
            for(int n=0;n<rivals.Length;n++)
            {
                var r=rivals[n];var box=new Rect(226,241+n*92,660,80);PanelBox(box);
                Label(new Rect(box.x+19,box.y+14,430,30),r.Name,Body);Label(new Rect(box.x+19,box.y+45,445,27),"Level "+r.Level+" • "+r.Fighters+" fighters • "+r.Category,Small);
                if(Click(new Rect(box.x+498,box.y+19,141,41),"Select",false,selectedTarget==r.Id))selectedTarget=r.Id;
            }
            var detail=new Rect(910,179,660,665);PanelBox(detail);var chosen=rivals.FirstOrDefault(r=>r.Id==selectedTarget);
            if(chosen==null){Label(new Rect(940,214,600,100),"Select a club to inspect the challenge.",Heading);return;}
            Warehouse(new Rect(930,200,620,168));Label(new Rect(953,226,574,44),chosen.Name,Heading,Gold);
            Label(new Rect(940,393,590,55),chosen.Category+" MATCHUP  /  "+chosen.Fighters+" FIGHTERS",Heading);
            Label(new Rect(940,459,590,65),"Mode: "+mode+"\nRating "+chosen.Rating+" • "+(chosen.Protected?"Protected":"Open for challenge"),Body);
            Label(new Rect(940,531,590,70),"Full-strength win preview: "+chosen.PreviewWinMoney+" Money / "+chosen.PreviewWinClubXp+" Club XP. Current HP and anti-farm eligibility can reduce this; settlement is final.",Small);
            if(mode=="Ranked"&&club.ShieldUntil>club.ServerNow)Label(new Rect(940,610,590,40),"Starting Ranked cancels your active shield.",Small,Orange);
            GUI.enabled=ActionsEnabled&&!chosen.Protected;
            if(Click(new Rect(940,662,590,49),"CONFIRM "+mode.ToUpperInvariant()+" BATTLE",true))StartCoroutine(Send(Intent("Attack",chosen.Id,mode)));
            GUI.enabled=ActionsEnabled;
            var ticket=(club.RevengeTickets??Array.Empty<RevengeView>()).FirstOrDefault(t=>t.Target==chosen.Id);
            if(ticket!=null&&Click(new Rect(940,731,590,41),"Revenge • "+ticket.Attempts+"/3 attempts")){
                var c=Intent("Attack",ticket.Target,"Revenge");c.Ticket=ticket.Origin;StartCoroutine(Send(c));}
            Label(new Rect(940,790,595,40),"Friend uses the local dev-social policy. Live Steam is pending.",Micro);
        }
    }
}
