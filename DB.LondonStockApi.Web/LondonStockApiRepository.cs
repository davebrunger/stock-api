namespace DB.LondonStockApi.Web;

public class LondonStockApiRepository : ILondonStockApiRepository
{
    private readonly LondonStockApiContext context;

    public LondonStockApiRepository(LondonStockApiContext context)
    {
        this.context = context;
    }

    IQueryable<Exchange> ILondonStockApiRepository.Exchanges => context.Exchanges;
    
    IQueryable<Average> ILondonStockApiRepository.Averages => context.Averages;

    public void Add(Exchange exchange)
    {
        context.Exchanges.Add(exchange);
    }

    public void Add(Average average)
    {
        context.Averages.Add(average);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
