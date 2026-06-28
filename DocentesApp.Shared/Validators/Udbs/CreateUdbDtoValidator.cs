using DocentesApp.Shared.DTOs.Udbs;
using FluentValidation;

namespace DocentesApp.Shared.Validators.Udbs
{
    public class CreateUdbDtoValidator : AbstractValidator<CreateUdbDto>
    {
        public CreateUdbDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");
        }
    }
}
