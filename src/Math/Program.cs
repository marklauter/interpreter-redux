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
    var syntaxTree = parser.Parse(source);
    var color = Console.ForegroundColor;
    if (syntaxTree.Errors.Any())
    {
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var error in syntaxTree.Errors)
        {
            Console.WriteLine(error);
        }

        Console.ForegroundColor = color;

        Console.WriteLine();
        Console.Write("math:> ");
        source = Console.ReadLine();

        continue;
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    syntaxTree.Print();
    Console.ForegroundColor = color;

    Console.WriteLine("-------------");

    Console.ForegroundColor = ConsoleColor.Green;
    var result = syntaxTree.Evaluate();
    Console.WriteLine($"math:> {result}");
    Console.ForegroundColor = color;
    Console.WriteLine();
    Console.Write("math:> ");
    source = Console.ReadLine();
}
