﻿using System.Runtime.CompilerServices;

namespace Lexi;

public sealed class Lexi(
    TokenPattern[] patterns)
{
    private readonly TokenPattern[] patterns = patterns
        ?? throw new ArgumentNullException(nameof(patterns));

    private readonly ref struct SymbolMatch(
        Symbol symbol,
        int index)
    {
        public readonly Symbol Symbol = symbol;
        public readonly int Index = index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareSymbolMatch(
        SymbolMatch l,
        SymbolMatch r)
    {
        var lLenth = l.Symbol.Length;
        var rLength = r.Symbol.Length;
        var lIndex = l.Index;
        var rIndex = r.Index;

        // longer match wins. tie goes to lowest index.
        // equal means no swap and probably an error (aka skill issue) in the pattern definitions.
        // probably should throw in that case, or create an error token
        return
            lLenth > rLength
            ? 1
            : lLenth < rLength
                ? -1
                : lIndex < rIndex
                    ? 1
                    : lIndex > rIndex
                        ? -1
                        : 0;
    }

    public NextTokenResult NextToken(Script script)
    {
        var offset = script.Offset;
        var line = script.Line;

        if (script.IsEndOfSource())
        {
            return new(
                script,
                new(offset, 0, Tokens.EndOfSource, TokenPattern.EndOfSource, line));
        }

        var source = script.Source;

        var match = AuxillaryPatterns.NewLine()
            .Match(source, offset);
        if (match.Success)
        {
            offset += match.Length;
            ++line;
        }

        match = AuxillaryPatterns.Whitespace()
            .Match(source, offset);
        if (match.Success)
        {
            offset += match.Length;
        }

        // dragon book says perform all match tests, and return best match based on length and pattern definition index. longest match wins. ties to go to lowest index.
        // could replace this loop with a pair of functions, one for this strategy and another to return first match.
        // first match is faster, but less tolerent pattern definition ordering. everything is tradeoffs.
        var patterns = this.patterns.AsSpan();
        var length = patterns.Length;
        var bestMatch = default(SymbolMatch);
        for (var i = 0; i < length; ++i)
        {
            var latestMatch = new SymbolMatch(
                patterns[i].Match(source, offset, line),
                i);

            if (CompareSymbolMatch(latestMatch, bestMatch) > 0)
            {
                bestMatch = latestMatch;
            }
        }

        var symbol = bestMatch.Symbol;

        return symbol.IsMatch()
            ? new(
                new(source, offset + symbol.Length, line),
                symbol)
            : new(
                new(source, offset, line),
                new(offset, 0, Tokens.LexError, TokenPattern.LexError, line));
    }
}
