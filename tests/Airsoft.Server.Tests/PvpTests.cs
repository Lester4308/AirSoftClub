using Airsoft.Server;
using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
internal static partial class Program
{
    static async Task PvpDatabaseTests()
    {
        await Test("wallet projection cannot diverge from immutable ledger", async () =>
        {
            string id = await Armed();
            await Reject(() => store.Command(id, "tamper", "tamper", 1, (db, s) => { s.Wallet.Money++; return Task.FromResult<object>(true); }, 0));
            await using var db = store.Open(); Check((await db.Clubs.FindAsync(id))!.Money == 900);
        });
        await Test("actual PostgreSQL fifth incoming start race rejected", async () =>
        {
            string target = await Armed(); var attackers = new List<string>(); for (int n = 0; n < 5; n++) attackers.Add(await Armed());
            var battles = new Battles(store);
            async Task<bool> Start(string id) { try { await battles.Start(id, "rank", 1, new StartIntent(target, "Ranked"), 1000); return true; } catch (InvalidOperationException) { return false; } }
            var outcomes = await Task.WhenAll(attackers.Select(Start)); Check(outcomes.Count(x => x) == 4);
            await battles.Recover(121000);
            await using var db = store.Open(); var p = Json.Read<PvpState>((await db.Policies.FindAsync(1))!.State);
            Check(!p.Exposures.Any(e => e.Defender == target));
        });
        await Test("database ledger cannot be edited", async () =>
        {
            string id = await Armed();
            await Reject(async () => { await using var db = store.Open(); var row = await db.Ledger.FirstAsync(x => x.Owner == id); row.Money = 999; await db.SaveChangesAsync(); });
        });
    }
}
