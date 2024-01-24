using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

public sealed class LinguisticContext
    : IEnumerable<LanguageExpression>
{
    private readonly LanguageExpression[] languages = null!;

    // todo: need to add a regex item for every value in TokenType

    public LinguisticContext(LanguageSpecification language)
    {
        ArgumentNullException.ThrowIfNull(language);

        languages = new[]
        {
            new {
                Type = TokenType.ReservedWord,
                Expression = language.Keywords.Any()
                    ? $@"\G({String.Join("|", language.Keywords.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                // todo: this expression needs to match operator between a (literal or identifier) and a (literal or identifier)
                Type = TokenType.InfixOperator,
                Expression = language.InfixOperators.Any()
                    ? $@"\G({String.Join("|", language.InfixOperators.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                // todo: this expression needs look-ahead to match operator followed by literal or identifier
                Type = TokenType.PrefixOperator,
                Expression = language.PrefixOperators.Any()
                    ? $@"\G({String.Join("|", language.PrefixOperators.Select(Regex.Escape))})"
                    : String.Empty
            },
            new {
                // todo: this expression needs to match preceded by literal or identifier
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
                // todo: this expression needs to exclude keywords
                Type = TokenType.Identifier,
                Expression = $@"\G([a-zA-Z_][a-zA-Z0-9_]*)"
            },
            new {
                Type = TokenType.NumericLiteral,
                Expression = $@"\G(\d+(\.\d+)?)"
            },
            // todo: try this $@"\G(""(?:[^""\\\n\r]|\\.)*"")" to support escaped quotes
            new {
                Type = TokenType.StringLiteral,
                Expression = $@"\G(""[^""\n\r]*"")"
            },
            new {
                Type = TokenType.Whitespace,
                Expression = $@"\G(\s+)"
            },
            new {
                Type = TokenType.NewLine,
                Expression = $@"\G(\r\n|\r|\n)"
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
