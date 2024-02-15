using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Predicate.Parser;
using Predicate.Parser.Exceptions;
using Predicate.REPL;

using var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddParser())
    .Build();

var parser = host.Services.GetRequiredService<Parser>();

var fgcolor = Console.ForegroundColor;
Console.Write("predicate:> ");
var source = Console.ReadLine();
while (!String.IsNullOrWhiteSpace(source))
{
    try
    {
        var statement = parser.Parse(source);
        statement.Print();
        Console.ForegroundColor = fgcolor;
    }
    catch (ParseException ex)
    {
        ex.Print();
    }

    Console.WriteLine();
    Console.Write("predicate:> ");
    source = Console.ReadLine();
}

