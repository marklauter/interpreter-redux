using System.Text.RegularExpressions;

namespace Lexi;

internal partial class AuxillaryPatterns
{
    private const RegexOptions PatternOptions =
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline |
        RegexOptions.CultureInvariant;

    [GeneratedRegex(@"\G\r\n|[\r\n]", PatternOptions)]
    public static partial Regex NewLine();

    [GeneratedRegex(@"\G\s+", PatternOptions)]
    public static partial Regex Whitespace();

    [GeneratedRegex(@"\G\-?\d+", PatternOptions)]
    public static partial Regex IntegerLiteral();

    [GeneratedRegex(@"\G\-?\d+\.\d+", PatternOptions)]
    public static partial Regex FloatingPointLiteral();

    [GeneratedRegex(@"\G\-?\d+(?:\.\d+)?[eE]\-?\d+", PatternOptions)]
    public static partial Regex ScientificNotationLiteral();

    [GeneratedRegex(@"\G""(?:[^""\\\n\r]|\\.)*""", PatternOptions)]
    public static partial Regex QuotedStringLiteral();

    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    [GeneratedRegex(@"\G'[^']'", PatternOptions)]
    public static partial Regex CharacterLiteral();

    [GeneratedRegex(@"\G[a-zA-Z_]\w*", PatternOptions)]
    public static partial Regex Identifier();
}
