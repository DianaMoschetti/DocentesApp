using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Cursos;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Cursos
{
    public class CreateCursoDtoValidator : AbstractValidator<CreateCursoDto>
    {
        public CreateCursoDtoValidator()
        {
            RuleFor(x => x.Turno)
                .Must(v => Enum.IsDefined(typeof(Turno), v))
                .WithMessage("El turno seleccionado no es válido.");

            RuleFor(x => x.Año)
                .Must(v => Enum.IsDefined(typeof(Nivel), v))
                .WithMessage("El año seleccionado no es válido.");

            RuleFor(x => x.Carrera)
                .Must(v => Enum.IsDefined(typeof(Especialidad), v))
                .WithMessage("La carrera seleccionada no es válida.");

            RuleFor(x => x.NroComision)
                .GreaterThan(0).WithMessage("El número de comisión debe ser mayor a cero.");
        }
    }
}
