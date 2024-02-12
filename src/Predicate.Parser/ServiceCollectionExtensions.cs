using Luthor.Symbols;
using Luthor.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Predicate.Parser;

public static class ServiceCollectionExtensions
{
    private static readonly TerminalSymbolOptions Options =
        TerminalSymbolOptions.IncludeStringLiterals
        | TerminalSymbolOptions.IncludeNumericLiterals
        | TerminalSymbolOptions.IncludeCharacterLiterals
        | TerminalSymbolOptions.IncludeIdentifiers;
    private static readonly string[] Operators = [
        "&&", // and
        "||", // or
        "==",
        "!=",
        ">",
        "<",
        ">=",
        "<=",
        "-s", // starts with
        "-e", // ends with
        "-c"]; // contains
    private static readonly string[] ReservedWords = ["from", "where"];
    private static readonly string[] BooleanLiterals = ["true", "false"];
    private static readonly CircumfixPair[] CircumfixDelimiterPairs = [new("(", ")")];

    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        services.TryAddSingleton(
            new TerminalSymbolSpec()
            {
                Options = Options,
                Operators = Operators,
                ReservedWords = ReservedWords,
                BooleanLiterals = BooleanLiterals,
                CircumfixDelimiterPairs = CircumfixDelimiterPairs,
            });

        services.TryAddSingleton<Tokenizers>();
        services.TryAddTransient<Parser>();

        return services;
    }
}
