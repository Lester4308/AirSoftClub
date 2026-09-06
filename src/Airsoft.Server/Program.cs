using Airsoft.Server;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration["AIRSOFT_CONNECTION"] ?? throw new InvalidOperationException("Set AIRSOFT_CONNECTION");
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddSingleton(new Store(connection));
builder.Services.AddSingleton<DevelopmentSessions>();
builder.Services.AddSingleton(new SteamGateway(new HttpClient { Timeout = TimeSpan.FromSeconds(10) }, builder.Configuration["STEAM_PUBLISHER_KEY"] ?? "", uint.TryParse(builder.Configuration["STEAM_APP_ID"], out var appId) ? appId : 0));
builder.Services.AddSingleton<IPlatformIdentity>(sp => sp.GetRequiredService<SteamGateway>());
builder.Services.AddSingleton<SteamSessions>();
builder.Services.AddSingleton<IdentityResolver>();
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 16384);
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http => RateLimitPartition.GetFixedWindowLimiter(http.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions { PermitLimit = 240, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
builder.Services.AddSingleton<Battles>();
builder.Services.AddHostedService<BattleWorker>();
builder.Logging.AddJsonConsole();
var app = builder.Build();
if (args.Contains("--migrate")) { await using var db = app.Services.GetRequiredService<Store>().Open(); await db.Database.MigrateAsync(); return; }
app.UseRateLimiter();
app.Use(async (http, next) =>
{
    try
    {
        if (http.Request.Path.StartsWithSegments("/api"))
        {
            var token = http.Request.Headers.Authorization.ToString();
            var owner = token.StartsWith("Bearer ") ? await app.Services.GetRequiredService<IdentityResolver>().Resolve(token[7..], Api.Now, app.Environment.IsDevelopment() && app.Configuration["AIRSOFT_DEV_AUTH"] == "1") : null;
            if (owner == null) { http.Response.StatusCode = 401; return; }
            http.Items["owner"] = owner;
        }
        await next(http);
    }
    catch (InvalidOperationException e) { http.Response.StatusCode = 409; await http.Response.WriteAsJsonAsync(new { Error = e.Message }); }
    catch (HttpRequestException) { http.Response.StatusCode = 503; await http.Response.WriteAsJsonAsync(new { Error = "Platform temporarily unavailable" }); }
    catch (ArgumentException) { http.Response.StatusCode = 400; await http.Response.WriteAsJsonAsync(new { Error = "Invalid request" }); }
});
app.MapGet("/health", () => Results.Ok(new { status = "alive" }));
app.MapGet("/ready", async (Store store) => { await using var db = store.Open(); return await db.Database.CanConnectAsync() && !(await db.Database.GetPendingMigrationsAsync()).Any() ? Results.Ok() : Results.StatusCode(503); });
Api.Map(app);
DevelopmentTools.Map(app);
app.MapPost("/steam/login", async (SteamLogin intent, SteamSessions sessions) => Results.Json(new { Token = await sessions.Login(intent.Ticket, Api.Now) }));
if (app.Environment.IsDevelopment() && app.Configuration["AIRSOFT_DEV_AUTH"] == "1") await Api.SeedDevelopment(app.Services.GetRequiredService<Store>());
app.Run();

public sealed record SteamLogin(string Ticket);
