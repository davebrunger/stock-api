namespace DB.LondonStockApi.Web.Controllers;

[Route("averages")]
[ApiController]
public class AveragesController : ControllerBase
{
    private readonly ILondonStockApiRepository repository;

    public AveragesController(ILondonStockApiRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>
    /// Gets the average price in pounds for some or all ticker symbols
    /// </summary>
    /// <param name="tickerSymbols">A list of ticker symbols that will filter the output</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Supplying one or more values for the <paramref name="tickerSymbols"/> query parameter
    /// will restrict the output to averages for those ticker symbols only. Call this method with no
    /// query parameters to return averages for all ticker symbols.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string[] tickerSymbols, CancellationToken cancellationToken)
    {
        var filteredAverages = tickerSymbols.Length > 0
            ? repository.Averages.Where(a => tickerSymbols.Contains(a.TickerSymbol))
            : repository.Averages;
        var averages = await filteredAverages.ToArrayAsync(cancellationToken);
        return Ok(averages.Select(a => new { a.TickerSymbol, a.AveragePriceInPounds }));
    }

    /// <summary>
    /// Gets the average price in pounds for a ticker symbol
    /// </summary>
    /// <param name="tickerSymbol"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{tickerSymbol}")]
    public async Task<ActionResult> Get(string tickerSymbol, CancellationToken cancellationToken)
    {
        var average = await repository.Averages.SingleOrDefaultAsync(a => a.TickerSymbol == tickerSymbol, cancellationToken);
        if (average == null)
        {
            return NotFound();
        }
        return Ok(new { average.TickerSymbol, average.AveragePriceInPounds });
    }
}
