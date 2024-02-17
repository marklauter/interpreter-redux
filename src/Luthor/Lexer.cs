using Luthor.Tokens;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor;

[DebuggerDisplay("{code}, {regex}")]
public sealed class TokenDefinition(
   string pattern,
   int code)
{
    public const int NoMatchCode = -1;
    public const int EndOfSourceCode = -2;
    public const int ErrorCode = -3;

    private const RegexOptions Options =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private readonly int code = code;
    private readonly Regex regex = new(
        pattern ?? throw new ArgumentNullException(nameof(pattern)),
        Options);

    internal RefToken Match(
        string source,
        int line,
        int offset)
    {
        var match = regex.Match(source, offset);

        return match.Success
           ? new(line, match.Index, match.Length, code)
           : new(line, offset, 0, NoMatchCode);
    }
}

public sealed partial class Lexi(
    string source,
    TokenDefinition[] tokenDefs)
{
    private readonly string source = source
        ?? throw new ArgumentNullException(nameof(source));

    private readonly TokenDefinition[] tokenDefs = tokenDefs
        ?? throw new ArgumentNullException(nameof(tokenDefs));

    private int offset;
    private int line;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadSymbol(ref readonly RefToken token)
    {
        return token.Code == TokenDefinition.EndOfSourceCode
            ? "EOF"
            : token.Code == TokenDefinition.ErrorCode
            ? $"error at line: {token.Line}, column: {token.Offset}"
            : source[token.Offset..(token.Offset + token.Length)];
    }

    public RefToken NextToken()
    {
        if (offset >= source.Length)
        {
            return new RefToken(line, offset, 0, TokenDefinition.EndOfSourceCode);
        }

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

        var length = tokenDefs.Length;
        for (var i = 0; i < length; ++i)
        {
            var token = tokenDefs[i].Match(source, line, offset);
            if (token.Code != TokenDefinition.NoMatchCode)
            {
                offset += token.Length;
                return token;
            }
        }

        return new RefToken(line, offset, 0, TokenDefinition.ErrorCode);
    }

    [GeneratedRegex(@"\G\r\n|[\r\n]", RegexConstants.Options)]
    private static partial Regex NewLineExpression();

    [GeneratedRegex(@"\G\s+", RegexConstants.Options)]
    private static partial Regex WhitespaceExpression();
}

public sealed class Lexer(
    Tokenizers tokenizers,
    string source)
{
    private readonly Tokenizers tokenizers = tokenizers ?? throw new ArgumentNullException(nameof(tokenizers));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly List<int> newLineOffsets = [];
    private int offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int line, int column) LineAndColumn(int offset)
    {
        var offsets = newLineOffsets.ToArray().AsSpan();
        for (var line = 0; line < offsets.Length; ++line)
        {
            var newLineOffset = offsets[line];
            if (offset > newLineOffset)
            {
                return (line, offset - newLineOffset);
            }
        }

        return (1, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadSymbol(Token token)
    {
        if (token.IsEndOfSource())
        {
            return "EOF";
        }

        if (token.IsError())
        {
            var (lineNumber, column) = LineAndColumn(token.Offset);
            return $"{{\"line\": {lineNumber}, \"column\": {column}}}";
        }

        return source[token.Offset..(token.Offset + token.Length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadSymbol(string source, Token token)
    {
        return token.IsEndOfSource()
            ? "EOF"
            : source[token.Offset..(token.Offset + token.Length)];
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "span created with collection initialization can't be exposed outside the function")]
    public Token[] Tokens()
    {
        var tokens = new List<Token>();
        var token = NextToken();
        while (!token.IsEndOfSource()
            && !token.IsError())
        {
            tokens.Add(token);
            token = NextToken();
        }

        tokens.Add(token);

        offset = 0;

        return tokens
            .ToArray();
    }

    public Token NextToken()
    {
        if (offset >= source.Length)
        {
            return new Token(offset, 0, TokenType.EndOfSource);
        }

        var length = tokenizers.Length;
        for (var i = 0; i < length; ++i)
        {
            var match = tokenizers[i];
            var token = match(source, offset);
            if (token.IsMatch())
            {
                offset += token.Length;
                if (token.Type == TokenType.NewLine)
                {
                    // for error tracking. it helps establish the column where the error occurred
                    newLineOffsets.Add(token.Offset);
                }

                return token;
            }
        }

        return new Token(offset, 0, TokenType.Error);
    }
}
