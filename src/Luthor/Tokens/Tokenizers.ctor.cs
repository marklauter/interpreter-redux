using Luthor.Symbols;
using System.Collections.ObjectModel;

namespace Luthor.Tokens;

public sealed partial class Tokenizers
{
    private readonly ReadOnlyDictionary<TokenType, PatternMatcher> map;
    private readonly PatternMatcher[] patternMatchers = null!;

    public Tokenizers(TerminalSymbolSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var index = 0;
        var matchers = new PatternMatcher[13];
        var map = new Dictionary<TokenType, PatternMatcher>();

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

        patternMatchers = matchers[..index];
        this.map = map.AsReadOnly();
    }
}
