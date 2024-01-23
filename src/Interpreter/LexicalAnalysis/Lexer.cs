using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

public sealed class Lexer
{
    private readonly Regex regex;
    private readonly HashSet<string> groupNames;
    private readonly Dictionary<string, TokenType> symbolTypeMap = new()
    {
        { nameof(TokenType.Keyword), TokenType.Keyword },
        { nameof(TokenType.InfixOperator), TokenType.InfixOperator },
        { nameof(TokenType.PrefixOperator), TokenType.PrefixOperator },
        { nameof(TokenType.PostfixOperator), TokenType.PostfixOperator },
        { nameof(TokenType.Punctuation), TokenType.Punctuation },
        { nameof(TokenType.Identifier), TokenType.Identifier },
        { nameof(TokenType.IntegerConstant), TokenType.IntegerConstant },
        { nameof(TokenType.DecimalConstant), TokenType.DecimalConstant },
        { nameof(TokenType.String), TokenType.String},
        { nameof(TokenType.Whitespace), TokenType.Whitespace},
    };

    public Lexer(Language languge)
    {
        ArgumentNullException.ThrowIfNull(languge);

        var expressions = new string[]
        {
            languge.Keywords.Any()
                ? $@"(?<{nameof(TokenType.Keyword)}>\b{String.Join("|", languge.Keywords.Select(Regex.Escape))}\b)"
                : String.Empty,
            languge.InfixOperators.Any()
                ? $@"(?<{nameof(TokenType.InfixOperator)}>{String.Join("|", languge.InfixOperators.Select(Regex.Escape))})"
                : String.Empty,
            languge.PrefixOperators.Any()
                ? $@"(?<{nameof(TokenType.PrefixOperator)}>{String.Join("|", languge.PrefixOperators.Select(Regex.Escape))})"
                : String.Empty,
            languge.PostfixOperators.Any()
                ? $@"(?<{nameof(TokenType.PostfixOperator)}>{String.Join("|", languge.PostfixOperators.Select(Regex.Escape))})"
                : String.Empty,
            languge.Punctuation.Any()
                ? $@"(?<{nameof(TokenType.Punctuation)}>{String.Join("|", languge.Punctuation.Select(Regex.Escape))})"
                : String.Empty,
            $@"(?<{nameof(TokenType.Identifier)}>\b[a-zA-Z_][a-zA-Z0-9_]*\b)",
            $@"(?<{nameof(TokenType.DecimalConstant)}>\b\d+\.\d+?\b)",
            $@"(?<{nameof(TokenType.IntegerConstant)}>\b\d+\b)",
            $@"(?<{nameof(TokenType.String)}>""[^""]*"")",
            $@"(?<{nameof(TokenType.Whitespace)}>\s+)",
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
    public IEnumerable<Token> ReadTokens(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return regex
            .Matches(source)
            .SelectMany(m => m.Groups.Values)
            .Where(g => g.Success && groupNames.Contains(g.Name))
            .Select(CreateSymbol);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Token CreateSymbol(Group group)
    {
        var type = AsTokenType(group.Name);
        return new Token(
            group.Index,
            group.Length,
            type,
            type == TokenType.Whitespace ? String.Empty : group.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TokenType AsTokenType(string tokenName)
    {
        return !symbolTypeMap.TryGetValue(tokenName, out var type)
            ? throw new TokenKeyNotFoundException($"can't find matching token type from value: '{tokenName}'")
            : type;
    }
}
