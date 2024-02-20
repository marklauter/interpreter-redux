using Lexi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.RegularExpressions;

namespace Predicate.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        var builder = LexerBuilder
            .CreateWithRegexOptions(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            .AddKeyword($"{nameof(TokenIds.FROM)}", TokenIds.FROM)
            .AddKeyword($"{nameof(TokenIds.WHERE)}", TokenIds.WHERE)
            .AddKeyword($"{nameof(TokenIds.SKIP)}", TokenIds.SKIP)
            .AddKeyword($"{nameof(TokenIds.TAKE)}", TokenIds.TAKE)
            .AddOperator("contains|c", TokenIds.CONTAINS)
            .AddOperator("startswith|sw", TokenIds.STARTS_WITH)
            .AddOperator("endswith|ew", TokenIds.ENDS_WITH)
            .AddOperator(@"and|&&", TokenIds.LOGICAL_AND)
            .AddOperator(@"or|\|\|", TokenIds.LOGICAL_OR)
            .AddIdentifier(TokenIds.IDENTIFIER)
            .AddBooleanTrueLiteral("true", TokenIds.TRUE)
            .AddBooleanFalseLiteral("false", TokenIds.FALSE)
            .AddIntegerLiteral(TokenIds.INTEGER_LITERAL)
            .AddFloatingPointLiteral(TokenIds.FLOATING_POINT_LITERAL)
            .AddScientificNotationLiteral(TokenIds.SCIENTIFIC_NOTATION_LITERAL)
            .AddStringLiteral(TokenIds.STRING_LITERAL)
            .AddCharacterLiteral(TokenIds.CHAR_LITERAL)
            .AddOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .AddClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS)
            .AddOperator("=|==", TokenIds.EQUAL)
            .AddOperator(">", TokenIds.GREATER_THAN)
            .AddOperator(">=", TokenIds.GREATER_THAN_OR_EQUAL)
            .AddOperator("<", TokenIds.LESS_THAN)
            .AddOperator("<=", TokenIds.LESS_THAN_OR_EQUAL)
            .AddOperator("!=", TokenIds.NOT_EQUAL);

        services.TryAddTransient(serviceProvider => builder.Build());
        services.TryAddTransient<Parser>();

        return services;
    }
}
