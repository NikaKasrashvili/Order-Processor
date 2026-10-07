using System.Globalization;

namespace OrdersApp;

public static class ConsoleMenu
{
    public static void Run(IReadOnlyList<Order> orders, TextReader input, TextWriter output)
    {
        while (true)
        {
            output.WriteLine();
            output.WriteLine("1. Show all orders");
            output.WriteLine("2. Search orders by customer");
            output.WriteLine("3. Show statistics");
            output.WriteLine("0. Exit");
            output.Write("Choose an option: ");

            switch (input.ReadLine()?.Trim())
            {
                case null:
                case "0":
                    return;
                case "1":
                    if (orders.Count == 0)
                        output.WriteLine("No orders.");
                    else
                        PrintOrders(orders, output);
                    break;
                case "2":
                    output.Write("Customer name: ");
                    var found = OrderService.FindByCustomer(orders, input.ReadLine() ?? "");
                    if (found.Count == 0)
                        output.WriteLine("No orders found.");
                    else
                        PrintOrders(found, output);
                    break;
                case "3":
                    PrintStatistics(OrderService.CalculateStatistics(orders), output);
                    break;
                default:
                    output.WriteLine("Unknown option.");
                    break;
            }
        }
    }

    private static void PrintOrders(IReadOnlyList<Order> orders, TextWriter output)
    {
        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            if (i > 0)
                output.WriteLine();
            output.WriteLine($"Order #{order.OrderId} | {order.Customer} | {order.Status}");
            foreach (var item in order.Items)
                output.WriteLine($"  {item.Product} x {item.Quantity} @ {Money(item.Price)}");
            output.WriteLine($"  Total: {Money(OrderService.GetOrderTotal(order))}");
        }
    }

    private static void PrintStatistics(OrderStatistics stats, TextWriter output)
    {
        output.WriteLine($"Completed orders: {stats.CompletedCount}");
        output.WriteLine($"Total sales: {Money(stats.TotalSales)}");
        output.WriteLine($"Average order value: {Money(stats.AverageOrderValue)}");
        output.WriteLine("Most popular product: " +
            (stats.MostPopularProducts.Count == 0 ? "n/a" : string.Join(", ", stats.MostPopularProducts)));
    }

    private static string Money(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero).ToString("F2", CultureInfo.InvariantCulture);
}
