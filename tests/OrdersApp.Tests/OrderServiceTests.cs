namespace OrdersApp.Tests;

public class OrderServiceTests
{
    // Mirrors src/OrdersApp/orders.json
    private static List<Order> SampleOrders() =>
    [
        new(1, "Nino", "completed", [new("Keyboard", 2, 50m), new("Mouse", 1, 25m)]),
        new(2, "Giorgi", "cancelled", [new("Keyboard", 1, 50m)]),
        new(3, "Nino", "completed", [new("Mouse", 2, 25m), new("Monitor", 1, 200m)]),
        new(4, "Ana", "completed", [new("Keyboard", 1, 50m)]),
    ];

    [Fact]
    public void Statistics_ProvidedData_ExcludesCancelled()
    {
        var stats = OrderService.CalculateStatistics(SampleOrders());

        Assert.Equal(3, stats.CompletedCount);
        Assert.Equal(425m, stats.TotalSales);
        Assert.Equal(141.67m, stats.AverageOrderValue, 2);
    }

    [Fact]
    public void Statistics_Tie_ReturnsAllTiedProductsSortedAlphabetically()
    {
        var stats = OrderService.CalculateStatistics(SampleOrders());

        Assert.Equal(["Keyboard", "Mouse"], stats.MostPopularProducts);
    }

    [Fact]
    public void Statistics_UniqueWinner_ReturnsOnlyThatProduct()
    {
        List<Order> orders =
        [
            new(1, "Nino", "completed", [new("Mouse", 5, 25m), new("Keyboard", 1, 50m)]),
            new(2, "Ana", "completed", [new("Keyboard", 2, 50m)]),
        ];

        var stats = OrderService.CalculateStatistics(orders);

        Assert.Equal(["Mouse"], stats.MostPopularProducts);
    }

    [Fact]
    public void FindByCustomer_MatchesTrimmedCaseInsensitive_AllStatuses()
    {
        var orders = SampleOrders();

        Assert.Equal([1, 3], OrderService.FindByCustomer(orders, "Nino").Select(o => o.OrderId));
        Assert.Equal([1, 3], OrderService.FindByCustomer(orders, " nino ").Select(o => o.OrderId));
        Assert.Equal([2], OrderService.FindByCustomer(orders, "Giorgi").Select(o => o.OrderId));
        Assert.Empty(OrderService.FindByCustomer(orders, "Unknown"));
        Assert.Empty(OrderService.FindByCustomer(orders, "  "));
    }

    [Fact]
    public void Statistics_OnlyCompletedCounts_CaseInsensitiveAndTrimmed()
    {
        List<Order> orders =
        [
            new(1, "Nino", "Completed ", [new("Mouse", 1, 25m)]),
            new(2, "Ana", " CANCELLED ", [new("Mouse", 1, 25m)]),
            new(3, "Giorgi", "pending", [new("Mouse", 1, 25m)]),
        ];

        var stats = OrderService.CalculateStatistics(orders);

        Assert.Equal(1, stats.CompletedCount);
        Assert.Equal(25m, stats.TotalSales);
    }

    [Fact]
    public void Statistics_NoCompletedOrders_ReturnsZerosAndNoTopProduct()
    {
        List<Order> onlyCancelled = [new(1, "Ana", "cancelled", [new("Mouse", 1, 25m)])];

        foreach (var orders in new[] { new List<Order>(), onlyCancelled })
        {
            var stats = OrderService.CalculateStatistics(orders);

            Assert.Equal(0, stats.CompletedCount);
            Assert.Equal(0m, stats.TotalSales);
            Assert.Equal(0m, stats.AverageOrderValue);
            Assert.Empty(stats.MostPopularProducts);
        }
    }

    [Fact]
    public void Statistics_CompletedOrderWithEmptyItems_CountsWithZeroTotal()
    {
        List<Order> orders = [new(1, "Nino", "completed", [])];

        var stats = OrderService.CalculateStatistics(orders);

        Assert.Equal(1, stats.CompletedCount);
        Assert.Equal(0m, stats.TotalSales);
        Assert.Equal(0m, stats.AverageOrderValue);
        Assert.Empty(stats.MostPopularProducts);
    }
}
