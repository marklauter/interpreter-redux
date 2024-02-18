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
            ++line;
            offset += match.Length;
        }

        match = WhitespaceExpression()
            .Match(source, offset);
        if (match.Success)
        {
            offset += match.Length;
        }

        var length = patterns.Length;
        for (var i = 0; i < length; ++i)
        {
            var symbol = patterns[i].Match(source, offset, line);
            if (symbol.TokenId > TokenPattern.NoMatch)
            {
                return new(
                    new(source, offset + symbol.Length, line),
                    symbol);
            }
        }

        return new(
            new(source, offset, line),
            new(offset, 0, TokenPattern.Error, line));
    }

    [GeneratedRegex(@"\G\r\n|[\r\n]", RegexConstants.Options)]
    private static partial Regex NewLineExpression();

    [GeneratedRegex(@"\G\s+", RegexConstants.Options)]
    private static partial Regex WhitespaceExpression();
}
