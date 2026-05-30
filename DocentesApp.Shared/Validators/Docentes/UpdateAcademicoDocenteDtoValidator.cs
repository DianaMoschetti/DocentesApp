using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Domain.Enums;
using FluentValidation;

namespace DocentesApp.Application.Validators.Docentes
{
    public class UpdateAcademicoDocenteDtoValidator : AbstractValidator<UpdateAcademicoDocenteDto>
    {
        public UpdateAcademicoDocenteDtoValidator()
        {
            RuleFor(x => x.MaxNivelAcademico)
                .NotNull().WithMessage("El máximo nivel académico es obligatorio.")
                .IsInEnum().WithMessage("El nivel académico seleccionado no es válido.");
        }
    }
}