using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Validators.Docentes;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Docentes;

public class CreateDocenteDtoValidatorTests
{
    private readonly CreateDocenteDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Dni_Is_Empty()
    {
        var dto = BuildValidDto();
        dto.Dni = string.Empty;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Dni)
            .WithErrorMessage("El DNI es obligatorio.");
    }

    [Fact]
    public void Should_Have_Error_When_Legajo_Is_Invalid()
    {
        var dto = BuildValidDto();
        dto.Legajo = 0;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Legajo)
            .WithErrorMessage("El legajo debe ser mayor a 0.");
    }

    [Fact]
    public void Should_Have_Error_When_Observaciones_Exceeds_Max_Length()
    {
        var dto = BuildValidDto();
        dto.Observaciones = new string('a', 501);

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Observaciones)
            .WithErrorMessage("Las observaciones no pueden superar los 500 caracteres.");
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
        dto.FechaNacimiento = "20/01/1990";

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.FechaNacimiento)
            .WithErrorMessage("La fecha de nacimiento debe tener un formato válido (yyyy-MM-dd).");
    }

    [Fact]
    public void Should_Have_Error_When_FechaNacimiento_Is_Future()
    {
        var dto = BuildValidDto();
        dto.FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)).ToString("yyyy-MM-dd");

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.FechaNacimiento)
            .WithErrorMessage("La fecha de nacimiento no puede ser futura.");
    }

    private static CreateDocenteDto BuildValidDto() => new()
    {
        Dni = "33.333.333",
        Legajo = 12345,
        Nombre = "Juan",
        Apellido = "Perez",
        Email = "juan@test.com",
        Celular = "3415551234",
        Direccion = "Calle Falsa 123",
        MaxNivelAcademico = "Universitario",
        Observaciones = "Observaciones"
    };
}
