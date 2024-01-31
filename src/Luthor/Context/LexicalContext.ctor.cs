using System.Collections.ObjectModel;

namespace Luthor.Context;

public sealed partial class LexicalContext
{
    private readonly ReadOnlyDictionary<Tokens, TokenMatcher> map;
    private readonly TokenMatcher[] matchers = null!;

    public LexicalContext(LanguageSpecification spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var index = 0;
        var matchers = new TokenMatcher[13];
        var map = new Dictionary<Tokens, TokenMatcher>();

        (index, var reservedWordPattern) = AddReservedWordMatcher(spec, matchers, map, index);
        (index, var booleanLiteralPattern) = AddBooleanLiteralMatcher(spec, matchers, map, index);
        index = AddIdentifierMatcher(matchers, map, index, booleanLiteralPattern, reservedWordPattern);
        index = AddNumericLiteralMatcher(matchers, map, index);
        index = AddStringLiteralMatcher(matchers, map, index);
        index = AddCharacterLiteralMatcher(matchers, map, index);
        index = AddNewLineMatcher(matchers, map, index);
        index = AddWhitespaceMatcher(matchers, map, index);
        index = AddInfixDelimiterMatcher(spec, matchers, map, index);
        index = AddCircumfixDelimiterMatcher(spec, matchers, map, index);
        index = AddCommentMatcher(spec, matchers, map, index);
        index = AddOperatorMatcher(spec, matchers, map, index);

        this.matchers = matchers[..index];
        this.map = map.AsReadOnly();
    }
}
