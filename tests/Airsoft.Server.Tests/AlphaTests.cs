using Airsoft.Club;
using Airsoft.Server;
using Microsoft.EntityFrameworkCore;
internal static partial class Program
{
    static async Task<bool> Attempt(Func<Task> action)
    { try { await action(); return true; } catch (InvalidOperationException) { return false; } }
    static async Task AlphaDatabaseTests()
    {
        await Test("catalog refresh republishes legacy capped defense once without charging", async () =>
        {
            string id = await Armed();
            await using (var db = store.Open())
            {
                var row = (await db.Clubs.FindAsync(id))!;
                var s = Json.Read<ClubState>(row.State); s.CatalogRevision = "catalog-013-v1";
                s.Items[s.Fighters[0].Equipment[Slot.Weapon]] = "AssaultRifle-MK2";
                s.Unlocks.Add("AssaultRifle-MK2"); row.State = Json.Write(s);
                await db.SaveChangesAsync();
            }
            await store.RefreshCatalog(1);
            await using (var db = store.Open())
            {
                var row = (await db.Clubs.FindAsync(id))!; var s = Json.Read<ClubState>(row.State);
                var team = Airsoft.Battle.BattleWire.ReadTeam(row.Defense!);
                Check(s.CatalogRevision == Catalog.Version && s.Version == 2 && row.Money == 900 && s.Unlocks.Contains("AssaultRifle-MK2"));
                Check(team.Fighters[0].Weapon!.Damage.Raw < EarlyAccess.Native(Catalog.Get("AssaultRifle-MK2")).Damage.Raw);
            }
            await store.RefreshCatalog(2);
            await using (var db = store.Open()) Check((await db.Clubs.FindAsync(id))!.Version == 2);
        });
        await Test("development attacks cannot cross into Steam identity scope", async () =>
        {
            string id = "dev-scope-" + Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            await Reject(() => new Battles(store).Start(id, "cross-provider", 0, new StartIntent("steam-12345678901234567"), 0));
            await using var db = store.Open(); Check(!await db.Matches.AnyAsync(m => m.Attacker == id) && (await db.Clubs.FindAsync(id))!.Version == 0);
        });
        await Test("refresh race and refresh versus hire cannot consume stale offers", async () =>
        {
            string id = await Armed();
            Task<string> Refresh(string key) => store.Command(id, key, key, 1, (db, s) => { Clubs.Refresh(s, 3600000, 13); return Task.FromResult<object>(true); }, 3600000);
            var outcomes = await Task.WhenAll(Attempt(() => Refresh("r1")), Attempt(() => Refresh("r2"))); Check(outcomes.Count(x => x) == 1);
            await Reject(() => store.Command(id, "hire-old", "old", 2, (db, s) => { Clubs.Hire(s, "1-1", 1, false, 3600000, "hire"); return Task.FromResult<object>(true); }, 3600000));
            await using var db = store.Open(); var s = Json.Read<ClubState>((await db.Clubs.FindAsync(id))!.State);
            Check(s.OfferVersion == 2 && s.CompletedSinceRefresh == 0 && s.FreeRecruitClaimed && s.Fighters.Count == 1);
        });
        await Test("two actual item purchases at one version commit once", async () =>
        {
            string id = await Armed();
            Task<string> Buy(string key) => store.Command(id, key, key, 1, (db, s) => Task.FromResult<object>(Clubs.Buy(s, "Pistol-MK1", key)), 1);
            var outcomes = await Task.WhenAll(Attempt(() => Buy("a")), Attempt(() => Buy("b"))); Check(outcomes.Count(x => x) == 1);
            await using var db = store.Open(); var s = Json.Read<ClubState>((await db.Clubs.FindAsync(id))!.State);
            Check(s.Items.Count == 2 && s.Wallet.Money == 800);
        });
        await Test("pending battle blocks equip and heal; final history immutable and receipt once", async () =>
        {
            string a = await Armed(), d = await Armed(); var battles = new Battles(store);
            string matchId = MatchId(await battles.Start(a, "battle", 1, new StartIntent(d, "Ranked"), 1));
            await Reject(() => store.Command(a, "equip", "equip", 2, (db, s) => { Clubs.Equip(s, s.Fighters[0].Id, s.Items.Keys.First()); return Task.FromResult<object>(true); }, 2));
            await Reject(() => store.Command(a, "heal", "heal", 2, (db, s) => { Clubs.Heal(s, s.Fighters[0].Id, 2, "heal"); return Task.FromResult<object>(true); }, 2));
            await battles.Resolve(matchId, 3); await battles.Resolve(matchId, 4);
            await using (var db = store.Open())
            {
                var m = (await db.Matches.FindAsync(matchId))!; var receipt = Json.Read<SettlementReceipt>(m.Settlement);
                Check(Json.Read<CapturedEconomy>(m.Economy).Appearance.Length == 2);
                Check(receipt.Schema == 1 && receipt.At == 3 && m.AttackerVersion == 1 && m.DefenderVersion == 1 && m.CatalogVersion == Catalog.Version);
                Check((await db.Clubs.FindAsync(a))!.Rating == 100 + receipt.AttackerRatingDelta);
                Check((await db.Clubs.FindAsync(d))!.Rating == 100 + receipt.DefenderRatingDelta);
            }
            await Reject(async () => { await using var db = store.Open(); var m = (await db.Matches.FindAsync(matchId))!; m.Settlement = "tampered"; await db.SaveChangesAsync(); });
        });
        await Test("shield racing incoming acceptance blocks or preserves accepted battle", async () =>
        {
            string a = await Armed(), d = await Armed(); var battles = new Battles(store); string? match = null;
            var start = Attempt(async () => { match = MatchId(await battles.Start(a, "attack", 1, new StartIntent(d), 1)); });
            var shield = store.Command(d, "shield", "shield", 1, (db, s) => { Pvp.Shield(s, 8, 1, "shield"); return Task.FromResult<object>(true); }, 1);
            await Task.WhenAll(start, shield);
            if (match != null) await battles.Resolve(match, 2);
            await using var db = store.Open(); var s = Json.Read<ClubState>((await db.Clubs.FindAsync(d))!.State); Check(s.ShieldUntil == 28800001);
            if (match != null) Check((await db.Matches.FindAsync(match))!.Status == "Completed");
            else Check(!await db.Matches.AnyAsync(m => m.Attacker == a));
        });
        await Test("offline recovery read projection does not mutate version and shield persists", async () =>
        {
            string id = await Armed();
            await store.Command(id, "wound", "fixture", 1, (db, s) => { s.Fighters[0].Hp = 0; Pvp.Shield(s, 8, Api.Now, "shield"); return Task.FromResult<object>(true); }, 0);
            var view = System.Text.Json.JsonDocument.Parse(Json.Write(await Api.View(store, id))).RootElement;
            Check(view.GetProperty("Fighters")[0].GetProperty("Ready").GetBoolean() && view.GetProperty("ShieldUntil").GetInt64() > Api.Now);
            await using var db = store.Open(); var s = Json.Read<ClubState>((await db.Clubs.FindAsync(id))!.State); Check(s.Version == 2 && s.Fighters[0].Hp == 0);
        });
        await Test("dev tools reject Steam ownership and preserve idempotent fixture grants", async () =>
        {
            string id = "dev-test-" + Guid.NewGuid().ToString("N"); await store.Create(id, 0);
            var c = new CommandIntent("grant", 0, "Grant");
            await Reject(() => DevelopmentTools.Execute(store, "steam-123", c, 0));
            Check(await DevelopmentTools.Execute(store, id, c, 0) == await DevelopmentTools.Execute(store, id, c, 1));
            await using var db = store.Open(); Check((await db.Clubs.FindAsync(id))!.Credits == 30);
        });
        await Test("commerce fixture port unknown then paid reconciles once", async () =>
        {
            string id = await Armed(), order = Guid.NewGuid().ToString("N"); var p = new Purchases(store); await p.Create(order, id, "dev-credits-10");
            var states = new Dictionary<string, string>(); var provider = new DevelopmentCommerceProvider(states);
            Check(!await p.RefreshFromProvider(order, provider, 0)); states[order] = "Paid";
            Check(await p.RefreshFromProvider(order, provider, 1) && await p.RefreshFromProvider(order, provider, 2));
            await using var db = store.Open(); Check((await db.Clubs.FindAsync(id))!.Credits == 20);
        });
    }
}
