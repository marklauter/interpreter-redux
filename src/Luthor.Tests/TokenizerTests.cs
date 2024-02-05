using Luthor.Symbols;
using Luthor.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Luthor.Tests;

[ExcludeFromCodeCoverage]
public sealed class TokenizerTests(TerminalSymbolSpec spec)
{
    private readonly TerminalSymbolSpec spec = spec ?? throw new ArgumentNullException(nameof(spec));

    [Fact]
    public void CanCreateTokenizers()
    {
        var tokenizers = new Tokenizers(spec);
        Assert.NotNull(tokenizers);
        var t = tokenizers.AsReadOnlySpan();
        Assert.True(t.Length > 0);
    }
}
