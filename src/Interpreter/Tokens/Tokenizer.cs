namespace Interpreter.Tokens;

internal sealed class Tokenizer
{
    private enum State
    {
        Whitespace = 0,
        Token = 1,
    }

    public IEnumerable<Token> Tokenize(string source)
    {
        var state = State.Whitespace;
        var offset = 0;
        for (var i = 0; i < source.Length; ++i)
        {
            switch (state)
            {
                case State.Whitespace:
                    if (!Char.IsWhiteSpace(source[i]))
                    {
                        state = State.Token;
                        offset = i;
                    }

                    break;
                case State.Token:
                    if (Char.IsWhiteSpace(source[i]))
                    {
                        yield return new Token(
                            offset,
                            i - offset);
                        state = State.Whitespace;
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        if (state == State.Token)
        {
            yield return new Token(
                offset,
                source.Length - offset);
        }
    }
}

//internal const string[] Keyword = new
//{
//    "if",
//    "else",
//    "while",
//    "for",
//    "foreach",
//    "in",
//    "do",
//    "switch",
//    "case",
//    "default",
//    "break",
//    "continue",
//    "return",
//    "try",
//    "catch",
//    "finally",
//    "throw",
//    "new",
//    "typeof",
//    "sizeof",
//    "null",
//    "true",
//    "false",
//    "this",
//    "base",
//    "namespace",
//    "using",
//    "class",
//    "struct",
//    "interface",
//    "enum",
//    "delegate",
//    "event",
//    "public",
//    "private",
//    "protected",
//    "internal",
//    "static",
//    "readonly",
//    "volatile",
//    "override",
//    "virtual",
//    "abstract",
//    "extern",
//    "unsafe",
//    "ref",
//    "out",
//    "in",
//    "is",
//    "as",
//    "params",
//    "void",
//    "object",
//    "string",
//    "bool",
//    "byte",
//    "sbyte",
//    "short",
//    "ushort",
//    "int",
//    "uint",
//    "long",
//    "ulong",
//    "float",
//    "double",
//    "char",
//    "decimal",
//    "dynamic",
//    "var",
//    "get",
//    "set",
//    "add",
//    "remove",
//    "value",
//    "alias",
//    "global",
//    "stackalloc",
//    "checked",
//    "unchecked",
//    "lock",
//    "sizeof",
//    "stackalloc",
//    "fixed",
//    "from",
//    "group",
//    "into",
//    "join",
//    "let",
//    "orderby",
//    "select",
//    "where",
//    "yield",
//    "partial",
//    "async",
//    "await",
//    "when",
//    "nameof",
//    "using",
//    "get",
//    "set",
//    "add",
//    "remove",
//    "value",
//    "alias",
//    "global",
//    "stackalloc",
//    "checked",
//    "unchecked",
//    "lock",
//    "sizeof",
//    "stackalloc",
//    "fixed",
//    "from",
//    "group",
//    "into",
//    "join",
//    "let",
//    "orderby",
//    "select",
//    "where",
//    "yield",
//    "partial"
//}

