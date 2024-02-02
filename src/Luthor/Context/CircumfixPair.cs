using System.Diagnostics.CodeAnalysis;

namespace Luthor.Context;

[ExcludeFromCodeCoverage]
public sealed record CircumfixPair(
    string Open,
    string Close);
