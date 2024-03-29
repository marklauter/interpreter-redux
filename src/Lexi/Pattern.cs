﻿using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Lexi;

[DebuggerDisplay("{id}, {regex}")]
public sealed class Pattern(
    Regex regex,
    Tokens tokenClass,
    int id)
{
    public Pattern(
        string pattern,
        RegexOptions regexOptions,
        Tokens tokenClass,
        int id)
        : this(
              new Regex(
                  @$"\G(?:{pattern})" ?? throw new ArgumentNullException(nameof(pattern)),
                  regexOptions | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline),
              tokenClass,
              id)
    { }

    public Pattern(
        string pattern,
        Tokens tokenClass,
        int id)
    : this(
          pattern,
          RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline,
          tokenClass,
          id)
    { }

    public const int EndOfSource = -1;
    public const int LexError = -2;
    public const int NoMatch = -3;

    private readonly int id = id;
    private readonly Tokens tokenClass = tokenClass;
    private readonly Regex regex = regex ??
        throw new ArgumentNullException(nameof(regex));

    internal Symbol Match(
        string source,
        int offset)
    {
        var match = regex.Match(source, offset);
        return match.Success
           ? new(match.Index, match.Length, tokenClass, id)
           : new(offset, 0, tokenClass | Tokens.NoMatch, id);
    }
}
