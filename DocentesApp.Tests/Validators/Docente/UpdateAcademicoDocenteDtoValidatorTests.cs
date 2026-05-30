using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.Validators.Docentes;
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
            MaxNivelAcademico = null
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
            MaxNivelAcademico = Titulo.Universitario
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_MaxNivelAcademico_IsNull()
    {
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = null
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.MaxNivelAcademico)
            .WithErrorMessage("El máximo nivel académico es obligatorio.");
    }

    [Fact]
    public void Should_Have_Error_When_MaxNivelAcademico_IsInvalidEnum()
    {
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = (Titulo)99 // valor que no existe en el enum
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.MaxNivelAcademico)
            .WithErrorMessage("El nivel académico seleccionado no es válido.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_MaxNivelAcademico_IsValid()
    {
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = Titulo.Universitario
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.MaxNivelAcademico);
    }
}
