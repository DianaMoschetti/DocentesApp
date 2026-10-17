using DocentesApp.Shared.DTOs.Usuarios;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Usuarios
{
    public class CambiarRolDtoValidator : AbstractValidator<CambiarRolDto>
    {
        public CambiarRolDtoValidator()
        {
            RuleFor(x => x.Rol).RolValido();
        }
    }
}
