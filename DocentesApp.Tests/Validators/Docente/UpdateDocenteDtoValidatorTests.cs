using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Validators.Docentes;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Docentes;

public class UpdateDocenteDtoValidatorTests
{
    private readonly UpdateDocenteDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Nombre_Is_Empty()
    {
        var dto = BuildValidDto();
        dto.Nombre = string.Empty;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Nombre)
            .WithErrorMessage("El nombre es obligatorio.");
    }

    [Fact]
    public void Should_Have_Error_When_Dni_Exceeds_Max_Length()
    {
        var dto = BuildValidDto();
        dto.Dni = new string('1', 16);

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Dni)
            .WithErrorMessage("El DNI no puede superar los 15 caracteres.");
    }

    [Fact]
    public void Should_Have_Error_When_MaxNivelAcademico_Exceeds_Max_Length()
    {
        var dto = BuildValidDto();
        dto.MaxNivelAcademico = new string('a', 121);

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MaxNivelAcademico)
            .WithErrorMessage("El máximo nivel académico no puede superar los 120 caracteres.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Is_Valid()
    {
        var dto = BuildValidDto();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_FechaNacimiento_Has_Invalid_Format()
    {
        var dto = BuildValidDto();
        dto.FechaNacimiento = "31-12-2000";

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.FechaNacimiento)
            .WithErrorMessage("La fecha de nacimiento debe tener un formato válido (yyyy-MM-dd).");
    }

    private static UpdateDocenteDto BuildValidDto() => new()
    {
        Nombre = "Juan",
        Apellido = "Perez",
        Dni = "33.333.333",
        Legajo = 12345,
        Email = "juan@test.com",
        Celular = "3415551234",
        Direccion = "Calle Falsa 123",
        MaxNivelAcademico = "Universitario",
        Observaciones = "Observaciones"
    };
}
