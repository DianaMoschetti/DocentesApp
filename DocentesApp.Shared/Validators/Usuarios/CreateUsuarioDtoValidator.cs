using DocentesApp.Shared.DTOs.Usuarios;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Usuarios
{
    public class CreateUsuarioDtoValidator : AbstractValidator<CreateUsuarioDto>
    {
        public CreateUsuarioDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email no tiene un formato válido.")
                .MaximumLength(256).WithMessage("El email no puede superar los 256 caracteres.");

            // Identity solo acepta letras, números y -._@+ en el nombre de usuario
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MaximumLength(256).WithMessage("El nombre de usuario no puede superar los 256 caracteres.")
                .Matches(@"^[a-zA-Z0-9\-._@+]+$").WithMessage("El nombre de usuario solo puede tener letras sin acentos, números y - . _ @ +");

            RuleFor(x => x.Password).PasswordValida("contraseña");

            RuleFor(x => x.Nombre).NombreOpcional("nombre");
            RuleFor(x => x.Apellido).NombreOpcional("apellido");

            RuleFor(x => x.Rol).RolValido();
        }
    }
}
