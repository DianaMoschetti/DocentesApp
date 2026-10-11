using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Domain.Services;
using DocentesApp.Shared.DTOs.Designaciones;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Designaciones
{
    // Reglas de una actividad que no necesitan la base de datos.
    // Que la asignatura pertenezca a la UDB del cargo lo valida el DesignacionService.
    public class UpsertActividadDtoValidator : AbstractValidator<UpsertActividadDto>
    {
        public UpsertActividadDtoValidator()
        {
            RuleFor(x => x.Especificacion)
                .Must(e => e.EsValorVigente()).WithMessage("La especificación de la actividad no es válida.");

            // Asignatura y curso según la especificación (ver ActividadReglas)
            RuleFor(x => x.AsignaturaId)
                .NotNull()
                .When(x => ActividadReglas.Asignatura(x.Especificacion) == Requisito.Obligatorio)
                .WithMessage(x => $"La asignatura es obligatoria para {x.Especificacion.Descripcion()}.");
            RuleFor(x => x.AsignaturaId)
                .Null()
                .When(x => ActividadReglas.Asignatura(x.Especificacion) == Requisito.Vacio)
                .WithMessage(x => $"Una actividad de {x.Especificacion.Descripcion()} no lleva asignatura.");
            RuleFor(x => x.AsignaturaId)
                .GreaterThan(0).When(x => x.AsignaturaId.HasValue)
                .WithMessage("La asignatura seleccionada no es válida.");

            RuleFor(x => x.CursoId)
                .NotNull()
                .When(x => ActividadReglas.Curso(x.Especificacion) == Requisito.Obligatorio)
                .WithMessage(x => $"El curso es obligatorio para {x.Especificacion.Descripcion()}.");
            RuleFor(x => x.CursoId)
                .Null()
                .When(x => ActividadReglas.Curso(x.Especificacion) == Requisito.Vacio)
                .WithMessage(x => $"Una actividad de {x.Especificacion.Descripcion()} no lleva curso.");
            RuleFor(x => x.CursoId)
                .GreaterThan(0).When(x => x.CursoId.HasValue)
                .WithMessage("El curso seleccionado no es válido.");

            RuleFor(x => x.Horario)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Horario))
                .WithMessage("El horario no puede superar los 200 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Observaciones))
                .WithMessage("Las observaciones de la actividad no pueden superar los 500 caracteres.");
        }
    }
}
