namespace Luthor.Spec;

public sealed class LiteralSpecification
{
    public IEnumerable<string> Boolean { get; init; } = Array.Empty<string>();
    public IEnumerable<string> CommentPrefixes { get; init; } = Array.Empty<string>();
}
