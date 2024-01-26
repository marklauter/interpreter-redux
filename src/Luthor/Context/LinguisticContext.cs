using Luthor.Spec;
using Luthor.Tokens;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public sealed class LinguisticContext
    : IEnumerable<LinguisticExpression>
{
    //     [GeneratedRegex]
    // https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators

    public const string IdentifierExpression = @"[a-zA-Z_]\w*";
    public const string StringLiteralExpression = @"""(?:[^""\\\n\r]|\\.)*""";
    public const string NumericLiteralExpression = @"\b\d+(?:\.\d+)?\b";
    public const string CharacterLiteralExpression = @"'[^']'";
    public const string WhitespaceExpression = @"\s+";
    public const string NewLineExpression = @"\r\n|[\r\n]";

    private readonly LinguisticExpression[] languages = null!;

    // todo: need to add a regex item as required for values in TokenType
    public LinguisticContext(LanguageSpecification language)
    {
        ArgumentNullException.ThrowIfNull(language);

        var reservedWordExpression = language.ReservedWords.Any()
            ? $@"(?:{String.Join("|", language.ReservedWords.Select(Regex.Escape))})(?!\w)"
            : String.Empty;

        var booleanLiteralExpression = language.Literals.Boolean.Any()
            ? $@"(?:{String.Join("|", language.Literals.Boolean.Select(Regex.Escape))})(?!\w)"
            : String.Empty;

        // todo: this expression needs to match operator between a (literal or identifier) and a (literal or identifier)
        var infixOperatorExpression = language.Operators.Infix.Any()
            ? String.Join("|", language.Operators.Infix.Select(Regex.Escape))
            : String.Empty;

        // todo: this expression needs look-ahead to match operator followed by literal or identifier
        var prefixOperatorExpression = language.Operators.Prefix.Any()
            ? String.Join("|", language.Operators.Prefix.Select(Regex.Escape))
            : String.Empty;

        // todo: this expression needs to match preceded by literal or identifier
        var postfixOperatorExpressoin = language.Operators.Postfix.Any()
            ? String.Join("|", language.Operators.Postfix.Select(Regex.Escape))
            : String.Empty;

        // todo: this expression needs to match preceded by literal or identifier
        var postfixOperatorExpression = language.Operators.Postfix.Any()
            ? String.Join("|", language.Operators.Postfix.Select(Regex.Escape))
            : String.Empty;

        var punctuationExpression = language.Punctuation.Any()
            ? String.Join("|", language.Punctuation.Select(Regex.Escape))
            : String.Empty;

        var commentExpressions = language
            .Literals
            .CommentPrefixes
            .Select(s => $@"{Regex.Escape(s)}.*?(?=\r?\n|$)");
        var commentExpression = language.Literals.CommentPrefixes.Any()
            ? String.Join("|", commentExpressions)
            : String.Empty;

        languages = new[]
        {
            new {
                Type = TokenType.ReservedWord,
                Expression = String.IsNullOrEmpty(reservedWordExpression)
                    ? String.Empty
                    : $@"\G(?:{reservedWordExpression})"
            },
            new {
                Type = TokenType.BooleanLiteral,
                Expression = String.IsNullOrEmpty(booleanLiteralExpression)
                    ? String.Empty
                    : $@"\G(?:{booleanLiteralExpression})"
            },
            new {
                Type = TokenType.Identifier,
                Expression = String.IsNullOrEmpty(reservedWordExpression)
                    ? $@"\G(?:{IdentifierExpression})"
                    : $@"\G(?!{reservedWordExpression})(?:{IdentifierExpression})"
            },
            new {
                Type = TokenType.NumericLiteral,
                Expression = $@"\G(?:{NumericLiteralExpression})"
            },
            new {
                Type = TokenType.StringLiteral,
                Expression = $@"\G(?:{StringLiteralExpression})"
            },
            new {
                Type = TokenType.CharacterLiteral,
                Expression = $@"\G(?:{CharacterLiteralExpression})"
            },
            new {
                Type = TokenType.InfixOperator,
                Expression = String.IsNullOrEmpty(infixOperatorExpression)
                    ? String.Empty
                    : $@"\G(?:{infixOperatorExpression})"
            },
            new {
                Type = TokenType.PrefixOperator,
                Expression = String.IsNullOrEmpty(prefixOperatorExpression)
                    ? String.Empty
                    : $@"\G(?:{prefixOperatorExpression})"
            },
            new {
                Type = TokenType.PostfixOperator,
                Expression = String.IsNullOrEmpty(postfixOperatorExpression)
                    ? String.Empty
                    : $@"\G(?:{postfixOperatorExpression})"
            },
            new {
                Type = TokenType.Punctuation,
                Expression = String.IsNullOrEmpty(punctuationExpression)
                    ? String.Empty
                    : $@"\G(?:{punctuationExpression})"
            },
            new {
                Type = TokenType.NewLine,
                Expression = $@"\G(?:{NewLineExpression})"
            },
            new {
                Type = TokenType.Whitespace,
                Expression = $@"\G(?:{WhitespaceExpression})"
            },
            new {
                Type = TokenType.Comment,
                Expression  = String.IsNullOrEmpty(commentExpression)
                    ? String.Empty
                    : $@"\G(?:{commentExpression})"
            },        }
        .Where(e => !String.IsNullOrEmpty(e.Expression))
        .Select(e => new LinguisticExpression(
            e.Type,
            new Regex(
                e.Expression,
                RegexOptions.CultureInvariant |
                RegexOptions.ExplicitCapture |
                RegexOptions.Compiled)))
        .ToArray();
    }

    public int Length => languages.Length;
    public LinguisticExpression this[int i] => languages[i];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<LinguisticExpression> AsSpan()
    {
        return languages.AsSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<LinguisticExpression> GetEnumerator()
    {
        return (IEnumerator<LinguisticExpression>)languages.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
