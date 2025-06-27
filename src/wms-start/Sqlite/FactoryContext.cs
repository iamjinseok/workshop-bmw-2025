using Microsoft.EntityFrameworkCore;

namespace wms_start.Sqlite;

public class FactoryContext : DbContext
{
    public string DbFilePath { get; set; } = "Factory.db";

    public DbSet<Inventory> Inventories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbFilePath}");
    }
}