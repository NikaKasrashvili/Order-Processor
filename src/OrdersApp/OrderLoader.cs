using System.Text.Json;

namespace OrdersApp;

public class OrderLoadException(string message, Exception? inner = null) : Exception(message, inner);

public static class OrderLoader
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static string DefaultPath() => Path.Combine(AppContext.BaseDirectory, "orders.json");

    public static IReadOnlyList<Order> Load(string path)
    {
        try
        {
            var orders = JsonSerializer.Deserialize<List<Order>>(File.ReadAllText(path), Options);
            return orders ?? throw new OrderLoadException($"'{path}' does not contain an orders list.");
        }
        catch (JsonException ex)
        {
            throw new OrderLoadException($"'{path}' is not valid orders JSON: {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            throw new OrderLoadException($"Cannot read orders file '{path}': {ex.Message}", ex);
        }
    }
}
