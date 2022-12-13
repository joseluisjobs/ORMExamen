// See https://aka.ms/new-console-template for more information

using ORM.Core.Enums;
using ORM.Infrastructure;
using ORM.Lexer;
using ORM.Parser;

var fileContent = File.ReadAllText("test.txt");
var logger = new Logger();
var scanner = new Scanner(new Input(fileContent), logger);

// uncomment to test scanner directly.
//var token = scanner.GetNextToken();
//while (token != TokenType.EOF)
//{
//    token = scanner.GetNextToken();
//    logger.Info(token.ToString());
//}

var parser = new Parser(scanner, logger);
parser.Parse();