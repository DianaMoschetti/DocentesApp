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
                .IsInEnum().When(x => x.MaxNivelAcademico.HasValue)
                .WithMessage("El nivel académico seleccionado no es válido.");
        }
    }
}