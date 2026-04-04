using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Services;
using DocentesApp.Domain.Entities;
using FluentAssertions;
using MapsterMapper;
using Moq;

namespace DocentesApp.Tests;

public class DocenteServiceUnitTests
{
    private readonly Mock<IDocenteRepository> _docenteRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DocenteService _service;

    public DocenteServiceUnitTests()
    {
        _docenteRepositoryMock = new Mock<IDocenteRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new DocenteService(
            _docenteRepositoryMock.Object,
            _mapperMock.Object);
    }

    #region Get

    [Fact]
    public async Task GetAllAsync_WhenDocentesExist_ReturnsListDocenteDto()
    {
        // Arrange
        var docentes = new List<Docente>
        {
            new()
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Legajo = 12345,
                Dni = "11.111.111",
                Email = "juan@test.com"
            },
            new()
            {
                Id = 2,
                Nombre = "Ana",
                Apellido = "Gomez",
                Legajo = 45678,
                Dni = "22.222.222",
                Email = "ana@test.com"
            }
        };

        var docentesDto = new List<ListDocenteDto>
        {
            new()
            {
                Id = 1,
                NombreCompleto = "Perez, Juan",
                Legajo = 12345,
                Dni = "11.111.111",
                Email = "juan@test.com"
            },
            new()
            {
                Id = 2,
                NombreCompleto = "Gomez, Ana",
                Legajo = 45678,
                Dni = "22.222.222",
                Email = "ana@test.com"
            }
        };

        _docenteRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(docentes);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ListDocenteDto>>(docentes))
            .Returns(docentesDto);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(d => d.NombreCompleto == "Perez, Juan");
        result.Should().Contain(d => d.NombreCompleto == "Gomez, Ana");

        _docenteRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<ListDocenteDto>>(docentes), Times.Once);
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

        var docenteDto = new DocenteDto
        {
            Id = 1,
            Legajo = 12345,
            NombreCompleto = "Perez, Juan",
            Dni = "11.111.111"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        _mapperMock
            .Setup(m => m.Map<DocenteDto>(docente))
            .Returns(docenteDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.NombreCompleto.Should().Be("Perez, Juan");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mapperMock.Verify(m => m.Map<DocenteDto>(docente), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Docente?)null);

        // Act
        Func<Task> act = async () => await _service.GetByIdAsync(999);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mapperMock.Verify(m => m.Map<DocenteDto>(It.IsAny<Docente>()), Times.Never);
    }

    #endregion

    #region Create

    [Fact]
    public async Task CreateAsync_WhenLegajoExists_ThrowsBadRequestException()
    {
        // Arrange
        var dto = new CreateDocenteDto
        {
            Dni = "12.345.678",
            Nombre = "Otro",
            Apellido = "Docente",
            Legajo = 12345
        };

        _docenteRepositoryMock
            .Setup(r => r.ExistsByLegajoAsync(dto.Legajo))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Ya existe un docente con ese legajo.");

        _docenteRepositoryMock.Verify(r => r.ExistsByLegajoAsync(dto.Legajo), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsByDniAsync(It.IsAny<string>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDniExists_ThrowsBadRequestException()
    {
        // Arrange
        var dto = new CreateDocenteDto
        {
            Dni = "12.345.678",
            Nombre = "Juan",
            Apellido = "Perez",
            Legajo = 12345
        };

        _docenteRepositoryMock
            .Setup(r => r.ExistsByLegajoAsync(dto.Legajo))
            .ReturnsAsync(false);

        _docenteRepositoryMock
            .Setup(r => r.ExistsByDniAsync(dto.Dni))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Ya existe un docente con ese DNI.");

        _docenteRepositoryMock.Verify(r => r.ExistsByLegajoAsync(dto.Legajo), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsByDniAsync(dto.Dni), Times.Once);
        _docenteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_CreatesDocenteAndReturnsDto()
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

        var docente = new Docente
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "12.345.678",
            Legajo = 12345,
            MaxNivelAcademico = "Universitario",
            Observaciones = "Nuevo docente"
        };

        var docenteDto = new DocenteDto
        {
            Id = 1,
            Legajo = 12345,
            NombreCompleto = "Perez, Juan",
            Dni = "12.345.678"
        };

        _docenteRepositoryMock
            .Setup(r => r.ExistsByLegajoAsync(dto.Legajo))
            .ReturnsAsync(false);

        _docenteRepositoryMock
            .Setup(r => r.ExistsByDniAsync(dto.Dni))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(m => m.Map<Docente>(dto))
            .Returns(docente);

        _docenteRepositoryMock
            .Setup(r => r.AddAsync(docente))
            .Callback(() => docente.Id = 1)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<DocenteDto>(docente))
            .Returns(docenteDto);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.NombreCompleto.Should().Be("Perez, Juan");

        _docenteRepositoryMock.Verify(r => r.ExistsByLegajoAsync(dto.Legajo), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsByDniAsync(dto.Dni), Times.Once);
        _mapperMock.Verify(m => m.Map<Docente>(dto), Times.Once);
        _docenteRepositoryMock.Verify(r => r.AddAsync(docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<DocenteDto>(docente), Times.Once);
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

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Docente?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenLegajoAlreadyExistsInAnotherDocente_ThrowsBadRequestException()
    {
        // Arrange
        var docente = new Docente
        {
            Id = 2,
            Nombre = "Ana",
            Apellido = "Gomez",
            Legajo = 45678,
            Dni = "22.222.222"
        };

        var dto = new UpdateDocenteDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Dni = "22.222.222",
            Legajo = 12345,
            Email = "ana@test.com"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(docente);

        _docenteRepositoryMock
            .Setup(r => r.ExistsByLegajoAsync(dto.Legajo))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.UpdateAsync(2, dto);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Ya existe un docente con ese legajo.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(2), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsByLegajoAsync(dto.Legajo), Times.Once);
        _mapperMock.Verify(m => m.Map(dto, docente), Times.Never);
        _docenteRepositoryMock.Verify(r => r.Update(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenDniAlreadyExistsInAnotherDocente_ThrowsBadRequestException()
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

        var dto = new UpdateDocenteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "22.222.222",
            Legajo = 12345,
            Email = "nuevo@test.com"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        _docenteRepositoryMock
            .Setup(r => r.ExistsAnotherByDniAsync(dto.Dni!, 1))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.UpdateAsync(1, dto);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Ya existe un docente con ese DNI.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsAnotherByDniAsync(dto.Dni!, 1), Times.Once);
        _mapperMock.Verify(m => m.Map(dto, docente), Times.Never);
        _docenteRepositoryMock.Verify(r => r.Update(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        _docenteRepositoryMock
            .Setup(r => r.ExistsByLegajoAsync(dto.Legajo))
            .ReturnsAsync(false);

        _docenteRepositoryMock
            .Setup(r => r.ExistsAnotherByDniAsync(dto.Dni!, 1))
            .ReturnsAsync(false);

        // Act
        await _service.UpdateAsync(1, dto);

        // Assert
        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsByLegajoAsync(dto.Legajo), Times.Once);
        _docenteRepositoryMock.Verify(r => r.ExistsAnotherByDniAsync(dto.Dni!, 1), Times.Once);
        _mapperMock.Verify(m => m.Map(dto, docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateContactoAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateContactoDocenteDto
        {
            Email = "nuevo@test.com"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Docente?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateContactoAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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
            Observaciones = "No cambiar"
        };

        var dto = new UpdateContactoDocenteDto
        {
            Email = "nuevo@test.com",
            EmailAlternativo = "alternativo@test.com",
            Celular = "222222",
            Direccion = "Direccion nueva"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        // Act
        await _service.UpdateContactoAsync(1, dto);

        // Assert
        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mapperMock.Verify(m => m.Map(dto, docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAcademicoAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "Universitario"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Docente?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateAcademicoAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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

        var dto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "Universitario"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        // Act
        await _service.UpdateAcademicoAsync(1, dto);

        // Assert
        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mapperMock.Verify(m => m.Map(dto, docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateObservacionesAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Nueva observación"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Docente?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateObservacionesAsync(999, dto);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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

        var dto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Nueva observación"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        // Act
        await _service.UpdateObservacionesAsync(1, dto);

        // Assert
        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mapperMock.Verify(m => m.Map(dto, docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Update(docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteAsync_WhenDocenteDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Docente?)null);

        // Act
        Func<Task> act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("No se encontró el docente con ID 999.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _docenteRepositoryMock.Verify(r => r.HasDesignacionesAsync(It.IsAny<int>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.Delete(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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
            Dni = "11.111.111"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        _docenteRepositoryMock
            .Setup(r => r.HasDesignacionesAsync(1))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.DeleteAsync(1);

        // Assert
        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("No se puede eliminar el docente porque tiene designaciones asociadas.");

        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _docenteRepositoryMock.Verify(r => r.HasDesignacionesAsync(1), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Delete(It.IsAny<Docente>()), Times.Never);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
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
            Dni = "11.111.111"
        };

        _docenteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(docente);

        _docenteRepositoryMock
            .Setup(r => r.HasDesignacionesAsync(1))
            .ReturnsAsync(false);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _docenteRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _docenteRepositoryMock.Verify(r => r.HasDesignacionesAsync(1), Times.Once);
        _docenteRepositoryMock.Verify(r => r.Delete(docente), Times.Once);
        _docenteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion
}