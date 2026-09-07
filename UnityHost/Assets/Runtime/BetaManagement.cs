using System;
using System.Linq;
using UnityEngine;
using static AirsoftClub.Unity.BetaTheme;

namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        string Gear(FighterView f,string slot)=>f.Equipment.FirstOrDefault(e=>e.Slot==slot)?.Definition;
        void DrawFighter(Rect r,FighterView f)
        {TacticalArt.Fighter(r,f.Id,true,Gear(f,"Weapon"),Gear(f,"HeadProtection")!=null,Gear(f,"LoadBearingArmor")!=null,Gear(f,"Camouflage")!=null,f.Hp>0);}
        void BetaArtSheet()
        {
            Label(new Rect(226,128,690,42),"MODULAR FIGHTER BASES",Heading);
            for(int n=0;n<6;n++)
            {
                var r=new Rect(226+n%3*227,200+n/3*312,212,295);PanelBox(r);
                TacticalArt.Fighter(new Rect(r.x+36,r.y+10,140,220),n<3?"base-a":"base-b",true,n%3==2?"AssaultRifle-MK3":null,n%3>=1,n%3==2,n%3!=0);
                Label(new Rect(r.x+15,r.y+239,186,44),(n<3?"Male":"Female")+" / "+new[]{"base + mask","camo + helmet","rig + weapon"}[n%3],Small);
            }
            Label(new Rect(938,128,629,42),"WEAPON FAMILY / MK VARIATIONS",Heading);
            for(int mk=1;mk<=3;mk++)Label(new Rect(950+(mk-1)*205,180,198,25),"MK"+mk,Small,mk==3?Gold:Blue);
            string[] families={"Pistol","Smg","AssaultRifle","Shotgun","Dmr","SniperRifle"};
            for(int n=0;n<families.Length;n++)
            {
                Label(new Rect(950,219+n*100,600,20),families[n],Micro);
                for(int mk=1;mk<=3;mk++)TacticalArt.DrawWeapon(new Rect(941+(mk-1)*207,240+n*100,203,57),families[n]+"-MK"+mk);
            }
        }
        void BetaRoster()
        {
            Label(new Rect(226,128,680,38),page=="Recovery"?"RECOVERY BAY":page=="Training"?"TRAINING & DEVELOPMENT":"YOUR FIGHTERS",Heading);
            Label(new Rect(226,171,680,29),club.Fighters.Length+" / 16 fighters • Select a fighter to manage their build.",Small);
            rosterScroll=GUI.BeginScrollView(new Rect(226,213,724,631),rosterScroll,new Rect(0,0,700,Math.Max(630,Mathf.CeilToInt(club.Fighters.Length/3f)*263)));
            for(int n=0;n<club.Fighters.Length;n++)
            {
                var f=club.Fighters[n];var r=new Rect(n%3*233,n/3*263,220,249);
                PanelBox(r);if(r.Contains(Event.current.mousePosition))Box(r,new Color(.3f,.47f,.54f,.15f));if(selectedFighter==f.Id)Box(new Rect(r.x,r.y,3,r.height),Gold);
                DrawFighter(new Rect(r.x+37,r.y+8,140,181),f);
                Label(new Rect(r.x+14,r.y+190,197,26),f.Name,Body);
                Label(new Rect(r.x+14,r.y+219,197,21),"LV "+f.Level+"  /  "+(f.Ready?"READY":"RECOVERING"),Micro,f.Ready?Blue:Orange);
                Bar(new Rect(r.x+14,r.y+242,190,3),f.Hp/(float)f.MaxHp,Blue);
                if(GUI.Button(r,GUIContent.none,GUIStyle.none))selectedFighter=f.Id;
                Tip(r,f.Name+" • Level "+f.Level+"\nACC "+f.Accuracy+" / END "+f.Endurance+" / AGI "+f.Agility+"\n"+(Gear(f,"Weapon")??"No weapon equipped"));
            }
            GUI.EndScrollView();
            var chosen=club.Fighters.FirstOrDefault(f=>f.Id==selectedFighter)??club.Fighters.FirstOrDefault();
            if(chosen==null){if(Click(new Rect(980,250,540,50),"Recruit your first fighter",true))page="Recruitment";return;}
            selectedFighter=chosen.Id;FighterDetail(new Rect(972,128,598,716),chosen);
        }
        void FighterDetail(Rect p,FighterView f)
        {
            PanelBox(p);Label(new Rect(p.x+23,p.y+18,550,37),f.Name.ToUpperInvariant(),Heading);
            Label(new Rect(p.x+24,p.y+58,550,26),"LEVEL "+f.Level+"  •  "+f.Xp+" XP  •  "+(f.Ready?"READY":"RECOVERING"),Small,f.Ready?Blue:Orange);
            Box(new Rect(p.x+20,p.y+98,204,266),Background);DrawFighter(new Rect(p.x+37,p.y+103,174,242),f);
            float x=p.x+247,y=p.y+101;
            Label(new Rect(x,y,320,27),"HEALTH  "+f.Hp/10000f+" / "+f.MaxHp/10000f,Small);
            Bar(new Rect(x,y+32,319,7),f.Hp/(float)f.MaxHp,Blue);
            string[] names={"Accuracy","Endurance","Agility"};int[] values={f.Accuracy,f.Endurance,f.Agility};
            for(int n=0;n<3;n++)
            {
                Label(new Rect(x,y+56+n*60,222,25),names[n].ToUpperInvariant()+"  "+values[n],Small);
                Bar(new Rect(x,y+88+n*60,210,4),values[n]/(float)f.TrainingCap,Gold);
                GUI.enabled=ActionsEnabled && values[n]<f.TrainingCap;
                if(Click(new Rect(x+231,y+53+n*60,85,34),"+1"))StartCoroutine(Send(Intent("Train",f.Id,names[n])));
                GUI.enabled=ActionsEnabled;
                Tip(new Rect(x+231,y+53+n*60,85,34),club.TrainingMoney+" Money per point\nTraining cap "+f.TrainingCap+" • raise with fighter XP.");
            }
            Label(new Rect(x,y+247,319,42),"Full recovery: "+Math.Ceiling(f.RecoveryRemainingMs/60000d)+" min\nFree recovery continues offline.",Micro);
            if(Click(new Rect(p.x+22,p.y+381,269,38),"Heal / "+f.HealMoney+" Money"))StartCoroutine(Send(Intent("Heal",f.Id)));
            if(Click(new Rect(p.x+307,p.y+381,269,38),"Heal up to 10 HP"))StartCoroutine(Send(Intent("Heal",f.Id,number:10)));
            string[] slots={"Weapon","Camouflage","HeadProtection","LoadBearingArmor"};
            string[] titles={"WEAPON","CAMOUFLAGE","HEAD / FACE PROTECTION","LOAD-BEARING / ARMOR"};
            for(int n=0;n<4;n++)
            {
                var r=new Rect(p.x+22+n%2*285,p.y+441+n/2*101,269,88);Box(r,Card);
                Label(new Rect(r.x+12,r.y+9,r.width-22,21),titles[n],Micro,Gold);
                Label(new Rect(r.x+12,r.y+32,r.width-22,40),Gear(f,slots[n])??"Empty slot",Small,Ink);
                if(GUI.Button(r,GUIContent.none,GUIStyle.none))inventorySlot=slots[n];
                Tip(r,"Manage this equipment slot.\nOwned gear can be equipped or returned to inventory.");
            }
            if(Click(new Rect(p.x+22,p.y+661,269,33),"Visit shop")){page="Shop";shopCategory="Weapons";}
            if(club.Fighters.Length>1&&Click(new Rect(p.x+307,p.y+661,269,33),"Dismiss / return gear"))StartCoroutine(Send(Intent("Dismiss",f.Id)));
        }
        void InventoryOverlay()
        {
            Box(new Rect(0,0,1600,900),new Color(0,0,0,.72f));var p=new Rect(430,171,810,558);PanelBox(p);
            Label(new Rect(457,195,650,35),"EQUIPMENT / "+inventorySlot.ToUpperInvariant(),Heading);
            if(Click(new Rect(1124,194,86,32),"Close")){inventorySlot="";return;}
            GUI.enabled=!busy;
            var f=club.Fighters.FirstOrDefault(x=>x.Id==selectedFighter);if(f==null){inventorySlot="";return;}
            if(Gear(f,inventorySlot)!=null&&Click(new Rect(457,251,750,37),"Return equipped item to inventory"))StartCoroutine(Send(Intent("Unequip",f.Id,inventorySlot)));
            var items=club.Items.Where(i=>i.Slot==inventorySlot&&!i.Equipped).ToArray();
            scroll=GUI.BeginScrollView(new Rect(456,307,756,376),scroll,new Rect(0,0,730,Math.Max(370,items.Length*65)));
            for(int n=0;n<items.Length;n++){var i=items[n];Label(new Rect(12,n*65+8,480,38),i.Definition,Body);if(Click(new Rect(525,n*65,180,40),"Equip"))StartCoroutine(Send(Intent("Equip",f.Id,i.Id)));}
            if(items.Length==0)Label(new Rect(12,10,690,85),"No unequipped items in this slot.\nBuy equipment in the shop, then return here.",Body);
            GUI.EndScrollView();
        }
        void BetaRecruitment()
        {
            Label(new Rect(226,128,790,39),"RECRUITMENT",Heading);
            Label(new Rect(226,172,790,38),"Pool "+club.OfferVersion+" • "+club.CompletedSinceRefresh+"/10 battles • refresh in "+Math.Max(0,(club.RefreshAvailableAt-club.ServerNow)/60000)+" min",Small);
            if(Click(new Rect(1031,129,220,39),"Free refresh"))StartCoroutine(Send(Intent("Refresh")));
            if(Click(new Rect(1264,129,306,39),"Refresh / "+club.RefreshMoney+" Money"))StartCoroutine(Send(Intent("Refresh",flag:true)));
            for(int n=0;n<club.Offers.Length;n++)
            {
                var o=club.Offers[n];var r=new Rect(226+n%4*202,231+n/4*298,188,280);PanelBox(r);
                TacticalArt.Fighter(new Rect(r.x+29,r.y+10,122,170),o.Id,true,null,false,false,true);
                Label(new Rect(r.x+15,r.y+189,163,26),o.Name,Body);
                Label(new Rect(r.x+15,r.y+219,165,24),"A "+o.Accuracy+"  E "+o.Endurance+"  G "+o.Agility,Small);
                if(Click(new Rect(r.x+12,r.y+248,164,27),"Inspect",false,selectedOffer==o.Id))selectedOffer=o.Id;
                if(r.Contains(Event.current.mousePosition)){selectedOffer=o.Id;Tip(r,"Compare this recruit in the right panel.\nAccuracy / Endurance / Agility");}
            }
            var choice=club.Offers.FirstOrDefault(o=>o.Id==selectedOffer)??club.Offers.FirstOrDefault();if(choice==null)return;
            var p=new Rect(1050,231,520,613);PanelBox(p);Label(new Rect(p.x+24,p.y+19,470,39),choice.Name.ToUpperInvariant(),Heading);
            TacticalArt.Fighter(new Rect(p.x+125,p.y+66,228,300),choice.Id,true,null,false,false,true);
            var baseline=club.Fighters.FirstOrDefault(f=>f.Id==selectedFighter);
            Label(new Rect(p.x+27,p.y+380,468,63),"ACC "+choice.Accuracy+"    END "+choice.Endurance+"    AGI "+choice.Agility,Heading);
            if(baseline!=null)Label(new Rect(p.x+27,p.y+425,468,54),"vs "+baseline.Name+":  "+(choice.Accuracy-baseline.Accuracy).ToString("+0;-0;0")+" / "+(choice.Endurance-baseline.Endurance).ToString("+0;-0;0")+" / "+(choice.Agility-baseline.Agility).ToString("+0;-0;0"),Small);
            bool free=!club.FreeRecruitClaimed&&(choice.Id.EndsWith("-0")||choice.Id.EndsWith("-1")||choice.Id.EndsWith("-2"));
            if(Click(new Rect(p.x+26,p.y+500,468,50),free?"CHOOSE FREE RECRUIT":"HIRE / "+choice.Price+" MONEY",true))StartCoroutine(Send(Intent("Hire",choice.Id,flag:free)));
            Label(new Rect(p.x+27,p.y+568,466,35),"One permanent starter choice. All ready fighters join battles.",Micro);
        }
        void BetaShop()
        {
            Label(new Rect(226,128,1330,40),page=="BB"?"AMMUNITION SUPPLY":"CLUB ARMORY",Heading);
            string[] categories={"Weapons","Head Protection","Armor","Camouflage","BB","Premium"};
            if(page=="BB"){BetaAmmo(new Rect(226,193,1344,651));return;}
            PanelBox(new Rect(226,193,190,651));
            for(int n=0;n<categories.Length;n++)if(Click(new Rect(240,212+n*55,162,43),categories[n],false,shopCategory==categories[n])){shopCategory=categories[n];catalogScroll=Vector2.zero;}
            Label(new Rect(243,637,153,148),"Build with Money.\nUnlock earlier with Credits.\n\nPremium advantages are part of progression.",Small);
            if(shopCategory=="BB"){BetaAmmo(new Rect(435,193,1135,651));return;}
            var entries=club.Catalog.Where(i=>shopCategory=="Premium"?i.Credits>0||i.EarlyAllowed:shopCategory=="Weapons"?i.Slot=="Weapon":shopCategory=="Head Protection"?i.Slot=="HeadProtection":shopCategory=="Armor"?i.Slot=="LoadBearingArmor":i.Slot=="Camouflage").ToArray();
            catalogScroll=GUI.BeginScrollView(new Rect(435,193,700,651),catalogScroll,new Rect(0,0,676,Math.Max(644,Mathf.CeilToInt(entries.Length/2f)*188)));
            for(int n=0;n<entries.Length;n++)
            {
                var i=entries[n];var r=new Rect(n%2*338,n/2*188,325,174);PanelBox(r);if(r.Contains(Event.current.mousePosition))Box(r,new Color(.3f,.47f,.54f,.15f));
                if(selectedItem==i.Id)Box(new Rect(r.x,r.y,3,r.height),Gold);
                if(i.Slot=="Weapon")TacticalArt.DrawWeapon(new Rect(r.x+10,r.y+12,300,74),i.Id);
                else TacticalArt.Fighter(new Rect(r.x+107,r.y+4,88,100),i.Id,true,null,i.Slot=="HeadProtection",i.Slot=="LoadBearingArmor",i.Slot=="Camouflage",idle:false);
                Label(new Rect(r.x+13,r.y+94,300,25),i.Id,Body);
                int owned=club.Items.Count(it=>it.Definition==i.Id);bool equipped=club.Items.Any(it=>it.Definition==i.Id&&it.Equipped);
                Label(new Rect(r.x+13,r.y+125,300,24),"LV "+i.Level+"  /  "+(equipped?"EQUIPPED":owned>0?"OWNED "+owned:i.Access?"AVAILABLE":i.EarlyAllowed?"EARLY ACCESS":"LOCKED"),Micro,i.Credits>0?Gold:Muted);
                Label(new Rect(r.x+13,r.y+147,300,24),i.Credits>0?i.Credits+" Credits":i.Money+" Money",Small,Gold);
                if(GUI.Button(r,GUIContent.none,GUIStyle.none))selectedItem=i.Id;
                Tip(r,i.Id+"\nSelect to preview stats and purchase options.");
            }GUI.EndScrollView();
            var selected=entries.FirstOrDefault(i=>i.Id==selectedItem)??entries.FirstOrDefault();if(selected==null)return;
            selectedItem=selected.Id;ItemDetail(new Rect(1153,193,417,651),selected);
        }
        void ItemDetail(Rect p,CatalogView i)
        {
            PanelBox(p);Label(new Rect(p.x+22,p.y+21,375,57),i.Id,Heading,i.Credits>0?Gold:Ink);
            Box(new Rect(p.x+20,p.y+87,p.width-40,213),Background);
            if(i.Slot=="Weapon")TacticalArt.DrawWeapon(new Rect(p.x+22,p.y+134,p.width-44,128),i.Id);
            else TacticalArt.Fighter(new Rect(p.x+128,p.y+92,148,204),i.Id,true,null,i.Slot=="HeadProtection",i.Slot=="LoadBearingArmor",i.Slot=="Camouflage");
            Label(new Rect(p.x+23,p.y+321,370,31),"CLUB LEVEL "+i.Level+"  /  "+i.Slot,Small);
            if(i.Slot=="Weapon")
            {
                Label(new Rect(p.x+23,p.y+359,366,67),"Base damage "+i.Damage+"  •  "+i.Projectiles+" projectiles\nInterval "+i.Interval+" ms  •  MK"+i.Mk,Body);
                Bar(new Rect(p.x+23,p.y+431,365,5),i.Damage/50f,Gold);
            }
            else Label(new Rect(p.x+23,p.y+359,367,65),"Protection "+i.Protection+"\nAgility penalty "+i.AgilityPenalty,Body);
            Tip(new Rect(p.x+20,p.y+350,375,89),"Catalog base profile. MK, BB, fighter stats and early limits apply when the server accepts battle.");
            bool early=i.EarlyCapped&&(i.Access||i.EarlyAllowed);
            Label(new Rect(p.x+23,p.y+452,369,62),early?"Early contribution limited until Level "+i.Level+". Native power returns automatically; no rebuy.":i.Credits>0?"Premium MK presentation • visual attachments included.":"Money equipment • permanent ownership.",Small);
            if(i.EarlyAllowed&&Click(new Rect(p.x+23,p.y+525,370,39),"UNLOCK ACCESS / "+i.EarlyPrice+" CREDITS"))StartCoroutine(Send(Intent("EarlyUnlock",i.Id)));
            GUI.enabled=ActionsEnabled&&i.Access;
            if(Click(new Rect(p.x+23,p.y+578,370,49),i.Access?"BUY / "+(i.Credits>0?i.Credits+" CREDITS":i.Money+" MONEY"):"REQUIRES CLUB LEVEL "+i.Level,true))StartCoroutine(Send(Intent("Buy",i.Id)));
            GUI.enabled=ActionsEnabled;
        }
        void BetaAmmo(Rect p)
        {
            PanelBox(p);Label(new Rect(p.x+24,p.y+20,p.width-50,42),"ONE TIER. THE WHOLE TEAM.",Heading);
            Label(new Rect(p.x+24,p.y+67,p.width-50,52),"Distinct BB flight colors identify your active ammunition. Reserve is shared across the club.",Small);
            for(int n=0;n<5;n++)
            {
                var b=club.BbCatalog[n];var r=new Rect(p.x+24,p.y+139+n*77,p.width-48,64);Box(r,Card);Box(new Rect(r.x+14,r.y+21,20,20),BbColor(n));
                Label(new Rect(r.x+52,r.y+12,300,29),b.Name+(club.ActiveBbTier==n?" / ACTIVE":""),Body,BbColor(n));
                Label(new Rect(r.x+52,r.y+39,280,23),club.BbStock[n]+" / "+club.Capacity+" BB",Micro);
                if(Click(new Rect(r.xMax-459,r.y+12,181,40),"Select tier"))StartCoroutine(Send(Intent("BbTier",number:n)));
                if(Click(new Rect(r.xMax-265,r.y+12,251,40),"Refill / "+(b.Credits>0?b.Credits+" Credit":b.Money+" Money"),n==club.ActiveBbTier))StartCoroutine(Send(Intent("Refill",number:n)));
            }
            if(club.Level>=3&&Click(new Rect(p.x+24,p.y+555,p.width-48,39),club.AutoBuyBasic?"Disable automatic Basic refill":"Enable automatic Basic refill"))StartCoroutine(Send(Intent("AutoBuyBasic",flag:!club.AutoBuyBasic)));
            Label(new Rect(p.x+24,p.y+607,p.width-48,30),"Auto Basic unlocks at Level 3 and never spends Credits. Emergency Basic remains in Club.",Micro);
        }
    }
}
