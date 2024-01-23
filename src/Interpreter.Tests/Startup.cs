using Interpreter.LexicalAnalysis;
using Interpreter.SyntacticAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Interpreter.Tests;

public sealed class Startup
{
    private readonly Language language = new()
    {
        InfixOperators = new string[] { "+", "-", "/", "*", "%", "|", "&", "?", "=", "<", ">", ">=", "<=", "^" },
        Keywords = new string[] { "if", "else", "let" },
        Punctuation = new string[] { "(", ")", "{", "}", "[", "]", ":", ";", "//" },
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddTransient(services => new Lexer(language));
        services.TryAddTransient<Parser>();
    }
}
