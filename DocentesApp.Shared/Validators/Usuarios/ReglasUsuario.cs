using DocentesApp.Shared.Auth;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Usuarios
{
    // Reglas compartidas por los validators de usuarios.
    // Las de contraseña repiten las de Identity (Program.cs de la API, AddIdentity: RequiredLength 6, RequireDigit,
    // RequireLowercase): si se cambian allá, cambiarlas acá. Identity las vuelve a validar, pero en inglés.
    internal static class ReglasUsuario
    {
        public const int PasswordLongitudMinima = 6;

        public static IRuleBuilderOptions<T, string> PasswordValida<T>(this IRuleBuilder<T, string> rule, string campo) =>
            rule
                .NotEmpty().WithMessage($"La {campo} es obligatoria.")
                .MinimumLength(PasswordLongitudMinima).WithMessage($"La {campo} debe tener al menos {PasswordLongitudMinima} caracteres.")
                .Matches("[0-9]").WithMessage($"La {campo} debe tener al menos un número.")
                .Matches("[a-z]").WithMessage($"La {campo} debe tener al menos una letra minúscula.");

        public static IRuleBuilderOptions<T, string> RolValido<T>(this IRuleBuilder<T, string> rule) =>
            rule
                .Must(Roles.EsValido)
                .WithMessage($"El rol debe ser uno de: {string.Join(", ", Roles.Todos)}.");

        public static IRuleBuilderOptions<T, string?> NombreOpcional<T>(this IRuleBuilder<T, string?> rule, string campo) =>
            rule
                .MaximumLength(100)
                .WithMessage($"El {campo} no puede superar los 100 caracteres.");
    }
}
