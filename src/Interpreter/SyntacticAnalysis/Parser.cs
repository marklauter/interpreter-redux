//using Interpreter.LexicalAnalysis;
//using System.Globalization;

//namespace Interpreter.SyntacticAnalysis;

//// block is a collection of expression statements that make an AST
//public record Block(IEnumerable<Expression> Expressions);

//public record Expression();

//public record ValueExpression<T>(T Value)
//    : Expression;

//public record IntegerConstantExpression(int Value)
//    : ValueExpression<int>(Value);

//public record DecimalConstantExpression(double Value)
//    : ValueExpression<double>(Value);

//public record StringLiteralExpression(string Value)
//    : ValueExpression<string>(Value);

//public record IdentifierExpression(string Value)
//    : ValueExpression<string>(Value);

//public record BinaryExpression(Expression LeftOperand, Expression RightOperand, string Operator)
//    : Expression;

//public sealed class Parser(Lexer lexer)
//{
//    private readonly Lexer lexer = lexer ?? throw new ArgumentNullException(nameof(lexer));

//    public Block Parse(string source)
//    {
//        ArgumentNullException.ThrowIfNull(source);

//        var tokens = lexer
//            .ReadTokens(source);

//        var expressions = ReadExpressions(tokens);

//        return new Block(expressions);
//    }

//    private IEnumerable<Expression> ReadExpressions(IEnumerable<Token> tokens)
//    {
//        foreach (var token in tokens)
//        {
//            switch (token.Type)
//            {
//                case TokenType.IntegerConstant:
//                    yield return ReadIntegerConstant(token);
//                    break;
//                case TokenType.DecimalConstant:
//                    yield return ReadDecimalConstant(token);
//                    break;
//                case TokenType.StringLiteral:
//                    yield return ReadStringLiteral(token);
//                    break;
//                case TokenType.Identifier:
//                    yield return ReadIdentifier(token);
//                    break;
//                case TokenType.InfixOperator:
//                    yield return ReadBinaryOperator(token);
//                    break;
//                //case TokenType.Punctuation:
//                //    yield return ReadPunctuation(token);
//                //    break;
//                //case TokenType.Keyword:
//                //    yield return ReadKeyword(token);
//                //    break;
//                case TokenType.Whitespace:
//                    continue;
//                default:
//                    throw new NotSupportedException(token.Type.ToString());
//            }
//        }
//    }

//    private Expression ReadBinaryOperator(Token token)
//    {
//        if (token.Type != TokenType.InfixOperator)
//        {
//            throw new ArgumentException("token type mismatch");
//        }

//        throw new NotImplementedException();
//    }

//    private IntegerConstantExpression ReadIntegerConstant(Token token)
//    {
//        return token.Type != TokenType.IntegerConstant
//           ? throw new ArgumentException("token type mismatch")
//           : new IntegerConstantExpression(Int32.Parse(token.Value, NumberStyles.Integer, CultureInfo.InvariantCulture));
//    }

//    private DecimalConstantExpression ReadDecimalConstant(Token token)
//    {
//        return token.Type != TokenType.DecimalConstant
//           ? throw new ArgumentException("token type mismatch")
//            : new DecimalConstantExpression(Double.Parse(token.Value, NumberStyles.Float, CultureInfo.InvariantCulture));
//    }

//    private StringLiteralExpression ReadStringLiteral(Token token)
//    {
//        return token.Type != TokenType.StringLiteral
//           ? throw new ArgumentException("token type mismatch")
//            : new StringLiteralExpression(token.Value[1..^1]);
//    }

//    private IdentifierExpression ReadIdentifier(Token token)
//    {
//        return token.Type != TokenType.Identifier
//           ? throw new ArgumentException("token type mismatch")
//            : new IdentifierExpression(token.Value);
//    }
//}
