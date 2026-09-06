namespace Airsoft.Server;

public interface IPlatformIdentity
{
    Task<SteamIdentity> Verify(string ticket, CancellationToken ct = default);
    Task<string[]> Friends(string steamId, CancellationToken ct = default);
}
public interface IIdentitySessions
{
    Task<string?> Resolve(string token, long now);
}
public sealed class IdentityResolver(DevelopmentSessions development, SteamSessions steam)
{
    public async Task<string?> Resolve(string token, long now, bool developmentEnabled)
    {
        var owner = developmentEnabled ? development.Resolve(token, now) : null;
        return owner ?? await steam.Resolve(token, now);
    }
}
public sealed record SocialProfile(string PlatformId, string DisplayName, string AvatarUrl);
public interface ISocialProfiles
{
    Task<IReadOnlyList<SocialProfile>> Lookup(IReadOnlyList<string> platformIds, CancellationToken ct = default);
}
public interface ICommerceProvider
{
    Task<string> Status(string orderId, CancellationToken ct = default);
}
// Explicit fixture transport. Never registered by the production server.
public sealed class DevelopmentCommerceProvider(IReadOnlyDictionary<string, string> states) : ICommerceProvider
{
    public Task<string> Status(string orderId, CancellationToken ct = default) => Task.FromResult(states.TryGetValue(orderId, out var state) ? state : "Unknown");
}
