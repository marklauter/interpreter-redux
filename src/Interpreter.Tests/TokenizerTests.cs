using Interpreter.Tokens;

namespace Interpreter.Tests;

public class TokenizerTests
{
    private readonly Tokenizer tokenizer = new();

    [Fact]
    public void Tokenize_Returns_Tokens()
    {
        var page = "Hello, World.";
        var tokens = tokenizer.Tokenize(page);
        Assert.True(tokens.Any());
    }

    [Fact]
    public void Tokenize_Returns_Correct_Number_Of_Tokens()
    {
        var page = "0 2 4 6";
        var tokens = tokenizer.Tokenize(page);
        Assert.Equal(4, tokens.Count());
    }

    [Fact]
    public void Tokenize_Returns_Tokens_With_Correct_Offsets()
    {
        var page = "0 2 4 6";
        var tokens = tokenizer
            .Tokenize(page)
            .ToArray();
        Assert.Equal(0, tokens[0].Offset);
        Assert.Equal(2, tokens[1].Offset);
        Assert.Equal(4, tokens[2].Offset);
        Assert.Equal(6, tokens[3].Offset);
    }

    [Fact]
    public void Tokenize_Returns_Tokens_With_Correct_Lengths()
    {
        var page = "1 22 333 4444";
        var tokens = tokenizer
            .Tokenize(page)
            .ToArray();
        Assert.Equal(1, tokens[0].Length);
        Assert.Equal(2, tokens[1].Length);
        Assert.Equal(3, tokens[2].Length);
        Assert.Equal(4, tokens[3].Length);
    }
}
