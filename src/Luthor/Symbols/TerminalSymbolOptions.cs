namespace Luthor.Symbols;

[Flags]
public enum TerminalSymbolOptions
    : uint
{
    None = 0,

    IncludeInfixDelimiters = 1u << 0,
    IncludeCircumfixDelimiters = 1u << 1,

    IncludeOperators = 1u << 2,

    IncludeReservedWords = 1u << 3,
    IncludeIdentifiers = 1u << 4,

    IncludeNumericLiterals = 1u << 5,
    IncludeStringLiterals = 1u << 6,
    IncludeBooleanLiterals = 1u << 7,
    IncludeCharacterLiterals = 1u << 8,
    IncludeComments = 1u << 9,

    IncludeAll = 0xFFFF,
}
