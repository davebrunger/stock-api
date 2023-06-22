namespace DB.LondonStockApi.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController : ControllerBase
{
    private readonly LondonStockApiContext context;

    public StocksController(LondonStockApiContext context)
    {
        this.context = context;
    }

    [HttpGet("{tickerSymbol}")]
    public async Task<ActionResult> GetAverage(string tickerSymbol, CancellationToken cancellationToken)
    {
        var average = await context.Averages.SingleOrDefaultAsync(a => a.TickerSymbol == tickerSymbol);
        if (average == null)
        {
            return NotFound();
        }
        return Ok(new { average.TickerSymbol, average.AveragePriceInPounds });
    }
}
