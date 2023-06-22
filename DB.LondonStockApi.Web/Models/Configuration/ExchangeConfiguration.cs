namespace DB.LondonStockApi.Web.Models.Configuration;

public class ExchangeConfiguration : IEntityTypeConfiguration<Exchange>
{
    public void Configure(EntityTypeBuilder<Exchange> builder)
    {
        builder.Property(e => e.PriceInPounds).HasColumnType("Money");
        builder.Property(e => e.SharesExchanged).HasColumnType("Money");
    }
}
