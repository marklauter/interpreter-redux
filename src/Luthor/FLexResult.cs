using Luthor.Tokens;

namespace Luthor;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct FLexResult(RefToken token, int position, int lastNewLineOffset, int lineNumber)
{
    public readonly RefToken Token = token;
    public readonly int Position = position;
    public readonly int LastNewLineOffset = lastNewLineOffset;
    public readonly int LineNumber = lineNumber;
}
