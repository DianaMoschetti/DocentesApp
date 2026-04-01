using DocentesApp.Application.DTOs.Docentes;
using FluentValidation;

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
        }
    }
}