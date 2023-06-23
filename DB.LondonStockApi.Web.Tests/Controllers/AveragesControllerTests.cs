using DB.LondonStockApi.Web.Models;

namespace DB.LondonStockApi.Web.Tests.Controllers;

public class AveragesControllerTests
{
    [Test]
    public async Task TestGetOneSucceeds()
    {
        var cancellationToken = new CancellationToken();
        var repository = new Mock<ILondonStockApiRepository>();
        var average = new Average
        {
            AveragePriceInPounds = 1.20m,
            TickerSymbol = "ABC",
            TotalSharesExchanged = 1000m
        };
        var averages = new[] { average }.BuildMock();
        repository
            .Setup(r => r.Averages)
            .Returns(averages);

        var controller = new AveragesController(repository.Object);
        var actual = (await controller.Get(average.TickerSymbol, cancellationToken)) as OkObjectResult;

        actual.Should().NotBeNull();
        actual!.Value.Should().BeEquivalentTo(new { average.TickerSymbol, average.AveragePriceInPounds });
    }
     
    [Test]
    public async Task TestGetOneFails()
    {
        var cancellationToken = new CancellationToken();
        var repository = new Mock<ILondonStockApiRepository>();
        var averages = Array.Empty<Average>().BuildMock();
        repository
            .Setup(r => r.Averages)
            .Returns(averages);

        var controller = new AveragesController(repository.Object);
        var actual = (await controller.Get("ABC", cancellationToken)) as NotFoundResult;

        actual.Should().NotBeNull();
    }

    [Test]
    [TestCaseSource(nameof(TestGetManyTestCases))]
    public async Task TestGetMany(Average[] averages, string[] tickerSymbols, object expected)
    {
        var cancellationToken = new CancellationToken();
        var repository = new Mock<ILondonStockApiRepository>();
        repository
            .Setup(r => r.Averages)
            .Returns(averages.BuildMock());

        var controller = new AveragesController(repository.Object);
        var actual = (await controller.Get(tickerSymbols, cancellationToken)) as OkObjectResult;

        actual.Should().NotBeNull();
        actual!.Value.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<TestCaseData> TestGetManyTestCases
    {
        get
        {
            yield return new TestCaseData(
                Array.Empty<Average>(),
                new[] { "ABC" },
                Array.Empty<dynamic>()).SetName("One ticker symbol, no average");
            yield return new TestCaseData(
                Array.Empty<Average>(),
                new[] { "ABC", "DEF", "GHI" },
                Array.Empty<dynamic>()).SetName("Multiple ticker symbols, no average");
            yield return new TestCaseData(
                Array.Empty<Average>(),
                Array.Empty<string>(),
                Array.Empty<dynamic>()).SetName("All ticker symbols, no average");
            yield return new TestCaseData(
                new[]
                {
                    new Average
                    {
                        AveragePriceInPounds = 1.20m,
                        TickerSymbol = "ABC",
                        TotalSharesExchanged = 1000m
                    }
                },
                new[] { "ABC" },
                new[] {
                    new
                    {
                        TickerSymbol = "ABC",
                        AveragePriceInPounds = 1.20m
                    }
                }).SetName("One ticker symbol, one average");
            yield return new TestCaseData(
                new[]
                {
                    new Average
                    {
                        AveragePriceInPounds = 1.20m,
                        TickerSymbol = "ABC",
                        TotalSharesExchanged = 1000m
                    }
                },
                new[] { "ABC", "DEF", "GHI" },
                new[] {
                    new
                    {
                        TickerSymbol = "ABC",
                        AveragePriceInPounds = 1.20m
                    }
                }).SetName("Multiple ticker symbols, one average");
            yield return new TestCaseData(
                new[]
                {
                    new Average
                    {
                        AveragePriceInPounds = 1.20m,
                        TickerSymbol = "ABC",
                        TotalSharesExchanged = 1000m
                    }
                },
                Array.Empty<string>(),
                new[] {
                    new
                    {
                        TickerSymbol = "ABC",
                        AveragePriceInPounds = 1.20m
                    }
                }).SetName("All ticker symbols, one average");
            yield return new TestCaseData(
                new[]
                {
                    new Average
                    {
                        AveragePriceInPounds = 1.30m,
                        TickerSymbol = "DEF",
                        TotalSharesExchanged = 500m
                    },
                    new Average
                    {
                        AveragePriceInPounds = 0.78m,
                        TickerSymbol = "GHI",
                        TotalSharesExchanged = 10.6m
                    }
                },
                new[] { "DEF", "GHI", "JKL" },
                new[]
                {
                    new
                    {
                        TickerSymbol = "DEF",
                        AveragePriceInPounds = 1.30m
                    },
                    new
                    {
                        TickerSymbol = "GHI",
                        AveragePriceInPounds = 0.78m
                    }
                }).SetName("Multiple ticker symbols, multiple averages");
            yield return new TestCaseData(
                new[]
                {
                    new Average
                    {
                        AveragePriceInPounds = 1.30m,
                        TickerSymbol = "DEF",
                        TotalSharesExchanged = 500m
                    },
                    new Average
                    {
                        AveragePriceInPounds = 0.78m,
                        TickerSymbol = "GHI",
                        TotalSharesExchanged = 10.6m
                    }
                },
                Array.Empty<string>(),
                new[]
                {
                    new
                    {
                        TickerSymbol = "DEF",
                        AveragePriceInPounds = 1.30m
                    },
                    new
                    {
                        TickerSymbol = "GHI",
                        AveragePriceInPounds = 0.78m
                    }
                }).SetName("All ticker symbols, multiple averages");
        }
    }
}