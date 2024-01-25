using Luthor.Context;
using System.Text.RegularExpressions;

namespace Luthor.Tests;

public sealed class RegExTests
{
    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string value, string expected, bool expectedSuccess)
    {
        var regex = new Regex(
            $@"\G(?:{LinguisticContext.StringLiteralExpression})",
            RegexOptions.Compiled);

        var match = regex.Match(value);
        Assert.Equal(expectedSuccess, match.Success);
        if (match.Success)
        {
            Assert.Equal(expected, match.Value[1..^1]);
        }
    }
}
