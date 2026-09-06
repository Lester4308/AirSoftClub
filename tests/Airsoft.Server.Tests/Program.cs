using Airsoft.Server;
using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
internal static partial class Program
{
    static int passed, failed;
    static Store store = null!;
    static void Check(bool v) { if (!v) throw new Exception("Invariant failed"); }
    static async Task Test(string name, Func<Task> body)
    { try { await body(); passed++; Console.WriteLine("PASS " + name); } catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + " " + e); } }
    static async Task Reject(Func<Task> action)
    { try { await action(); } catch (InvalidOperationException) { return; } catch (DbUpdateException) { return; } throw new Exception("Expected rejection"); }
    static async Task<int> Main()
    {
        store = new Store(Environment.GetEnvironmentVariable("AIRSOFT_CONNECTION") ?? throw new Exception("Actual disposable PostgreSQL required"));
        await using (var db = store.Open()) { await db.Database.MigrateAsync(); Check(await db.Database.CanConnectAsync()); }
        await PersistenceTests(); await BattleTests(); await PvpDatabaseTests(); await SteamTests(); await CommerceTests();
        Console.WriteLine($"SERVER SUMMARY passed={passed} failed={failed}"); return failed == 0 ? 0 : 1;
    }
    static async Task PersistenceTests()
    {
        await Test("actual migrations and persistent create/restart", async () =>
        {
            string id = Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            await using var db = store.Open(); var row = await db.Clubs.FindAsync(id); Check(row?.Credits == 10 && row.Money == 1000);
            Check((await store.Create(id, 0)).Wallet.Credits == 10); Check(await db.Ledger.CountAsync(x => x.Owner == id) == 1);
        });
        await Test("last-resource concurrency and stale version", async () =>
        {
            string id = Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            async Task<bool> Spend(string key) { try { await store.Command(id, key, key, 0, (db, s) => { s.Wallet.Apply(key, "buy", -1000, 0); return Task.FromResult<object>("ok"); }, 0); return true; } catch (InvalidOperationException) { return false; } }
            var outcomes = await Task.WhenAll(Spend("a"), Spend("b")); Check(outcomes.Count(x => x) == 1);
            await using var db = store.Open(); var row = await db.Clubs.FindAsync(id); Check(row!.Money == 0 && row.Version == 1);
        });
        await Test("lost response retry and payload collision", async () =>
        {
            string id = Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            Task<string> Run(string payload) => store.Command(id, "key", payload, 0, (db, s) => { s.Wallet.Apply("buy", "buy", -500, 0); return Task.FromResult<object>(new { Receipt = "same" }); }, 0);
            Check(await Run("one") == await Run("one")); await Reject(() => Run("two"));
            await using var db = store.Open(); Check((await db.Clubs.FindAsync(id))!.Money == 500);
        });
        await Test("rollback after mutation and before commit", async () =>
        {
            string id = Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            await Reject(() => store.Command(id, "failed", "payload", 0, (db, s) => { s.Wallet.Apply("fail", "buy", -300, 0); throw new InvalidOperationException("injected crash"); }, 0));
            await using var db = store.Open(); Check((await db.Clubs.FindAsync(id))!.Money == 1000 && !await db.Operations.AnyAsync(x => x.Owner == id));
        });
        await Test("database nonnegative constraint and ownership FK", async () =>
        {
            string id = Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            await Reject(async () => { await using var db = store.Open(); var row = await db.Clubs.FindAsync(id); row!.Money = -1; await db.SaveChangesAsync(); });
            await Reject(async () => { await using var db = store.Open(); db.Ledger.Add(new LedgerRow { Owner = "absent-" + id, Operation = "x" }); await db.SaveChangesAsync(); });
        });
    }
}
