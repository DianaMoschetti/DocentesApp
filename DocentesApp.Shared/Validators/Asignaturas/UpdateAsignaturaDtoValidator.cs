using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Asignaturas;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Asignaturas
{
    public class UpdateAsignaturaDtoValidator : AbstractValidator<UpdateAsignaturaDto>
    {
        public UpdateAsignaturaDtoValidator()
        {
            RuleFor(x => x.NombreAsignatura)
                .Must(v => Enum.IsDefined(typeof(Materia), v))
                .WithMessage("La materia seleccionada no es válida.");

            RuleFor(x => x.Frecuencia)
                .Must(v => Enum.IsDefined(typeof(Frecuencia), v))
                .WithMessage("La frecuencia seleccionada no es válida.");

            RuleFor(x => x.Nivel)
                .Must(v => Enum.IsDefined(typeof(Nivel), v))
                .WithMessage("El nivel seleccionado no es válido.");
        }
    }
}
