namespace Interpreter.LexicalAnalysis;

public sealed record Symbol(
    int Offset,
    int Length,
    SymbolType Type,
    string Value);

