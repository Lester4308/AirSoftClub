using Airsoft.Club;
using Airsoft.Battle;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Airsoft.Server;

public sealed record CommandIntent(string Key, long Version, string Type, string Target = "", string Value = "", int Number = 0, bool Flag = false, int OfferVersion = 0, string CatalogVersion = Catalog.Version, string Ticket = "");
public sealed record LoginIntent(string Account);
public sealed class DevelopmentSessions : IIdentitySessions
{
    readonly ConcurrentDictionary<string, (string Owner, long Until)> sessions = new();
    public string Issue(string owner, long now) { var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); sessions[token] = (owner, now + 3600000); return token; }
    Task<string?> IIdentitySessions.Resolve(string token, long now) => Task.FromResult(Resolve(token, now));
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
            .ToListAsync();
        var policyRow = await db.Policies.FindAsync(1);
        var policy = policyRow == null ? new PvpState() : Json.Read<PvpState>(policyRow.State);
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
            s.AutoBuyBasic,
            s.Emblem,
            s.Retention.Streak,
            s.Retention.LastDay,
            s.Unlocks,
            ContractVersion = 2,
            ConfigVersion = AlphaConfig.Version,
            TrainingMoney = AlphaConfig.TrainingMoney,
            ConvertRate = new EconomyConfig().CreditsToMoney,
            RefreshMoney = AlphaConfig.RecruitRefreshMoney,
            RefreshAvailableAt = s.OffersAt + AlphaConfig.RecruitRefreshMs,
            EmergencyAvailableAt = s.EmergencyAt + AlphaConfig.EmergencyCooldownMs,
            DefensePublished = row.Defense != null,
            DefenseVersion = row.Version,
            BbCatalog = Enumerable.Range(0, 5).Select(t => new { Tier = t, Name = AlphaConfig.BbNames[t], Money = AlphaConfig.BbMoney(t), Credits = AlphaConfig.BbCredits(t), Amount = AlphaConfig.BbRefill }),
            Shields = new[] { 8, 24, 72, 168 }.Select(h => new { Hours = h, Credits = AlphaConfig.ShieldCredits(h) }),
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
                f.RecoveryAt,
                f.RecoveryRemainder,
                HealMoney = (f.MaxHp - f.Hp + Fixed.Scale - 1) / Fixed.Scale,
                RecoveryRemainingMs = (f.MaxHp - f.Hp) * AlphaConfig.FullRecoveryMs / f.MaxHp,
                Equipment = f.Equipment.Select(e => new { Slot = e.Key.ToString(), Item = e.Value, Definition = s.Items[e.Value] })
            }),
            Items = s.Items.Select(i => new { Id = i.Key, Definition = i.Value, Slot = Catalog.Get(i.Value).Slot.ToString(), Equipped = s.Fighters.Any(f => f.Equipment.Values.Contains(i.Key)) }),
            Catalog = Catalog.Items.Select(i => new { i.Id, Slot = i.Slot.ToString(), i.Mk, i.Money, i.Credits, i.Level, Access = i.Level <= s.Level || s.Unlocks.Contains(i.Id), i.Damage, i.Interval, i.Projectiles, i.Protection, i.AgilityPenalty, EarlyAllowed = EarlyAccess.CanUnlock(s, i), EarlyPrice = EarlyAccess.CanUnlock(s, i) ? EarlyAccess.Price(i.Level - s.Level) : 0, EarlyCapped = i.Level > s.Level }),
            RevengeTickets = policy.Tickets.Where(t => t.Owner == owner && !t.Consumed && t.Attempts < 3 && t.Expires > now),
            History = history.Select(m => new
            {
                m.Id,
                m.Status,
                m.Mode,
                m.Attacker,
                m.Defender,
                m.AcceptedAt,
                m.AttackerVersion,
                m.DefenderVersion,
                Outcome = m.Result == null ? "" : BattleWire.ReadResult(m.Result).Outcome.ToString(),
                RatingKnown = m.Settlement.Length > 0,
                RatingDelta = m.Settlement.Length == 0 ? 0 : owner == m.Attacker ? Json.Read<SettlementReceipt>(m.Settlement).AttackerRatingDelta : Json.Read<SettlementReceipt>(m.Settlement).DefenderRatingDelta,
                Money = m.Settlement.Length == 0 || owner != m.Attacker ? 0 : Json.Read<SettlementReceipt>(m.Settlement).Rewards.Money
            }),
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
            var self = (await db.Clubs.FindAsync(owner))!; string prefix = owner.StartsWith("steam-") ? "steam-" : "dev-";
            var rows = await db.Clubs.Where(c => c.Id != owner && c.Defense != null && c.Id.StartsWith(prefix)).OrderBy(c => Math.Abs(c.Rating - self.Rating)).ThenBy(c => c.Id).Take(5).ToListAsync();
            long ownPower = self.Defense == null ? 1 : Rewards.Power(BattleWire.ReadTeam(self.Defense));
            return Results.Json(new { Opponents = rows.Select(r => { var s = Json.Read<ClubState>(r.State); return new { s.Id, s.Name, s.Level, s.Rating, Fighters = s.Fighters.Count(f => f.Active), Category = Rewards.Category(ownPower, Rewards.Power(BattleWire.ReadTeam(r.Defense!))), Protected = s.ShieldUntil > Now }; }) });
        });
        app.MapGet("/api/leaderboard", async (HttpContext http, Store store) =>
        {
            string prefix = Owner(http).StartsWith("steam-") ? "steam-" : "dev-"; await using var db = store.Open();
            var rows = await db.Clubs.Where(c => c.Id.StartsWith(prefix) && c.Defense != null).OrderByDescending(c => c.Rating).ThenBy(c => c.Id).Take(20).ToListAsync();
            return Results.Json(new { Leaders = rows.Select(c => new { c.Id, Name = Json.Read<ClubState>(c.State).Name, c.Rating }) });
        });
        app.MapPost("/api/command", async (CommandIntent c, HttpContext http, Store store, Battles battles) =>
        {
            if (string.IsNullOrWhiteSpace(c.Type) || c.Target == null || c.Value == null || c.Ticket == null || c.CatalogVersion == null) throw new ArgumentException("Invalid command fields");
            string owner = Owner(http); long now = Now;
            if (c.Type == "Attack" && c.Value == "Friend" && owner.StartsWith("steam-"))
            {
                var friends = await app.Services.GetRequiredService<IPlatformIdentity>().Friends(owner[6..]);
                if (!c.Target.StartsWith("steam-") || !friends.Contains(c.Target[6..])) throw new InvalidOperationException("Verified Steam friendship required");
            }
            if (c.Type == "Attack") return Results.Content(await battles.Start(owner, c.Key, c.Version, new StartIntent(c.Target, c.Value, c.Ticket), now), "application/json");
            string json = await store.Command(owner, c.Key, Json.Write(c), c.Version, async (db, s) =>
            {
                Clubs.Available(s);
                if (c.CatalogVersion != Catalog.Version) throw new InvalidOperationException("Stale policy/catalog quote");
                switch (c.Type)
                {
                    case "Hire": Clubs.Hire(s, c.Target, c.OfferVersion, c.Flag, now, c.Key); break;
                    case "Refresh": Clubs.Refresh(s, now, BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)), c.Flag, c.Key); break;
                    case "Train": Clubs.Train(s, c.Target, c.Value, now, c.Key); break;
                    case "Heal": Clubs.HealAmount(s, c.Target, c.Number, now, c.Key); break;
                    case "Dismiss": Clubs.Dismiss(s, c.Target, c.Key); break;
                    case "Buy": if (c.CatalogVersion != Catalog.Version) throw new InvalidOperationException("Stale catalog quote"); Clubs.Buy(s, c.Target, c.Key); break;
                    case "Equip": Clubs.Equip(s, c.Target, c.Value); break;
                    case "Unequip": if (!Enum.TryParse<Slot>(c.Value, out var slot)) throw new InvalidOperationException("Unknown slot"); Clubs.Owned(s, c.Target).Equipment.Remove(slot); break;
                    case "Refill": Clubs.Refill(s, c.Number, c.Key); break;
                    case "BbTier": _ = Catalog.Bb(c.Number); s.ActiveBbTier = c.Number; break;
                    case "AutoBuyBasic": if (c.Flag && s.Level < AlphaConfig.AutoBuyLevel) throw new InvalidOperationException("Auto-buy unlocks at Club Level3"); s.AutoBuyBasic = c.Flag; break;
                    case "Emergency": Clubs.Emergency(s, now); break;
                    case "Convert": s.Wallet.Convert(c.Key, c.Number, new()); break;
                    case "Daily": Retention.Daily(s, now); break;
                    case "Progression": Retention.Progression(s); break;
                    case "EarlyUnlock": Retention.EarlyUnlock(s, c.Target, c.Key); break;
                    case "Name": Moderation.Name(s, c.Value); break;
                    case "Emblem": if (c.Number < 0 || c.Number > 7) throw new InvalidOperationException("Unknown emblem"); s.Emblem = c.Number; break;
                    case "Report":
                        if (!await db.Clubs.AnyAsync(x => x.Id == c.Target)) throw new InvalidOperationException("Target missing");
                        var policyRow = await Battles.Policies(db); var policy = Json.Read<PvpState>(policyRow.State);
                        Moderation.Report(policy.Moderation, owner, c.Target, c.Value, now); policyRow.State = Json.Write(policy); break;
                    case "Shield": Pvp.Shield(s, c.Number, now, c.Key); break;
                    default: throw new InvalidOperationException("Unknown intent");
                }
                return new { Applied = true };
            }, now);
            return Results.Content(json, "application/json");
        });
        app.MapGet("/api/match/{id}", async (string id, HttpContext http, Store store) =>
        {
            string owner = Owner(http); await using var db = store.Open(); var m = await db.Matches.FindAsync(id);
            if (m == null || m.Attacker != owner && m.Defender != owner) return Results.NotFound();
            var reward = new Reward(0, 0, 0);
            if (m.Result != null && owner == m.Attacker)
            {
                var capture = Json.Read<PvpCapture>(m.Policy); var economy = Json.Read<CapturedEconomy>(m.Economy);
                reward = Rewards.Calculate(BattleWire.ReadResult(m.Result).Outcome!.Value, economy.OpponentLevel, economy.AttackerPower, economy.DefenderPower, capture.FriendWins, capture.FriendBudget, economy.Config);
            }
            return Results.Json(new
            {
                RatingKnown = m.Settlement.Length > 0,
                RatingDelta = m.Settlement.Length == 0 ? (int?)null : owner == m.Attacker ? Json.Read<SettlementReceipt>(m.Settlement).AttackerRatingDelta : Json.Read<SettlementReceipt>(m.Settlement).DefenderRatingDelta,
                m.AcceptedAt,
                m.AttackerVersion,
                m.DefenderVersion,
                m.CatalogVersion,
                RevengeOrigin = Json.Read<PvpCapture>(m.Policy).Ticket,
                RewardMoney = reward.Money,
                RewardClubXp = reward.ClubXp,
                RewardFighterXp = reward.FighterXp,
                m.Id,
                m.Status,
                Appearance = Json.Read<CapturedEconomy>(m.Economy).Appearance,
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
