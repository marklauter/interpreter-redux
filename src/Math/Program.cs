using Math.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddParser())
    .Build();

var parser = host.Services.GetRequiredService<Parser>();

Console.Write("math:> ");
var source = Console.ReadLine();
while (!String.IsNullOrWhiteSpace(source))
{
    var color = Console.ForegroundColor;
    try
    {
        var expression = parser.Parse(source);
        expression.Print();

        Console.WriteLine("-------------");

        Console.ForegroundColor = ConsoleColor.White;
        var result = expression.Evaluate();
        Console.WriteLine($"result:> {result}");
        Console.ForegroundColor = color;
        Console.WriteLine();
        Console.Write("math:> ");
        source = Console.ReadLine();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{ex.GetType().Name}, {ex.Message}");
        Console.ForegroundColor = color;

        Console.WriteLine();
        Console.Write("math:> ");
        source = Console.ReadLine();
    }
}
