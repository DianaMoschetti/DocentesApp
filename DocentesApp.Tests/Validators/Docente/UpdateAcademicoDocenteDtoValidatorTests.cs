using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Validators.Docentes;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Docentes;

public class UpdateAcademicoDocenteDtoValidatorTests
{
    private readonly UpdateAcademicoDocenteDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_MaxNivelAcademico_Is_Null()
    {
        var dto = new UpdateAcademicoDocenteDto();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Debe enviar el máximo nivel académico a actualizar.");
    }

    [Fact]
    public void Should_Have_Error_When_MaxNivelAcademico_Is_Empty()
    {
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "   "
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Debe enviar el máximo nivel académico a actualizar.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_MaxNivelAcademico_Is_Valid()
    {
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "Universitario"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_MaxNivelAcademico_Exceeds_Max_Length()
    {
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = new string('a', 121)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MaxNivelAcademico)
            .WithErrorMessage("El máximo nivel académico no puede superar los 120 caracteres.");
    }
}
