using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Services;
using DocentesApp.Data.Context;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Tests.Helpers;
using FluentAssertions;

public class DocenteServiceTests
{
    private readonly DocentesDbContext _context;
    private readonly MapsterMapper.IMapper _mapper;
    private readonly DocenteService _service;
    private readonly IDocenteRepository _docenteRepository;

    public DocenteServiceTests()
    {
        _docenteRepository = new DocenteRepository(_context);
        _context = DbContextFactory.Create();
        _mapper = MapperFactory.Create();
        _service = new DocenteService(_docenteRepository, _mapper);
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
            Legajo = 123
        };

        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(docente.Id);

        // Assert
        //FluentAssertions.AssertionExtensions.Should(result).NotBeNull();
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

    [Fact]
    public async Task CreateAsync_WhenLegajoExists_ThrowsBadRequestException()
    {
        // Arrange
        var docenteExistente = new Docente
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 123
        };

        _context.Docentes.Add(docenteExistente);
        await _context.SaveChangesAsync();

        var dto = new CreateDocenteDto
        {
            Nombre = "Otro",
            Apellido = "Docente",
            Legajo = 123
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
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 123
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        //result.Should().NotBeNull();
        result.NombreCompleto.Should().Be("Perez, Juan");
        _context.Docentes.Count().Should().Be(1);
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
            Legajo = 123
        };

        _context.Docentes.Add(docente);

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
}