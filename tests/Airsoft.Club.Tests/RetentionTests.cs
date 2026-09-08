using Airsoft.Club;
internal static partial class Program
{
    static void RetentionTests()
    {
        Test("UTC daily boundary, missed day and seven-day cycle", () =>
        {
            var s = Starter(); Retention.Daily(s, 86399999); Reject(() => Retention.Daily(s, 86399999)); Retention.Daily(s, 86400000); Check(s.Retention.Streak == 2);
            Retention.Daily(s, 3 * 86400000L); Check(s.Retention.Streak == 1);
            for (int d = 4; d <= 10; d++) Retention.Daily(s, d * 86400000L); Check(s.Retention.Streak == 1);
        });
        Test("UTC daily rejects pre-epoch time without mutation", () =>
        {
            var s = Starter(); long money = s.Wallet.Money; long credits = s.Wallet.Credits;
            Reject(() => Retention.Daily(s, -1));
            Check(s.Retention.LastDay == -1 && s.Retention.Streak == 0 && s.Wallet.Money == money && s.Wallet.Credits == credits);
        });
        Test("achievement/level grants once and access separate from item", () =>
        {
            var s = Starter(); s.CompletedMatches.Add("match"); s.Xp = 1000; Retention.Progression(s); Retention.Progression(s); Check(s.Wallet.Credits == 13);
            s.Xp = 1000; Retention.EarlyUnlock(s, "HeadProtection-3", "unlock"); Retention.EarlyUnlock(s, "HeadProtection-3", "unlock");
            Check(s.Items.Count == 0 && s.Wallet.Entries.Count(e => e.Operation == "unlock") == 1);
            Reject(() => Retention.EarlyUnlock(s, "BodyProtection-3", "unlock"));
            Clubs.Buy(s, "HeadProtection-3", "buy"); Check(s.Items.Count == 1 && s.Wallet.Credits == 12);
        });
        Test("trusted order ownership, duplicate payment and refund", () =>
        {
            var s = Starter(); var order = Commerce.Create("order", s.Id, "dev-credits-10"); Commerce.Reconcile(order, s, "Unknown"); Check(s.Wallet.Credits == 10);
            Commerce.Reconcile(order, s, "Paid"); Commerce.Reconcile(order, s, "Paid"); Check(s.Wallet.Credits == 20);
            Commerce.Reconcile(order, s, "Refunded"); Commerce.Reconcile(order, s, "Refunded"); Check(s.Wallet.Credits == 10);
            Reject(() => Commerce.Reconcile(order, Starter(), "Paid")); Reject(() => Commerce.Create("bad", s.Id, "arbitrary"));
        });
        Test("spent-credit refund needs review without invented debt", () =>
        {
            var s = Starter(); var o = Commerce.Create("paid", s.Id, "dev-credits-10"); Commerce.Reconcile(o, s, "Paid"); s.Wallet.Apply("spend", "fixture", 0, -20);
            Commerce.Reconcile(o, s, "Refunded"); Check(o.Status == "NeedsReview" && s.Wallet.Credits == 0);
        });
        Test("moderation rejects unauthorized sanctions and audits authorized", () =>
        {
            var s = Starter(); var m = new ModerationState(); Reject(() => Moderation.Name(s, "<script>")); Moderation.Name(s, "New club");
            Reject(() => Moderation.Sanction(m, s, "player", false, "Suspension", 0)); Check(m.Audit.Count == 0);
            Moderation.Report(m, "other", s.Id, "Name", 0); Moderation.Report(m, "other", s.Id, "Name", 1); Check(m.Reports.Count == 1);
            Reject(() => Moderation.Report(m, "other", s.Id, "Chat", 2));
            Moderation.Sanction(m, s, "moderator", true, "Warning", 0); Check(s.RestrictedUntil == 0 && m.Audit.Count == 1);
            Moderation.Sanction(m, s, "moderator", true, "Rename", 1); Check(s.Name.StartsWith("Club ", StringComparison.Ordinal) && m.Audit.Count == 2);
            Moderation.Sanction(m, s, "moderator", true, "Restriction", 2); Check(s.RestrictedUntil == 86400002 && m.Audit.Count == 3);
            Moderation.Sanction(m, s, "moderator", true, "Suspension", 3); Check(s.RestrictedUntil == 604800003 && m.Audit.Count == 4);
        });
    }
}
