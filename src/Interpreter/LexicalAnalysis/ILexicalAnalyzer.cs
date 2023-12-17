using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

public interface ILexicalAnalyzer
{
    MatchCollection Analyze(string source);
}
