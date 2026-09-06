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
        Test("achievement/level grants once and access separate from item", () =>
        {
            var s = Starter(); s.CompletedMatches.Add("match"); s.Xp = 1000; Retention.Progression(s); Retention.Progression(s); Check(s.Wallet.Credits == 13);
            s.Xp = 0; Retention.EarlyUnlock(s, "HeadProtection-3", "unlock"); Check(s.Items.Count == 0); Clubs.Buy(s, "HeadProtection-3", "buy"); Check(s.Items.Count == 1 && s.Wallet.Credits == 12);
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
            Moderation.Sanction(m, s, "moderator", true, "Restriction", 0); Check(s.RestrictedUntil == 86400000 && m.Audit.Count == 1);
            Moderation.Report(m, "other", s.Id, "Name", 0); Moderation.Report(m, "other", s.Id, "Name", 1); Check(m.Reports.Count == 1);
        });
    }
}
