using System.Diagnostics.CodeAnalysis;

namespace Luthor.Symbols;

[ExcludeFromCodeCoverage]
public sealed record CircumfixPair(
    string Open,
    string Close);
