using System;
using UnityEngine;
using static AirsoftClub.Unity.BetaTheme;
namespace AirsoftClub.Unity
{
    public sealed partial class ClubClient
    {
        void RevengeOverlay()
        {
            Box(new Rect(0,0,1600,900),new Color(0,0,0,.74f));PanelBox(new Rect(392,166,856,589));
            Label(new Rect(418,190,720,43),"REVENGE OPPORTUNITIES",Heading);
            if(Click(new Rect(1130,190,93,35),"Close")){revengeOverlay=false;return;}
            Label(new Rect(418,244,784,66),"A rated win restores 120% of the origin loss; the target loses no rating. A rated start cancels your shield. At the cap: no rating recovery.",Small);
            var tickets=club.RevengeTickets??Array.Empty<RevengeView>();GUI.enabled=!busy;
            scroll=GUI.BeginScrollView(new Rect(418,326,795,382),scroll,new Rect(0,0,770,Math.Max(380,tickets.Length*101)));
            for(int n=0;n<tickets.Length;n++)
            {
                var t=tickets[n];Label(new Rect(10,n*101,501,30),t.Target,Body);
                Label(new Rect(10,n*101+36,505,48),"Loss "+t.ActualLoss+" • Attempts "+t.Attempts+"/3 • "+Math.Max(0,(t.Expires-club.ServerNow)/60000)+" min remaining",Small);
                if(Click(new Rect(541,n*101+10,202,44),"CONFIRM REVENGE",true)){var c=Intent("Attack",t.Target,"Revenge");c.Ticket=t.Origin;revengeOverlay=false;StartCoroutine(Send(c));}
            }
            if(tickets.Length==0)Label(new Rect(10,10,710,70),"No active tickets. Eligible defense losses appear here.",Body);
            GUI.EndScrollView();
        }
    }
}
