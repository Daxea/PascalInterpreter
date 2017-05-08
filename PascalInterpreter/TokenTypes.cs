using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    public static class TokenTypes
    {
        public const string Addition = "ADD";
        public const string Subtraction = "SUB";
        public const string Multiplication = "MULTI";
        public const string Division = "DIV";
        public const string RealDivision = "REAL-DIV";

        public const string LeftParenthesis = "LPAREN";
        public const string RightParenthesis = "RPAREN";

        public const string Program = "PROGRAM";
        public const string Variable = "VAR";
        public const string Procedure = "PROCEDURE";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Dot = "DOT";
        public const string Comma = "COMMA";
        public const string Colon = "COLON";
        public const string Assign = "ASSIGN";
        public const string Empty = "EMPTY";
        public const string Semi = "SEMI";
        public const string Id = "ID";

        public const string LiteralInteger = "LITERAL-INTEGER";
        public const string LiteralReal = "LITERAL-REAL";

        public const string Integer = "INTEGER";
        public const string Real = "REAL";
        public const string String = "STRING";
        public const string Character = "CHARACTER";
        public const string Boolean = "BOOLEAN";

        public static bool IsUnaryOp(Token token)
        {
            return new[] { Addition, Subtraction }.Contains(token.Type);
        }
    }
}