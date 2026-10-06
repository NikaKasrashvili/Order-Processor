namespace OrdersApp;

public static class OrderService
{
    public static decimal GetOrderTotal(Order order) => throw new NotImplementedException();

    public static bool IsCompleted(Order order) => throw new NotImplementedException();

    public static IReadOnlyList<Order> FindByCustomer(IReadOnlyList<Order> orders, string customer)
        => throw new NotImplementedException();

    public static OrderStatistics CalculateStatistics(IReadOnlyList<Order> orders)
        => throw new NotImplementedException();
}
