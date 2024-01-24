using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

public sealed class LinguisticContext
    : IEnumerable<LanguageExpression>
{
    private readonly LanguageExpression[] languages = null!;

    public LinguisticContext(LanguageSpecification language)
    {
        ArgumentNullException.ThrowIfNull(language);

        languages = new[]
        {
            new {
                Type = TokenType.Keyword,
                Expression = language.Keywords.Any()
                    ? $@"\G({String.Join("|", language.Keywords.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                Type = TokenType.InfixOperator,
                Expression = language.InfixOperators.Any()
                    ? $@"\G({String.Join("|", language.InfixOperators.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                Type = TokenType.PrefixOperator,
                Expression = language.PrefixOperators.Any()
                    ? $@"\G({String.Join("|", language.PrefixOperators.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                Type = TokenType.PostfixOperator,
                Expression = language.PostfixOperators.Any()
                    ? $@"\G({String.Join("|", language.PostfixOperators.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                Type = TokenType.Punctuation,
                Expression = language.Punctuation.Any()
                    ? $@"\G({String.Join("|", language.Punctuation.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                Type = TokenType.Identifier,
                Expression = $@"\G([a-zA-Z_][a-zA-Z0-9_]*)"
            },
            new {
                Type = TokenType.NumericConstant,
                Expression = $@"\G(\d+(\.\d+)?)"
            },
            new {
                Type = TokenType.StringConstant,
                Expression = $@"\G(""[^""]*"")"
            },
            new {
                Type = TokenType.Whitespace,
                Expression = $@"\G(\s+)"
            },
        }
        .Where(e => !String.IsNullOrEmpty(e.Expression))
        .Select(e => new LanguageExpression(
            e.Type,
            new Regex(
                e.Expression,
                RegexOptions.CultureInvariant |
                RegexOptions.ExplicitCapture |
                RegexOptions.Compiled)))
        .ToArray();

        // $@"(?<{nameof(TokenType.DecimalConstant)}>\b\d+\.\d+?\b)",
        // $@"(?<{nameof(TokenType.IntegerConstant)}>\b\d+\b)",
    }

    public int Length => languages.Length;
    public LanguageExpression this[int i] => languages[i];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<LanguageExpression> GetEnumerator()
    {
        return (IEnumerator<LanguageExpression>)languages.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
