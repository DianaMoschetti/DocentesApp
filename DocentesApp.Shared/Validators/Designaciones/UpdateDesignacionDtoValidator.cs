using DocentesApp.Domain.Extensions;
using DocentesApp.Shared.DTOs.Designaciones;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Designaciones
{
    public class UpdateDesignacionDtoValidator : AbstractValidator<UpdateDesignacionDto>
    {
        public UpdateDesignacionDtoValidator()
        {
            RuleFor(x => x.FechaInicio)
                .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

            RuleFor(x => x.FechaFin)
                .GreaterThanOrEqualTo(x => x.FechaInicio)
                .When(x => x.FechaFin.HasValue)
                .WithMessage("La fecha de fin no puede ser anterior a la fecha de inicio.");

            RuleFor(x => x.EstadoDesignacion)
                .Must(e => e.EsValorVigente()).WithMessage("El estado seleccionado no es válido.");

            RuleFor(x => x.NroResolucion)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.NroResolucion))
                .WithMessage("El número de resolución no puede superar los 50 caracteres.");

            RuleFor(x => x.NroNota)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.NroNota))
                .WithMessage("El número de nota no puede superar los 50 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Observaciones))
                .WithMessage("Las observaciones no pueden superar los 500 caracteres.");

            RuleFor(x => x.Cargos)
                .NotEmpty().WithMessage("La designación debe tener al menos un cargo.");

            RuleForEach(x => x.Cargos)
                .SetValidator(new UpsertCargoDesignacionDtoValidator());

            // Ids de cargos repetidos en el mismo envío
            RuleFor(x => x.Cargos)
                .Must(c => c.Where(x => x.Id.HasValue).GroupBy(x => x.Id).All(g => g.Count() == 1))
                .WithMessage("Hay cargos repetidos en la designación.");
        }
    }
}
