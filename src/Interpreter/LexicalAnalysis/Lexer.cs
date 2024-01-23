using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

public sealed class Lexer
{
    private readonly Regex regex;
    private readonly HashSet<string> groupNames;
    private readonly Dictionary<string, Symbols> symbolTypeMap = new()
    {
        { nameof(Symbols.Keyword), Symbols.Keyword },
        { nameof(Symbols.InfixOperator), Symbols.InfixOperator },
        { nameof(Symbols.PrefixOperator), Symbols.PrefixOperator },
        { nameof(Symbols.PostfixOperator), Symbols.PostfixOperator },
        { nameof(Symbols.Punctuation), Symbols.Punctuation },
        { nameof(Symbols.Identifier), Symbols.Identifier },
        { nameof(Symbols.Constant), Symbols.Constant },
        { nameof(Symbols.String), Symbols.String},
        { nameof(Symbols.Whitespace), Symbols.Whitespace},
    };

    public Lexer(Language languge)
    {
        ArgumentNullException.ThrowIfNull(languge);

        var keywords = languge.Keywords;
        var infixOperators = languge.InfixOperators;
        var prefixOperators = languge.PrefixOperators;
        var postfixOperators = languge.PostfixOperators;
        var punctuation = languge.Punctuation;

        var x = new Regex($@"\s*(?<identifier>[a-zA-Z_][a-zA-Z0-9_]*)|(?<constant>[0-9]+)|(?<string>""[^""]*"")|(?<boolean>true|false)|(?<whitespace>\s+)", RegexOptions.Compiled);

        var keywordExpression = keywords.Any()
            ? $@"(?<{nameof(Symbols.Keyword)}>{String.Join("|", keywords)})"
            : String.Empty;

        var infixOperatorExpression = infixOperators.Any()
            ? $@"(?<{nameof(Symbols.InfixOperator)}>{String.Join("|", infixOperators.Select(Regex.Escape))})"
            : String.Empty;

        var prefixOperatorExpression = prefixOperators.Any()
            ? $@"(?<{nameof(Symbols.PrefixOperator)}>{String.Join("|", prefixOperators.Select(Regex.Escape))})"
            : String.Empty;

        var postfixOperatorExpression = postfixOperators.Any()
            ? $@"(?<{nameof(Symbols.PostfixOperator)}>{String.Join("|", postfixOperators.Select(Regex.Escape))})"
            : String.Empty;

        var punctuationExpression = punctuation.Any()
            ? $@"(?<{nameof(Symbols.Punctuation)}>{String.Join("|", punctuation.Select(Regex.Escape))})"
            : String.Empty;

        var constantExpression = $@"(?<{nameof(Symbols.Constant)}>[0-9]+.?[0-9]+|true|false)";
        var identifierExpression = $@"(?<{nameof(Symbols.Identifier)}>[a-zA-Z_][a-zA-Z0-9_]*)";
        var stringExpression = $@"(?<{nameof(Symbols.String)}>""[^""]*"")";
        var whitespaceExpression = $@"(?<{nameof(Symbols.Whitespace)}>\s+)";

        var expressions = new string[]
        {
            keywordExpression,
            infixOperatorExpression,
            prefixOperatorExpression,
            postfixOperatorExpression,
            punctuationExpression,
            identifierExpression,
            constantExpression,
            stringExpression,
        }
        .Where(s => !String.IsNullOrWhiteSpace(s));

        var expression = String.Join(
            '|',
            expressions);

        regex = new Regex(
            expression,
            RegexOptions.Compiled);

        groupNames = regex
            .GetGroupNames()
            .Where(n => !n.Equals("0", StringComparison.OrdinalIgnoreCase))
            .ToHashSet();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Symbol> ReadSymbols(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return regex
            .Matches(source)
            .SelectMany(m => m.Groups.Values)
            .Where(g => g.Success && groupNames.Contains(g.Name))
            .Select(CreateSymbol);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Symbol CreateSymbol(Group group)
    {
        return new Symbol(
            group.Index,
            group.Length,
            AsSymbolType(group.Name),
            group.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Symbols AsSymbolType(string value)
    {
        return !symbolTypeMap.TryGetValue(value, out var symbolType)
            ? throw new KeyNotFoundException($"can't find matching symbol type from value: '{value}'")
            : symbolType;
    }
}
