using OrdersApp;

var path = args.Length > 0 ? args[0] : OrderLoader.DefaultPath();

try
{
    var orders = OrderLoader.Load(path);
    ConsoleMenu.Run(orders, Console.In, Console.Out);
    return 0;
}
catch (OrderLoadException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}
