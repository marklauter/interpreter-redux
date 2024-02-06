using Luthor.Symbols;
using Luthor.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Math.Parser;

public static class ServiceCollectionExtensions
{
    private static readonly string[] Operators = ["+", "-", "*", "/", "%", "==", "!=", ">", ">=", "<", "<=", "&&", "||"];
    private static readonly string[] BooleanLiterals = ["true", "false"];
    private static readonly CircumfixPair[] CircumfixDelimiterPairs = [new("(", ")")];

    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        services.TryAddSingleton(
            new TerminalSymbolSpec()
            {
                Options = TerminalSymbolOptions.IncludeNumericLiterals,
                Operators = Operators,
                BooleanLiterals = BooleanLiterals,
                CircumfixDelimiterPairs = CircumfixDelimiterPairs,
            });

        services.TryAddSingleton<Tokenizers>();
        services.TryAddTransient<Parser>();

        return services;
    }
}
