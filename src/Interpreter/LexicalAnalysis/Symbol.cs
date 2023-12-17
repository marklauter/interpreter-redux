namespace Interpreter.LexicalAnalysis;

internal readonly record struct Symbol(
    SymbolType Type,
    int Offset,
    int Length);
