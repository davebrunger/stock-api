using Moq;

namespace DB.LondonStockApi.Web.Tests.Conrtrollers;

public class ExchangesControllerTests
{
    [Test]
    [TestCaseSource(nameof(TestPostBadRequestTestCases))]
    public async Task TestPostBadRequest(ExchangeToAdd exchangeToAdd, object expected)
    {
        var controller = new ExchangesController(null!);
        var actual = (await controller.Post(exchangeToAdd, CancellationToken.None)) as BadRequestObjectResult;
        actual.Should().NotBeNull();
        actual!.Value.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<TestCaseData> TestPostBadRequestTestCases
    {
        get
        {
            yield return new TestCaseData(
                new ExchangeToAdd(),
                new Dictionary<string, string[]>
                {
                    { "BrokerId", new [] { "BrokerId cannot be null or empty" } },
                    { "TickerSymbol", new [] { "TickerSymbol cannot be null or empty" } },
                    { "PriceInPounds", new [] { "PriceInPounds must be greater than zero" } },
                    { "SharesExchanged", new [] { "SharesExchanged must be greater than zero" } }
                }).SetName("Empty object");
            yield return new TestCaseData(
                new ExchangeToAdd
                {
                    BrokerId = string.Empty,
                    TickerSymbol = "ABC",
                    PriceInPounds = 1,
                    SharesExchanged = 1
                },
                new Dictionary<string, string[]>
                {
                    { "BrokerId", new [] { "BrokerId cannot be null or empty" } },
                }).SetName("Empty BrokerId");
            yield return new TestCaseData(
                new ExchangeToAdd
                {
                    BrokerId = "a-broker-id",
                    TickerSymbol = string.Empty,
                    PriceInPounds = 1,
                    SharesExchanged = 1
                },
                new Dictionary<string, string[]>
                {
                    { "TickerSymbol", new [] { "TickerSymbol cannot be null or empty" } },
                }).SetName("Empty TickerSymbol");
            yield return new TestCaseData(
                new ExchangeToAdd
                {
                    BrokerId = "a-broker-id",
                    TickerSymbol = "ABC",
                    PriceInPounds = -1,
                    SharesExchanged = 1
                },
                new Dictionary<string, string[]>
                {
                    { "PriceInPounds", new [] { "PriceInPounds must be greater than zero" } },
                }).SetName("Negative PriceInPounds");
            yield return new TestCaseData(
                new ExchangeToAdd
                {
                    BrokerId = "a-broker-id",
                    TickerSymbol = "ABC",
                    PriceInPounds = 0,
                    SharesExchanged = 1
                },
                new Dictionary<string, string[]>
                {
                    { "PriceInPounds", new [] { "PriceInPounds must be greater than zero" } },
                }).SetName("Zero PriceInPounds");
            yield return new TestCaseData(
                new ExchangeToAdd
                {
                    BrokerId = "a-broker-id",
                    TickerSymbol = "ABC",
                    PriceInPounds = 1,
                    SharesExchanged = -1
                },
                new Dictionary<string, string[]>
                {
                    { "SharesExchanged", new [] { "SharesExchanged must be greater than zero" } }
                }).SetName("Negative SharesExchanged");
            yield return new TestCaseData(
                new ExchangeToAdd
                {
                    BrokerId = "a-broker-id",
                    TickerSymbol = "ABC",
                    PriceInPounds = 1,
                    SharesExchanged = 0
                },
                new Dictionary<string, string[]>
                {
                    { "SharesExchanged", new [] { "SharesExchanged must be greater than zero" } }
                }).SetName("Zero SharesExchanged");
        }
    }

    [Test]
    public async Task TestPostSuccessNoAverage()
    {
        var cancellationToken = new CancellationToken();
        var repository = new Mock<ILondonStockApiRepository>(MockBehavior.Strict);
        var averages = Array.Empty<Average>().BuildMock();

        var exchangeToAdd = new ExchangeToAdd
        {
            BrokerId = "a-broker-id",
            PriceInPounds = 1.20m,
            SharesExchanged = 500,
            TickerSymbol = "ABC"
        };

        var sequence = new MockSequence();

        Average average = null!;
        Exchange exchange = null!;

        repository
            .InSequence(sequence)
            .Setup(r => r.Add(It.Is<Exchange>(e => e.BrokerId == exchangeToAdd.BrokerId && e.TickerSymbol == exchangeToAdd.TickerSymbol)))
            .Callback<Exchange>(e => { exchange = e; });
        repository
            .InSequence(sequence)
            .Setup(r => r.Averages)
            .Returns(averages);
        repository
            .InSequence(sequence)
            .Setup(r => r.Add(It.Is<Average>(a => a.TickerSymbol == a.TickerSymbol)))
            .Callback<Average>(a => { average = a; });
        repository
            .InSequence(sequence)
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Callback(() => {
                exchange.PriceInPounds.Should().Be(exchangeToAdd.PriceInPounds);
                exchange.SharesExchanged.Should().Be(exchangeToAdd.SharesExchanged);
                average.AveragePriceInPounds.Should().Be(exchangeToAdd.PriceInPounds);
                average.TotalSharesExchanged.Should().Be(exchangeToAdd.SharesExchanged);
            })
            .Returns(Task.CompletedTask);

        var controller = new ExchangesController(repository.Object);

        var actual = (await controller.Post(exchangeToAdd, cancellationToken)) as CreatedAtActionResult;

        actual.Should().NotBeNull();

        repository.Verify(r => r.Add(It.Is<Exchange>(e => e.BrokerId == exchangeToAdd.BrokerId && e.TickerSymbol == exchangeToAdd.TickerSymbol)), Times.Once);
        repository.Verify(r => r.Averages, Times.Once);
        repository.Verify(r => r.Add(It.Is<Average>(a => a.TickerSymbol == a.TickerSymbol)), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Test]
    public async Task TestPostSuccessExistingAverage()
    {
        var cancellationToken = new CancellationToken();
        var repository = new Mock<ILondonStockApiRepository>(MockBehavior.Strict);

        var exchangeToAdd = new ExchangeToAdd
        {
            BrokerId = "a-broker-id",
            PriceInPounds = 1.50m,
            SharesExchanged = 1000,
            TickerSymbol = "DEF"
        };

        var average = new Average
        {
            AveragePriceInPounds = 2.00m,
            TickerSymbol = "DEF",
            TotalSharesExchanged = 600m
        };

        var averages = new[] { average }.BuildMock();

        var sequence = new MockSequence();

        Exchange exchange = null!;

        repository
            .InSequence(sequence)
            .Setup(r => r.Add(It.Is<Exchange>(e => e.BrokerId == exchangeToAdd.BrokerId && e.TickerSymbol == exchangeToAdd.TickerSymbol)))
            .Callback<Exchange>(e => { exchange = e; });
        repository
            .InSequence(sequence)
            .Setup(r => r.Averages)
            .Returns(averages);
        repository
            .InSequence(sequence)
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Callback(() => {
                exchange.PriceInPounds.Should().Be(exchangeToAdd.PriceInPounds);
                exchange.SharesExchanged.Should().Be(exchangeToAdd.SharesExchanged);
                average.AveragePriceInPounds.Should().Be(1.6875M);
                average.TotalSharesExchanged.Should().Be(1600M);
            })
            .Returns(Task.CompletedTask);

        var controller = new ExchangesController(repository.Object);

        var actual = (await controller.Post(exchangeToAdd, cancellationToken)) as CreatedAtActionResult;

        actual.Should().NotBeNull();

        repository.Verify(r => r.Add(It.Is<Exchange>(e => e.BrokerId == exchangeToAdd.BrokerId && e.TickerSymbol == exchangeToAdd.TickerSymbol)), Times.Once);
        repository.Verify(r => r.Averages, Times.Once);
        repository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
