using DocentesApp.Shared.DTOs.PuntosPorCargo;
using FluentValidation;

namespace DocentesApp.Shared.Validators.PuntosPorCargo
{
    public class UpdatePuntosPorCargoDtoValidator : AbstractValidator<UpdatePuntosPorCargoDto>
    {
        public UpdatePuntosPorCargoDtoValidator()
        {
            RuleFor(x => x.PuntosBase)
                .GreaterThan(0).WithMessage("Los puntos base deben ser mayores a cero.");
        }
    }
}
