using Airsoft.Club;
using Airsoft.Battle;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;
namespace Airsoft.Server;

public sealed record StartIntent(string Target, string Mode = "Practice", string Ticket = "");
public sealed record CapturedEconomy(EconomyConfig Config, int OpponentLevel, long AttackerPower, long DefenderPower);
public sealed class Battles(Store store)
{
    public static readonly Meter Meter = new("Airsoft.Club.Server", "006");
    static readonly Counter<long> Settlements = Meter.CreateCounter<long>("airsoft.battles.settled");
    public Task<string> Start(string owner, string key, long version, StartIntent intent, long now) =>
        store.Command(owner, key, Json.Write(intent), version, async (db, s) =>
        {
            Clubs.Available(s);
            if (intent.Target == owner || intent.Mode != "Practice") throw new InvalidOperationException("Invalid battle mode/target");
            var target = await db.Clubs.FindAsync(intent.Target) ?? throw new InvalidOperationException("Target unavailable");
            var defender = Json.Read<ClubState>(target.State);
            if (defender.ShieldUntil > now) throw new InvalidOperationException("Target protected");
            if (target.Defense == null) throw new InvalidOperationException("No defense roster");
            var offense = Clubs.Snapshot(s, false, now);
            if (offense.BbBudget < 1) throw new InvalidOperationException("Refill BB before attack");
            var defense = BattleWire.ReadTeam(target.Defense);
            var config = new MatchConfig(offense, defense, BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)), new BattleRules());
            string id = Guid.NewGuid().ToString("N"); s.PendingMatch = id;
            db.Matches.Add(new MatchRow
            {
                Id = id,
                Attacker = owner,
                Defender = intent.Target,
                Mode = intent.Mode,
                Input = BattleWire.WriteConfig(config),
                AcceptedAt = now,
                LeaseUntil = checked(now + 120000),
                Fence = 1,
                Economy = Json.Write(new CapturedEconomy(new(), defender.Level, Rewards.Power(offense), Rewards.Power(defense)))
            });
            return new { MatchId = id };
        }, now);
    public async Task Resolve(string id, long now)
    {
        MatchRow captured;
        await using (var db = store.Open()) { captured = await db.Matches.AsNoTracking().SingleAsync(m => m.Id == id); }
        if (captured.Status != "Pending") return;
        // Resolve outside transaction. Only this trusted worker computes results; client has no result endpoint.
        var result = new BattleEngine().Run(BattleWire.ReadConfig(captured.Input));
        await Settle(id, captured.Fence, result, now);
    }
    internal Task<bool> Settle(string id, int fence, MatchResult result, long now) => store.Transaction(async db =>
    {
        var row = await db.Matches.FindAsync(id) ?? throw new InvalidOperationException("Match absent");
        if (row.Status != "Pending" || row.Fence != fence) return false;
        var owner = await db.Clubs.FindAsync(row.Attacker) ?? throw new InvalidOperationException("Owner absent");
        var s = Json.Read<ClubState>(owner.State);
        if (s.PendingMatch != id) throw new InvalidOperationException("Offense fence mismatch");
        if (now >= row.LeaseUntil || result.Status != ResultStatus.Completed)
        {
            row.Status = "Failed"; row.Fence++; s.PendingMatch = null; s.Version++;
            await Store.Save(db, owner, s, now); return false;
        }
        var input = BattleWire.ReadConfig(row.Input); var economy = Json.Read<CapturedEconomy>(row.Economy);
        var reward = Rewards.Calculate(result.Outcome!.Value, economy.OpponentLevel, economy.AttackerPower, economy.DefenderPower, 0, false, economy.Config);
        s.Wallet.Apply("match:" + id, "battle:" + economy.Config.Version, reward.Money, 0); s.Xp += reward.ClubXp;
        foreach (var f in result.Attacker.Fighters)
        {
            var owned = Clubs.Owned(s, f.Id); owned.Hp = f.FinalHp.Raw; owned.Xp += reward.FighterXp;
            owned.RecoveryAt = now; owned.RecoveryRemainder = 0;
        }
        if (s.BbStock[s.ActiveBbTier] < result.Attacker.BbConsumed) throw new InvalidOperationException("Reserved BB mismatch");
        s.BbStock[s.ActiveBbTier] -= result.Attacker.BbConsumed;
        Clubs.Completed(s, id); s.PendingMatch = null; s.Version++;
        row.Result = BattleWire.WriteResult(result); row.Status = "Completed";
        await Store.Save(db, owner, s, now); Settlements.Add(1); return true;
    });
    public async Task Recover(long now)
    {
        await using var db = store.Open();
        var ids = await db.Matches.Where(m => m.Status == "Pending").Select(m => m.Id).Take(100).ToListAsync();
        foreach (var id in ids) await Resolve(id, now);
    }
}
public sealed class BattleWorker(Battles battles, ILogger<BattleWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await battles.Recover(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()); }
            catch (Exception e) { logger.LogError("Battle recovery failed: {ErrorType}", e.GetType().Name); }
            await Task.Delay(500, stoppingToken);
        }
    }
}
