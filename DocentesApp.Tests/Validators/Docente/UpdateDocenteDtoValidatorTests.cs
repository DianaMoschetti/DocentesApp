using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Shared.Validators.Docentes;
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
      
    // v5: el DNI es obligatorio también al editar
    [Fact]
    public void Should_Have_Error_When_Dni_Is_Empty()
    {
        var dto = BuildValidDto();
        dto.Dni = string.Empty;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Dni)
            .WithErrorMessage("El DNI es obligatorio.");
    }

    // v5: el legajo es opcional, pero si viene debe ser mayor a 0
    [Fact]
    public void Should_Not_Have_Error_When_Legajo_Is_Null()
    {
        var dto = BuildValidDto();
        dto.Legajo = null;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Legajo);
    }

    [Fact]
    public void Should_Have_Error_When_Legajo_Is_Zero()
    {
        var dto = BuildValidDto();
        dto.Legajo = 0;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Legajo)
            .WithErrorMessage("El legajo debe ser mayor a 0.");
    }

    // El máximo nivel académico es opcional
    [Fact]
    public void Should_Not_Have_Error_When_MaxNivelAcademico_Is_Null()
    {
        var dto = BuildValidDto();
        dto.MaxNivelAcademico = null;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.MaxNivelAcademico);
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
        MaxNivelAcademico = Titulo.Universitario,
        Observaciones = "Observaciones"
    };
}
