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
    var expression = parser.Parse(source);
    var result = expression.Evaluate();
    Console.WriteLine($"result: {result}");

    Console.Write("math:> ");
    source = Console.ReadLine();
}
