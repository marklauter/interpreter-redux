# Interpreter Redux
This started as a just-for-fun port, from C to C#, of the TeCom Interlane residential utility billing language interpreter. The interpreter was originally conceived by Gary Speegle and was written by Speegle and myself sometime between 1996 and 1998 when we worked together at TeCom, A TECO Energy Company. TeCom built energy management systems for commercial and residential markets. The original language was a DSL applied to a set of precalculated utility bill values. The output of a billing program was presented on a Windows-like VGA user interface, something like how a Crystal Report would be presented in a web browser.

The original code is based on my 1996 understanding of lexing-parsing-interpreting gained by skimming the Green Dragon Book, "Principles of Compiler Design." Yes, the Green one from 1977.

## Project Goals
Duplicating the original DSL with a rewrite turned out to be an impractical goal because of the dependency on electric utility usage and billing data, so I've come up with a few new goal targets. The first target is a functional lexer that can be seeded with a simple language spec that maps regex fragments to terminal tokens. The next target is a simple mathmatical expression parser and a CLI that evaluates the AST and prints the result. The next known target is a simple boolean predicate language, like a SQL where clause, that can be transformed into a boolean expression tree.

I'm aware of ANTLR and the C# scripting libraries. However, the point is to excercize my brain by building a recursive descent parser.

Here are the core goals of the project, both general and specific
- have fun futzing around with my old code from the 1990s
- excercise my brain a bit
- detox from video game addiction
- write a reusable lexer
- explore ref struct and pass stack items by reference in C#
- explore readonly ref arguments in C#
- explore performance differential between heap and stack allocated types in C#
- write a really simple math repl
- write a prototype predicate expresion parser that can be used as reference for a project at work and for a graph QL like SPARQL or Cypher.

- Update: As of 7 FEB 2024, the first goal, a simple math expression parser and repl, is complete.
- Update: AS of 22 FEB 2024, all the core goals are complete, so I'm moving on to work on parser combinators in my new Kryptonite repository.

## The Lexer
The intent is for the lexer to support simple L1 parsers. The lexer accepts a terminal symbol specification used to map token types to regex or specialized parse methods. The spec defines basic tokens like general operators, infix and circumfix delimiters, reserved works, boolean literals, comment prefixes and options to enable/disable parsing of identifiers, string literals, numeric literals, etc.

## The Parser
I'm using the recursive descent pattern to implement some simple L1 parsers, including a predicate expression parser. To refamiliarize myself with recursive descent, I built a math repl that accepts simple math expressions. While conducting research for this project I discovered parser combinators and Nicholas Blumhardt's SuperPower library. Next step for this project is building something similar to SuperPower.

## Dev Log
 - 29 JAN 2024 - Built a functional, immutable lexer. state passed on stack. expected to be faster in real-world use cases.
 - 05 FEB 2024 - Abandoned the functional due to complexity, though it worked well and was fast. went back to OOP design for simplicity of object managed state
 - 07 FEB 2024 - Completed simple math expression parser and evaluator with CLI based REPL. The output looks like:
```console
math:> (1 + 1) / 2 * 3
BinaryOperation
Left Expression
   BinaryOperation
   Left Expression
      Group Expression
         BinaryOperation
         Left Expression
            Number: 1
         Op Add
         Right Expression
            Number: 1
   Op Divide
   Right Expression
      Number: 2
Op Multiply
Right Expression
   Number: 3
-------------
result:> 3

math:>
```
- 11 FEB 2024 - Started predicate expression parser. worked out a draft BNF to describe the behavior of the parser. 
- 14 FEB 2024 - Nearly completed a simple query statement parser with CLI based REPL. The output looks like:
```yaml
predicate:> from t where x == "y" || b == "c"
From: t
LogicalExpression:
   ComparisonExpression:
      Identifier: x
      Operator: ==
      StringLiteral: y
   Operator: ||
   ComparisonExpression:
      Identifier: b
      Operator: ==
      StringLiteral: c
predicate:>
```
- 15 FEB 2024 - Added parenthetical grouping query statement parser. The output looks like:
```yaml
predicate:> from Address where Street startswith "Cypress" and (City = "Tampa" or City = "Miami")
From: Address
LogicalExpression:
|-- L: ComparisonExpression:
|-- L: |-- L: Identifier: Street
|-- L: |-- Operator: StartsWith
|-- L: |-- R: StringLiteral: Cypress
|-- Operator: And
|-- R: ParentheticalExpression:
|-- R: |-- (: LogicalExpression:
|-- R: |-- (: |-- L: ComparisonExpression:
|-- R: |-- (: |-- L: |-- L: Identifier: City
|-- R: |-- (: |-- L: |-- Operator: Equal
|-- R: |-- (: |-- L: |-- R: StringLiteral: Tampa
|-- R: |-- (: |-- Operator: Or
|-- R: |-- (: |-- R: ComparisonExpression:
|-- R: |-- (: |-- R: |-- L: Identifier: City
|-- R: |-- (: |-- R: |-- Operator: Equal
|-- R: |-- (: |-- R: |-- R: StringLiteral: Miami
predicate:>
```
- 18 FEB 2024 - Refactoring lexer to improve pattern definition and to make lexer stateless
- 18 FEB 2024 - Found this great C# parser combinator library called SuperPower https://github.com/datalust/superpower
- 18 FEB 2024 - Intro to the SuperPower library from Nicholas Blumhardt https://www.youtube.com/watch?v=klHyc9HQnNQ
- 19 FEB 2024 - Completed second draft of the lexer in the Lexi project. The lexer is now much simpler due to pattern definitions buidler and statelessness. State is now maintained within the Script struct returned as part of the NextMatchResult.
- 19 FEB 2024 - Refactored math parser to use Lexi.
- 20 FEB 2024 - Refactored predicate parser to use Lexi. Lexer pattern builder looks like this:
```csharp
    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        var builder = LexerBuilder
            .Create(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            .MatchKeyword($"{nameof(TokenIds.FROM)}", TokenIds.FROM)
            .MatchKeyword($"{nameof(TokenIds.WHERE)}", TokenIds.WHERE)
            .MatchKeyword($"{nameof(TokenIds.SKIP)}", TokenIds.SKIP)
            .MatchKeyword($"{nameof(TokenIds.TAKE)}", TokenIds.TAKE)
            .MatchOperator($"{nameof(TokenIds.CONTAINS)}", TokenIds.CONTAINS)
            .MatchOperator("startswith|sw", TokenIds.STARTS_WITH)
            .MatchOperator("endswith|ew", TokenIds.ENDS_WITH)
            .MatchOperator(@"and|&&", TokenIds.LOGICAL_AND)
            .MatchOperator(@"or|\|\|", TokenIds.LOGICAL_OR)
            .MatchIdentifier(TokenIds.IDENTIFIER)
            .MatchBooleanTrueLiteral("true", TokenIds.TRUE)
            .MatchBooleanFalseLiteral("false", TokenIds.FALSE)
            .MatchIntegerLiteral(TokenIds.INTEGER_LITERAL)
            .MatchFloatingPointLiteral(TokenIds.FLOATING_POINT_LITERAL)
            .MatchScientificNotationLiteral(TokenIds.SCIENTIFIC_NOTATION_LITERAL)
            .MatchStringLiteral(TokenIds.STRING_LITERAL)
            .MatchCharacterLiteral(TokenIds.CHAR_LITERAL)
            .MatchOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .MatchClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS)
            .MatchOperator("=|==", TokenIds.EQUAL)
            .MatchOperator(">", TokenIds.GREATER_THAN)
            .MatchOperator(">=", TokenIds.GREATER_THAN_OR_EQUAL)
            .MatchOperator("<", TokenIds.LESS_THAN)
            .MatchOperator("<=", TokenIds.LESS_THAN_OR_EQUAL)
            .MatchOperator("!=", TokenIds.NOT_EQUAL);

        services.TryAddTransient(serviceProvider => builder.Build());
        services.TryAddTransient<Parser>();

        return services;
    }
```
- 20 FEB 2024 - Fixed issue with regex patterns that caused false matches by adding noncapturing groups. This obviously impacts performance, but working code is better than fast broken code I guess.
- 22 FEB 2024 - Created a new repo for a parser combinator probject called Kryptonite. So I think interpreter-redux is complete. The something resembling Lexi the lexer might live on in Kryptonite.  https://github.com/marklauter/kryptonite
