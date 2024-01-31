namespace Luthor.Context;

public delegate ReadTokenResult TokenReader(
    string source,
    int offset,
    int lastNewLineOffset,
    int lineNumber);
