using Airsoft.Battle;
namespace Airsoft.Club;

public sealed class FriendWindow
{
    public string Attacker { get; set; } = "";
    public string Defender { get; set; } = "";
    public long Anchor { get; set; }
    public bool LossUsed { get; set; }
    public int Wins { get; set; }
}
public sealed class Exposure
{
    public string Match { get; set; } = "";
    public string Defender { get; set; } = "";
    public long At { get; set; }
    public long LeaseUntil { get; set; }
    public bool Committed { get; set; }
}
public sealed class RevengeTicket
{
    public string Origin { get; set; } = "";
    public string Owner { get; set; } = "";
    public string Target { get; set; } = "";
    public int ActualLoss { get; set; }
    public long Expires { get; set; }
    public int Attempts { get; set; }
    public bool Consumed { get; set; }
}
public sealed class PvpState
{
    public ModerationState Moderation { get; set; } = new();
    public List<FriendWindow> Friends { get; set; } = new();
    public List<Exposure> Exposures { get; set; } = new();
    public List<RevengeTicket> Tickets { get; set; } = new();
}
public sealed record PvpCapture(string Mode, bool Rated, int FriendWins, bool FriendBudget, bool FriendLossUsed,
    int AttackerRating, int DefenderRating, string Ticket, long FriendAnchor);
public static class Pvp
{
    public const long EightHours = 28800000, Day = 86400000;
    public static PvpCapture Accept(PvpState p, ClubState a, ClubState d, string mode, string match, long now, string ticket = "", bool developmentRevenge = false)
    {
        if (a.Id == d.Id || d.ShieldUntil > now) throw new InvalidOperationException("Target unavailable/protected");
        if (mode is not ("Ranked" or "Friend" or "Practice" or "Revenge")) throw new InvalidOperationException("Unknown mode");
        var pair = p.Friends.SingleOrDefault(x => x.Attacker == a.Id && x.Defender == d.Id && now < x.Anchor + EightHours);
        bool rated = mode is "Ranked" or "Revenge" || mode == "Friend" && (pair == null || pair.Wins == 0);
        if (mode == "Revenge" && !developmentRevenge) throw new InvalidOperationException("Revenge live settlement awaits counterparty product policy");
        if (rated && p.Exposures.Count(x => x.Defender == d.Id && (x.Committed ? x.At > now - Day : x.LeaseUntil > now)) >= 4)
            throw new InvalidOperationException("Incoming rating exposure cap; choose explicit Practice");
        if (mode == "Friend" && pair == null)
        {
            pair = new FriendWindow { Attacker = a.Id, Defender = d.Id, Anchor = now }; p.Friends.Add(pair);
        }
        if (mode == "Revenge")
        {
            var r = p.Tickets.SingleOrDefault(x => x.Origin == ticket && x.Owner == a.Id && x.Target == d.Id);
            if (r == null || r.Consumed || r.Attempts >= 3 || now >= r.Expires) throw new InvalidOperationException("Revenge ticket unavailable");
            r.Attempts++;
        }
        if (rated) p.Exposures.Add(new Exposure { Match = match, Defender = d.Id, At = now, LeaseUntil = now + 120000 });
        if (mode == "Ranked" || mode == "Revenge" && rated) a.ShieldUntil = 0;
        // Existing exhausted pair applies across modes; switching mode cannot reopen reward budget.
        return new PvpCapture(mode, rated, pair?.Wins ?? 0, pair != null, pair?.LossUsed ?? false, a.Rating, d.Rating, ticket, pair?.Anchor ?? -1);
    }
    public static void Finish(PvpState p, ClubState a, ClubState d, string match, PvpCapture c, MatchOutcome? outcome, long now)
    {
        var exposure = p.Exposures.SingleOrDefault(x => x.Match == match);
        if (c.Rated && (exposure == null || exposure.Committed || now >= exposure.LeaseUntil)) throw new InvalidOperationException("Exposure lease fenced");
        if (outcome == null) { if (exposure != null) p.Exposures.Remove(exposure); return; }
        var pair = p.Friends.SingleOrDefault(x => x.Attacker == a.Id && x.Defender == d.Id && x.Anchor == c.FriendAnchor);
        int defenderRatingBefore = d.Rating;
        int gain = Math.Clamp(10 + (c.DefenderRating - c.AttackerRating) / 100, 5, 20);
        int delta = !c.Rated || outcome == MatchOutcome.Draw ? 0 : outcome == MatchOutcome.AttackerWin ? gain : -gain;
        if (c.Mode == "Friend" && (c.FriendWins > 0 || outcome == MatchOutcome.DefenderWin && c.FriendLossUsed)) delta = 0;
        if (c.Mode == "Revenge")
        {
            var r = p.Tickets.Single(x => x.Origin == c.Ticket);
            delta = outcome == MatchOutcome.AttackerWin ? r.ActualLoss * 120 / 100 : 0;
            if (delta > 0) r.Consumed = true;
            // Test-only recovery policy: no counterparty debit. Live path remains disabled pending approval.
            a.Rating += delta;
        }
        else
        {
            int actualDefenderLoss = Math.Min(d.Rating, Math.Max(0, delta));
            a.Rating = Math.Max(0, a.Rating + delta); d.Rating = Math.Max(0, d.Rating - delta);
            if (c.Mode == "Ranked" && outcome == MatchOutcome.AttackerWin && actualDefenderLoss > 0 && !p.Tickets.Any(x => x.Origin == match))
                p.Tickets.Add(new RevengeTicket { Origin = match, Owner = d.Id, Target = a.Id, ActualLoss = actualDefenderLoss, Expires = now + Day });
        }
        if (pair != null)
        {
            if (outcome == MatchOutcome.AttackerWin) pair.Wins++;
            if (c.Mode == "Friend" && delta < 0) pair.LossUsed = true;
        }
        if (exposure != null)
        {
            if (delta == 0 || c.Mode != "Revenge" && d.Rating == defenderRatingBefore) p.Exposures.Remove(exposure);
            else { exposure.Committed = true; exposure.At = now; }
        }
    }
    public static void Shield(ClubState s, int hours, long now, string operation)
    {
        if (hours is not (8 or 24 or 72 or 168)) throw new InvalidOperationException("Unknown shield");
        if (s.ShieldUntil > now) throw new InvalidOperationException("Active shield cannot stack");
        s.Wallet.Apply(operation, "shield:" + hours, 0, -(hours == 8 ? 1 : hours == 24 ? 2 : hours == 72 ? 4 : 7));
        s.ShieldUntil = now + hours * 3600000L;
    }
}
