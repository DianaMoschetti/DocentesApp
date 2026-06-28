using DocentesApp.Shared.DTOs.Dedicaciones;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Dedicaciones
{
    public class CreateDedicacionDtoValidator : AbstractValidator<CreateDedicacionDto>
    {
        public CreateDedicacionDtoValidator()
        {
            RuleFor(x => x.DescTipo)
                .IsInEnum().WithMessage("El tipo de dedicación es obligatorio.");

            RuleFor(x => x.CantidadHoras)
                .GreaterThan(0).WithMessage("La cantidad de horas debe ser mayor a cero.");

            RuleFor(x => x.CantidadDedicacion)
                .GreaterThan(0).WithMessage("La cantidad de dedicación debe ser mayor a cero.");
        }
    }
}