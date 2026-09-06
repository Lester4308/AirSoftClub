using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Airsoft.Server;

public sealed class DesignFactory : IDesignTimeDbContextFactory<ClubDb>
{
    public ClubDb CreateDbContext(string[] args) => new Store(Environment.GetEnvironmentVariable("AIRSOFT_CONNECTION") ?? "Host=localhost;Database=design_only").Open();
}
