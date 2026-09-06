using Airsoft.Server;
using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
internal static partial class Program
{
    static async Task CommerceTests()
    {
        await Test("payment observed then DB fails, reconcile grants once", async () =>
        {
            string a = await Armed(), id = Guid.NewGuid().ToString("N"); var p = new Purchases(store);
            await p.Create(id, a, "dev-credits-10"); await p.Create(id, a, "dev-credits-10"); await p.ObserveTrustedProvider(id, "Paid");
            await Reject(() => p.Reconcile(id, 1, true));
            await using (var db = store.Open()) { Check((await db.Clubs.FindAsync(a))!.Credits == 10); Check((await db.Set<OrderRow>().FindAsync(id))!.ProviderState == "Paid"); }
            await new Purchases(store).Reconcile(id, 2); await p.Reconcile(id, 3);
            await using (var db = store.Open()) Check((await db.Clubs.FindAsync(a))!.Credits == 20);
            await p.ObserveTrustedProvider(id, "Refunded"); await p.Reconcile(id, 4); await p.Reconcile(id, 5);
            await using (var db = store.Open()) Check((await db.Clubs.FindAsync(a))!.Credits == 10);
        });
        await Test("UTC duplicate claim under actual database race", async () =>
        {
            string a = await Armed();
            async Task<bool> Claim(string key) { try { await store.Command(a, key, key, 1, (db, s) => { Retention.Daily(s, 86400000); return Task.FromResult<object>(true); }, 86400000); return true; } catch (InvalidOperationException) { return false; } }
            var result = await Task.WhenAll(Claim("a"), Claim("b")); Check(result.Count(x => x) == 1);
            await using var db = store.Open(); Check(await db.Ledger.CountAsync(x => x.Owner == a && x.Operation.StartsWith("daily:")) == 1);
        });
    }
}
