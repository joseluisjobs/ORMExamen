using ORM.Core.Models;

namespace ORM.Core.Interfaces
{
    public interface IScanner
    {
        Token GetNextToken();
    }
}