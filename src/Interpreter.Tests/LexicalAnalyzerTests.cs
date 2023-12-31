﻿using Interpreter.LexicalAnalysis;

namespace Interpreter.Tests;

public sealed class LexicalAnalyzerTests(ILexicalAnalyzer lexicalAnalyzer)
{
    private readonly ILexicalAnalyzer lexicalAnalyzer = lexicalAnalyzer ?? throw new ArgumentNullException(nameof(lexicalAnalyzer));

    [Fact]
    public void ReadSymbols_Returns_SymbolTable()
    {
        var source = "if (a + b) { let c = a + b; }";

        var symbols = lexicalAnalyzer
            .ReadSymbols(source)
            .ToArray();
        Assert.Equal(15, symbols.Length);

        Assert.Equal("if", symbols[0].Value);
        Assert.Equal(0, symbols[0].Offset);
        Assert.Equal(2, symbols[0].Length);

        Assert.Equal("(", symbols[1].Value);
        Assert.Equal(3, symbols[1].Offset);
        Assert.Equal(1, symbols[1].Length);

        Assert.Equal("a", symbols[2].Value);
        Assert.Equal("+", symbols[3].Value);
        Assert.Equal("b", symbols[4].Value);
        Assert.Equal(")", symbols[5].Value);
        Assert.Equal("{", symbols[6].Value);

        Assert.Equal("let", symbols[7].Value);
        Assert.Equal(13, symbols[7].Offset);
        Assert.Equal(3, symbols[7].Length);

        Assert.Equal("c", symbols[8].Value);
        Assert.Equal("=", symbols[9].Value);
        Assert.Equal("a", symbols[10].Value);
        Assert.Equal("+", symbols[11].Value);
        Assert.Equal("b", symbols[12].Value);
        Assert.Equal(";", symbols[13].Value);
        Assert.Equal("}", symbols[14].Value);
    }
}
