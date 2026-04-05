using DocentesApp.Application.DTOs.Docentes;
using FluentValidation;
using System.Globalization;

namespace DocentesApp.Application.Validators.Docentes
{
    public class CreateDocenteDtoValidator : AbstractValidator<CreateDocenteDto>
    {
        public CreateDocenteDtoValidator()
        {
            RuleFor(x => x.Dni)
                .NotEmpty().WithMessage("El DNI es obligatorio.")
                .MaximumLength(15).WithMessage("El DNI no puede superar los 15 caracteres.");

            RuleFor(x => x.Legajo)
                .GreaterThan(0).WithMessage("El legajo debe ser mayor a 0.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("El email no tiene un formato válido.");

            RuleFor(x => x.EmailAlternativo)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.EmailAlternativo))
                .WithMessage("El email alternativo no tiene un formato válido.");

            RuleFor(x => x.Celular)
                .MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.Celular))
                .WithMessage("El celular no puede superar los 30 caracteres.");

            RuleFor(x => x.Direccion)
                .MaximumLength(250).When(x => !string.IsNullOrWhiteSpace(x.Direccion))
                .WithMessage("La dirección no puede superar los 250 caracteres.");

            RuleFor(x => x.MaxNivelAcademico)
                .MaximumLength(120).When(x => !string.IsNullOrWhiteSpace(x.MaxNivelAcademico))
                .WithMessage("El máximo nivel académico no puede superar los 120 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Observaciones))
                .WithMessage("Las observaciones no pueden superar los 500 caracteres.");

            RuleFor(x => x.FechaNacimiento)
                .Must(BeAValidDate)
                .When(x => !string.IsNullOrWhiteSpace(x.FechaNacimiento))
                .WithMessage("La fecha de nacimiento debe tener un formato válido (yyyy-MM-dd).");

            RuleFor(x => x.FechaNacimiento)
                .Must(NotBeFutureDate)
                .When(x => !string.IsNullOrWhiteSpace(x.FechaNacimiento))
                .WithMessage("La fecha de nacimiento no puede ser futura.");
        }

        private static bool BeAValidDate(string? fechaNacimiento) =>
            DateOnly.TryParseExact(fechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

        private static bool NotBeFutureDate(string? fechaNacimiento)
        {
            var isValid = DateOnly.TryParseExact(fechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha);
            return !isValid || fecha <= DateOnly.FromDateTime(DateTime.UtcNow.Date);
        }
    }
}
