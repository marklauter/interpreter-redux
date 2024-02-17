using Luthor.Symbols;
using Luthor.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Luthor.Tests;

[ExcludeFromCodeCoverage]
public sealed class Startup
{
    private static readonly TerminalSymbolSpec spec = new()
    {
        Options =
            TerminalSymbolOptions.IncludeStringLiterals
            | TerminalSymbolOptions.IncludeNumericLiterals
            | TerminalSymbolOptions.IncludeCharacterLiterals
            | TerminalSymbolOptions.IncludeIdentifiers,
        Operators = new string[] { "!", "~", "++", "--", "+", "-", "/", "*", "%", "|", "&", "?", "=", "<", ">", ">=", "<=", "^", },
        BooleanLiterals = new string[] { "true", "false" },
        ReservedWords = new string[] { "if", "else", "let" },
        CommentPrefixes = new string[] { "//", "#" },
        CircumfixDelimiterPairs = new CircumfixPair[]
        {
            new ("(", ")"),
            new ("{", "}"),
            new ("[", "]"),
            new ("<", "/>"),
        },
        InfixDelimiters = new string[] { ",", ";", ".", ":", },
    };

    private static readonly TokenDefinition[] tokenDefs =
    [
        new(@"\G\-?\d+\.\d+", 1),
        new(@"\G\-?\d+", 0),
        new(@"\G[+-]", 2),
        new(@"\G[*/%]", 3),
    ];

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "xunit requires instance method")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(spec);
        services.TryAddSingleton(tokenDefs);
        services.TryAddSingleton<Tokenizers>();
    }
}
