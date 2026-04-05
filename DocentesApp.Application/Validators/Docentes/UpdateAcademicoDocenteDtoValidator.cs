using DocentesApp.Application.DTOs.Docentes;
using FluentValidation;

namespace DocentesApp.Application.Validators.Docentes
{
    public class UpdateAcademicoDocenteDtoValidator : AbstractValidator<UpdateAcademicoDocenteDto>
    {
        public UpdateAcademicoDocenteDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.MaxNivelAcademico))
                .WithMessage("Debe enviar el máximo nivel académico a actualizar.");

            RuleFor(x => x.MaxNivelAcademico)
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("El máximo nivel académico no puede estar vacío.");

            RuleFor(x => x.MaxNivelAcademico)
                .MaximumLength(120)
                .When(x => !string.IsNullOrWhiteSpace(x.MaxNivelAcademico))
                .WithMessage("El máximo nivel académico no puede superar los 120 caracteres.");
        }
    }
}
