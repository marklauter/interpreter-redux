using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public sealed class LanguageSpecification
{
    public IEnumerable<string> Operators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> BooleanLiterals { get; init; } = Array.Empty<string>();
    public IEnumerable<string> CommentPrefixes { get; init; } = Array.Empty<string>();
    public IEnumerable<string> ReservedWords { get; init; } = Array.Empty<string>();
    /// <summary>
    /// delimiters that appear in pairs, like parentheses, brackets, and braces
    /// </summary>
    public IEnumerable<CircumfixPair> CircumfixDelimiterPairs { get; init; } = Array.Empty<CircumfixPair>();
    /// <summary>
    /// delimiters that appear alone, like comma, semicolon, and period
    /// </summary>
    public IEnumerable<string> InfixDelimiters { get; init; } = Array.Empty<string>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetOperatorPattern(out string? pattern)
    {
        return TryGetSimplePattern(Operators, out pattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetBooleanLiteralPattern(out string? pattern)
    {
        pattern = BooleanLiterals.Any()
            ? $@"(?:{String.Join(
                "|",
                BooleanLiterals.Select(Regex.Escape))})(?!\w)"
            : null;

        return pattern is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCommentPattern(out string? pattern)
    {
        pattern = CommentPrefixes.Any()
            ? String.Join(
                "|",
                CommentPrefixes.Select(s =>
                    $@"{Regex.Escape(s)}.*?(?=\r?\n|$)"))
            : null;

        return pattern is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetReservedWordPattern(out string? pattern)
    {
        pattern = ReservedWords.Any()
            ? $@"(?:{String.Join(
                "|",
                ReservedWords.Select(Regex.Escape))})(?!\w)"
            : null;

        return pattern is not null;
    }

    private string GetCircumfixDelimiterOpenPattern(CircumfixPair pair)
    {
        // https://stackoverflow.com/questions/546433/regular-expression-to-match-balanced-parentheses
        // \((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\) 
        // open(?>open(?<c>)|[^openclose]+|close(?<-c>))*(?(c)(?!))close 

        var open = Regex.Escape(pair.Open);
        var closeIsSquareBracket = pair.Close.Equals("]", StringComparison.OrdinalIgnoreCase);
        var close = closeIsSquareBracket
            ? @"\]"
            : Regex.Escape(pair.Close);

        var innerClose = closeIsSquareBracket
            ? close
            : close[^1].ToString();

        return $@"{open}(?>{open}(?<c>)|[^{open}{innerClose}]+|{close}(?<-c>))*(?(c)(?!)){close}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCircumfixDelimiterOpenPattern(out string? pattern)
    {
        pattern = null;
        if (CircumfixDelimiterPairs.Any())
        {
            var patterns = CircumfixDelimiterPairs
                .Select(GetCircumfixDelimiterOpenPattern);
            pattern = String.Join("|", patterns);
        }

        return pattern is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCircumfixDelimiterClosePattern(out string? pattern)
    {
        return TryGetSimplePattern((IEnumerable<string>)CircumfixDelimiterPairs, out pattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetInfixDelimiterPattern(out string? pattern)
    {
        return TryGetSimplePattern(InfixDelimiters, out pattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetSimplePattern(IEnumerable<string> items, out string? pattern)
    {
        pattern = items.Any()
            ? String.Join(
                "|",
                items.Select(Regex.Escape))
            : null;

        return pattern is not null;
    }
}
