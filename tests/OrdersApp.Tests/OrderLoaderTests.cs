namespace OrdersApp.Tests;

public class OrderLoaderTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("orders-tests-").FullName;

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private string WriteTempFile(string json)
    {
        var path = Path.Combine(_dir, "orders.json");
        File.WriteAllText(path, json);
        return path;
    }

    [Fact]
    public void Load_OrderWithoutItems_HasEmptyItemsList()
    {
        var path = WriteTempFile("""[{ "orderId": 1, "customer": "Nino", "status": "completed" }]""");

        var orders = OrderLoader.Load(path);

        var order = Assert.Single(orders);
        Assert.NotNull(order.Items);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void Load_InvalidJson_ThrowsOrderLoadException()
    {
        var path = WriteTempFile("{ not json");

        Assert.Throws<OrderLoadException>(() => OrderLoader.Load(path));
    }

    [Fact]
    public void Load_MissingFile_ThrowsOrderLoadException()
    {
        var path = Path.Combine(_dir, "does-not-exist.json");

        Assert.Throws<OrderLoadException>(() => OrderLoader.Load(path));
    }
}
