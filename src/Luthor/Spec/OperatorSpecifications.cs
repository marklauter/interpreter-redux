namespace Luthor.Spec;

public sealed class OperatorSpecifications
{
    public IEnumerable<string> Prefix { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Infix { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Postfix { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Circumfix { get; init; } = Array.Empty<string>();
}
