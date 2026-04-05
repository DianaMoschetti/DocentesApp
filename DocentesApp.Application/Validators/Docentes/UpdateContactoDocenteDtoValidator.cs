using DocentesApp.Application.DTOs.Docentes;
using FluentValidation;

namespace DocentesApp.Application.Validators.Docentes
{
    public class UpdateContactoDocenteDtoValidator : AbstractValidator<UpdateContactoDocenteDto>
    {
        public UpdateContactoDocenteDtoValidator()
        {
            RuleFor(x => x)
                .Must(HasAtLeastOneValue)
                .WithMessage("Debe enviar al menos un dato de contacto para actualizar.");

            RuleFor(x => x.Email)
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("El email no puede estar vacío.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("El email no tiene un formato válido.");

            RuleFor(x => x.EmailAlternativo)
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("El email alternativo no puede estar vacío.");

            RuleFor(x => x.EmailAlternativo)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.EmailAlternativo))
                .WithMessage("El email alternativo no tiene un formato válido.");

            RuleFor(x => x.Celular)
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("El celular no puede estar vacío.");

            RuleFor(x => x.Direccion)
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("La dirección no puede estar vacía.");

            RuleFor(x => x)
                .Must(x => !AreSameEmails(x.Email, x.EmailAlternativo))
                .WithMessage("El email y el email alternativo no pueden ser iguales.");
        }

        private static bool HasAtLeastOneValue(UpdateContactoDocenteDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Email)
                || !string.IsNullOrWhiteSpace(dto.EmailAlternativo)
                || !string.IsNullOrWhiteSpace(dto.Celular)
                || !string.IsNullOrWhiteSpace(dto.Direccion);
        }

        private static bool AreSameEmails(string? email, string? emailAlternativo)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(emailAlternativo))
                return false;

            return email.Trim().Equals(emailAlternativo.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}