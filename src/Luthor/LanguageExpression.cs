using System.Text.RegularExpressions;

namespace Luthor;

public readonly record struct LanguageExpression(
    TokenType Type,
    Regex Regex);
