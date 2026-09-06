using Airsoft.Server;
using Airsoft.Club;
using Airsoft.Battle;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
internal static partial class Program
{
    static async Task<string> Armed()
    {
        string id = Guid.NewGuid().ToString("N"); await store.Create(id, 0);
        await store.Command(id, "prepare", "prepare", 0, (db, s) =>
        {
            var f = Clubs.Hire(s, s.Offers[0].Id, 1, true, 0, "free"); Clubs.Equip(s, f.Id, Clubs.Buy(s, "Pistol-MK1", "buy")); return Task.FromResult<object>(true);
        }, 0); return id;
    }
    static string MatchId(string json) => JsonDocument.Parse(json).RootElement.GetProperty("MatchId").GetString()!;
    static async Task BattleTests()
    {
        await Test("accepted snapshot, repeated start and once-only settlement", async () =>
        {
            string a = await Armed(), d = await Armed(); var battles = new Battles(store);
            var intent = new StartIntent(d); string response = await battles.Start(a, "start", 1, intent, 0);
            Check(response == await battles.Start(a, "start", 1, intent, 0));
            await Reject(() => battles.Start(a, "start", 1, new StartIntent("different"), 0));
            await Reject(() => store.Command(a, "heal", "heal", 2, (db, s) => { Clubs.Heal(s, s.Fighters[0].Id, 1, "heal"); return Task.FromResult<object>(true); }, 1));
            byte[] accepted; string before;
            await using (var db = store.Open()) { accepted = (await db.Matches.FindAsync(MatchId(response)))!.Input; before = (await db.Clubs.FindAsync(d))!.State; }
            // A new Store/Battles instance recovers durable work after process loss.
            await new Battles(store).Recover(10); await battles.Resolve(MatchId(response), 20);
            await using (var db = store.Open())
            {
                var row = (await db.Matches.FindAsync(MatchId(response)))!; var owner = Json.Read<ClubState>((await db.Clubs.FindAsync(a))!.State);
                Check(row.Status == "Completed" && row.Input.SequenceEqual(accepted));
                Check(before == (await db.Clubs.FindAsync(d))!.State && owner.PendingMatch == null && owner.CompletedSinceRefresh == 1);
                Check(await db.Ledger.CountAsync(x => x.Owner == a && x.Operation.StartsWith("match:")) == 1);
                var input = BattleWire.ReadConfig(accepted); var result = BattleWire.ReadResult(row.Result!);
                Check(BattleWire.Digest(result) == BattleWire.Digest(new BattleEngine().Run(input)));
                Check(owner.BbStock[0] == Clubs.StarterBb - result.Attacker.BbConsumed);
            }
        });
        await Test("concurrent offense and expired worker no rewards", async () =>
        {
            string a = await Armed(), d = await Armed(); var battles = new Battles(store);
            string response = await battles.Start(a, "a", 1, new StartIntent(d), 0);
            await Reject(() => battles.Start(a, "b", 2, new StartIntent(d), 0)); await battles.Resolve(MatchId(response), 120000);
            await using var db = store.Open(); var s = Json.Read<ClubState>((await db.Clubs.FindAsync(a))!.State);
            Check(s.PendingMatch == null && s.CompletedSinceRefresh == 0 && s.BbStock[0] == Clubs.StarterBb);
            Check((await db.Matches.FindAsync(MatchId(response)))!.Status == "Failed");
        });
        await Test("snapshot publication after commit and rollback", async () =>
        {
            string a = await Armed(); byte[] before;
            await using (var db = store.Open()) before = (await db.Clubs.FindAsync(a))!.Defense!;
            await Reject(() => store.Command(a, "fail", "fail", 1, async (db, s) =>
            {
                s.Fighters[0].Accuracy++; var row = (await db.Clubs.FindAsync(a))!; await Store.Save(db, row, s, 0); await db.SaveChangesAsync(); throw new InvalidOperationException("before commit");
            }, 0));
            await using var after = store.Open(); var persisted = (await after.Clubs.FindAsync(a))!.Defense!; Check(before.SequenceEqual(persisted));
        });
    }
}
