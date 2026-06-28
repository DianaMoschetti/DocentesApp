using DocentesApp.Shared.DTOs.Cargos;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Cargos
{
    public class CreateCargoDtoValidator : AbstractValidator<CreateCargoDto>
    {
        public CreateCargoDtoValidator()
        {
            RuleFor(x => x.Denominacion)
                .IsInEnum().WithMessage("La denominación es obligatoria.");

            RuleFor(x => x.TipoCargo)
                .IsInEnum().WithMessage("El tipo de cargo es obligatorio.");

            RuleFor(x => x.Condicion)
                .IsInEnum().WithMessage("La condición es obligatoria.");

            RuleFor(x => x.PuntosBase)
                .GreaterThan(0).WithMessage("Los puntos base deben ser mayores a cero.");
        }
    }
}