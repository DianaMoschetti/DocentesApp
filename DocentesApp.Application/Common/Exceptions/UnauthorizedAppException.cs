
namespace DocentesApp.Application.Common.Exceptions
{
    public class UnauthorizedAppException : AppException
    {
        public UnauthorizedAppException(string message) : base(message) //401
        {
        }
    }
}