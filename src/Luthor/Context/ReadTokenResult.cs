namespace Luthor.Context;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct ReadTokenResult(
    Token token,
    int nextOffset,
    int lastNewLineOffset,
    int lineNumber)
{
    public readonly Token Token = token;
    public readonly int NextOffset = nextOffset;
    public readonly int LastNewLineOffset = lastNewLineOffset;
    public readonly int LineNumber = lineNumber;
    public bool IsMatch => Token.Type.IsMatch();
}
