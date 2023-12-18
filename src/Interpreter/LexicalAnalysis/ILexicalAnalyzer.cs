namespace Interpreter.LexicalAnalysis;

public interface ILexicalAnalyzer
{
    IEnumerable<Symbol> ReadSymbols(string source);
}
