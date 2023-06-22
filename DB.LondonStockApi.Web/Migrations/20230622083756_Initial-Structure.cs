using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.LondonStockApi.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Averages",
                columns: table => new
                {
                    TickerSymbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AveragePriceInPounds = table.Column<decimal>(type: "Money", nullable: false),
                    TotalSharesExchanged = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Averages", x => x.TickerSymbol);
                });

            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    ExchangeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TickerSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceInPounds = table.Column<decimal>(type: "Money", nullable: false),
                    SharesExchanged = table.Column<decimal>(type: "Money", nullable: false),
                    BrokerId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.ExchangeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Averages");

            migrationBuilder.DropTable(
                name: "Exchanges");
        }
    }
}
