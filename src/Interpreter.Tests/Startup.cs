using Interpreter.LexicalAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interpreter.Tests;

public sealed class Startup
{
    private readonly IConfiguration configuration;

    public Startup()
    {
        configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "LexicalAnalyzerOptions:Keywords:0", "if" },
                { "LexicalAnalyzerOptions:Keywords:1", "else" },
                { "LexicalAnalyzerOptions:Keywords:2", "let" },
                { "LexicalAnalyzerOptions:Operators:0", "+" },
                { "LexicalAnalyzerOptions:Operators:1", "-" },
                { "LexicalAnalyzerOptions:Operators:2", "/" },
                { "LexicalAnalyzerOptions:Operators:3", "*" },
                { "LexicalAnalyzerOptions:Operators:4", "%" },
                { "LexicalAnalyzerOptions:Operators:5", "|" },
                { "LexicalAnalyzerOptions:Operators:6", "&" },
                { "LexicalAnalyzerOptions:Operators:7", "?" },
                { "LexicalAnalyzerOptions:Operators:8", "=" },
                { "LexicalAnalyzerOptions:Operators:9", "<" },
                { "LexicalAnalyzerOptions:Operators:10", ">" },
                { "LexicalAnalyzerOptions:Operators:11", ">=" },
                { "LexicalAnalyzerOptions:Operators:12", "<=" },
                { "LexicalAnalyzerOptions:PunctuationMarks:0", "(" },
                { "LexicalAnalyzerOptions:PunctuationMarks:1", ")" },
                { "LexicalAnalyzerOptions:PunctuationMarks:2", "{" },
                { "LexicalAnalyzerOptions:PunctuationMarks:3", "}" },
                { "LexicalAnalyzerOptions:PunctuationMarks:4", "[" },
                { "LexicalAnalyzerOptions:PunctuationMarks:5", "]" },
                { "LexicalAnalyzerOptions:PunctuationMarks:6", "." },
                { "LexicalAnalyzerOptions:PunctuationMarks:7", ":" },
                { "LexicalAnalyzerOptions:PunctuationMarks:8", ";" },
                { "LexicalAnalyzerOptions:PunctuationMarks:9", "\"" },
                { "LexicalAnalyzerOptions:PunctuationMarks:10", "'" },
            })
            .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddLexicalAnalyzer(configuration);
    }
}
