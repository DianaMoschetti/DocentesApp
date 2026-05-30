using DocentesApp.Application.DTOs.Docentes;
using FluentValidation;

namespace DocentesApp.Application.Validators.Docentes
{
    public class UpdateObservacionesDocenteDtoValidator : AbstractValidator<UpdateObservacionesDocenteDto>
    {
        public UpdateObservacionesDocenteDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Observaciones))
                .WithMessage("Debe enviar observaciones para actualizar.");

            RuleFor(x => x.Observaciones)
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("Las observaciones no pueden estar vacías.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Observaciones))
                .WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }
    }
}
