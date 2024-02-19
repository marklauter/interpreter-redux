using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Lexi.Tests;

[ExcludeFromCodeCoverage]
public sealed class Startup
{
    private static readonly TokenPattern[] Patterns =
    [
        new(@"\G\-?\d+\.\d+", Tokens.FloatingPointLiteral, 1),
        new(@"\G\-?\d+", Tokens.IntegerLiteral, 0),
        new(@"\G\+", Tokens.Operator, 2),
        new(@"\G\-", Tokens.Operator, 3),
        new(@"\G\*", Tokens.Operator, 4),
        new(@"\G/", Tokens.Operator, 5),
        new(@"\G%", Tokens.Operator, 6),
        new(@"\G<", Tokens.Operator, 7),
        new(@"\G<=", Tokens.Operator, 8),
    ];

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "xunit requires instance method")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddTransient(serviceProvider =>
            LexerBuilder
            .CreateWithRegexOptions(RegexOptions.CultureInvariant)
            .WithPatterns(Patterns)
            .Build());
    }
}
