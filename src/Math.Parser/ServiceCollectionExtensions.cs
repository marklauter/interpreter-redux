using Lexi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Math.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        var builder = LexiPatternBuilder
            .Create()
            .AddBooleanFalseLiteral("false", TokenIds.FALSE)
            .AddBooleanFalseLiteral("true", TokenIds.TRUE)
            .AddIntegerLiteral(TokenIds.INTEGER_LITERAL)
            .AddFloatingPointLiteral(TokenIds.FLOATING_POINT_LITERAL)
            .AddScientificNotationLiteral(TokenIds.SCIENTIFIC_NOTATION_LITERAL)
            .AddOperator(@"\+", TokenIds.ADD)
            .AddOperator("-", TokenIds.SUBTRACT)
            .AddOperator(@"\*", TokenIds.MULTIPLY)
            .AddOperator("/", TokenIds.DIVIDE)
            .AddOperator("%", TokenIds.MODULUS)
            .AddOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .AddClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS);

        services.TryAddTransient(serviceProvider => builder.Build());
        services.TryAddTransient<Parser>();

        return services;
    }
}
