using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interpreter.LexicalAnalysis;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddLexicalAnalyzer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        return services
            .AddOptions()
            .Configure<LexicalAnalyzerOptions>(configuration.GetSection(nameof(LexicalAnalyzerOptions)))
            .AddTransient<ILexicalAnalyzer, LexicalAnalyzer>();
    }
}
