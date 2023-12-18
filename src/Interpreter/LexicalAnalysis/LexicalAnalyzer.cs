using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

internal sealed class LexicalAnalyzer
    : ILexicalAnalyzer
{
    private readonly IEnumerable<string> keywords;
    private readonly IEnumerable<string> operators;
    private readonly IEnumerable<string> punctuationMarks;
    private readonly Regex regex;
    private readonly HashSet<string> groupNames;
    private readonly Dictionary<string, SymbolType> symbolTypeMap = new()
    {
        { nameof(SymbolType.Keyword), SymbolType.Keyword },
        { nameof(SymbolType.Operator), SymbolType.Operator },
        { nameof(SymbolType.Punctuation), SymbolType.Punctuation },
        { nameof(SymbolType.Identifier), SymbolType.Identifier },
        { nameof(SymbolType.Constant), SymbolType.Constant },
    };

    public LexicalAnalyzer(IOptions<LexicalAnalyzerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        keywords = options.Value.Keywords;
        operators = options.Value.Operators;
        punctuationMarks = options.Value.PunctuationMarks;

        //regex = new Regex($@"\s*(?<keyword>{String.Join("|", keywords)})\s*|(?<operator>{String.Join("|", operators)})|(?<identifier>[a-zA-Z_][a-zA-Z0-9_]*)|(?<constant>[0-9]+)|(?<string>""[^""]*"")|(?<boolean>true|false)|(?<whitespace>\s+)", RegexOptions.Compiled);

        var keywordExpression = $@"(?<{nameof(SymbolType.Keyword)}>{String.Join("|", keywords)})";
        var operatorExpression = $@"(?<{nameof(SymbolType.Operator)}>{String.Join("|", operators.Select(Regex.Escape))})";
        var punctuationExpression = $@"(?<{nameof(SymbolType.Punctuation)}>{String.Join("|", punctuationMarks.Select(Regex.Escape))})";
        var identifierExpression = $@"(?<{nameof(SymbolType.Identifier)}>[a-zA-Z_][a-zA-Z0-9_]*)";
        // todo: constant expression should allow for decimals and booleans
        var constantExpression = $@"(?<{nameof(SymbolType.Constant)}>[0-9]+)";
        regex = new Regex(
            $"{keywordExpression}|{operatorExpression}|{punctuationExpression}|{identifierExpression}|{constantExpression}",
            RegexOptions.Compiled);
        groupNames = regex
            .GetGroupNames()
            .Where(n => !n.Equals("0", StringComparison.OrdinalIgnoreCase))
            .ToHashSet();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MatchCollection Analyze(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return regex
            .Matches(source);
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
    private SymbolType AsSymbolType(string value)
    {
        return !symbolTypeMap.TryGetValue(value, out var symbolType)
            ? throw new KeyNotFoundException($"can't find matching symbol type from value: '{value}'")
            : symbolType;
    }
}
