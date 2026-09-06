using Airsoft.Server;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration["AIRSOFT_CONNECTION"] ?? throw new InvalidOperationException("Set AIRSOFT_CONNECTION");
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddSingleton(new Store(connection));
builder.Services.AddSingleton<DevelopmentSessions>();
builder.Services.AddSingleton<Battles>();
builder.Services.AddHostedService<BattleWorker>();
builder.Logging.AddJsonConsole();
var app = builder.Build();
if (args.Contains("--migrate")) { await using var db = app.Services.GetRequiredService<Store>().Open(); await db.Database.MigrateAsync(); return; }
app.Use(async (http, next) =>
{
    try
    {
        if (http.Request.Path.StartsWithSegments("/api"))
        {
            var token = http.Request.Headers.Authorization.ToString();
            var owner = app.Environment.IsDevelopment() && app.Configuration["AIRSOFT_DEV_AUTH"] == "1" && token.StartsWith("Bearer ")
                ? app.Services.GetRequiredService<DevelopmentSessions>().Resolve(token[7..], Api.Now) : null;
            if (owner == null) { http.Response.StatusCode = 401; return; }
            http.Items["owner"] = owner;
        }
        await next(http);
    }
    catch (InvalidOperationException e) { http.Response.StatusCode = 409; await http.Response.WriteAsJsonAsync(new { Error = e.Message }); }
    catch (ArgumentException) { http.Response.StatusCode = 400; await http.Response.WriteAsJsonAsync(new { Error = "Invalid request" }); }
});
app.MapGet("/health", () => Results.Ok(new { status = "alive" }));
app.MapGet("/ready", async (Store store) => { await using var db = store.Open(); return await db.Database.CanConnectAsync() ? Results.Ok() : Results.StatusCode(503); });
Api.Map(app);
if (app.Environment.IsDevelopment() && app.Configuration["AIRSOFT_DEV_AUTH"] == "1") await Api.SeedDevelopment(app.Services.GetRequiredService<Store>());
app.Run();
