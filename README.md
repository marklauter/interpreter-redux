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

Update: As of 7 FEB 2024, the first goal, a simple math expression parser and repl, is complete.

## The Lexer
The intent is for the lexer to support simple L1 parsers. The lexer accepts a terminal symbol specification used to map token types to regex or specialized parse methods. The spec defines basic tokens like general operators, infix and circumfix delimiters, reserved works, boolean literals, comment prefixes and options to enable/disable parsing of identifiers, string literals, numeric literals, etc.

## The Parser
I'm using the recursive descent pattern to implement some simple L1 parsers, including a predicate expression parser. To refamiliarize myself with recursive descent, I built a math repl that accepts simple math expressions.

## Dev Log
 - 29 JAN 2024 - built a functional, immutable lexer. state passed on stack. expected to be faster in real-world use cases.
 - 05 FEB 2024 - abandoned the functional due to complexity, though it worked well and was fast. went back to OOP design for simplicity of object managed state
 - 07 FEB 2024 - completed simple math expression parser and evaluator with CLI based REPL. The output looks like:
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
- 11 FEB 2024 - started predicate expression parser. worked out a draft BNF to describe the behavior of the parser. 
- 14 FEB 2024 - nearly completed a simple query statement parser with CLI based REPL. The output looks like:
```console
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