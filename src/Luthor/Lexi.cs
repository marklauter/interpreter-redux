using Luthor.Tokens;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor;

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct Symbol(
    int offset,
    int length,
    int tokenId,
    int line)
{
    public readonly int Offset = offset;
    public readonly int Length = length;
    public readonly int TokenId = tokenId;
    public readonly int Line = line;
}

public sealed record Script(
    string Source,
    int Offset,
    int Line)
{
    public Script(string source)
        : this(source, 0, 0)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEndOfSource()
    {
        return Offset >= Source.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadSymbol(ref readonly Symbol symbol)
    {
        return symbol.TokenId == TokenPattern.EndOfSource
            ? "EOF"
            : symbol.TokenId == TokenPattern.Error
                ? $"error at line: {symbol.Line}, column: {symbol.Offset}"
                : Source[symbol.Offset..(symbol.Offset + symbol.Length)];
    }
}

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct NextTokenResult(
    Script script,
    Symbol symbol)
{
    public readonly Script Script = script;
    public readonly Symbol Symbol = symbol;
}


//public class LexBuilder
//{
//    private readonly List<TokenPattern> patterns = [];

//    public Lexi Build()
//    {
//        return new Lexi(source, patterns.ToArray());
//    }
//}

[DebuggerDisplay("{id}, {regex}")]
public sealed class TokenPattern(
   string pattern,
   int id)
{
    public const int NoMatch = -1;
    public const int EndOfSource = -2;
    public const int Error = -3;

    private const RegexOptions Options =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private readonly int id = id;
    private readonly Regex regex = new(
        pattern ?? throw new ArgumentNullException(nameof(pattern)),
        Options);

    internal Symbol Match(
        string source,
        int offset,
        int line)
    {
        var match = regex.Match(source, offset);
        return match.Success
           ? new(match.Index, match.Length, id, line)
           : new(offset, 0, NoMatch, line);
    }
}

public sealed partial class Lexi(
    TokenPattern[] patterns)
{
    private readonly TokenPattern[] patterns = patterns
        ?? throw new ArgumentNullException(nameof(patterns));

    private readonly ref struct SymbolMatch(
        Symbol symbol,
        int index)
    {
        public readonly Symbol Symbol = symbol;
        public readonly int Index = index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareSymbolMatch(
        SymbolMatch l,
        SymbolMatch r)
    {
        var lLenth = l.Symbol.Length;
        var rLength = r.Symbol.Length;
        var lIndex = l.Index;
        var rIndex = r.Index;

        // longer match wins. tie goes to lowest index.
        // equal means no swap and probably an error (aka skill issue) in the pattern definitions.
        // probably should throw in that case, or create an error token
        return
            lLenth > rLength
            ? 1
            : lLenth < rLength
                ? -1
                : lIndex < rIndex
                    ? 1
                    : lIndex > rIndex
                        ? -1
                        : 0;
    }

    public NextTokenResult NextToken(Script script)
    {
        var offset = script.Offset;
        var line = script.Line;

        if (script.IsEndOfSource())
        {
            return new(
                script,
                new(offset, 0, TokenPattern.EndOfSource, line));
        }

        var source = script.Source;

        var match = NewLineExpression()
            .Match(source, offset);
        if (match.Success)
        {
            offset += match.Length;
            ++line;
        }

        match = WhitespaceExpression()
            .Match(source, offset);
        if (match.Success)
        {
            offset += match.Length;
        }

        // dragon book says perform all match tests, and return best match based on length and pattern definition index. longest match wins. ties to go to lowest index.
        // could replace this loop with a pair of functions, one for this strategy and another to return first match.
        // first match is faster, but less tolerent pattern definition ordering. everything is tradeoffs.
        var patterns = this.patterns.AsSpan();
        var length = patterns.Length;
        var bestMatch = default(SymbolMatch);
        for (var i = 0; i < length; ++i)
        {
            var latestMatch = new SymbolMatch(
                patterns[i].Match(source, offset, line),
                i);

            if (CompareSymbolMatch(latestMatch, bestMatch) > 0)
            {
                bestMatch = latestMatch;
            }
        }

        var symbol = bestMatch.Symbol;

        return symbol.TokenId > TokenPattern.NoMatch
            ? new(
                    new(source, offset + symbol.Length, line),
                    symbol)
            : new(
                new(source, offset, line),
                new(offset, 0, TokenPattern.Error, line));
    }

    [GeneratedRegex(@"\G\r\n|[\r\n]", RegexConstants.Options)]
    private static partial Regex NewLineExpression();

    [GeneratedRegex(@"\G\s+", RegexConstants.Options)]
    private static partial Regex WhitespaceExpression();
}
