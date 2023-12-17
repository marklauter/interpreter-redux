namespace Interpreter.LexicalAnalysis;

public sealed class LexicalAnalyzerOptions
{
    public IEnumerable<string> Keywords { get; set; } = null!;
    public IEnumerable<string> Operators { get; set; } = null!;
    public IEnumerable<string> PunctuationMarks { get; set; } = null!;
}
