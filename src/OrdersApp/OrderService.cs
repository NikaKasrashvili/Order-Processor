namespace OrdersApp;

public static class OrderService
{
    public static decimal GetOrderTotal(Order order) =>
        order.Items.Sum(i => i.Quantity * i.Price);

    public static bool IsCompleted(Order order) =>
        string.Equals(order.Status.Trim(), "completed", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<Order> FindByCustomer(IReadOnlyList<Order> orders, string customer)
    {
        if (string.IsNullOrWhiteSpace(customer))
            return [];

        var name = customer.Trim();
        return orders
            .Where(o => string.Equals(o.Customer.Trim(), name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static OrderStatistics CalculateStatistics(IReadOnlyList<Order> orders)
    {
        var completed = orders.Where(IsCompleted).ToList();
        if (completed.Count == 0)
            return new OrderStatistics(0, 0m, 0m, []);

        var totalSales = completed.Sum(GetOrderTotal);

        var quantityByProduct = completed
            .SelectMany(o => o.Items)
            .GroupBy(i => i.Product)
            .Select(g => (Product: g.Key, Quantity: g.Sum(i => i.Quantity)))
            .ToList();
        var maxQuantity = quantityByProduct.Select(p => p.Quantity).DefaultIfEmpty(0).Max();
        var topProducts = quantityByProduct
            .Where(p => p.Quantity == maxQuantity)
            .Select(p => p.Product)
            .Order(StringComparer.Ordinal)
            .ToList();

        return new OrderStatistics(completed.Count, totalSales, totalSales / completed.Count, topProducts);
    }
}
