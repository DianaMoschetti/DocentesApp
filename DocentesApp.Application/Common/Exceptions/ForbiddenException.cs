
namespace DocentesApp.Application.Common.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message) //403
        {
        }
    }
}