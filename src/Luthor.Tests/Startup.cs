using Luthor.Spec;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Luthor.Tests;

public sealed class Startup
{
    private readonly LanguageSpecification language = new()
    {
        Operators = new OperatorSpecifications
        {
            Prefix = new string[] { "!", "~", "++", "--" },
            Infix = new string[] { "+", "-", "/", "*", "%", "|", "&", "?", "=", "<", ">", ">=", "<=", "^" },
            Postfix = new string[] { "++", "--", "l", "s", "ul", "us", "u" },
            Circumfix = new string[] { "()", "{}", "[]" },
        },

        Literals = new LiteralSpecification
        {
            Boolean = new string[] { "true", "false" },
            CommentPrefixes = new string[] { "//", "##" },
        },

        ReservedWords = new string[] { "if", "else", "let" },
        Punctuation = new string[] { "(", ")", "{", "}", "[", "]", ":", ";", "," },
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(language);
    }
}
