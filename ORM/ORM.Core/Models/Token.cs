using ORM.Core.Enums;

namespace ORM.Core.Models
{
    public class Token
    {
        public TokenType TokenType { get; init; }

        public string? Lexeme { get; init; }

        public int Line { get; init; }

        public int Column { get; init; }

        public override string ToString()
        {
            return $"Lexeme: {Lexeme}, type: {TokenType} found in line: {Line}, column: {Column}";
        }

        public static implicit operator TokenType(Token token) => token.TokenType;
    }
}