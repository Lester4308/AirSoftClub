using Airsoft.Server;
using System.Net;
using Microsoft.EntityFrameworkCore;
internal static partial class Program
{
    sealed class SteamFixture(Func<Uri, string> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(respond(request.RequestUri!)) });
    }
    static SteamGateway Gateway(string id = "76561198000000001", bool owns = true, bool valid = true, string? owner = null, string? friends = null, Action<Uri>? inspect = null) => new(new HttpClient(new SteamFixture(uri =>
    {
        inspect?.Invoke(uri);
        if (uri.AbsolutePath.Contains("Authenticate")) return "{\"response\":{\"params\":{\"result\":\"" + (valid ? "OK" : "Fail") + "\",\"steamid\":\"" + id + "\",\"ownersteamid\":\"" + (owner ?? id) + "\"}}}";
        if (uri.AbsolutePath.Contains("CheckAppOwnership")) return "{\"appownership\":{\"ownsapp\":" + (owns ? "true" : "false") + "}}";
        if (uri.AbsolutePath.Contains("GetFriendList")) return friends ?? "{}";
        throw new InvalidOperationException("Unexpected Steam fixture request");
    })), "fixture-not-a-key", 123);
    static async Task SteamTests()
    {
        await Test("Steam adapter rejects invalid expired wrong-owner nonownership", async () =>
        {
            await Reject(() => Gateway().Verify("not-hex")); await Reject(() => Gateway(valid: false).Verify("abcd"));
            await Reject(() => Gateway(owns: false).Verify("abcd")); await Reject(() => Gateway(owner: "76561198000000002").Verify("abcd"));
            Check((await Gateway().Verify("abcd")).SteamId == "76561198000000001");
        });
        await Test("Steam Friends uses server relationship and filters identities", async () =>
        {
            var requests = new List<Uri>();
            var friends = await Gateway(
                friends: "{\"friendslist\":{\"friends\":[{\"steamid\":\"76561198000000002\"},{\"steamid\":\"invalid\"}]}}",
                inspect: requests.Add).Friends("76561198000000001");
            Check(friends.SequenceEqual(["76561198000000002"]));
            Check(requests.Count == 1 && requests[0].AbsolutePath.Contains("GetFriendList") && requests[0].Query.Contains("relationship=friend") && requests[0].Query.Contains("steamid=76561198000000001"));
            Check((await Gateway(friends: "{}").Friends("76561198000000001")).Length == 0);
            await Reject(() => Gateway().Friends("dev-spoof"));
        });
        await Test("Steam exchange persists replay guard and expires session", async () =>
        {
            string ticket = Guid.NewGuid().ToString("N"); var sessions = new SteamSessions(store, Gateway()); var token = await sessions.Login(ticket, 100);
            Check(await sessions.Resolve(token, 101) == "steam-76561198000000001"); Check(await sessions.Resolve(token, 3600100) == null);
            await Reject(() => new SteamSessions(store, Gateway()).Login(ticket, 102)); Check(await sessions.Resolve("spoof", 102) == null);
        });
        await Test("development session lifetime", () =>
        {
            var s = new DevelopmentSessions(); string token = s.Issue("dev-test", 0); Check(s.Resolve(token, 3599999) == "dev-test" && s.Resolve(token, 3600000) == null); return Task.CompletedTask;
        });
    }
}
