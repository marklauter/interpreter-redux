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
                items
                    .OrderByDescending(o => o.Length)
                    .Select(Regex.Escape))
            : null;

        return pattern is not null;
    }
}
