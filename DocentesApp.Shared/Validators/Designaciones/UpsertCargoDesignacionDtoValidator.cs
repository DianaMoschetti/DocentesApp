using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Domain.Services;
using DocentesApp.Shared.DTOs.Designaciones;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Designaciones
{
    // Reglas de un cargo que no necesitan la base de datos.
    // Las de suplencia que sí la necesitan (el cargo reemplazado existe, está en licencia, es de la misma UDB,
    // de otro docente y no tiene otro suplente vigente) las valida el DesignacionService.
    public class UpsertCargoDesignacionDtoValidator : AbstractValidator<UpsertCargoDesignacionDto>
    {
        public UpsertCargoDesignacionDtoValidator()
        {
            RuleFor(x => x.Cargo)
                .IsInEnum().WithMessage("El cargo seleccionado no es válido.");

            RuleFor(x => x.Condicion)
                .Must(c => c.EsValorVigente()).WithMessage("La condición seleccionada no es válida.");

            RuleFor(x => x.TipoDedicacion)
                .Must(d => d.EsValorVigente()).WithMessage("La dedicación seleccionada no es válida.");

            // El Ayudante de 2° solo puede tener dedicación Simple
            RuleFor(x => x.TipoDedicacion)
                .Must((cargo, dedicacion) => CalculadoraPuntos.EsDedicacionValida(cargo.Cargo, dedicacion))
                .WithMessage("El Ayudante de 2° solo puede tener dedicación Simple.");

            RuleFor(x => x.CantidadDedicacion)
                .GreaterThan(0).WithMessage("La cantidad de dedicaciones debe ser mayor a cero.")
                .Must(c => c * 2 == decimal.Truncate(c * 2))
                .WithMessage("La cantidad de dedicaciones debe ser múltiplo de 0,5 (0,5 / 1 / 1,5 ...).");

            RuleFor(x => x.UdbId)
                .GreaterThan(0).WithMessage("La UDB del cargo es obligatoria.");

            // Suplencia: si reemplaza a otro cargo, la condición debe ser Suplente (un suplente puede no tener reemplazo)
            RuleFor(x => x.Condicion)
                .Equal(Condicion.Suplente)
                .When(x => x.ReemplazaACargoId.HasValue)
                .WithMessage("Si el cargo reemplaza a otro, la condición debe ser Suplente.");

            RuleFor(x => x.ReemplazaACargoId)
                .NotEqual(x => x.Id)
                .When(x => x.Id.HasValue && x.ReemplazaACargoId.HasValue)
                .WithMessage("Un cargo no puede reemplazarse a sí mismo.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Observaciones))
                .WithMessage("Las observaciones del cargo no pueden superar los 500 caracteres.");

            RuleFor(x => x.Actividades)
                .NotEmpty().WithMessage("Cada cargo debe tener al menos una actividad.");

            RuleForEach(x => x.Actividades)
                .SetValidator(new UpsertActividadDtoValidator());
        }
    }
}
