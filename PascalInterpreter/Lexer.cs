using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    public class Lexer
    {
        private string _text;
        private int _pos;
        private char _currentChar;

        public Lexer(string text)
        {
            _text = text;
            _pos = 0;
            _currentChar = _text[0];
        }

        private void AdvancePosition()
        {
            _pos++;
            _currentChar = _pos >= _text.Length ? char.MinValue : _text[_pos];
        }

        private void AdvancePosition(int num)
        {
            while (num-- > 0)
                AdvancePosition();
        }

        private void SkipWhitespace()
        {
            while (_currentChar != char.MinValue && char.IsWhiteSpace(_currentChar))
                AdvancePosition();
        }

        private void SkipComment()
        {
            while (_currentChar != '}')
                AdvancePosition();
            AdvancePosition();
        }

        private Token Number()
        {
            var result = string.Empty;
            while (_currentChar != char.MinValue && char.IsDigit(_currentChar))
            {
                result += _currentChar;
                AdvancePosition();
            }

            if (_currentChar == '.')
            {
                result += _currentChar;
                AdvancePosition();

                while (_currentChar != char.MinValue && char.IsDigit(_currentChar))
                {
                    result += _currentChar;
                    AdvancePosition();
                }

                return new Token(TokenTypes.LiteralReal, float.Parse(result));
            }
            return new Token(TokenTypes.LiteralInteger, int.Parse(result));
        }

        private char Peek()
        {
            var peekPos = _pos + 1;
            if (peekPos >= _text.Length)
                return char.MinValue;
            return _text[peekPos];
        }

        private Dictionary<string, Token> ReservedKeywords = new Dictionary<string, Token>
        {
            { "BEGIN", new Token(TokenTypes.Begin, "BEGIN") },
            { "END", new Token(TokenTypes.End, "END") },
            { "DIV", new Token(TokenTypes.Division, "DIV") },
            { "INTEGER", new Token(TokenTypes.Integer, "INTEGER") },
            { "REAL", new Token(TokenTypes.Real, "REAL") },
            { "PROGRAM", new Token(TokenTypes.Program, "Program") },
            { "VAR", new Token(TokenTypes.Variable, "VAR") },
            { "PROCEDURE", new Token(TokenTypes.Procedure, "PROCEDURE") }
        };

        private Token Id()
        {
            var result = string.Empty;
            while (_currentChar != char.MinValue && char.IsLetterOrDigit(_currentChar))
            {
                result += _currentChar;
                AdvancePosition();
            }
            var key = result.ToUpperInvariant();
            if (ReservedKeywords.ContainsKey(key))
                return ReservedKeywords[key];
            return new Token(TokenTypes.Id, result);
        }

        /*
        def get_next_token(self):
            while self.current_char is not None:
                ...
                if self.current_char == '{':
                    self.advance()
                    self.skip_comment()
                    continue
                ...
                if self.current_char.isdigit():
                    return self.number()

                if self.current_char == ':':
                    self.advance()
                    return Token(COLON, ':')

                if self.current_char == ',':
                    self.advance()
                    return Token(COMMA, ',')
                ...
                if self.current_char == '/':
                    self.advance()
                    return Token(FLOAT_DIV, '/')
                ...
        */

        public Token GetNextToken()
        {
            while (_currentChar != char.MinValue)
            {
                if (char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (_currentChar == '{')
                {
                    AdvancePosition();
                    SkipComment();
                    continue;
                }

                if (char.IsLetter(_currentChar))
                    return Id();

                if (_currentChar == ':' && Peek() == '=')
                {
                    AdvancePosition(2);
                    return new Token(TokenTypes.Assign, ":=");
                }

                if (_currentChar == ':')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Colon, ":");
                }

                if (_currentChar == ',')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Comma, ",");
                }

                if (_currentChar == '.')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Dot, ".");
                }

                if (_currentChar == ';')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Semi, ";");
                }

                if (char.IsDigit(_currentChar))
                    return Number();

                if (_currentChar == '+')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Addition, '+');
                }

                if (_currentChar == '-')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Subtraction, '-');
                }

                if (_currentChar == '*')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.Multiplication, '*');
                }

                if (_currentChar == '/')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.RealDivision, '/');
                }

                if (_currentChar == '(')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.LeftParenthesis, '(');
                }

                if (_currentChar == ')')
                {
                    AdvancePosition();
                    return new Token(TokenTypes.RightParenthesis, ')');
                }

                throw new Exception("Y'all done broke something.");
            }

            return new Token("EOF", null);
        }
    }
}