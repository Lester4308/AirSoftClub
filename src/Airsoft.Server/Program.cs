using Airsoft.Server;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration["AIRSOFT_CONNECTION"] ?? throw new InvalidOperationException("Set AIRSOFT_CONNECTION");
builder.Services.AddSingleton(new Store(connection));
var app = builder.Build();
if (args.Contains("--migrate")) { await using var db = app.Services.GetRequiredService<Store>().Open(); await db.Database.MigrateAsync(); return; }
app.MapGet("/health", () => Results.Ok(new { status = "alive" }));
app.MapGet("/ready", async (Store store) => { await using var db = store.Open(); return await db.Database.CanConnectAsync() ? Results.Ok() : Results.StatusCode(503); });
app.Run();
