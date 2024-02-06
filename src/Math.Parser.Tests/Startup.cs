using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Math.Parser.Tests;

[ExcludeFromCodeCoverage]
public sealed class Startup
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "can't be static")]
    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddParser();
    }
}
