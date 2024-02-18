using System.Diagnostics.CodeAnalysis;

namespace Lexi;

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct Symbol(
    int offset,
    int length,
    Tokens token,
    int tokenId,
    int line)
{
    public readonly int Offset = offset;
    public readonly int Length = length;
    public readonly Tokens Token = token;
    public readonly int TokenId = tokenId;
    public readonly int Line = line;
}
