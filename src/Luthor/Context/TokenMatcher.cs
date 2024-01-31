namespace Luthor.Context;

public delegate void TokenMatcher(
    string source,
    int offset,
    int lastNewLineOffset,
    int lineNumber,
    ref MatchResult match);
