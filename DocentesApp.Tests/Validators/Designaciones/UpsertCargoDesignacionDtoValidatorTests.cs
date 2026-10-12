using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Designaciones;
using DocentesApp.Shared.Validators.Designaciones;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Designaciones;

public class UpsertCargoDesignacionDtoValidatorTests
{
    private readonly UpsertCargoDesignacionDtoValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Cargo_Is_Valid()
    {
        var result = _validator.TestValidate(BuildValidDto());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(TipoDedicacion.SemiExclusiva)]
    [InlineData(TipoDedicacion.Exclusiva)]
    public void Should_Have_Error_When_AyudanteSegunda_Is_Not_Simple(TipoDedicacion dedicacion)
    {
        var dto = BuildValidDto();
        dto.Cargo = CargoDocente.AyudanteSegunda;
        dto.TipoDedicacion = dedicacion;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.TipoDedicacion)
            .WithErrorMessage("El Ayudante de 2° solo puede tener dedicación Simple.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.75)]
    public void Should_Have_Error_When_CantidadDedicacion_Is_Not_Positive_Multiple_Of_Half(double cantidad)
    {
        var dto = BuildValidDto();
        dto.CantidadDedicacion = (decimal)cantidad;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CantidadDedicacion);
    }

    [Fact]
    public void Should_Not_Have_Error_When_CantidadDedicacion_Is_One_And_A_Half()
    {
        var dto = BuildValidDto();
        dto.CantidadDedicacion = 1.5m;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.CantidadDedicacion);
    }

#pragma warning disable CS0618 // valores obsoletos usados a propósito
    [Fact]
    public void Should_Have_Error_When_Condicion_Is_Obsolete()
    {
        var dto = BuildValidDto();
        dto.Condicion = Condicion.LicenciaConHaberes;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Condicion)
            .WithErrorMessage("La condición seleccionada no es válida.");
    }

    [Fact]
    public void Should_Have_Error_When_TipoDedicacion_Is_Obsolete()
    {
        var dto = BuildValidDto();
        dto.TipoDedicacion = TipoDedicacion.MediaSimple;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.TipoDedicacion)
            .WithErrorMessage("La dedicación seleccionada no es válida.");
    }
#pragma warning restore CS0618

    // Suplencia (e): si reemplaza a otro cargo, la condición debe ser Suplente
    [Fact]
    public void Should_Have_Error_When_ReemplazaACargo_And_Condicion_Is_Not_Suplente()
    {
        var dto = BuildValidDto();
        dto.ReemplazaACargoId = 10;
        dto.Condicion = Condicion.Interino;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Condicion)
            .WithErrorMessage("Si el cargo reemplaza a otro, la condición debe ser Suplente.");
    }

    // Un suplente puede no tener reemplazo
    [Fact]
    public void Should_Not_Have_Error_When_Suplente_Without_ReemplazaACargo()
    {
        var dto = BuildValidDto();
        dto.Condicion = Condicion.Suplente;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // Suplencia (a): no puede reemplazarse a sí mismo
    [Fact]
    public void Should_Have_Error_When_ReemplazaACargo_Is_Itself()
    {
        var dto = BuildValidDto();
        dto.Id = 10;
        dto.ReemplazaACargoId = 10;
        dto.Condicion = Condicion.Suplente;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ReemplazaACargoId)
            .WithErrorMessage("Un cargo no puede reemplazarse a sí mismo.");
    }

    [Fact]
    public void Should_Have_Error_When_UdbId_Is_Missing()
    {
        var dto = BuildValidDto();
        dto.UdbId = 0;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.UdbId)
            .WithErrorMessage("La UDB del cargo es obligatoria.");
    }

    [Fact]
    public void Should_Have_Error_When_Actividades_Is_Empty()
    {
        var dto = BuildValidDto();
        dto.Actividades.Clear();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Actividades)
            .WithErrorMessage("Cada cargo debe tener al menos una actividad.");
    }

    [Fact]
    public void Should_Validate_Nested_Actividades()
    {
        var dto = BuildValidDto();
        dto.Actividades[0].CursoId = null; // Docencia sin curso

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("Actividades[0].CursoId");
    }

    private static UpsertCargoDesignacionDto BuildValidDto()
    {
        return new UpsertCargoDesignacionDto
        {
            Cargo = CargoDocente.ProfesorAsociado,
            Condicion = Condicion.Regular,
            TipoDedicacion = TipoDedicacion.SemiExclusiva,
            CantidadDedicacion = 1,
            UdbId = 3,
            Actividades = new List<UpsertActividadDto>
            {
                new UpsertActividadDto
                {
                    Especificacion = EspecificacionCargo.Docencia,
                    AsignaturaId = 1,
                    CursoId = 1
                }
            }
        };
    }
}
