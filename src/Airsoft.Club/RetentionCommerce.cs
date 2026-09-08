using System.Text.RegularExpressions;
namespace Airsoft.Club;

public sealed class RetentionState
{
    public long LastDay { get; set; } = -1;
    public int Streak { get; set; }
    public HashSet<string> Grants { get; set; } = new();
}
public static class Retention
{
    public static void Daily(ClubState s, long now)
    {
        long day = now / 86400000;
        if (now < 0 || day <= s.Retention.LastDay) throw new InvalidOperationException("Daily already claimed or clock invalid");
        int streak = day == s.Retention.LastDay + 1 ? s.Retention.Streak % 7 + 1 : 1;
        s.Wallet.Apply("daily:" + day, "daily-013-v1:" + streak, AlphaConfig.DailyMoney + (streak - 1) * AlphaConfig.DailyStep, streak == 7 ? AlphaConfig.SeventhDayCredits : 0);
        s.Retention.LastDay = day; s.Retention.Streak = streak;
    }
    public static void Progression(ClubState s)
    {
        if (s.CompletedMatches.Count > 0 && s.Retention.Grants.Add("first-battle")) s.Wallet.Apply("achievement:first-battle", "achievement-v1", 0, AlphaConfig.FirstBattleCredits);
        for (int level = 2; level <= Math.Min(AlphaConfig.CreditLevelLimit, s.Level); level++)
            if (s.Retention.Grants.Add("level:" + level)) s.Wallet.Apply("level:" + level, "level-v1", 0, AlphaConfig.LevelCredits);
    }
    public static void EarlyUnlock(ClubState s, string item, string operation)
    {
        var definition = Catalog.Get(item);
        var prior = s.Wallet.Find(operation);
        if (prior != null)
        {
            if (prior.Reason != "access:" + item || prior.MoneyDelta != 0 || prior.CreditsDelta >= 0)
                throw new InvalidOperationException("Idempotency payload conflict");
            if (!s.Unlocks.Contains(item)) throw new InvalidOperationException("Ledger/access state conflict");
            return;
        }
        if (!EarlyAccess.CanUnlock(s, definition)) throw new InvalidOperationException("Early unlock unavailable");
        int credits = EarlyAccess.Price(definition.Level - s.Level);
        s.Wallet.Apply(operation, "access:" + item, 0, -credits);
        s.Unlocks.Add(item);
    }
}
public sealed class PurchaseOrder
{
    public string Id { get; set; } = "";
    public string Owner { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public int Credits { get; set; }
}
public static class Commerce
{
    // Development SKU only. No price or real-money request is created by this foundation.
    public static PurchaseOrder Create(string id, string owner, string sku) => sku == "dev-credits-10"
        ? new PurchaseOrder { Id = id, Owner = owner, Sku = sku, Credits = 10 } : throw new InvalidOperationException("Unknown trusted SKU");
    public static void Reconcile(PurchaseOrder order, ClubState s, string trustedProviderState)
    {
        if (order.Owner != s.Id) throw new InvalidOperationException("Order ownership mismatch");
        if (trustedProviderState == "Paid" && order.Status is "Pending" or "Unknown")
        { s.Wallet.Apply("purchase:" + order.Id, "trusted-sku:" + order.Sku, 0, order.Credits); order.Status = "Granted"; }
        else if (trustedProviderState == "Refunded" && order.Status == "Granted")
        {
            if (s.Wallet.Credits < order.Credits) { order.Status = "NeedsReview"; return; }
            s.Wallet.Apply("refund:" + order.Id, "refund:" + order.Sku, 0, -order.Credits); order.Status = "Refunded";
        }
        else if (order.Status == "Pending") order.Status = "Unknown";
    }
}
public sealed record ModerationAudit(string Actor, string Target, string Action, long At);
public sealed record ReportEntry(string Reporter, string Target, string Reason, long At);
public sealed class ModerationState
{
    public List<ModerationAudit> Audit { get; set; } = new();
    public List<ReportEntry> Reports { get; set; } = new();
}
public static class Moderation
{
    public static void Name(ClubState s, string name)
    {
        if (!Regex.IsMatch(name, @"^[\p{L}\p{N} _-]{3,24}$")) throw new InvalidOperationException("Name:3–24 letters/numbers/spaces");
        s.Name = name;
    }
    public static void Report(ModerationState state, string actor, string target, string reason, long now)
    {
        if (actor == target || reason is not ("Name" or "Emblem" or "Cheating")) throw new InvalidOperationException("Invalid report");
        if (!state.Reports.Any(r => r.Reporter == actor && r.Target == target && r.Reason == reason && now - r.At < 86400000)) state.Reports.Add(new(actor, target, reason, now));
    }
    public static void Sanction(ModerationState state, ClubState target, string actor, bool authorizedModerator, string action, long now)
    {
        if (!authorizedModerator) throw new InvalidOperationException("Moderator permission required");
        if (action is not ("Warning" or "Rename" or "Restriction" or "Suspension")) throw new InvalidOperationException("Unknown sanction");
        if (action == "Rename") target.Name = "Club " + target.Id[..Math.Min(8, target.Id.Length)];
        if (action is "Restriction" or "Suspension") target.RestrictedUntil = now + (action == "Suspension" ? 7 : 1) * 86400000L;
        state.Audit.Add(new(actor, target.Id, action, now));
    }
}
