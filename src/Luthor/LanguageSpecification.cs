namespace Luthor;

// todo: update to support more items from TokenType
public sealed class LanguageSpecification
{
    public IEnumerable<string> Keywords { get; init; } = Array.Empty<string>();
    public IEnumerable<string> InfixOperators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> PrefixOperators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> PostfixOperators { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Punctuation { get; init; } = Array.Empty<string>();
    public IEnumerable<string> SingleLineCommentIndicators { get; init; } = Array.Empty<string>();
}
