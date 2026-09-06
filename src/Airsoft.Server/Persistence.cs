using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Airsoft.Server;

public sealed class ClubRow
{
    public string Id { get; set; } = "";
    public long Version { get; set; }
    public long Money { get; set; }
    public long Credits { get; set; }
    public int Rating { get; set; }
    public string State { get; set; } = "{}";
    public byte[]? Defense { get; set; }
}
public sealed class OperationRow
{
    public string Owner { get; set; } = "";
    public string Key { get; set; } = "";
    public string Hash { get; set; } = "";
    public string Response { get; set; } = "{}";
}
public sealed class LedgerRow
{
    public string Owner { get; set; } = "";
    public string Operation { get; set; } = "";
    public string Reason { get; set; } = "";
    public long Money { get; set; }
    public long Credits { get; set; }
}
public sealed class MatchRow
{
    public string Id { get; set; } = "";
    public string Attacker { get; set; } = "";
    public string Defender { get; set; } = "";
    public string Mode { get; set; } = "";
    public byte[] Input { get; set; } = Array.Empty<byte>();
    public byte[]? Result { get; set; }
    public string Status { get; set; } = "Pending";
    public long AcceptedAt { get; set; }
    public long LeaseUntil { get; set; }
    public int Fence { get; set; }
    public string Policy { get; set; } = "{}";
    public string Economy { get; set; } = "{}";
}
public sealed class PolicyRow { public int Id { get; set; } public string State { get; set; } = "{}"; }
public sealed class ClubDb(DbContextOptions<ClubDb> options) : DbContext(options)
{
    public DbSet<ClubRow> Clubs => Set<ClubRow>();
    public DbSet<OperationRow> Operations => Set<OperationRow>();
    public DbSet<LedgerRow> Ledger => Set<LedgerRow>();
    public DbSet<MatchRow> Matches => Set<MatchRow>();
    public DbSet<PolicyRow> Policies => Set<PolicyRow>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<OrderRow>().HasKey(x => x.Id);
        b.Entity<OrderRow>().HasOne<ClubRow>().WithMany().HasForeignKey(x => x.Owner).OnDelete(DeleteBehavior.Restrict);
        b.Entity<SessionRow>().HasKey(x => x.TokenHash);
        b.Entity<SessionRow>().HasIndex(x => x.TicketHash).IsUnique();
        b.Entity<SessionRow>().HasOne<ClubRow>().WithMany().HasForeignKey(x => x.Owner).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ClubRow>().ToTable("Clubs", t => t.HasCheckConstraint("club_nonnegative", "\"Money\" >= 0 AND \"Credits\" >= 0 AND \"Rating\" >= 0 AND \"Version\" >= 0"));
        b.Entity<ClubRow>().HasKey(x => x.Id); b.Entity<ClubRow>().Property(x => x.Version).IsConcurrencyToken();
        b.Entity<ClubRow>().Property(x => x.State).HasColumnType("jsonb");
        b.Entity<OperationRow>().HasKey(x => new { x.Owner, x.Key });
        b.Entity<OperationRow>().HasOne<ClubRow>().WithMany().HasForeignKey(x => x.Owner).OnDelete(DeleteBehavior.Restrict);
        b.Entity<LedgerRow>().HasKey(x => new { x.Owner, x.Operation });
        b.Entity<LedgerRow>().HasOne<ClubRow>().WithMany().HasForeignKey(x => x.Owner).OnDelete(DeleteBehavior.Restrict);
        b.Entity<MatchRow>().HasKey(x => x.Id);
        b.Entity<MatchRow>().HasOne<ClubRow>().WithMany().HasForeignKey(x => x.Attacker).OnDelete(DeleteBehavior.Restrict);
        b.Entity<MatchRow>().HasOne<ClubRow>().WithMany().HasForeignKey(x => x.Defender).OnDelete(DeleteBehavior.Restrict);
        b.Entity<MatchRow>().HasIndex(x => new { x.Status, x.LeaseUntil });
        b.Entity<PolicyRow>().HasKey(x => x.Id); b.Entity<PolicyRow>().Property(x => x.State).HasColumnType("jsonb");
    }
}
public static class Json
{
    public static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
    public static string Write<T>(T value) => JsonSerializer.Serialize(value, Options);
    public static T Read<T>(string value) => JsonSerializer.Deserialize<T>(value, Options) ?? throw new InvalidOperationException("Missing persisted state");
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
public sealed class Store(string connection)
{
    public ClubDb Open() => new(new DbContextOptionsBuilder<ClubDb>().UseNpgsql(connection, o => o.SetPostgresVersion(18, 0)).Options);
    // A transaction-level advisory lock serializes local slice mutations across processes, including absent rows.
    // Deliberate bounded-development tradeoff: partition by club/pair before production-scale deployment.
    public async Task<T> Transaction<T>(Func<ClubDb, Task<T>> body)
    {
        await using var db = Open(); await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(734028119)");
        T result = await body(db); await db.SaveChangesAsync(); await tx.CommitAsync(); return result;
    }
    public async Task<string> Command(string owner, string key, string payload, long expectedVersion, Func<ClubDb, ClubState, Task<object>> action, long now)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 100) throw new InvalidOperationException("Operation key required");
        return await Transaction(async db =>
        {
            var prior = await db.Operations.FindAsync(owner, key); string hash = Json.Hash(payload);
            if (prior != null) { if (prior.Hash != hash) throw new InvalidOperationException("Idempotency payload conflict"); return prior.Response; }
            var row = await db.Clubs.FindAsync(owner) ?? throw new InvalidOperationException("Account unavailable");
            if (row.Version != expectedVersion) throw new InvalidOperationException("Stale club version");
            var s = Json.Read<ClubState>(row.State);
            if (s.RestrictedUntil > now) throw new InvalidOperationException("Account temporarily restricted");
            object result = await action(db, s); s.Version++; await Save(db, row, s, now);
            string response = Json.Write(result);
            db.Operations.Add(new OperationRow { Owner = owner, Key = key, Hash = hash, Response = response });
            return response;
        });
    }
    public static async Task Save(ClubDb db, ClubRow row, ClubState state, long now)
    {
        if (state.Fighters.Count(x => x.Active) > 16 || state.BbStock.Length != 5 || state.BbStock.Any(x => x < 0 || x > state.Capacity)) throw new InvalidOperationException("Invalid roster/BB invariant");
        if (state.Fighters.Any(f => f.Hp < 0 || f.Hp > f.MaxHp) || state.Fighters.Select(f => f.Id).Distinct().Count() != state.Fighters.Count) throw new InvalidOperationException("Invalid fighter state");
        var assigned = state.Fighters.SelectMany(f => f.Equipment.Values).ToArray();
        if (assigned.Distinct().Count() != assigned.Length || assigned.Any(id => !state.Items.ContainsKey(id))) throw new InvalidOperationException("Equipment ownership violation");
        row.Version = state.Version; row.Money = state.Wallet.Money; row.Credits = state.Wallet.Credits; row.Rating = state.Rating;
        row.State = Json.Write(state);
        row.Defense = state.Fighters.Any(f => f.Active) ? Airsoft.Battle.BattleWire.WriteTeam(Clubs.Snapshot(state, true, now)) : null;
        var persistedLedger = await db.Ledger.Where(x => x.Owner == state.Id).ToListAsync();
        if (state.Wallet.Entries.Select(e => e.Operation).Distinct().Count() != state.Wallet.Entries.Count ||
            state.Wallet.Entries.Sum(e => e.MoneyDelta) != state.Wallet.Money || state.Wallet.Entries.Sum(e => e.CreditsDelta) != state.Wallet.Credits ||
            persistedLedger.Any(old => !state.Wallet.Entries.Any(e => e.Operation == old.Operation && e.Reason == old.Reason && e.MoneyDelta == old.Money && e.CreditsDelta == old.Credits)))
            throw new InvalidOperationException("Wallet ledger conservation violated");
        var existing = persistedLedger.Select(x => x.Operation).ToList();
        foreach (var e in state.Wallet.Entries.Where(e => !existing.Contains(e.Operation)))
            db.Ledger.Add(new LedgerRow { Owner = state.Id, Operation = e.Operation, Reason = e.Reason, Money = e.MoneyDelta, Credits = e.CreditsDelta });
    }
    public Task<ClubState> Create(string owner, long now) => Transaction(async db =>
    {
        var prior = await db.Clubs.FindAsync(owner); if (prior != null) return Json.Read<ClubState>(prior.State);
        var s = Clubs.Create(owner, now); var row = new ClubRow { Id = owner }; db.Clubs.Add(row); await Save(db, row, s, now); return s;
    });
}
