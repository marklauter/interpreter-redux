using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Symbols;

public sealed class TerminalSymbolSpec
{
    public TerminalSymbolOptions Options { get; init; } = TerminalSymbolOptions.None;

    private readonly IEnumerable<string> operators = Array.Empty<string>();
    public IEnumerable<string> Operators
    {
        get => operators;
        init
        {
            if (value.Any())
            {
                Options |= TerminalSymbolOptions.IncludeOperators;
                operators = value;
            }
        }
    }

    private readonly IEnumerable<string> booleanLiterals = Array.Empty<string>();
    public IEnumerable<string> BooleanLiterals
    {
        get => booleanLiterals;
        init
        {
            if (value.Any())
            {
                Options |= TerminalSymbolOptions.IncludeBooleanLiterals;
                booleanLiterals = value;
            }
        }
    }

    private IEnumerable<string> commentPrefixes = Array.Empty<string>();
    public IEnumerable<string> CommentPrefixes
    {
        get => commentPrefixes;
        init
        {
            if (value.Any())
            {
                Options |= TerminalSymbolOptions.IncludeComments;
                commentPrefixes = value;
            }
        }
    }

    private IEnumerable<string> reservedWords = Array.Empty<string>();
    public IEnumerable<string> ReservedWords
    {
        get => reservedWords;
        init
        {
            if (value.Any())
            {
                Options |= TerminalSymbolOptions.IncludeReservedWords;
                reservedWords = value;
            }
        }
    }

    /// <summary>
    /// delimiters that appear in pairs and encapsulate sets of symbols to create blocks or groupings, like parentheses, brackets, and braces
    /// </summary>
    private IEnumerable<CircumfixPair> circumfixDelimiterPairs = Array.Empty<CircumfixPair>();
    public IEnumerable<CircumfixPair> CircumfixDelimiterPairs
    {
        get => circumfixDelimiterPairs;
        init
        {
            if (value.Any())
            {
                Options |= TerminalSymbolOptions.IncludeCircumfixDelimiters;
                circumfixDelimiterPairs = value;
            }
        }
    }

    /// <summary>
    /// delimiters that appear alone and separate elements, like comma, semicolon, and period
    /// </summary>
    private IEnumerable<string> infixDelimiters = Array.Empty<string>();
    public IEnumerable<string> InfixDelimiters
    {
        get => infixDelimiters;
        init
        {
            if (value.Any())
            {
                Options |= TerminalSymbolOptions.IncludeInfixDelimiters;
                infixDelimiters = value;
            }
        }
    }

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
