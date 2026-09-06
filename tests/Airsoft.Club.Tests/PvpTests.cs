using Airsoft.Club;
using Airsoft.Battle;
internal static partial class Program
{
    static void PvpTests()
    {
        Test("directed friend first loss first win rating then zero", () =>
        {
            var p = new PvpState(); var a = Starter(); var d = Starter(); int initial = a.Rating;
            for (int n = 0; n < 6; n++) { var c = Pvp.Accept(p, a, d, "Friend", "m" + n, n * 10); Pvp.Finish(p, a, d, "m" + n, c, n < 2 ? MatchOutcome.DefenderWin : MatchOutcome.AttackerWin, n * 10 + 1); }
            Check(a.Rating == initial && p.Friends.Single().Wins == 4);
            var exhausted = Pvp.Accept(p, a, d, "Practice", "p", 100); Check(exhausted.FriendWins == 4 && exhausted.FriendBudget);
            var reverse = Pvp.Accept(p, d, a, "Friend", "reverse", 100); Check(reverse.FriendWins == 0);
            var reset = Pvp.Accept(p, a, d, "Friend", "reset", Pvp.EightHours); Check(reset.FriendWins == 0);
        });
        Test("four incoming reservations, draw release, rolling boundary and stale worker", () =>
        {
            var p = new PvpState(); var a = Starter(); var d = Starter(); var captures = new List<PvpCapture>();
            for (int n = 0; n < 4; n++) captures.Add(Pvp.Accept(p, a, d, "Ranked", "m" + n, 0));
            Reject(() => Pvp.Accept(p, a, d, "Ranked", "fifth", 0));
            Pvp.Finish(p, a, d, "m0", captures[0], MatchOutcome.Draw, 1); var fourth = Pvp.Accept(p, a, d, "Ranked", "new", 1);
            Pvp.Finish(p, a, d, "new", fourth, MatchOutcome.AttackerWin, 2);
            Reject(() => Pvp.Finish(p, a, d, "m1", captures[1], MatchOutcome.AttackerWin, 120000));
            Check(p.Exposures.Count(x => x.Committed) == 1);
            for (int n = 0; n < 4; n++) Pvp.Accept(p, a, d, "Ranked", "later" + n, Pvp.Day + 2);
        });
        Test("shield mode semantics and accepted incoming finish", () =>
        {
            var p = new PvpState(); var a = Starter(); var d = Starter(); var accepted = Pvp.Accept(p, a, d, "Ranked", "in", 0);
            Pvp.Shield(d, 8, 1, "shield"); Reject(() => Pvp.Accept(p, a, d, "Practice", "blocked", 2));
            Pvp.Finish(p, a, d, "in", accepted, MatchOutcome.AttackerWin, 3);
            Pvp.Accept(p, d, a, "Friend", "friend", 4); Check(d.ShieldUntil > 4);
            Pvp.Accept(p, d, a, "Ranked", "rank", 5); Check(d.ShieldUntil == 0);
        });
        Test("revenge actual floor loss, three draws, expiry-at-start and no chains", () =>
        {
            var p = new PvpState(); var a = Starter(); var d = Starter(); d.Rating = 3;
            var c = Pvp.Accept(p, a, d, "Ranked", "origin", 0); Pvp.Finish(p, a, d, "origin", c, MatchOutcome.AttackerWin, 1);
            Check(p.Tickets.Single().ActualLoss == 3);
            Reject(() => Pvp.Accept(p, d, a, "Revenge", "live", 2, "origin"));
            for (int n = 0; n < 3; n++) { var r = Pvp.Accept(p, d, a, "Revenge", "r" + n, 2 + n, "origin", true); Pvp.Finish(p, d, a, "r" + n, r, MatchOutcome.Draw, 3 + n); }
            Reject(() => Pvp.Accept(p, d, a, "Revenge", "r4", 10, "origin", true)); Check(p.Tickets.Count == 1);
            p.Tickets[0].Attempts = 0; var win = Pvp.Accept(p, d, a, "Revenge", "win", Pvp.Day, "origin", true);
            Pvp.Finish(p, d, a, "win", win, MatchOutcome.AttackerWin, Pvp.Day + 2); Check(d.Rating == 3 && p.Tickets[0].Consumed && p.Tickets.Count == 1);
        });
    }
}
