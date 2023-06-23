using System.ComponentModel.DataAnnotations;

namespace DB.LondonStockApi.Web.Controllers.Models
{
    public class ExchangeToAdd
    {
        public string TickerSymbol { get; init; } = null!;

        public decimal PriceInPounds { get; init; }

        public decimal SharesExchanged { get; set; }

        public string BrokerId { get; init; } = null!;
    }
}
