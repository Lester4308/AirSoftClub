using Airsoft.Club;
using Airsoft.Battle;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Airsoft.Server;

public sealed record CommandIntent(string Key, long Version, string Type, string Target = "", string Value = "", int Number = 0, bool Flag = false, int OfferVersion = 0, string CatalogVersion = Catalog.Version);
public sealed record LoginIntent(string Account);
public sealed class DevelopmentSessions
{
    readonly ConcurrentDictionary<string, (string Owner, long Until)> sessions = new();
    public string Issue(string owner, long now) { var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); sessions[token] = (owner, now + 3600000); return token; }
    public string? Resolve(string token, long now) => sessions.TryGetValue(token, out var s) && now < s.Until ? s.Owner : null;
}
public static class Api
{
    public static long Now => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public static async Task<object> View(Store store, string owner)
    {
        await using var db = store.Open(); var row = await db.Clubs.FindAsync(owner) ?? throw new InvalidOperationException("Account missing");
        var s = Json.Read<ClubState>(row.State); long now = Now;
        // Read projection materializes recovery without persisting/versioning a read.
        if (s.PendingMatch == null) foreach (var f in s.Fighters.Where(f => f.Active)) f.Recover(Math.Max(now, f.RecoveryAt));
        var history = await db.Matches.Where(m => m.Attacker == owner || m.Defender == owner).OrderByDescending(m => m.AcceptedAt).Take(20)
            .Select(m => new { m.Id, m.Status, m.Mode, m.Attacker, m.Defender }).ToListAsync();
        return new
        {
            s.Id,
            s.Name,
            s.Version,
            s.Wallet.Money,
            s.Wallet.Credits,
            s.Xp,
            s.Level,
            s.Rating,
            s.FreeRecruitClaimed,
            s.Capacity,
            s.BbStock,
            s.ActiveBbTier,
            s.OfferVersion,
            s.Offers,
            s.OffersAt,
            s.CompletedSinceRefresh,
            s.PendingMatch,
            s.ShieldUntil,
            Fighters = s.Fighters.Where(f => f.Active).Select(f => new
            {
                f.Id,
                f.Name,
                f.Accuracy,
                f.Endurance,
                f.Agility,
                f.Hp,
                f.MaxHp,
                f.Ready,
                f.Xp,
                f.Level,
                f.TrainingCap,
                Equipment = f.Equipment.Select(e => new { Slot = e.Key.ToString(), Item = e.Value, Definition = s.Items[e.Value] })
            }),
            Items = s.Items.Select(i => new { Id = i.Key, Definition = i.Value, Slot = Catalog.Get(i.Value).Slot.ToString(), Equipped = s.Fighters.Any(f => f.Equipment.Values.Contains(i.Key)) }),
            Catalog = Catalog.Items.Select(i => new { i.Id, Slot = i.Slot.ToString(), i.Mk, i.Money, i.Credits, i.Level }),
            History = history,
            ServerNow = now,
            CatalogVersion = Catalog.Version
        };
    }
    public static void Map(WebApplication app)
    {
        app.MapPost("/dev/login", async (LoginIntent intent, HttpContext http, Store store, DevelopmentSessions sessions) =>
        {
            if (!app.Environment.IsDevelopment() || app.Configuration["AIRSOFT_DEV_AUTH"] != "1" || http.Connection.RemoteIpAddress == null || !System.Net.IPAddress.IsLoopback(http.Connection.RemoteIpAddress)) return Results.Unauthorized();
            if (!Regex.IsMatch(intent.Account, "^dev-[a-z0-9-]{1,40}$")) return Results.BadRequest(new { Error = "Use dev- followed by lowercase letters/numbers" });
            await store.Create(intent.Account, Now); return Results.Json(new { Token = sessions.Issue(intent.Account, Now), DevelopmentOnly = true });
        });
        app.MapGet("/api/club", async (HttpContext http, Store store) => Results.Json(await View(store, Owner(http))));
        app.MapGet("/api/opponents", async (HttpContext http, Store store) =>
        {
            string owner = Owner(http); await using var db = store.Open();
            var rows = await db.Clubs.Where(c => c.Id != owner && c.Defense != null && c.Id.StartsWith("dev-bot-")).Take(5).ToListAsync();
            return Results.Json(new { Opponents = rows.Select(r => { var s = Json.Read<ClubState>(r.State); return new { s.Id, s.Name, s.Level, s.Rating, Fighters = s.Fighters.Count(f => f.Active), Category = "Development rival", Protected = s.ShieldUntil > Now }; }) });
        });
        app.MapPost("/api/command", async (CommandIntent c, HttpContext http, Store store, Battles battles) =>
        {
            string owner = Owner(http); long now = Now;
            if (c.Type == "Attack" && c.Value == "Friend" && owner.StartsWith("steam-"))
            {
                var friends = await app.Services.GetRequiredService<SteamGateway>().Friends(owner[6..]);
                if (!c.Target.StartsWith("steam-") || !friends.Contains(c.Target[6..])) throw new InvalidOperationException("Verified Steam friendship required");
            }
            if (c.Type == "Attack") return Results.Content(await battles.Start(owner, c.Key, c.Version, new StartIntent(c.Target, c.Value, ""), now), "application/json");
            string json = await store.Command(owner, c.Key, Json.Write(c), c.Version, (db, s) =>
            {
                Clubs.Available(s);
                switch (c.Type)
                {
                    case "Hire": Clubs.Hire(s, c.Target, c.OfferVersion, c.Flag, now, c.Key); break;
                    case "Refresh": Clubs.Refresh(s, now, BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)), c.Flag, c.Key); break;
                    case "Train": Clubs.Train(s, c.Target, c.Value, now, c.Key); break;
                    case "Heal": Clubs.Heal(s, c.Target, now, c.Key); break;
                    case "Dismiss": Clubs.Dismiss(s, c.Target, c.Key); break;
                    case "Buy": if (c.CatalogVersion != Catalog.Version) throw new InvalidOperationException("Stale catalog quote"); Clubs.Buy(s, c.Target, c.Key); break;
                    case "Equip": Clubs.Equip(s, c.Target, c.Value); break;
                    case "Unequip": if (!Enum.TryParse<Slot>(c.Value, out var slot)) throw new InvalidOperationException("Unknown slot"); Clubs.Owned(s, c.Target).Equipment.Remove(slot); break;
                    case "Refill": Clubs.Refill(s, c.Number, c.Key); break;
                    case "BbTier": _ = Catalog.Bb(c.Number); s.ActiveBbTier = c.Number; break;
                    case "Emergency": Clubs.Emergency(s, now); break;
                    case "Convert": s.Wallet.Convert(c.Key, c.Number, new()); break;
                    case "Shield": Pvp.Shield(s, c.Number, now, c.Key); break;
                    default: throw new InvalidOperationException("Unknown intent");
                }
                return Task.FromResult<object>(new { Applied = true });
            }, now);
            return Results.Content(json, "application/json");
        });
        app.MapGet("/api/match/{id}", async (string id, HttpContext http, Store store) =>
        {
            string owner = Owner(http); await using var db = store.Open(); var m = await db.Matches.FindAsync(id);
            if (m == null || m.Attacker != owner && m.Defender != owner) return Results.NotFound();
            return Results.Json(new
            {
                m.Id,
                m.Status,
                Input = m.Result == null ? "" : Convert.ToBase64String(m.Input),
                Result = m.Result == null ? "" : Convert.ToBase64String(m.Result),
                Digest = m.Result == null ? "" : BattleWire.Digest(BattleWire.ReadResult(m.Result))
            });
        });
    }
    public static string Owner(HttpContext http) => (string?)http.Items["owner"] ?? throw new UnauthorizedAccessException();
    public static async Task SeedDevelopment(Store store)
    {
        for (int n = 1; n <= 5; n++)
        {
            string id = "dev-bot-" + n; await store.Create(id, Now);
            await store.Command(id, "fixture-v1", "fixture-v1", 0, (db, s) =>
            {
                s.Name = "Training club " + n;
                var f = Clubs.Hire(s, s.Offers[0].Id, 1, true, Now, "free");
                Clubs.Equip(s, f.Id, Clubs.Buy(s, "Pistol-MK1", "weapon"));
                return Task.FromResult<object>(true);
            }, Now);
        }
    }
}
