using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

public readonly record struct LanguageExpression(
    TokenType Type,
    Regex Regex);
