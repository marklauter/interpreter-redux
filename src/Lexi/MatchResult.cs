using System.Diagnostics.CodeAnalysis;

namespace Lexi;

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct MatchResult(
    Script script,
    Symbol symbol)
{
    public readonly Script Script = script;
    public readonly Symbol Symbol = symbol;
}
