using Airsoft.Battle;

namespace Airsoft.Club;

public enum Slot { Weapon, Camouflage, HeadProtection, LoadBearingArmor }
public sealed record ItemDefinition(string Id, Slot Slot, int Mk, long Money, long Credits, int Level,
    WeaponFamily Family = WeaponFamily.Pistol, int Damage = 20, int Interval = 1000, int Projectiles = 1,
    int Protection = 0, int AgilityPenalty = 0);
public static class Catalog
{
    public const string Version = "catalog-004-v1";
    public static readonly ItemDefinition[] Items = Build();
    static ItemDefinition[] Build()
    {
        var list = new List<ItemDefinition>();
        foreach (var family in Enum.GetValues<WeaponFamily>())
            for (int mk = 1; mk <= 3; mk++)
            {
                var (damage, interval, projectiles) = family switch
                {
                    WeaponFamily.Smg => (10, 850, 3),
                    WeaponFamily.AssaultRifle => (15, 1200, 3),
                    WeaponFamily.Shotgun => (6, 1600, 8),
                    WeaponFamily.Dmr => (34, 1500, 1),
                    WeaponFamily.SniperRifle => (50, 2100, 1),
                    _ => (20, 1000, 1)
                };
                list.Add(new($"{family}-MK{mk}", Slot.Weapon, mk, mk == 3 ? 0 : mk == 2 ? 600 : 100, mk == 3 ? 6 : 0,
                    1, family, damage, interval, projectiles));
            }
        foreach (var slot in new[] { Slot.Camouflage, Slot.HeadProtection, Slot.LoadBearingArmor })
            for (int weight = 1; weight <= 3; weight++)
                list.Add(new($"{slot}-{weight}", slot, 1, weight * 60, 0, weight, Protection: weight * 5, AgilityPenalty: weight - 1));
        return list.ToArray();
    }
    public static ItemDefinition Get(string id) => Items.SingleOrDefault(x => x.Id == id) ?? throw new InvalidOperationException("Unknown item");
    public static BbTierDefinition Bb(int tier)
    {
        int[] multipliers = { 100, 103, 105, 110, 115 };
        if (tier < 0 || tier >= multipliers.Length) throw new InvalidOperationException("Unknown BB tier");
        return new BbTierDefinition("bb-" + tier, Fixed.Ratio(multipliers[tier], 100), Fixed.Zero);
    }
}
public sealed class Fighter
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Accuracy { get; set; }
    public int Endurance { get; set; }
    public int Agility { get; set; }
    public long Xp { get; set; }
    public long Hp { get; set; }
    public long RecoveryAt { get; set; }
    public long RecoveryRemainder { get; set; }
    public long InitialPrice { get; set; }
    public bool Active { get; set; } = true;
    public Dictionary<Slot, string> Equipment { get; set; } = new();
    public long MaxHp => Formulas.MaxHp(Fixed.FromInt(Endurance), new BattleRules()).Raw;
    public int Level => 1 + (int)(Xp / 100);
    public int TrainingCap => 15 + Level * 5;
    public bool Ready => Active && Readiness.IsReady(Fixed.FromRaw(Hp), Fixed.FromRaw(MaxHp));
    public void Recover(long now)
    {
        if (now < RecoveryAt) throw new InvalidOperationException("Server clock moved backwards");
        long elapsed = now - RecoveryAt;
        // 1% per minute in raw HP. Saturate before multiplying arbitrarily long offline time.
        if (Hp >= MaxHp || elapsed >= 6000000) { Hp = MaxHp; RecoveryRemainder = 0; }
        else
        {
            long numerator = checked(MaxHp * elapsed + RecoveryRemainder);
            Hp = Math.Min(MaxHp, checked(Hp + numerator / 6000000));
            RecoveryRemainder = Hp == MaxHp ? 0 : numerator % 6000000;
        }
        RecoveryAt = now;
    }
}
public sealed record RecruitOffer(string Id, int Accuracy, int Endurance, int Agility, long Price, string Name);
public sealed class ClubState
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "Development club";
    public long Version { get; set; }
    public Wallet Wallet { get; set; } = new();
    public long Xp { get; set; }
    public int Rating { get; set; } = 100;
    public bool FreeRecruitClaimed { get; set; }
    public List<Fighter> Fighters { get; set; } = new();
    public Dictionary<string, string> Items { get; set; } = new();
    public int[] BbStock { get; set; } = new int[5];
    public int ActiveBbTier { get; set; }
    public long EmergencyAt { get; set; } = -10800000;
    public long OffersAt { get; set; }
    public int CompletedSinceRefresh { get; set; }
    public int OfferVersion { get; set; }
    public List<RecruitOffer> Offers { get; set; } = new();
    public HashSet<string> CompletedMatches { get; set; } = new();
    public HashSet<string> Unlocks { get; set; } = new();
    public RetentionState Retention { get; set; } = new();
    public long RestrictedUntil { get; set; }
    public int Emblem { get; set; }
    public string? PendingMatch { get; set; }
    public long ShieldUntil { get; set; }
    public int Level => 1 + (int)(Xp / 1000);
    public int Capacity => Math.Min(1000000, 1000 + (Level - 1) * 500);
}
public static class Clubs
{
    public static ClubState Create(string id, long now)
    {
        var s = new ClubState { Id = id }; s.Wallet.Starter(new()); s.BbStock[0] = 500;
        Refresh(s, now, 1); return s;
    }
    public static void Available(ClubState s)
    {
        if (s.PendingMatch != null) throw new InvalidOperationException("Offensive battle pending");
    }
    public static void Refresh(ClubState s, long now, ulong seed, bool paid = false, string operation = "")
    {
        Available(s);
        if (s.OfferVersion > 0)
        {
            if (paid) s.Wallet.Apply(operation, "refresh", -50, 0);
            else if (now - s.OffersAt < 3600000 && s.CompletedSinceRefresh < 10) throw new InvalidOperationException("Refresh not ready");
        }
        var rng = new SeededRandom(seed); int version = s.OfferVersion + 1;
        s.Offers = Enumerable.Range(0, 7).Select(i =>
        {
            int a = 5 + s.Level + rng.NextInt(10), e = 5 + s.Level + rng.NextInt(10), g = 5 + s.Level + rng.NextInt(10);
            return new RecruitOffer($"{version}-{i}", a, e, g, (a + e + g) * 5, "Recruit " + (i + 1));
        }).ToList();
        s.OfferVersion = version; s.OffersAt = now; s.CompletedSinceRefresh = 0;
    }
    public static Fighter Hire(ClubState s, string offerId, int version, bool free, long now, string operation)
    {
        Available(s);
        if (s.Fighters.Count(x => x.Active) >= 16) throw new InvalidOperationException("Roster full");
        if (version != s.OfferVersion) throw new InvalidOperationException("Stale recruitment quote");
        var offer = s.Offers.SingleOrDefault(x => x.Id == offerId) ?? throw new InvalidOperationException("Offer unavailable");
        if (free && (s.FreeRecruitClaimed || !s.Offers.Take(3).Contains(offer))) throw new InvalidOperationException("Starter recruit unavailable");
        s.Wallet.Apply(operation, "hire:" + offerId, free ? 0 : -offer.Price, 0);
        var fighter = new Fighter
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = offer.Name,
            Accuracy = offer.Accuracy,
            Endurance = offer.Endurance,
            Agility = offer.Agility,
            RecoveryAt = now,
            InitialPrice = free ? 0 : offer.Price
        };
        fighter.Hp = fighter.MaxHp; s.Fighters.Add(fighter); s.Offers.Remove(offer);
        if (free) s.FreeRecruitClaimed = true;
        return fighter;
    }
    public static Fighter Owned(ClubState s, string id) => s.Fighters.SingleOrDefault(f => f.Id == id && f.Active) ?? throw new InvalidOperationException("Fighter not owned/active");
    public static void Dismiss(ClubState s, string id, string operation)
    {
        Available(s); var f = Owned(s, id);
        if (s.Fighters.Count(x => x.Active) <= 1) throw new InvalidOperationException("Keep one defense fighter");
        s.Wallet.Apply(operation, "dismiss:" + id, f.InitialPrice * 30 / 100, 0);
        f.Equipment.Clear(); f.Active = false;
    }
    public static void Train(ClubState s, string id, string stat, long now, string operation)
    {
        Available(s); var f = Owned(s, id);
        int value = stat switch { "Accuracy" => f.Accuracy, "Endurance" => f.Endurance, "Agility" => f.Agility, _ => throw new InvalidOperationException("Unknown stat") };
        if (value >= f.TrainingCap) throw new InvalidOperationException("XP training cap");
        f.Recover(now); s.Wallet.Apply(operation, "train:" + id + ":" + stat, -25, 0);
        if (stat == "Accuracy") f.Accuracy++; else if (stat == "Endurance") f.Endurance++; else f.Agility++;
    }
    public static void Heal(ClubState s, string id, long now, string operation)
    {
        Available(s); var f = Owned(s, id); f.Recover(now);
        long price = (f.MaxHp - f.Hp + 9999) / 10000;
        s.Wallet.Apply(operation, "heal:" + id, -price, 0); f.Hp = f.MaxHp; f.RecoveryRemainder = 0;
    }
    public static string Buy(ClubState s, string definition, string operation)
    {
        Available(s); var item = Catalog.Get(definition);
        if (item.Level > s.Level && !s.Unlocks.Contains(definition)) throw new InvalidOperationException("Club level locked");
        s.Wallet.Apply(operation, "item:" + definition, -item.Money, -item.Credits);
        string id = Guid.NewGuid().ToString("N"); s.Items.Add(id, definition); return id;
    }
    public static void Equip(ClubState s, string fighter, string item)
    {
        Available(s); var f = Owned(s, fighter);
        if (!s.Items.TryGetValue(item, out var definition)) throw new InvalidOperationException("Item not owned");
        if (s.Fighters.Any(x => x.Equipment.Values.Contains(item))) throw new InvalidOperationException("Already equipped");
        f.Equipment[Catalog.Get(definition).Slot] = item;
    }
    public static void Refill(ClubState s, int tier, string operation)
    {
        Available(s); _ = Catalog.Bb(tier);
        // Capacity is per-tier, finite; explicit refill SKU is up to500 at flat price. No silent tier changes.
        if (s.BbStock[tier] >= s.Capacity) throw new InvalidOperationException("BB stock full");
        s.Wallet.Apply(operation, "bb:" + tier, tier == 4 ? 0 : -(50 + tier * 20), tier == 4 ? -1 : 0);
        s.BbStock[tier] = Math.Min(s.Capacity, s.BbStock[tier] + 500);
    }
    public static void Emergency(ClubState s, long now)
    {
        Available(s);
        if (s.BbStock[0] * 100 >= s.Capacity * 15 || s.Wallet.Money >= 50 || now - s.EmergencyAt < 10800000)
            throw new InvalidOperationException("Emergency Basic not eligible");
        s.BbStock[0] = Math.Min(s.Capacity, s.BbStock[0] + 500); s.EmergencyAt = now;
    }
    public static void Completed(ClubState s, string match)
    { if (s.CompletedMatches.Add(match)) s.CompletedSinceRefresh++; }
    public static TeamSnapshot Snapshot(ClubState s, bool defense, long now)
    {
        var fighters = new List<FighterSnapshot>();
        foreach (var f in s.Fighters.Where(x => x.Active))
        {
            if (!defense) f.Recover(now);
            if (!defense && !f.Ready) continue;
            WeaponSnapshot? weapon = null; int protection = 0, penalty = 0;
            foreach (var instance in f.Equipment.Values)
            {
                var item = Catalog.Get(s.Items[instance]);
                if (item.Slot == Slot.Weapon)
                    weapon = new WeaponSnapshot(item.Id, item.Family, Fixed.FromInt(item.Damage) * Fixed.Ratio(item.Mk == 3 ? 1035 : item.Mk == 2 ? 1020 : 1000, 1000),
                        Fixed.Zero, Fixed.Zero, item.Interval, item.Projectiles);
                else { protection += item.Protection; penalty += item.AgilityPenalty; }
            }
            fighters.Add(new FighterSnapshot(f.Id, Fixed.FromInt(f.Accuracy), Fixed.FromInt(f.Endurance), Fixed.FromInt(f.Agility),
                Fixed.FromRaw(defense ? f.MaxHp : f.Hp), weapon, new ArmorLoadout(Fixed.FromInt(protection), Fixed.Zero, Fixed.FromInt(penalty))));
        }
        if (fighters.Count == 0) throw new InvalidOperationException("No eligible fighters");
        return new TeamSnapshot(fighters, Catalog.Bb(s.ActiveBbTier), defense ? s.Capacity : s.BbStock[s.ActiveBbTier]);
    }
}
