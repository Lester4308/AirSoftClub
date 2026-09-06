using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
namespace Airsoft.Server;

public static class DevelopmentTools
{
    public static void Map(WebApplication app)
    {
        if (!app.Environment.IsDevelopment() || app.Configuration["AIRSOFT_DEV_AUTH"] != "1") return;
        app.MapGet("/api/dev/ledger", async (HttpContext http, Store store) =>
        {
            string owner = Owner(http); await using var db = store.Open();
            return Results.Json(new { Entries = await db.Ledger.Where(l => l.Owner == owner).OrderByDescending(l => l.At).Take(100).ToListAsync() });
        });
        app.MapPost("/api/dev/command", async (CommandIntent c, HttpContext http, Store store) =>
            Results.Content(await Execute(store, Owner(http), c, Api.Now), "application/json"));
    }
    static string Owner(HttpContext http)
    {
        string owner = Api.Owner(http);
        if (!owner.StartsWith("dev-", StringComparison.Ordinal) || http.Connection.RemoteIpAddress == null || !System.Net.IPAddress.IsLoopback(http.Connection.RemoteIpAddress))
            throw new InvalidOperationException("Local development identity required");
        return owner;
    }
    public static Task<string> Execute(Store store, string owner, CommandIntent c, long now)
    {
        if (!owner.StartsWith("dev-", StringComparison.Ordinal)) throw new InvalidOperationException("Development identity required");
        return store.Command(owner, c.Key, Json.Write(c), c.Version, async (db, s) =>
        {
            Clubs.Available(s);
            switch (c.Type)
            {
                case "Grant": s.Wallet.Apply(c.Key, "DEV fixture grant", 10000, 20); break;
                case "Recovery":
                    foreach (var f in s.Fighters.Where(f => f.Active)) { f.Recover(now + AlphaConfig.FullRecoveryMs); f.RecoveryAt = now; }
                    break;
                case "Refresh": s.OffersAt = now - AlphaConfig.RecruitRefreshMs; Clubs.Refresh(s, now, 13); break;
                case "Opponent":
                    if (!s.Fighters.Any(f => f.Active)) { var f = Clubs.Hire(s, s.Offers[0].Id, s.OfferVersion, !s.FreeRecruitClaimed, now, c.Key + ":hire"); Clubs.Equip(s, f.Id, Clubs.Buy(s, "Pistol-MK1", c.Key + ":gear")); }
                    break;
                case "Revenge":
                    if (!c.Target.StartsWith("dev-", StringComparison.Ordinal) || c.Target == owner || !await db.Clubs.AnyAsync(x => x.Id == c.Target && x.Defense != null)) throw new InvalidOperationException("Development target required");
                    var row = await Battles.Policies(db); var policy = Json.Read<PvpState>(row.State);
                    policy.Tickets.Add(new RevengeTicket { Origin = "dev-fixture-" + Guid.NewGuid().ToString("N"), Owner = owner, Target = c.Target, ActualLoss = 10, Expires = now + Pvp.Day });
                    row.State = Json.Write(policy); break;
                default: throw new InvalidOperationException("Unknown development tool");
            }
            return new { Applied = true, DevelopmentOnly = true };
        }, now);
    }
}
