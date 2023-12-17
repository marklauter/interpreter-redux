using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Interpreter.LexicalAnalysis;

internal sealed class LexicalAnalyzer
    : ILexicalAnalyzer
{
    private readonly IEnumerable<string> keywords;
    private readonly IEnumerable<string> operators;
    private readonly IEnumerable<string> punctuationMarks;
    private readonly Regex regex;

    public LexicalAnalyzer(IOptions<LexicalAnalyzerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        keywords = options.Value.Keywords;
        operators = options.Value.Operators;
        punctuationMarks = options.Value.PunctuationMarks;

        //regex = new Regex($@"\s*(?<keyword>{String.Join("|", keywords)})\s*|(?<operator>{String.Join("|", operators)})|(?<identifier>[a-zA-Z_][a-zA-Z0-9_]*)|(?<constant>[0-9]+)|(?<string>""[^""]*"")|(?<boolean>true|false)|(?<whitespace>\s+)", RegexOptions.Compiled);

        var keywordExpression = $@"(?<keyword>{String.Join("|", keywords)})";
        var operatorExpression = $@"(?<operator>{String.Join("|", operators.Select(Regex.Escape))})";
        var punctuationExpression = $@"(?<punctuation>{String.Join("|", punctuationMarks.Select(Regex.Escape))})";
        var identifierExpression = $@"(?<identifier>[a-zA-Z_][a-zA-Z0-9_]*)";
        var constantExpression = $@"(?<constant>[0-9]+)";
        regex = new Regex(
            $"{keywordExpression}|{operatorExpression}|{punctuationExpression}|{identifierExpression}|{constantExpression}",
            RegexOptions.Compiled);
    }

    public MatchCollection Analyze(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return regex.Matches(source);
        //.SelectMany(m => m.Groups.Values)
        //.Where(g => g.Success)
        //.Select(g => new Token(g.Index, g.Length));
    }
}
