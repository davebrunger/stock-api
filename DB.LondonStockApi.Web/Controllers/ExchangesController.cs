namespace DB.LondonStockApi.Web.Controllers;

[Route("exchanges")]
[ApiController]
public class ExchangesController : ControllerBase
{
    private readonly ILondonStockApiRepository repository;

    public ExchangesController(ILondonStockApiRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>
    /// Gets the details of a single exchange
    /// </summary>
    /// <param name="exchangeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{exchangeId}")]
    public async Task<ActionResult> Get(long exchangeId, CancellationToken cancellationToken)
    {
        var exchange = await repository.Exchanges.SingleOrDefaultAsync(e => e.ExchangeId == exchangeId, cancellationToken);
        if (exchange == null)
        {
            return NotFound();
        }
        return Ok(exchange);
    }

    /// <summary>
    /// Registers the details of a single exchange
    /// </summary>
    /// <param name="exchangeToAdd"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ExchangeToAdd exchangeToAdd, CancellationToken cancellationToken)
    {
        // This validation could be done with data annotations, but as there is no easy way to have
        // a decimal value checked to be greater than a value, it was just as well to do all the
        // validation in code.
        if (string.IsNullOrWhiteSpace(exchangeToAdd.BrokerId))
        {
            ModelState.AddModelError(nameof(exchangeToAdd.BrokerId), "BrokerId cannot be null or empty");
        }

        if (exchangeToAdd.PriceInPounds <= 0)
        {
            ModelState.AddModelError(nameof(exchangeToAdd.PriceInPounds), "PriceInPounds must be greater than zero");
        }

        if (exchangeToAdd.SharesExchanged <= 0)
        {
            ModelState.AddModelError(nameof(exchangeToAdd.SharesExchanged), "SharesExchanged must be greater than zero");
        }

        if (string.IsNullOrWhiteSpace(exchangeToAdd.TickerSymbol))
        {
            ModelState.AddModelError(nameof(exchangeToAdd.TickerSymbol), "TickerSymbol cannot be null or empty");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var exchange = new Exchange
        {
            BrokerId = exchangeToAdd.BrokerId,
            PriceInPounds = exchangeToAdd.PriceInPounds,
            SharesExchanged = exchangeToAdd.SharesExchanged,
            TickerSymbol = exchangeToAdd.TickerSymbol
        };

        repository.Add(exchange);

        var average = await repository.Averages.SingleOrDefaultAsync(a => a.TickerSymbol == exchangeToAdd.TickerSymbol, cancellationToken);
        if (average == null)
        {     
            average = new Average
            {
                TickerSymbol = exchangeToAdd.TickerSymbol
            };
            repository.Add(average);
        }
        var totalPriceInPounds = (average.AveragePriceInPounds * average.TotalSharesExchanged) + (exchangeToAdd.PriceInPounds * exchangeToAdd.SharesExchanged);
        average.TotalSharesExchanged += exchangeToAdd.SharesExchanged;
        average.AveragePriceInPounds = totalPriceInPounds / average.TotalSharesExchanged;

        await repository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(Get), new { exchangeId = exchange.ExchangeId }, exchange);
    }
}
