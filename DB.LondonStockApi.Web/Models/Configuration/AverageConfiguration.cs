namespace DB.LondonStockApi.Web.Models.Configuration;

public class AverageConfiguration : IEntityTypeConfiguration<Average>
{
    public void Configure(EntityTypeBuilder<Average> builder)
    {
        builder.HasKey(a => a.TickerSymbol);

        builder.Property(e => e.AveragePriceInPounds).HasColumnType("Money");
        builder.Property(e => e.TotalSharesExchanged).HasColumnType("Money");
    }
}
