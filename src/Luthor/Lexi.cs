using Luthor.Tokens;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor;

public sealed record Symbol(
    int Offset,
    int Length,
    int TokenId,
    int Line);

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

public sealed record NextTokenResult(
    Script Script,
    Symbol Symbol);


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


public sealed partial class Lexi(
    TokenPattern[] patterns)
{
    private readonly TokenPattern[] patterns = patterns
        ?? throw new ArgumentNullException(nameof(patterns));

    private readonly struct SymbolMatch(Symbol symbol, int index)
        : IComparable<SymbolMatch>
    {
        public readonly Symbol Symbol = symbol;
        public readonly int Index = index;

        /// <summary>
        /// sorts descending by length, then index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(SymbolMatch other)
        {
            var lenth = Symbol.Length;
            var otherLength = other.Symbol.Length;
            var index = Index;
            var otherIndex = other.Index;

            return lenth > otherLength
                ? -1
                : lenth < otherLength
                    ? 1
                    : index > otherIndex
                        ? -1
                        : index < otherIndex
                            ? 1
                            : 0;
        }
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

        // dragon book says perform all match tests, then sort matched items by length and index, and return first item if it is match
        var length = patterns.Length;
        var symbols = new SymbolMatch[length];
        for (var i = 0; i < length; ++i)
        {
            symbols[i] = new(
                patterns[i].Match(source, offset, line),
                i);
        }

        Array.Sort(symbols);
        var symbol = symbols[0].Symbol;

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
