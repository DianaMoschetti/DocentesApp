using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Docentes;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Docentes
{
    public class UpdateAcademicoDocenteDtoValidator : AbstractValidator<UpdateAcademicoDocenteDto>
    {
        public UpdateAcademicoDocenteDtoValidator()
        {
            // [Diana desde v4.0 OBSOLETO] el máximo nivel académico pasa a ser opcional
            //RuleFor(x => x.MaxNivelAcademico)
            //    .NotNull().WithMessage("El máximo nivel académico es obligatorio.")
            //    .IsInEnum().WithMessage("El nivel académico seleccionado no es válido.");
            RuleFor(x => x.MaxNivelAcademico)
                .IsInEnum().When(x => x.MaxNivelAcademico.HasValue)
                .WithMessage("El nivel académico seleccionado no es válido.");
        }
    }
}