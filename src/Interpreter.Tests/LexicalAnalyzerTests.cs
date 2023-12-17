using Interpreter.LexicalAnalysis;

namespace Interpreter.Tests;

public sealed class LexicalAnalyzerTests(ILexicalAnalyzer lexicalAnalyzer)
{
    private readonly ILexicalAnalyzer lexicalAnalyzer = lexicalAnalyzer ?? throw new ArgumentNullException(nameof(lexicalAnalyzer));

    [Fact]
    public void Analyze_Returns_Matches()
    {
        var source = "if (a + b) { let c = a + b; }";
        var matches = lexicalAnalyzer.Analyze(source);
        Assert.Equal(15, matches.Count);
        Assert.Equal("if", matches[0].Value);
        Assert.Equal("(", matches[1].Value);
        Assert.Equal("a", matches[2].Value);
        Assert.Equal("+", matches[3].Value);
        Assert.Equal("b", matches[4].Value);
        Assert.Equal(")", matches[5].Value);
        Assert.Equal("{", matches[6].Value);
        Assert.Equal("let", matches[7].Value);
        Assert.Equal("c", matches[8].Value);
        Assert.Equal("=", matches[9].Value);
        Assert.Equal("a", matches[10].Value);
        Assert.Equal("+", matches[11].Value);
        Assert.Equal("b", matches[12].Value);
        Assert.Equal(";", matches[13].Value);
        Assert.Equal("}", matches[14].Value);
    }
}
