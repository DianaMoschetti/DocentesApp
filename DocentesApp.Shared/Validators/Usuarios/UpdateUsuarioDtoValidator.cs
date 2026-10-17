using DocentesApp.Shared.DTOs.Usuarios;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Usuarios
{
    public class UpdateUsuarioDtoValidator : AbstractValidator<UpdateUsuarioDto>
    {
        public UpdateUsuarioDtoValidator()
        {
            RuleFor(x => x.Nombre).NombreOpcional("nombre");
            RuleFor(x => x.Apellido).NombreOpcional("apellido");
        }
    }
}
