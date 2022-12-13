using ORM.Core.Enums;
using ORM.Core.Interfaces;
using ORM.Core.Models;

namespace ORM.Parser
{
    public class Parser : IParser
    {
        private readonly IScanner _scanner;
        private readonly ILogger _logger;
        private Token _lookAhead;

        public Parser(IScanner scanner, ILogger logger)
        {
            this._scanner = scanner;
            this._logger = logger;
            this._lookAhead = this._scanner.GetNextToken();
        }

        public void Parse()
        {
            Code();
        }

        private void Code()
        {
            ClassDecls();
            Stmts();
        }
        private void ClassDecls()
        {
            var token = this._lookAhead.TokenType;
            if (token == TokenType.ClassKeyword)
            {
                Match(TokenType.ClassKeyword);
                Match(TokenType.Identifier);
                Block();
                ClassDecls();
            }
        }
        private void Stmts()
        {
            //{}
            if (this._lookAhead.TokenType == TokenType.EOF )
            {
                //eps
            }
            else
            {
                Stmt();
                Stmts();
            }
        }
        private void Stmt()
        {
            switch (this._lookAhead.TokenType)
            {
                case TokenType.Identifier:
                    SqlStatement();
                    break;

                default:
                    break;
            }
        }
        private void SqlStatement()
        {
           
            Match(TokenType.Identifier);
            Match(TokenType.Dot);
            if(this._lookAhead.TokenType == TokenType.WhereKeyword || this._lookAhead.TokenType == TokenType.SelectKeyword)
            {
                WhereStatement();
                if(this._lookAhead.TokenType == TokenType.Dot)
                   Match(TokenType.Dot);
                SelectStatement();
            }
            
            this.Match(TokenType.SemiColon);
        }
        private void LogicalOrExpr()
        {
            LogicalAndExpr();
            while (this._lookAhead.TokenType == TokenType.Or)
            {
                Move();
                LogicalAndExpr();
            }
        }
        private void LogicalAndExpr()
        {
            EqExpr();
            while (this._lookAhead.TokenType == TokenType.And)
            {
                Move();
                EqExpr();
            }
        }
        private void EqExpr()
        {
            RelExpr();
            while (this._lookAhead.TokenType == TokenType.EqualOperator || this._lookAhead.TokenType == TokenType.NotEqual)
            {
                Move();
                RelExpr();
            }
        }
        private void RelExpr()
        {
            Expr();
            while (this._lookAhead.TokenType == TokenType.LessThan || this._lookAhead.TokenType == TokenType.GreaterThan)
            {
                Move();
                Expr();
            }
        }
        private void Expr()
        {
            Term();
            while (this._lookAhead.TokenType == TokenType.Plus || this._lookAhead.TokenType == TokenType.Minus)
            {
                Move();
                Term();
            }
        }
        private void Term()
        {
            Factor();

            while (this._lookAhead.TokenType == TokenType.GreaterThan)
            {
                Move();
                Factor();
            }
        }
        private void Factor()
        {
            switch (this._lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    Match(TokenType.LeftParens);
                    Expr();
                    Match(TokenType.RightParens);
                    break;
                case TokenType.IntConstant:
                    Match(TokenType.IntConstant);
                    break;
                case TokenType.FloatConstant:
                    Match(TokenType.FloatConstant);
                    break;
                case TokenType.StringConstant:
                    Match(TokenType.StringConstant);
                    break;
                default:
                    Match(TokenType.Identifier);
                    if(this._lookAhead.TokenType == TokenType.Dot)
                    {
                        Match(TokenType.Dot);
                        RelExpr();
                    }
                        
                    break;
            }
        }
        private void SelectStatement()
        {
            if (this._lookAhead.TokenType != TokenType.SelectKeyword) return;
            Match(TokenType.SelectKeyword);
            Match(TokenType.LeftParens);
            Match(TokenType.Identifier);
            Match(TokenType.Arrow);
            Match(TokenType.NewKeyword);
            Match(TokenType.Identifier);
            AssignationBlockStatement();
            Match(TokenType.RightParens);
        }

        private void AssignationBlockStatement()
        {
            Match(TokenType.OpenBrace);
            AssignationStatement();
            Match(TokenType.CloseBrace);
        }
        private void AssignationStatement()
        {
            
            if (this._lookAhead.TokenType == TokenType.Identifier)
            {
                Match(TokenType.Identifier);
                Match(TokenType.Equal);
                LogicalOrExpr();
                this.Match(TokenType.Comma);
                AssignationStatement();
            }
            
           
            
        }
        private void WhereStatement()
        {
            if (this._lookAhead.TokenType != TokenType.WhereKeyword) return;
            Match(TokenType.WhereKeyword);
            Match(TokenType.LeftParens);
            Match(TokenType.Identifier);
            Match(TokenType.Arrow);
            LogicalOrExpr();
            Match(TokenType.RightParens);
        }
        private void Block()
        {
            Match(TokenType.OpenBrace);
            Decls();
            Match(TokenType.CloseBrace);
        }
        private void Decl(TokenType token)
        {
            Type();
            Match(TokenType.Identifier);
            Match(TokenType.SemiColon);

        }
        private void Decls()
        {
            var token = this._lookAhead.TokenType;
            if (token == TokenType.IntKeyword || token == TokenType.FloatKeyword || token == TokenType.StringKeyword)
            {
                Decl(token);
                Decls();
            }
        }
        private void Type()
        {
            switch (this._lookAhead.TokenType)
            {
                case TokenType.IntKeyword:
                    Match(TokenType.IntKeyword);
                    break;
                case TokenType.StringKeyword:
                    Match(TokenType.StringKeyword);
                    break;
                case TokenType.FloatKeyword:
                    Match(TokenType.FloatKeyword);
                    break;

            }
        }
        private void Move()
        {
            this._lookAhead = this._scanner.GetNextToken();
        }

        private void Match(TokenType expectedTokenType)
        {
            if (this._lookAhead != expectedTokenType)
            {
                this._logger.Error($"Syntax Error! expected token {expectedTokenType} but found {this._lookAhead.TokenType} on line {this._lookAhead.Line} and column {this._lookAhead.Column}");
                throw new ApplicationException($"Syntax Error! expected token {expectedTokenType} but found {this._lookAhead.TokenType} on line {this._lookAhead.Line} and column {this._lookAhead.Column}");
            }
            this.Move();
        }
    }
}