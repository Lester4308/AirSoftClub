using Airsoft.Club;
using Microsoft.EntityFrameworkCore;
namespace Airsoft.Server;

public sealed class OrderRow
{
    public string Id { get; set; } = "";
    public string Owner { get; set; } = "";
    public string State { get; set; } = "{}";
    public string ProviderState { get; set; } = "Unknown";
}
public sealed class Purchases(Store store)
{
    public async Task<bool> RefreshFromProvider(string orderId, ICommerceProvider provider, long now)
    {
        string status = await provider.Status(orderId);
        await ObserveTrustedProvider(orderId, status);
        return await Reconcile(orderId, now);
    }
    public Task<string> Create(string orderId, string owner, string sku) => store.Transaction(async db =>
    {
        var existing = await db.Set<OrderRow>().FindAsync(orderId);
        if (existing != null)
        {
            var prior = Json.Read<PurchaseOrder>(existing.State);
            if (prior.Owner != owner || prior.Sku != sku) throw new InvalidOperationException("Order payload conflict");
            return existing.State;
        }
        var order = Commerce.Create(orderId, owner, sku); var row = new OrderRow { Id = orderId, Owner = owner, State = Json.Write(order) };
        db.Set<OrderRow>().Add(row); return row.State;
    });
    // Internal trusted adapter seam only. There is deliberately no callback accepting client payment claims.
    public Task<bool> ObserveTrustedProvider(string orderId, string state) => store.Transaction(async db =>
    {
        if (state is not ("Paid" or "Refunded" or "Unknown")) throw new InvalidOperationException("Unknown provider status");
        var row = await db.Set<OrderRow>().FindAsync(orderId) ?? throw new InvalidOperationException("Order missing");
        if (row.ProviderState == "Refunded") return false;
        if (state != "Unknown" || row.ProviderState == "Unknown") row.ProviderState = state;
        return true;
    });
    public Task<bool> Reconcile(string orderId, long now, bool injectFailure = false) => store.Transaction(async db =>
    {
        var row = await db.Set<OrderRow>().FindAsync(orderId) ?? throw new InvalidOperationException("Order missing");
        var clubRow = (await db.Clubs.FindAsync(row.Owner))!; var s = Json.Read<ClubState>(clubRow.State);
        var order = Json.Read<PurchaseOrder>(row.State); string before = order.Status;
        Commerce.Reconcile(order, s, row.ProviderState); row.State = Json.Write(order);
        if (order.Status != before) { s.Version++; await Store.Save(db, clubRow, s, now); }
        if (injectFailure) { await db.SaveChangesAsync(); throw new InvalidOperationException("Injected post-payment DB failure"); }
        return order.Status == "Granted";
    });
}
