using Luthor.Tokens;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public readonly record struct LinguisticExpression(
    TokenType Type,
    Regex Regex);
