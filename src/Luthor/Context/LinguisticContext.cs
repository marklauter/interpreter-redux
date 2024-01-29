using Luthor.Spec;
using Luthor.Tokens;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public sealed class LinguisticContext
    : IEnumerable<LinguisticExpression>
{
    // todo: an idea for later
    // [GeneratedRegex]
    // https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators

    private const string Identifiers = @"[a-zA-Z_]\w*";
    private const string StringLiterals = @"""(?:[^""\\\n\r]|\\.)*""";
    private const string NumericLiterals = @"\b\d+(?:\.\d+)?\b";
    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    private const string CharacterLiterals = @"'[^']'";
    private const string Whitespace = @"\s+";
    private const string NewLine = @"\r\n|[\r\n]";

    private readonly ReadOnlyDictionary<TokenType, LinguisticExpression> map;
    private readonly LinguisticExpression[] languages = null!;

    public LinguisticContext(LanguageSpecification spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var reservedWords = spec.ReservedWords.Any()
            ? $@"(?:{String.Join("|", spec.ReservedWords.Select(Regex.Escape))})(?!\w)"
            : String.Empty;

        var booleanLiterals = spec.Literals.Boolean.Any()
            ? $@"(?:{String.Join("|", spec.Literals.Boolean.Select(Regex.Escape))})(?!\w)"
            : String.Empty;

        var identifiersAndLiterals = String.IsNullOrEmpty(booleanLiterals)
            ? $@"{Identifiers}|{StringLiterals}|{NumericLiterals}|{CharacterLiterals}"
            : $@"{Identifiers}|{StringLiterals}|{NumericLiterals}|{CharacterLiterals}|{booleanLiterals}";

        var infixOperators = spec.Operators.Infix.Any()
            ? String.Join("|", spec.Operators.Infix.Select(Regex.Escape))
            : String.Empty;

        infixOperators = String.IsNullOrEmpty(infixOperators)
            ? String.Empty
            : $@"(?<=(?:{identifiersAndLiterals})\s*)(?:{infixOperators})(?=\s*(?:{identifiersAndLiterals}))";

        var prefixOperators = spec.Operators.Prefix.Any()
            ? String.Join("|", spec.Operators.Prefix.Select(Regex.Escape))
            : String.Empty;
        prefixOperators = String.IsNullOrEmpty(prefixOperators)
            ? String.Empty
            : $@"(?<!(?:{identifiersAndLiterals})\s*)(?:{prefixOperators})(?=\s*(?:{identifiersAndLiterals}))";

        var postfixOperators = spec.Operators.Postfix.Any()
            ? String.Join("|", spec.Operators.Postfix.Select(Regex.Escape))
            : String.Empty;
        postfixOperators = String.IsNullOrEmpty(postfixOperators)
            ? String.Empty
            : $@"(?<=(?:{identifiersAndLiterals})\s*)(?:{postfixOperators})(?!\s*(?:{identifiersAndLiterals}))";

        var punctuation = spec.Punctuation.Any()
            ? String.Join("|", spec.Punctuation.Select(Regex.Escape))
            : String.Empty;

        var comments = spec.Literals.CommentPrefixes.Any()
            ? String.Join("|", spec
                .Literals
                .CommentPrefixes
                .Select(s => $@"{Regex.Escape(s)}.*?(?=\r?\n|$)"))
            : String.Empty;

        var regexs = new[]
        {
            new {
                Type = TokenType.Identifier,
                Expression = String.IsNullOrEmpty(reservedWords)
                    ? String.IsNullOrEmpty(booleanLiterals)
                        ? $@"\G(?:{Identifiers})"
                        : $@"\G(?!{booleanLiterals})(?:{Identifiers})"
                    : String.IsNullOrEmpty(booleanLiterals)
                        ? $@"\G(?!{reservedWords})(?:{Identifiers})"
                        : $@"\G(?!{reservedWords}|{booleanLiterals})(?:{Identifiers})"
            },
            new {
                Type = TokenType.ReservedWord,
                Expression = String.IsNullOrEmpty(reservedWords)
                    ? String.Empty
                    : $@"\G(?:{reservedWords})"
            },
            new {
                Type = TokenType.BooleanLiteral,
                Expression = String.IsNullOrEmpty(booleanLiterals)
                    ? String.Empty
                    : $@"\G(?:{booleanLiterals})"
            },
            new {
                Type = TokenType.NumericLiteral,
                Expression = $@"\G(?:{NumericLiterals})"
            },
            new {
                Type = TokenType.StringLiteral,
                Expression = $@"\G(?:{StringLiterals})"
            },
            new {
                Type = TokenType.CharacterLiteral,
                Expression = $@"\G(?:{CharacterLiterals})"
            },
            new {
                Type = TokenType.NewLine,
                Expression = $@"\G(?:{NewLine})"
            },
            new {
                Type = TokenType.Whitespace,
                Expression = $@"\G(?:{Whitespace})"
            },
            new {
                Type = TokenType.InfixOperator,
                Expression = String.IsNullOrEmpty(infixOperators)
                    ? String.Empty
                    : $@"\G(?:{infixOperators})"
            },
            new {
                Type = TokenType.PrefixOperator,
                Expression = String.IsNullOrEmpty(prefixOperators)
                    ? String.Empty
                    : $@"\G(?:{prefixOperators})"
            },
            new {
                Type = TokenType.PostfixOperator,
                Expression = String.IsNullOrEmpty(postfixOperators)
                    ? String.Empty
                    : $@"\G(?:{postfixOperators})"
            },
            new {
                Type = TokenType.Punctuation,
                Expression = String.IsNullOrEmpty(punctuation)
                    ? String.Empty
                    : $@"\G(?:{punctuation})"
            },
            new {
                Type = TokenType.Comment,
                Expression  = String.IsNullOrEmpty(comments)
                    ? String.Empty
                    : $@"\G(?:{comments})"
            },
        }
        .Where(e => !String.IsNullOrEmpty(e.Expression))
        .Select(e => new LinguisticExpression(
            e.Type,
            new Regex(
                e.Expression,
                RegexOptions.CultureInvariant |
                RegexOptions.ExplicitCapture |
                RegexOptions.Compiled)));

        map = regexs
            .ToDictionary(e => e.Type, e => e)
            .AsReadOnly();
        languages = [.. map.Values];
    }

    public int Length => languages.Length;
    public LinguisticExpression this[int i] => languages[i];
    public LinguisticExpression this[TokenType key] => map[key];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<LinguisticExpression> AsReadOnlySpan()
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
