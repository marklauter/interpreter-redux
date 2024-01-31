using System.Collections.ObjectModel;

namespace Luthor.Context;

public partial class LexicalContext
{
    private readonly ReadOnlyDictionary<Tokens, TokenReader> map;
    private readonly TokenReader[] readers = null!;

    public LexicalContext(LanguageSpecification spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var index = 0;
        var readers = new TokenReader[13];
        var map = new Dictionary<Tokens, TokenReader>();

        (index, var reservedWordPattern) = AddReservedWordReader(spec, readers, map, index);
        (index, var booleanLiteralPattern) = AddBooleanLiterals(spec, readers, map, index);
        index = AddIdentifiers(readers, map, index, booleanLiteralPattern, reservedWordPattern);
        index = AddNumericLiterals(readers, map, index);
        index = AddStringLiterals(readers, map, index);
        index = AddCharacterLiterals(readers, map, index);
        index = AddNewLine(readers, map, index);
        index = AddWhitespace(readers, map, index);
        index = AddInfixDelimiters(spec, readers, map, index);
        index = AddCircumfixDelimiters(spec, readers, map, index);
        index = AddComments(spec, readers, map, index);
        index = AddOperators(spec, readers, map, index);

        this.readers = readers[..index];
        this.map = map.AsReadOnly();
    }
}
