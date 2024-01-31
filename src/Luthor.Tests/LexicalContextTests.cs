using Luthor.Context;

namespace Luthor.Tests;

public sealed class LexicalContextTests(LanguageSpecification language)
{
    private readonly LanguageSpecification language = language ?? throw new ArgumentNullException(nameof(language));

    [Fact]
    public void CanCreateLexicalContext()
    {
        var context = new LexicalContext(language);
        Assert.NotNull(context);
    }
}
