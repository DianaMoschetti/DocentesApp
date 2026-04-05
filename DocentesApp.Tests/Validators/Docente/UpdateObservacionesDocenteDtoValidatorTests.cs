using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Validators.Docentes;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Docentes;

public class UpdateObservacionesDocenteDtoValidatorTests
{
    private readonly UpdateObservacionesDocenteDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Observaciones_Is_Null()
    {
        var dto = new UpdateObservacionesDocenteDto();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Debe enviar observaciones para actualizar.");
    }

    [Fact]
    public void Should_Have_Error_When_Observaciones_Is_Empty()
    {
        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "   "
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Debe enviar observaciones para actualizar.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Observaciones_Is_Valid()
    {
        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Nueva observación"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Observaciones_Exceeds_Max_Length()
    {
        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = new string('a', 501)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Observaciones)
            .WithErrorMessage("Las observaciones no pueden superar los 500 caracteres.");
    }
}
