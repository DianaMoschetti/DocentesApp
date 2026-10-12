using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Designaciones;
using DocentesApp.Shared.Validators.Designaciones;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Designaciones;

// Aplica ActividadReglas: qué especificación lleva asignatura y/o curso
public class UpsertActividadDtoValidatorTests
{
    private readonly UpsertActividadDtoValidator _validator = new();

    [Theory]
    [InlineData(EspecificacionCargo.Docencia, 1, 1)]
    [InlineData(EspecificacionCargo.Laboratorio, 1, null)]
    [InlineData(EspecificacionCargo.JefeDeCatedra, 1, null)]
    [InlineData(EspecificacionCargo.JefeDeCatedra, null, null)]
    [InlineData(EspecificacionCargo.Investigacion, null, null)]
    [InlineData(EspecificacionCargo.Gestion, null, null)]
    [InlineData(EspecificacionCargo.Otro, null, null)]
    [InlineData(EspecificacionCargo.Otro, 1, 1)]
    public void Should_Not_Have_Errors_When_Actividad_Follows_Rules(EspecificacionCargo especificacion, int? asignaturaId, int? cursoId)
    {
        var dto = new UpsertActividadDto { Especificacion = especificacion, AsignaturaId = asignaturaId, CursoId = cursoId };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(EspecificacionCargo.Docencia)]
    [InlineData(EspecificacionCargo.Laboratorio)]
    public void Should_Have_Error_When_Asignatura_Is_Required_And_Missing(EspecificacionCargo especificacion)
    {
        var dto = new UpsertActividadDto { Especificacion = especificacion, CursoId = especificacion == EspecificacionCargo.Docencia ? 1 : null };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AsignaturaId);
    }

    [Fact]
    public void Should_Have_Error_When_Docencia_Without_Curso()
    {
        var dto = new UpsertActividadDto { Especificacion = EspecificacionCargo.Docencia, AsignaturaId = 1 };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CursoId);
    }

    [Fact]
    public void Should_Have_Error_When_Gestion_Has_Asignatura()
    {
        var dto = new UpsertActividadDto { Especificacion = EspecificacionCargo.Gestion, AsignaturaId = 1 };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AsignaturaId);
    }

    [Theory]
    [InlineData(EspecificacionCargo.Laboratorio)]
    [InlineData(EspecificacionCargo.JefeDeCatedra)]
    [InlineData(EspecificacionCargo.Investigacion)]
    [InlineData(EspecificacionCargo.Gestion)]
    public void Should_Have_Error_When_Curso_Must_Be_Empty(EspecificacionCargo especificacion)
    {
        var dto = new UpsertActividadDto
        {
            Especificacion = especificacion,
            AsignaturaId = especificacion == EspecificacionCargo.Laboratorio ? 1 : null,
            CursoId = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CursoId);
    }

    [Fact]
    public void Should_Have_Error_When_Horario_Exceeds_Max_Length()
    {
        var dto = new UpsertActividadDto { Especificacion = EspecificacionCargo.Gestion, Horario = new string('x', 201) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Horario)
            .WithErrorMessage("El horario no puede superar los 200 caracteres.");
    }
}
