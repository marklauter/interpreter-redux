using System.Text.RegularExpressions;

namespace Lexi;

internal partial class AuxillaryPatterns
{
    [GeneratedRegex(@"\G\r\n|[\r\n]", TokenPattern.PatternOptions)]
    public static partial Regex NewLine();

    [GeneratedRegex(@"\G\s+", TokenPattern.PatternOptions)]
    public static partial Regex Whitespace();

    [GeneratedRegex(@"\G\-?\d+", TokenPattern.PatternOptions)]
    public static partial Regex IntegerLiteral();

    [GeneratedRegex(@"\G\-?\d+\.\d+", TokenPattern.PatternOptions)]
    public static partial Regex FloatingPointLiteral();

    [GeneratedRegex(@"\G\-?\d+(?:\.\d+)?[eE]\-?\d+", TokenPattern.PatternOptions)]
    public static partial Regex ScientificNotationLiteral();

    [GeneratedRegex(@"\G""(?:[^""\\\n\r]|\\.)*""", TokenPattern.PatternOptions)]
    public static partial Regex QuotedStringLiteral();

    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    [GeneratedRegex(@"\G'[^']'", TokenPattern.PatternOptions)]
    public static partial Regex CharacterLiteral();

    [GeneratedRegex(@"\G[a-zA-Z_]\w*", TokenPattern.PatternOptions)]
    public static partial Regex Identifier();
}
