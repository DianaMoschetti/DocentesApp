using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Docentes;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Docentes
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