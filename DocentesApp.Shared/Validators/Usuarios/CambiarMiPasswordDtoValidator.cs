using DocentesApp.Shared.DTOs.Usuarios;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Usuarios
{
    public class CambiarMiPasswordDtoValidator : AbstractValidator<CambiarMiPasswordDto>
    {
        public CambiarMiPasswordDtoValidator()
        {
            RuleFor(x => x.PasswordActual)
                .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

            RuleFor(x => x.PasswordNueva).PasswordValida("contraseña nueva");

            RuleFor(x => x.PasswordNueva)
                .NotEqual(x => x.PasswordActual)
                .When(x => !string.IsNullOrEmpty(x.PasswordNueva))
                .WithMessage("La contraseña nueva tiene que ser distinta de la actual.");
        }
    }
}
