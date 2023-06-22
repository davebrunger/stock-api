namespace DB.LondonStockApi.Web;

public class LondonStockApiContext : DbContext
{
    public LondonStockApiContext(DbContextOptions<LondonStockApiContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LondonStockApiContext).Assembly);
    }

    public DbSet<Exchange> Exchanges { get; init; }
    
    public DbSet<Average> Averages { get; init; }
}
