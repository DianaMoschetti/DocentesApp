using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Validators.Docentes;
using FluentValidation.TestHelper;

namespace DocentesApp.Tests.Validators.Docentes;

public class UpdateContactoDocenteDtoValidatorTests
{
    private readonly UpdateContactoDocenteDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_All_Fields_Are_Null()
    {
        var dto = new UpdateContactoDocenteDto();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Debe enviar al menos un dato de contacto para actualizar.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Email = "   "
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El email no puede estar vacío.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Has_Invalid_Format()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Email = "correo-invalido"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El email no tiene un formato válido.");
    }

    [Fact]
    public void Should_Have_Error_When_EmailAlternativo_Has_Invalid_Format()
    {
        var dto = new UpdateContactoDocenteDto
        {
            EmailAlternativo = "otro-invalido"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.EmailAlternativo)
            .WithErrorMessage("El email alternativo no tiene un formato válido.");
    }

    [Fact]
    public void Should_Have_Error_When_Emails_Are_Equal()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Email = "test@test.com",
            EmailAlternativo = "test@test.com"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("El email y el email alternativo no pueden ser iguales.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Only_Email_Is_Valid()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Email = "test@test.com"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Only_Celular_Is_Valid()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Celular = "3415555555"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Celular_Exceeds_Max_Length()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Celular = new string('1', 31)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Celular)
            .WithErrorMessage("El celular no puede superar los 30 caracteres.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Multiple_Valid_Fields_Are_Sent()
    {
        var dto = new UpdateContactoDocenteDto
        {
            Email = "test@test.com",
            EmailAlternativo = "alt@test.com",
            Celular = "3415555555",
            Direccion = "Calle 123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
