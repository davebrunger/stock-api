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

    public Task<Average[]> GetAverages(CancellationToken cancellationToken, params string[] tickerSymbols)
    {
        var filteredAverages = tickerSymbols.Length > 0
            ? context.Averages.Where(a => tickerSymbols.Contains(a.TickerSymbol))
            : context.Averages;
        return filteredAverages.ToArrayAsync(cancellationToken);
    }

    public Task<Exchange?> GetExchange(long exchangeId, CancellationToken cancellationToken)
    {
        return context.Exchanges.SingleOrDefaultAsync(e => e.ExchangeId == exchangeId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
