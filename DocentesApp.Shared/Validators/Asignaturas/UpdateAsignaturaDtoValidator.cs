using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Asignaturas;
using FluentValidation;
namespace DocentesApp.Shared.Validators.Asignaturas
{
    public class UpdateAsignaturaDtoValidator : AbstractValidator<UpdateAsignaturaDto>
    {
        public UpdateAsignaturaDtoValidator()
        {
            // [Diana desde v4.0 OBSOLETO] NombreAsignatura (enum Materia) reemplazado por Nombre (string)
            // RuleFor(x => x.NombreAsignatura)
            //     .Must(v => Enum.IsDefined(typeof(Materia), v))
            //     .WithMessage("La materia seleccionada no es válida.");
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la asignatura es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");
            RuleFor(x => x.Frecuencia)
                .IsInEnum().WithMessage("La frecuencia seleccionada no es válida.");
            RuleFor(x => x.Nivel)
                .IsInEnum().WithMessage("El nivel seleccionado no es válido.");
            RuleFor(x => x.UdbId)
                .NotNull().WithMessage("La UDB es obligatoria.")
                .GreaterThan(0).WithMessage("La UDB es obligatoria.");
        }
    }
}