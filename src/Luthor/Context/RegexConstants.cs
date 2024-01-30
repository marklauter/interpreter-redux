namespace Luthor.Context;

public static class RegexConstants
{
    public const string Whitespace = @"\s+";
    public const string NewLine = @"\r\n|[\r\n]";
    public const string Identifiers = @"[a-zA-Z_]\w*";
    public const string StringLiterals = @"""(?:[^""\\\n\r]|\\.)*""";
    public const string NumericLiterals = @"\b\d+(?:\.\d+)?\b";
    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    public const string CharacterLiterals = @"'[^']'";
    public const string X = CharacterLiterals + NumericLiterals;
}
