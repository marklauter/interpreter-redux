using Luthor.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Luthor.Tests;

public sealed class Startup
{
    private readonly LanguageSpecification language = new()
    {
        Operators = new string[] { "!", "~", "++", "--", "+", "-", "/", "*", "%", "|", "&", "?", "=", "<", ">", ">=", "<=", "^", },
        BooleanLiterals = new string[] { "true", "false" },
        ReservedWords = new string[] { "if", "else", "let" },
        CommentPrefixes = new string[] { "//", "##" },
        CircumfixDelimiterPairs = new CircumfixPair[]
        {
            new ("(", ")"),
            new ("{", "}"),
            new ("[", "]"),
            new ("<", "/>"),
        },
        InfixDelimiters = new string[] { ",", ";", ".", ":", },
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(language);
        services.TryAddSingleton(new LinguisticContext(language));
    }
}
