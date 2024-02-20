using Lexi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.RegularExpressions;

namespace Math.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        var builder = LexerBuilder
            .Create(RegexOptions.CultureInvariant)
            .MatchBooleanFalseLiteral("false", TokenIds.FALSE)
            .MatchBooleanTrueLiteral("true", TokenIds.TRUE)
            .MatchIntegerLiteral(TokenIds.INTEGER_LITERAL)
            .MatchFloatingPointLiteral(TokenIds.FLOATING_POINT_LITERAL)
            .MatchScientificNotationLiteral(TokenIds.SCIENTIFIC_NOTATION_LITERAL)
            .MatchOperator(@"\+", TokenIds.ADD)
            .MatchOperator("-", TokenIds.SUBTRACT)
            .MatchOperator(@"\*", TokenIds.MULTIPLY)
            .MatchOperator("/", TokenIds.DIVIDE)
            .MatchOperator("%", TokenIds.MODULUS)
            .MatchOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .MatchClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS);

        services.TryAddTransient(serviceProvider => builder.Build());
        services.TryAddTransient<Parser>();

        return services;
    }
}
