using Lexi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.RegularExpressions;

namespace Predicate.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        var builder = Vocabulary
            .Create(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            .MatchKeyword($"{nameof(TokenIds.FROM)}", TokenIds.FROM)
            .MatchKeyword($"{nameof(TokenIds.WHERE)}", TokenIds.WHERE)
            .MatchKeyword($"{nameof(TokenIds.SKIP)}", TokenIds.SKIP)
            .MatchKeyword($"{nameof(TokenIds.TAKE)}", TokenIds.TAKE)
            .MatchOperator($"{nameof(TokenIds.CONTAINS)}", TokenIds.CONTAINS)
            .MatchOperator("startswith|sw", TokenIds.STARTS_WITH)
            .MatchOperator("endswith|ew", TokenIds.ENDS_WITH)
            .MatchOperator(@"and|&&", TokenIds.LOGICAL_AND)
            .MatchOperator(@"or|\|\|", TokenIds.LOGICAL_OR)
            .MatchIdentifier(TokenIds.IDENTIFIER)
            .MatchBooleanTrueLiteral("true", TokenIds.TRUE)
            .MatchBooleanFalseLiteral("false", TokenIds.FALSE)
            .MatchIntegerLiteral(TokenIds.INTEGER_LITERAL)
            .MatchFloatingPointLiteral(TokenIds.FLOATING_POINT_LITERAL)
            .MatchScientificNotationLiteral(TokenIds.SCIENTIFIC_NOTATION_LITERAL)
            .MatchStringLiteral(TokenIds.STRING_LITERAL)
            .MatchCharacterLiteral(TokenIds.CHAR_LITERAL)
            .MatchOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .MatchClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS)
            .MatchOperator("=|==", TokenIds.EQUAL)
            .MatchOperator(">", TokenIds.GREATER_THAN)
            .MatchOperator(">=", TokenIds.GREATER_THAN_OR_EQUAL)
            .MatchOperator("<", TokenIds.LESS_THAN)
            .MatchOperator("<=", TokenIds.LESS_THAN_OR_EQUAL)
            .MatchOperator("!=", TokenIds.NOT_EQUAL);

        services.TryAddTransient(serviceProvider => builder.Build());
        services.TryAddTransient<Parser>();

        return services;
    }
}
