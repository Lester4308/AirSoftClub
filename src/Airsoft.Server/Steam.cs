using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
namespace Airsoft.Server;

public sealed record SteamIdentity(string SteamId, string OwnerId);
public sealed class SteamGateway(HttpClient http, string key, uint appId)
{
    public bool Configured => key.Length > 0 && appId > 0;
    async Task<JsonDocument> Get(string method, string query, CancellationToken ct)
    {
        if (!Configured) throw new InvalidOperationException("Steam app/publisher credentials unavailable");
        // Dedicated HttpClient: do not attach request-URL logging; Steam requires publisher key in query.
        using var response = await http.GetAsync("https://partner.steam-api.com/" + method + "?key=" + Uri.EscapeDataString(key) + query, ct);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("Steam verification unavailable");
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
    }
    public async Task<SteamIdentity> Verify(string ticket, CancellationToken ct = default)
    {
        if (!Regex.IsMatch(ticket, "^[0-9a-fA-F]{2,8192}$") || ticket.Length % 2 != 0) throw new InvalidOperationException("Malformed Steam ticket");
        using var auth = await Get("ISteamUserAuth/AuthenticateUserTicket/v1/", "&appid=" + appId + "&ticket=" + ticket + "&identity=airsoft-club-server", ct);
        if (!auth.RootElement.TryGetProperty("response", out var response) || !response.TryGetProperty("params", out var p) || !p.TryGetProperty("result", out var result) || result.GetString() != "OK") throw new InvalidOperationException("Steam ticket rejected");
        string id = p.GetProperty("steamid").GetString() ?? "", owner = p.GetProperty("ownersteamid").GetString() ?? "";
        if (!Regex.IsMatch(id, "^[0-9]{17}$") || id != owner || p.TryGetProperty("publisherbanned", out var ban) && ban.GetBoolean()) throw new InvalidOperationException("Steam owner/access rejected");
        using var ownership = await Get("ISteamUser/CheckAppOwnership/v4/", "&appid=" + appId + "&steamid=" + id, ct);
        if (!ownership.RootElement.GetProperty("appownership").GetProperty("ownsapp").GetBoolean()) throw new InvalidOperationException("Game ownership required");
        return new(id, owner);
    }
    public async Task<string[]> Friends(string steamId, CancellationToken ct = default)
    {
        if (!Regex.IsMatch(steamId, "^[0-9]{17}$")) throw new InvalidOperationException("Invalid Steam identity");
        using var data = await Get("ISteamUser/GetFriendList/v1/", "&steamid=" + steamId + "&relationship=friend", ct);
        if (!data.RootElement.TryGetProperty("friendslist", out var list)) return Array.Empty<string>();
        return list.GetProperty("friends").EnumerateArray().Select(x => x.GetProperty("steamid").GetString()!).Where(x => Regex.IsMatch(x, "^[0-9]{17}$")).ToArray();
    }
}
public sealed class SessionRow
{
    public string TokenHash { get; set; } = "";
    public string TicketHash { get; set; } = "";
    public string Owner { get; set; } = "";
    public long Until { get; set; }
}
public sealed class SteamSessions(Store store, SteamGateway gateway)
{
    public async Task<string> Login(string ticket, long now)
    {
        string ticketHash = Json.Hash(ticket.ToUpperInvariant());
        await using (var db = store.Open()) if (await db.Set<SessionRow>().AnyAsync(x => x.TicketHash == ticketHash)) throw new InvalidOperationException("Ticket already exchanged; request a fresh Steam ticket");
        var identity = await gateway.Verify(ticket); string owner = "steam-" + identity.SteamId;
        await store.Create(owner, now); string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        return await store.Transaction(async db =>
        {
            if (await db.Set<SessionRow>().AnyAsync(x => x.TicketHash == ticketHash)) throw new InvalidOperationException("Ticket replay rejected");
            db.Set<SessionRow>().Add(new SessionRow { TokenHash = Json.Hash(token), TicketHash = ticketHash, Owner = owner, Until = now + 3600000 });
            return token;
        });
    }
    public async Task<string?> Resolve(string token, long now)
    { await using var db = store.Open(); var row = await db.Set<SessionRow>().FindAsync(Json.Hash(token)); return row != null && now < row.Until ? row.Owner : null; }
}
