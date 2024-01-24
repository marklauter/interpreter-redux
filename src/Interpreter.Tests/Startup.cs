using Luthor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Interpreter.Tests;

public sealed class Startup
{
    private readonly LanguageSpecification language = new()
    {
        InfixOperators = new string[] { "+", "-", "/", "*", "%", "|", "&", "?", "=", "<", ">", ">=", "<=", "^" },
        Keywords = new string[] { "if", "else", "let" },
        Punctuation = new string[] { "(", ")", "{", "}", "[", "]", ":", ";", "//" },
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(language);
    }
}
