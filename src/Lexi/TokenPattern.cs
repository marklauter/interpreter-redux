using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Lexi;

[DebuggerDisplay("{id}, {regex}")]
public sealed class TokenPattern(
    Regex regex,
    Tokens tokenClass,
    int id)
{
    public TokenPattern(
        string pattern,
        Tokens tokenClass,
        int id)
        : this(
              new Regex(pattern ?? throw new ArgumentNullException(nameof(pattern)), PatternOptions),
              tokenClass,
              id)
    { }

    public const int EndOfSource = -1;
    public const int LexError = -2;

    public const RegexOptions PatternOptions =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private readonly int id = id;
    private readonly Tokens tokenClass = tokenClass;
    private readonly Regex regex = regex ??
        throw new ArgumentNullException(nameof(regex));

    internal Symbol Match(
        string source,
        int offset)
    {
        var match = regex.Match(source, offset);
        return match.Success
           ? new(match.Index, match.Length, tokenClass, id)
           : new(offset, 0, tokenClass | Tokens.NoMatch, id);
    }
}
