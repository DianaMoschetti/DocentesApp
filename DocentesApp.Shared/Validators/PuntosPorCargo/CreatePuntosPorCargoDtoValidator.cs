using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.PuntosPorCargo;
using FluentValidation;

namespace DocentesApp.Shared.Validators.PuntosPorCargo
{
    public class CreatePuntosPorCargoDtoValidator : AbstractValidator<CreatePuntosPorCargoDto>
    {
        public CreatePuntosPorCargoDtoValidator()
        {
            RuleFor(x => x.Denominacion)
                .IsInEnum().WithMessage("La denominación es obligatoria.");

            RuleFor(x => x.TipoCargo)
                .IsInEnum().WithMessage("El tipo de cargo es obligatorio.");

            RuleFor(x => x.PuntosBase)
                .GreaterThan(0).WithMessage("Los puntos base deben ser mayores a cero.");
        }
    }
}
