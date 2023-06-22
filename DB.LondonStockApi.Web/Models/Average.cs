namespace DB.LondonStockApi.Web.Models;

public class Average
{
    public string TickerSymbol { get; init; } = null!;

    public decimal AveragePriceInPounds { get; set; }

    public decimal TotalSharesExchanged { get; set; }
}
