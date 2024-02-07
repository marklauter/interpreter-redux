# Interpreter Redux
This started as a just-for-fun port, from C to C#, of the TeCom Interlane residential utility billing language interpreter. Interpreter was originally conceived by Gary Speegle and written by Speegle and myself sometime between 1996 and 1998 when we worked together at TeCom, A TECO Energy Company. TeCom built energy management systems for commercial and residential markets. The original language was a DSL applied to a set of precalculated utility bill values. The output of a billing program was presented on a Windows-like VGA interface, something like a Crystal Report would be presented in a web browser.

The original code is based on my 1996 understanding of lexing-parsing-interpreting gained by skimming the Green Dragon Book. Yes, the Green one from 1977.

## Goals
Duplicating the original DSL with a rewrite turned out to be an impractical goal, so I've come up with a few new goal targets. The first target is a functional lexer that can be seeded with a simple language spec that maps regex fragments to terminal tokens. The next target is a simple mathmatical expression parser and a CLI that evaluates the AST and prints the result. The next known target is a simple boolean predicate language, like a SQL where clause, that can be transformed into a boolean expression tree.

Update: As of 7 FEB 2024, the first goal, a simple math expression parser and evaluator, is complete.

I'm aware of ANTLR and the C# scripting libraries. However, the point is to excercize my brain by building an efficient recursive descent parser.
[Parsing expressions by precedence climbing](https://eli.thegreenplace.net/2012/08/02/parsing-expressions-by-precedence-climbing)

## The Lexer
The lexer accepts a linguistic contex that maps token types to regular expressions. As of 29 JAN 2024, there are two lexers. One is typical OO design with mutable state and heap allocations. The other is a functional design that keeps all the data on the stack. I won't know which one will is more performant until a concrete parser exists from which bench tests can be built.

## The Parser
Not yet built. I saw an interesting video on parsing where the guy started with the parser, and step by step, moved functionality out to the lexer. I feel like I have a better understanding of the lexer, so I started there instead. Maybe the current lexer code continues to exist in a month. Maybe it doesn't.

## Dev Log
 - 29 JAN 2024 - built a functional, immutable lexer. state passed on stack. expected to be faster in real-world use cases.
 - 05 FEB 2024 - abandoned the functional due to complexity, though it worked well and was fast. went back to OOP design for simplicity of object managed state
 - 07 FEB 2024 - completed simple math expression parser with CLI based REPL output looks like
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
