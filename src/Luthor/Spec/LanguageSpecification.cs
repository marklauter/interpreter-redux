namespace Luthor.Spec;

public sealed class LanguageSpecification
{
    public OperatorSpecifications Operators { get; init; } = new();
    public LiteralSpecification Literals { get; init; } = new();
    public IEnumerable<string> ReservedWords { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Punctuation { get; init; } = Array.Empty<string>();
}
