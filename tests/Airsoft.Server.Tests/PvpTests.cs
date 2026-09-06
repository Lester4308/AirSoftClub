using Airsoft.Server;
using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
internal static partial class Program
{
    static async Task PvpDatabaseTests()
    {
        await Test("live revenge duplicate start and restarted settlement preserve one attempt and target rating", async () =>
        {
            string a = await Armed(), d = await Armed(); string origin = Guid.NewGuid().ToString("N");
            await store.Transaction(async db =>
            {
                var row = await Battles.Policies(db); var p = Json.Read<PvpState>(row.State);
                p.Tickets.Add(new RevengeTicket { Origin = origin, Owner = a, Target = d, ActualLoss = 17, Expires = 2 });
                row.State = Json.Write(p); return true;
            });
            var battles = new Battles(store); var intent = new StartIntent(d, "Revenge", origin);
            var starts = await Task.WhenAll(battles.Start(a, "revenge", 1, intent, 1), battles.Start(a, "revenge", 1, intent, 1));
            Check(starts[0] == starts[1]);
            await new Battles(store).Recover(3); await battles.Resolve(MatchId(starts[0]), 4);
            await using var db = store.Open(); var match = (await db.Matches.FindAsync(MatchId(starts[0])))!;
            var p = Json.Read<PvpState>((await db.Policies.FindAsync(1))!.State); var ticket = p.Tickets.Single(t => t.Origin == origin);
            var result = Airsoft.Battle.BattleWire.ReadResult(match.Result!); bool won = result.Outcome == Airsoft.Battle.MatchOutcome.AttackerWin;
            Check(ticket.Attempts == 1 && ticket.Consumed == won && match.Status == "Completed");
            Check((await db.Clubs.FindAsync(d))!.Rating == 100 && (await db.Clubs.FindAsync(a))!.Rating == (won ? 120 : 100));
            Check(p.Tickets.Count(t => t.Origin == origin || t.Origin == match.Id) == 1);
            Check(await db.Ledger.CountAsync(x => x.Owner == a && x.Operation == "match:" + match.Id) == 1);
            var view = Json.Write(await Api.View(store, a)); Check(view.Contains("RevengeTickets"));
        });
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
