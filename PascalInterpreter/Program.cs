using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PascalInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("@> ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;
                var isSymbolMode = input.Contains("-s");
                if (input.ToLowerInvariant().StartsWith("test"))
                    input = "PROGRAM Part10AST; VAR a, b : INTEGER; y: REAL; BEGIN { Part10AST } a:= 2; b:= 10 * a + 10 * a DIV 4; y:= 20 / 7 + 3.14; END.  { Part10AST }";
                if (input.ToLowerInvariant().StartsWith("run"))
                {
                    var lineArgs = input.Split(' ');
                    var path = lineArgs[lineArgs.Length - 1];
                    input = File.ReadAllText(path);
                }
                else if ("quit".Contains(input.ToLowerInvariant()))
                    break;
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                // Parse the script
                var tree = parser.Parse();

                if (isSymbolMode)
                {
                    var tableBuilder = new SymbolTableBuilder();
                    tableBuilder.Visit(tree);
                    Console.WriteLine($"{tableBuilder}\n");
                }

                var interpreter = new Interpreter();
                Console.WriteLine(interpreter.Interpret(tree));
                // -- begin memory display
                Console.WriteLine("Runtime Memory Display");
                foreach (var kvp in interpreter.GlobalVariables)
                    Console.WriteLine($"\t{kvp.Key}\t\t{kvp.Value}");
                // -- end memory display
                Console.WriteLine();
            }
        }
    }
}