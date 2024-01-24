namespace Interpreter.LexicalAnalysis;

public sealed class LanguageSpecification
{
    public IEnumerable<string> Keywords { get; init; } = Array.Empty<string>();
    public IEnumerable<string> InfixOperators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> PrefixOperators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> PostfixOperators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Punctuation { get; init; } = Array.Empty<string>();
}
