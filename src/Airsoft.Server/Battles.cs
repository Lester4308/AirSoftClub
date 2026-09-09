using Airsoft.Club;
using Airsoft.Battle;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;
namespace Airsoft.Server;

public sealed record StartIntent(string Target, string Mode = "Practice", string Ticket = "");
public sealed record FighterAppearance(string Id, string Side, bool Head, bool Rig, bool Camo, string AppearanceId = "");
public sealed record CapturedEconomy(EconomyConfig Config, int OpponentLevel, long AttackerPower, long DefenderPower)
{
    public FighterAppearance[] Appearance { get; init; } = [];
}
public sealed record SettlementReceipt(int Schema, long At, int AttackerRatingDelta, int DefenderRatingDelta, Reward Rewards);
public sealed class Battles(Store store)
{
    public static readonly Meter Meter = new("Airsoft.Club.Server", "006");
    static readonly Counter<long> Settlements = Meter.CreateCounter<long>("airsoft.battles.settled");
    public Task<string> Start(string owner, string key, long version, StartIntent intent, long now) =>
        store.Command(owner, key, Json.Write(intent), version, async (db, s) =>
        {
            Clubs.Available(s);
            if (owner.StartsWith("dev-") && !intent.Target.StartsWith("dev-") || owner.StartsWith("steam-") && !intent.Target.StartsWith("steam-")) throw new InvalidOperationException("Opponent identity provider mismatch");
            if (intent.Target == owner) throw new InvalidOperationException("Invalid battle mode/target");
            var target = await db.Clubs.FindAsync(intent.Target) ?? throw new InvalidOperationException("Target unavailable");
            var defender = Json.Read<ClubState>(target.State);
            if (defender.ShieldUntil > now) throw new InvalidOperationException("Target protected");
            if (target.Defense == null) throw new InvalidOperationException("No defense roster");
            Clubs.AutoRefill(s, "auto-basic:" + key);
            var offense = Clubs.Snapshot(s, false, now);
            if (offense.BbBudget < 1) throw new InvalidOperationException("Refill BB before attack");
            var defense = BattleWire.ReadTeam(target.Defense);
            var config = new MatchConfig(offense, defense, BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)), new BattleRules());
            string id = Guid.NewGuid().ToString("N"); s.PendingMatch = id;
            var policyRow = await Policies(db); var policy = Json.Read<PvpState>(policyRow.State);
            var capture = Pvp.Accept(policy, s, defender, intent.Mode, id, now, intent.Ticket);
            policyRow.State = Json.Write(policy);
            db.Matches.Add(new MatchRow
            {
                Id = id,
                AttackerVersion = s.Version,
                DefenderVersion = target.Version,
                CatalogVersion = Catalog.Version,
                Attacker = owner,
                Defender = intent.Target,
                Mode = intent.Mode,
                Input = BattleWire.WriteConfig(config),
                AcceptedAt = now,
                LeaseUntil = checked(now + AlphaConfig.BattleLeaseMs),
                Fence = 1,
                Policy = Json.Write(capture),
                Economy = Json.Write(new CapturedEconomy(new(), defender.Level, Rewards.Power(offense), Rewards.Power(defense)) { Appearance = Appearance(s, offense, "A").Concat(Appearance(defender, defense, "D")).ToArray() })
            });
            return new { MatchId = id };
        }, now);
    static IEnumerable<FighterAppearance> Appearance(ClubState s, TeamSnapshot team, string side) =>
        team.Fighters.Select(f => { var owned = s.Fighters.Single(x => x.Id == f.Id); var gear = owned.Equipment; return new FighterAppearance(f.Id, side, gear.ContainsKey(Slot.HeadProtection), gear.ContainsKey(Slot.LoadBearingArmor), gear.ContainsKey(Slot.Camouflage), string.IsNullOrEmpty(owned.AppearanceId) ? Clubs.StableAppearance(owned.Id) : owned.AppearanceId); });
    public async Task Resolve(string id, long now)
    {
        MatchRow captured;
        await using (var db = store.Open()) { captured = await db.Matches.AsNoTracking().SingleAsync(m => m.Id == id); }
        if (captured.Status != "Pending") return;
        // Resolve outside transaction. Only this trusted worker computes results; client has no result endpoint.
        var result = new BattleEngine().Run(BattleWire.ReadConfig(captured.Input));
        if (await Settle(id, captured.Fence, result, now)) Settlements.Add(1);
    }
    internal Task<bool> Settle(string id, int fence, MatchResult result, long now) => store.Transaction(async db =>
    {
        var row = await db.Matches.FindAsync(id) ?? throw new InvalidOperationException("Match absent");
        if (row.Status != "Pending" || row.Fence != fence) return false;
        var owner = await db.Clubs.FindAsync(row.Attacker) ?? throw new InvalidOperationException("Owner absent");
        var s = Json.Read<ClubState>(owner.State);
        var policyRow = await Policies(db); var policy = Json.Read<PvpState>(policyRow.State);
        var capture = Json.Read<PvpCapture>(row.Policy);
        if (s.PendingMatch != id) throw new InvalidOperationException("Offense fence mismatch");
        if (now >= row.LeaseUntil || result.Status != ResultStatus.Completed)
        {
            policy.Exposures.RemoveAll(e => e.Match == id && !e.Committed); policyRow.State = Json.Write(policy);
            row.Status = "Failed"; row.Fence++; s.PendingMatch = null; s.Version++;
            await Store.Save(db, owner, s, now); return false;
        }
        var input = BattleWire.ReadConfig(row.Input); var economy = Json.Read<CapturedEconomy>(row.Economy);
        var reward = Rewards.Calculate(result.Outcome!.Value, economy.OpponentLevel, economy.AttackerPower, economy.DefenderPower, capture.FriendWins, capture.FriendBudget, economy.Config);
        var defenderRow = (await db.Clubs.FindAsync(row.Defender))!;
        var defender = Json.Read<ClubState>(defenderRow.State); int oldRating = defender.Rating; int oldAttackerRating = s.Rating;
        Pvp.Finish(policy, s, defender, id, capture, result.Outcome, now); policyRow.State = Json.Write(policy);
        if (defender.Rating != oldRating) { defender.Version++; await Store.Save(db, defenderRow, defender, now); }
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
        row.Settlement = Json.Write(new SettlementReceipt(1, now, s.Rating - oldAttackerRating, defender.Rating - oldRating, reward));
        await Store.Save(db, owner, s, now); return true;
    });
    public static async Task<PolicyRow> Policies(ClubDb db)
    {
        var row = await db.Policies.FindAsync(1);
        if (row == null) { row = new PolicyRow { Id = 1, State = Json.Write(new PvpState()) }; db.Policies.Add(row); }
        return row;
    }
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
