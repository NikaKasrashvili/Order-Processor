namespace OrdersApp;

public record OrderItem(string Product, int Quantity, decimal Price);

// A missing "items" in the JSON becomes an empty list.
public record Order(int OrderId, string Customer, string Status, IReadOnlyList<OrderItem>? Items = null)
{
    public IReadOnlyList<OrderItem> Items { get; init; } = Items ?? [];
}

public record OrderStatistics(
    int CompletedCount,
    decimal TotalSales,
    decimal AverageOrderValue,
    IReadOnlyList<string> MostPopularProducts);
