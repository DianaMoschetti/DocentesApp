using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Services;
using DocentesApp.Data.Context;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Tests.Helpers;
using FluentAssertions;

public class DocenteServiceIntegrationTests
{
    private readonly DocentesDbContext _context;
    private readonly MapsterMapper.IMapper _mapper;
    private readonly DocenteService _service;
    private readonly IDocenteRepository _docenteRepository;

    public DocenteServiceIntegrationTests()
    {
        _context = DbContextFactory.Create();
        _mapper = MapperFactory.Create();
        _docenteRepository = new DocenteRepository(_context);
        _service = new DocenteService(_docenteRepository, _mapper);
    }

    #region Get

    [Fact]
    public async Task GetAllAsync_WhenDocentesExist_ReturnsListDocenteDto()
    {
        // Arrange
        _context.Docentes.AddRange(
            new Docente
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Legajo = 12345,
                Dni = "11.111.111",
                Email = "juan@test.com"
            },
            new Docente
            {
                Id = 2,
                Nombre = "Ana",
                Apellido = "Gomez",
                Legajo = 45678,
                Dni = "22.222.222",
                Email = "ana@test.com"
            });

        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(d => d.NombreCompleto == "Perez, Juan");
        result.Should().Contain(d => d.NombreCompleto == "Gomez, Ana");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocenteExists_ReturnsDocenteDto()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111"
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(docente.Id);

        // Assert
        result.Id.Should().Be(docente.Id);
        result.NombreCompleto.Should().Be("Perez, Juan");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _service.GetByIdAsync(999);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");
    }
    #endregion

    #region Create

    [Fact]
    public async Task CreateAsync_WhenLegajoExists_ThrowsBadRequestException()
    {
        // Arrange
        var docenteExistente = new Docente
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "11.111.111",
            Legajo = 12345
        };

        _context.Docentes.Add(docenteExistente);
        await _context.SaveChangesAsync();

        var dto = new CreateDocenteDto
        {
            Dni = "12.345.678",
            Nombre = "Otro",
            Apellido = "Docente",
            Legajo = 12345,

        };

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Ya existe un docente con ese legajo.");
    }

    [Fact]
    public async Task CreateAsync_WhenValid_CreatesDocente()
    {
        // Arrange
        var dto = new CreateDocenteDto
        {
            Dni = "12.345.678",
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            MaxNivelAcademico = "Universitario",
            Observaciones = "Nuevo docente"

        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.NombreCompleto.Should().Be("Perez, Juan");
        _context.Docentes.Count().Should().Be(1);
    }

    #endregion

    #region Update
    [Fact]
    public async Task UpdateAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateDocenteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "12.345.678",
            Legajo = 12345,
            Email = "juan@test.com"
        };

        // Act
        Func<Task> act = async () => await _service.UpdateAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task UpdateAsync_WhenLegajoAlreadyExistsInAnotherDocente_ThrowsBadRequestException()
    {
        // Arrange
        var docente1 = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111"
        };

        var docente2 = new Docente
        {
            Id = 2,
            Nombre = "Ana",
            Apellido = "Gomez",
            Legajo = 45678,
            Dni = "22.222.222"
        };

        _context.Docentes.AddRange(docente1, docente2);
        await _context.SaveChangesAsync();

        var dto = new UpdateDocenteDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Dni = "22.222.222",
            Legajo = 12345,
            Email = "ana@test.com"
        };

        // Act
        Func<Task> act = async () => await _service.UpdateAsync(docente2.Id, dto);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Ya existe un docente con ese legajo.");
    }

    [Fact]
    public async Task UpdateAsync_WhenValid_UpdatesDocente()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "11.111.111",
            Legajo = 12345,
            Email = "viejo@test.com"
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        var dto = new UpdateDocenteDto
        {
            Nombre = "Carlos",
            Apellido = "Lopez",
            Dni = "22.222.222",
            Legajo = 999,
            Email = "nuevo@test.com",
            EmailAlternativo = "alt@test.com",
            Celular = "123456789",
            Direccion = "Calle Falsa 123",
            FechaNacimiento = "2000-01-01",
            MaxNivelAcademico = "Universitario",
            Observaciones = "Actualizado"
        };

        // Act
        await _service.UpdateAsync(docente.Id, dto);

        // Assert
        var docenteActualizado = await _context.Docentes.FindAsync(docente.Id);
        Assert.NotNull(docenteActualizado);

        var actualizado = docenteActualizado!;
        actualizado.Nombre.Should().Be("Carlos");
        actualizado.Apellido.Should().Be("Lopez");
        actualizado.Legajo.Should().Be(999);
        actualizado.Email.Should().Be("nuevo@test.com");
        actualizado.Observaciones.Should().Be("Actualizado");
    }

    [Fact]
    public async Task UpdateContactoAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateContactoDocenteDto
        {
            Email = "nuevo@test.com"
        };

        // Act
        Func<Task> act = async () => await _service.UpdateContactoAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task UpdateContactoAsync_WhenValid_UpdatesOnlyContactoFields()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111",
            Email = "viejo@test.com",
            Celular = "111111",
            Direccion = "Direccion vieja",
            Observaciones = "No cambiar"
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        var dto = new UpdateContactoDocenteDto
        {
            Email = "nuevo@test.com",
            EmailAlternativo = "alternativo@test.com",
            Celular = "222222",
            Direccion = "Direccion nueva"
        };

        // Act
        await _service.UpdateContactoAsync(docente.Id, dto);

        // Assert
        var actualizado = await _context.Docentes.FindAsync(docente.Id);
        actualizado!.Email.Should().Be("nuevo@test.com");
        actualizado.EmailAlternativo.Should().Be("alternativo@test.com");
        actualizado.Celular.Should().Be("222222");
        actualizado.Direccion.Should().Be("Direccion nueva");
        actualizado.Observaciones.Should().Be("No cambiar");
    }

    [Fact]
    public async Task UpdateAcademicoAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "Universitario"
        };

        // Act
        Func<Task> act = async () => await _service.UpdateAcademicoAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task UpdateAcademicoAsync_WhenValid_UpdatesOnlyAcademicoFields()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111",
            MaxNivelAcademico = "Secundario",
            Observaciones = "No cambiar"
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "Universitario"
        };

        // Act
        await _service.UpdateAcademicoAsync(docente.Id, dto);

        // Assert
        var actualizado = await _context.Docentes.FindAsync(docente.Id);
        actualizado!.MaxNivelAcademico.Should().Be("Universitario");
        actualizado.Observaciones.Should().Be("No cambiar");
    }

    [Fact]
    public async Task UpdateObservacionesAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Nueva observación"
        };

        // Act
        Func<Task> act = async () => await _service.UpdateObservacionesAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task UpdateObservacionesAsync_WhenValid_UpdatesOnlyObservaciones()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111",
            Email = "correo@test.com",
            Observaciones = "Vieja observación"
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Nueva observación"
        };

        // Act
        await _service.UpdateObservacionesAsync(docente.Id, dto);

        // Assert
        var actualizado = await _context.Docentes.FindAsync(docente.Id);
        actualizado!.Observaciones.Should().Be("Nueva observación");
        actualizado.Email.Should().Be("correo@test.com");
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task DeleteAsync_WhenDocenteExistsAndHasNoDesignaciones_DeletesDocente()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111",
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeleteAsync(docente.Id);

        // Assert
        _context.Docentes.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenDocenteHasDesignaciones_ThrowsConflictException()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345,
            Dni = "11.111.111",
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        _context.Designaciones.Add(new Designacion
        {
            DocenteId = docente.Id,
            CargoId = 1,
            DedicacionId = 1,
            FechaInicio = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _service.DeleteAsync(docente.Id);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("No se puede eliminar el docente porque tiene designaciones asociadas.");
    }
    #endregion
}