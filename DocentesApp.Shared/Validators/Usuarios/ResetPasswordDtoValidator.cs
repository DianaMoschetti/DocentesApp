using DocentesApp.Shared.DTOs.Usuarios;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Usuarios
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.PasswordNueva).PasswordValida("contraseña nueva");
        }
    }
}
