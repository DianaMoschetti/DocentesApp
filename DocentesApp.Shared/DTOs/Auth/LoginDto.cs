
namespace DocentesApp.Application.DTOs.Auth
{
    public class LoginDto
    {
        // Diana, ver de sacar el user name y q se use el mail para loguearse
        public string EmailOrUserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
