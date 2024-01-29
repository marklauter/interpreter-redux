# Interpreter Redux
This started as a just-for-fun port, from C to C#, of the TeCom Interlane residential utility billing language interpreter. Interpreter was originally conceived by Gary Speegle and written by Speegle and myself sometime between 1996 and 1998 when we worked together at TeCom, A TECO Energy Company. TeCom built energy management systems for commercial and residential markets. The original language was a DSL applied to a set of precalculated utility bill values. The output of a billing program was presented on a Windows-like VGA interface, something like a Crystal Report would be presented in a web browser.

The original code is based on my 1996 understanding of lexing-parsting-interpreting gained by skimming the Green Dragon Book. Yes, the Green one from 1977.

## Goals
Duplicateding the original DSL with a rewrite turned out to be an impractical goal, so I've come up with a few new goal targets. The first target is a functional lexer that can be seeded with a language spec that maps regex fragments to token types. The next target is a simple mathmatical expression parser and a CLI that evaluates the AST and prints the result. The next known target is a simple boolean predicate language, like a SQL where clause, that can be transformed into a boolean expression tree.

## Luthor.Lexer
The lexer accepts a linguistic contex that maps token types to regular expressions. As of 29 JAN 2024, there are two lexers. One is typical OO design with mutable state and heap allocations. The other is a functional design that keeps all the data on the stack. I won't know which one will is more performant until a concrete parser exists from which bench tests can be built.
