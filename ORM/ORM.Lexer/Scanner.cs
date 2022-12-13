using ORM.Core.Enums;
using ORM.Core.Interfaces;
using ORM.Core.Models;
using System.Text;

namespace ORM.Lexer
{
    public class Scanner : IScanner
    {
        private Input _input;
        private readonly ILogger _logger;
        private readonly Dictionary<string, TokenType> keywords;

        public Scanner(Input input, ILogger logger)
        {
            this._input = input;
            this._logger = logger;
            this.keywords = new Dictionary<string, TokenType>()
            {
                ["class"] = TokenType.ClassKeyword,
                ["int"] = TokenType.IntKeyword,
                ["float"] = TokenType.FloatKeyword,
                ["string"] = TokenType.StringKeyword,
                ["where"] = TokenType.WhereKeyword,
                ["select"] = TokenType.SelectKeyword,
                ["new"] = TokenType.NewKeyword,
            };
        }

        public Token GetNextToken()
        {
            var lexeme = new StringBuilder();
            var currentChar = GetNextChar();
            while (true)
            {
                while (char.IsWhiteSpace(currentChar) || currentChar == '\n')
                {
                    currentChar = GetNextChar();
                }
                if (char.IsLetter(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsLetterOrDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }

                    if (this.keywords.ContainsKey(lexeme.ToString()))
                    {
                        return new Token
                        {
                            TokenType = this.keywords[lexeme.ToString()],
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    }

                    return new Token
                    {
                        TokenType = TokenType.Identifier,
                        Column = _input.Position.Column,
                        Line = _input.Position.Line,
                        Lexeme = lexeme.ToString(),
                    };
                }
                else if (char.IsDigit(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }

                    if (currentChar != '.')
                    {
                        return new Token
                        {
                            TokenType = TokenType.IntConstant,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString(),
                        };
                    }

                    currentChar = GetNextChar();
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }
                    return new Token
                    {
                        TokenType = TokenType.FloatConstant,
                        Column = _input.Position.Column,
                        Line = _input.Position.Line,
                        Lexeme = lexeme.ToString(),
                    };

                }
                switch (currentChar)
                {
                    case '+':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.Plus,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '-':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.Minus,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '>':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.GreaterThan,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '<':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.LessThan,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '(':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.LeftParens,
                            Column = _input.Position.Column,
                            Line =  _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case ')':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.RightParens,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case ';':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.SemiColon,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '{':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.OpenBrace,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '}':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.CloseBrace,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '.':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.Dot,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case ',':
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.Comma,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '=':
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                        if (currentChar == '>')
                        {
                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.Arrow,
                                Column = _input.Position.Column,
                                Line = _input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        }
                        else if(currentChar == '=')
                        {
                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.EqualOperator,
                                Column = _input.Position.Column,
                                Line = _input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        }
                        return new Token
                        {
                            TokenType = TokenType.Equal,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '!':
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                        if (currentChar != '=')
                            break;
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.NotEqual,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '&':
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                        if (currentChar != '&')
                            break;
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.And,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '\"':
                        {
                            lexeme.Append(currentChar);
                            currentChar = GetNextChar();
                            while (currentChar != '\"')
                            {
                                lexeme.Append(currentChar);
                                currentChar = GetNextChar();
                            }
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.StringConstant,
                                Column = _input.Position.Column,
                                Line = _input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        }
                    case '|':
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                        if (currentChar != '|')
                            break;
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        return new Token
                        {
                            TokenType = TokenType.Or,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = lexeme.ToString()
                        };
                    case '\0':
                        return new Token
                        {
                            TokenType = TokenType.EOF,
                            Column = _input.Position.Column,
                            Line = _input.Position.Line,
                            Lexeme = string.Empty
                        };
                }
                throw new ApplicationException($"Caracter {currentChar} invalido en la columna: {_input.Position.Column}, fila: {_input.Position.Line}");
            }
        }

        private Token BuildToken(string lexeme, TokenType tokenType)
        {
            return new Token
            {
                Column = this._input.Position.Column > 0 ? this._input.Position.Column - 1 : this._input.Position.Column,
                Line = this._input.Position.Line + 1,
                Lexeme = lexeme,
                TokenType = tokenType,
            };
        }

        private char GetNextChar()
        {
            var next = _input.NextChar();
            _input = next.Reminder;
            return next.Value;
        }

        private char PeekNextChar()
        {
            var next = _input.NextChar();
            return next.Value;
        }
    }
}