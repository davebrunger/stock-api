namespace DB.LondonStockApi.Web.Models;

public class Exchange
{
    public long ExchangeId { get; init; }

    public string TickerSymbol { get; init; } = null!;
    
    public decimal PriceInPounds { get; init; }

    public decimal SharesExchanged { get; set; }

    public string BrokerId { get; init; } = null!;
}
