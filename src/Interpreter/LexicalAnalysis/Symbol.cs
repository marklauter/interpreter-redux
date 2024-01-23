namespace Interpreter.LexicalAnalysis;

public sealed record Symbol(
    int Offset,
    int Length,
    Symbols Type,
    string Value);

