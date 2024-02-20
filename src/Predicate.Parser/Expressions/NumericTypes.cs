namespace Predicate.Parser.Expressions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "IDGAD")]
public enum NumericTypes
{
    NotANumber = 0, // NaN
    Integer = TokenIds.INTEGER_LITERAL,
    FloatingPoint = TokenIds.FLOATING_POINT_LITERAL,
    ScientificNotation = TokenIds.SCIENTIFIC_NOTATION_LITERAL,
}
