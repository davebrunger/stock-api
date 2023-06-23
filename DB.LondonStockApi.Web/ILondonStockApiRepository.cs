namespace DB.LondonStockApi.Web;

public interface ILondonStockApiRepository
{
    Task SaveChangesAsync(CancellationToken cancellationToken);

    IQueryable<Exchange> Exchanges { get; }

    IQueryable<Average> Averages { get; }

    void Add(Exchange exchange);

    void Add(Average average);
}
