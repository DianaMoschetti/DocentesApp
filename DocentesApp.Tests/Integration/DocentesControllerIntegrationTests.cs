using DocentesApp.Application.Common.Responses;
using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DocentesApp.Tests.Integration;

public class DocentesControllerIntegrationTests : IntegrationTestBase
{
    // [Diana desde v4.0 OBSOLETO] nunca se asignaban (el constructor los tiene comentados): se usan Client y Factory de IntegrationTestBase
    //private readonly HttpClient _client;
    //private readonly CustomWebApplicationFactory _factory;

    public DocentesControllerIntegrationTests(CustomWebApplicationFactory factory) :base(factory)
    {
        //_factory = factory;
        //_client = factory.CreateClient();

    }

    #region Get
    [Fact]
    public async Task GetDocentes_ReturnsOkAndList()
    {
        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        //var response = await _client.GetAsync("/api/docentes");
        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        var user = await Auth.CreateUserClientAsync();
        //var response = await Client.GetAsync("/api/docentes");
        var response = await user.GetAsync("/api/docentes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var docentes = await response.Content.ReadFromJsonAsync<List<ListDocenteDto>>();
        docentes.Should().NotBeNull();
        docentes.Should().HaveCountGreaterThanOrEqualTo(2);
        docentes.Should().Contain(d => d.NombreCompleto == "Perez, Juan");
    }

    [Fact]
    public async Task GetDocente_WhenExists_ReturnsOkAndDocente()
    {
        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        //var response = await _client.GetAsync("/api/docentes/1");
        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        var user = await Auth.CreateUserClientAsync();
        //var response = await Client.GetAsync("/api/docentes/1");
        var response = await user.GetAsync("/api/docentes/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var docente = await response.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();
        docente!.Id.Should().Be(1);
        docente.NombreCompleto.Should().Be("Perez, Juan");
    }

    [Fact]
    public async Task GetDocente_WhenDoesNotExist_ReturnsNotFound()
    {
        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        //var response = await _client.GetAsync("/api/docentes/999");
        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        var user = await Auth.CreateUserClientAsync();
        //var response = await Client.GetAsync("/api/docentes/999");
        var response = await user.GetAsync("/api/docentes/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("No se encontró el docente con ID 999.");
    }

    #endregion

    #region Post

    [Fact]
    public async Task PostDocente_WhenValid_ReturnsCreated()
    {
        // Arrange
        var admin = await Auth.CreateAdminClientAsync();
        var dto = new CreateDocenteDto
        {
            Nombre = "Carlos",
            Apellido = "Lopez",
            Dni = "33.333.333",
            Legajo = 99999,
            Email = "carlos@test.com",
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Creado para test de integracion"
        };

        // Act
        //var response = await _client.PostAsJsonAsync("/api/docentes", dto);
        var response = await admin.PostAsJsonAsync("/api/docentes", dto);

        // Assert
        var body = await response.Content.ReadAsStringAsync();
        
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);

        //response.StatusCode.Should().Be(HttpStatusCode.Created);

        var docente = await response.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();
        docente!.NombreCompleto.Should().Be("Lopez, Carlos");
        docente.Legajo.Should().Be(99999);

        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task PostDocente_WhenLegajoExists_ReturnsBadRequest()
    {
        // Arrange
        var admin = await Auth.CreateAdminClientAsync();
        var dto = new CreateDocenteDto
        {
            Nombre = "Duplicado",
            Apellido = "Legajo",
            Dni = "44.444.444",
            Legajo = 12345
        };

        // Act
        var response = await admin.PostAsJsonAsync("/api/docentes", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("Ya existe un docente con ese legajo.");
      
        //error!.Message.Should().Be("Ya existe un docente con ese legajo.");
    }

    [Fact]
    // [Diana desde v4.0 OBSOLETO] DNI duplicado ahora devuelve 409 Conflict
    //public async Task PostDocente_WhenDniExists_ReturnsBadRequest()
    public async Task PostDocente_WhenDniExists_ReturnsConflict()
    {
        // Arrange
        var admin = await Auth.CreateAdminClientAsync();
        var dto = new CreateDocenteDto
        {
            Nombre = "Duplicado",
            Apellido = "Dni",
            Dni = "11.111.111",
            Legajo = 88888,
            MaxNivelAcademico = Titulo.Universitario // obligatorio: sin esto el validator corta con 400 antes de llegar al chequeo de DNI
        };

        // Act
        var response = await admin.PostAsJsonAsync("/api/docentes", dto);


        // Assert
        // [Diana desde v4.0 OBSOLETO]
        //response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict, body);

        // el stream del TestServer no se puede leer dos veces: se deserializa desde el string
        var jsonOptions = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);

        // ProblemDetails: el mensaje para el usuario va en "detail"
        var problem = System.Text.Json.JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(body, jsonOptions);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
        problem.Detail.Should().Be("Ya existe un docente con el DNI 11.111.111.");

        // se mantiene compatibilidad con ApiErrorResponse (extensión "message")
        var error = System.Text.Json.JsonSerializer.Deserialize<ApiErrorResponse>(body, jsonOptions);
        error.Should().NotBeNull();
        // [Diana desde v4.0 OBSOLETO]
        //error!.Message.Should().Be("Ya existe un docente con ese DNI.");
        error!.Message.Should().Be("Ya existe un docente con el DNI 11.111.111.");
    }

    [Fact]
    public async Task PostDocente_WhenFechaNacimientoIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var admin = await Auth.CreateAdminClientAsync();
        var dto = new CreateDocenteDto
        {
            Nombre = "Fecha",
            Apellido = "Invalida",
            Dni = "11.222.333",
            Legajo = 456789,
            FechaNacimiento = "fecha-invalida"
        };

        // Act
        var response = await admin.PostAsJsonAsync("/api/docentes", dto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }
    #endregion

    #region Put
    [Fact]
    public async Task PutDocente_WhenValid_ReturnsNoContent_AndUpdatesDocente()
    {
        var admin = await Auth.CreateAdminClientAsync();
        // Arrange
        var dto = new UpdateDocenteDto
        {
            Nombre = "Actualizado",
            Apellido = "Docente",
            Dni = "11.111.111",
            Legajo = 12345,
            Email = "actualizado@test.com",
            EmailAlternativo = "alternativo@test.com",
            Celular = "3415555555",
            Direccion = "Calle Actualizada 123",
            FechaNacimiento = "1990-10-10",
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Actualizado desde PUT"
        };
                
        // Act
        var putResponse = await admin.PutAsJsonAsync("/api/docentes/1", dto);
        var putBody = await putResponse.Content.ReadAsStringAsync();

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, putBody);

        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        //var getResponse = await Client.GetAsync("/api/docentes/1");
        var getResponse = await admin.GetAsync("/api/docentes/1");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
        updated.Should().NotBeNull();
        updated!.NombreCompleto.Should().Contain("Actualizado");
        updated.Email.Should().Be("actualizado@test.com");
        updated.Observaciones.Should().Be("Actualizado desde PUT");
    }

    [Fact]
    public async Task PutDocente_WhenDocenteDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var admin = await Auth.CreateAdminClientAsync();

        var dto = new UpdateDocenteDto
        {
            Nombre = "No",
            Apellido = "Existe",
            Dni = "99.999.999",
            Legajo = 999999,
            Email = "noexiste@test.com"
        };


        // Act
        var response = await admin.PutAsJsonAsync("/api/docentes/999", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task PutDocente_WhenDniBelongsToAnotherDocente_ReturnsConflict()
    {
        // Arrange: el docente 1 (11.111.111) intenta tomar el DNI del docente 2 (22.222.222)
        var admin = await Auth.CreateAdminClientAsync();
        var dto = new UpdateDocenteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "22.222.222",
            Legajo = 12345,
            MaxNivelAcademico = Titulo.Universitario
        };

        // Act
        var response = await admin.PutAsJsonAsync("/api/docentes/1", dto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict, body);

        var problem = System.Text.Json.JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(
            body, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
        problem.Should().NotBeNull();
        problem!.Detail.Should().Be("Ya existe otro docente con el DNI 22.222.222.");
    }

    [Fact]
    public async Task PutDocente_WhenKeepsOwnDni_ReturnsNoContent()
    {
        // Arrange: el docente 1 guarda sin cambiar su propio DNI
        var admin = await Auth.CreateAdminClientAsync();
        var dto = new UpdateDocenteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Dni = "11.111.111",
            Legajo = 12345,
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Mismo DNI"
        };

        // Act
        var response = await admin.PutAsJsonAsync("/api/docentes/1", dto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, body);
    }
    #endregion

    #region Delete
    [Fact]
    public async Task DeleteDocente_WhenExistsAndHasNoDesignaciones_ReturnsNoContent()
    {
        // Arrange
        var unique = Guid.NewGuid().ToString("N")[..8];

        var dto = new CreateDocenteDto
        {
            Nombre = "Borrar",
            Apellido = "Test",
            Dni = $"55{unique[..6]}", // aseguro un dni unico para que no falle por duplicado
            Legajo = Random.Shared.Next(80000, 99999),
            Email = $"borrar-{unique}@test.com",
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Creado para test de delete"
        };

        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var createResponse = await _client.PostAsJsonAsync("/api/docentes", dto);
        var createResponse = await admin.PostAsJsonAsync("/api/docentes", dto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var docente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();

        // Act
        //var deleteResponse = await _client.DeleteAsync($"/api/docentes/{docente!.Id}");
        var deleteResponse = await admin.DeleteAsync($"/api/docentes/{docente!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        //var getResponse = await _client.GetAsync($"/api/docentes/{docente.Id}");
        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        //var getResponse = await Client.GetAsync($"/api/docentes/{docente.Id}");
        var getResponse = await admin.GetAsync($"/api/docentes/{docente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDocente_WhenHasDesignaciones_ReturnsConflict()
    {
        // Arrange
        var random = Random.Shared.Next(50000000, 59999999);
        var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
        var legajo = Random.Shared.Next(70000, 79999);
        var email = $"delete-conflict-{Guid.NewGuid():N}@test.com";

        var createDto = new CreateDocenteDto
        {
            Nombre = "Docente",
            Apellido = "ConDesignacion",
            Dni = dni,
            Legajo = legajo,
            Email = email,
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Creado para test de delete con designaciones"
        };

        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
        var createResponse = await admin.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        createdDocente.Should().NotBeNull();

        await SeedDesignacionForDocenteAsync(createdDocente!.Id);

        // Act
        //var deleteResponse = await _client.DeleteAsync($"/api/docentes/{createdDocente.Id}");
        var deleteResponse = await admin.DeleteAsync($"/api/docentes/{createdDocente.Id}");
        var deleteBody = await deleteResponse.Content.ReadAsStringAsync();

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Conflict, deleteBody);

        var error = await deleteResponse.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("No se puede eliminar el docente porque tiene designaciones asociadas.");
    }
    #endregion

    #region Patch
    // diana: arreglar estos test cuando tenga bien definido que devuelve el get de docente, hay que mejorar ese dto para que traiga mas detalles
    [Fact]
    public async Task PatchContacto_WhenValid_ReturnsNoContent_AndUpdatesContacto()
    {
        // Arrange
        var random = Random.Shared.Next(60000000, 69999999);
        var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
        var legajo = Random.Shared.Next(80000, 89999);
        var email = $"contacto-{Guid.NewGuid():N}@test.com";

        var createDto = new CreateDocenteDto
        {
            Nombre = "Contacto",
            Apellido = "Test",
            Dni = dni,
            Legajo = legajo,
            Email = email,
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Docente para patch contacto"
        };

        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
        var createResponse = await admin.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        createdDocente.Should().NotBeNull();

        var patchDto = new UpdateContactoDocenteDto
        {
            Email = $"nuevo-{Guid.NewGuid():N}@test.com",
            EmailAlternativo = $"alt-{Guid.NewGuid():N}@test.com",
            Celular = "3415551234",
            Direccion = "Calle Patch 123"
        };

        // Act
        //var patchResponse = await _client.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/contacto", patchDto);
        var patchResponse = await admin.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/contacto", patchDto);
        var patchBody = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, patchBody);

        //var getResponse = await _client.GetAsync($"/api/docentes/{createdDocente.Id}");
        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        //var getResponse = await Client.GetAsync($"/api/docentes/{createdDocente.Id}");
        var getResponse = await admin.GetAsync($"/api/docentes/{createdDocente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedDocente = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
        updatedDocente.Should().NotBeNull();
        updatedDocente!.Email.Should().Be(patchDto.Email);
        updatedDocente.EmailAlternativo.Should().Be(patchDto.EmailAlternativo);
        updatedDocente.Celular.Should().Be(patchDto.Celular);
        updatedDocente.Direccion.Should().Be(patchDto.Direccion);
    }

    //[Fact]
    //public async Task PatchAcademico_WhenValid_ReturnsNoContent_AndUpdatesAcademico()
    //{
    //    // Arrange
    //    var random = Random.Shared.Next(70000000, 79999999);
    //    var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
    //    var legajo = Random.Shared.Next(90000, 94999);
    //    var email = $"academico-{Guid.NewGuid():N}@test.com";

    //    var createDto = new CreateDocenteDto
    //    {
    //        Nombre = "Academico",
    //        Apellido = "Test",
    //        Dni = dni,
    //        Legajo = legajo,
    //        Email = email,
    //        MaxNivelAcademico = Titulo.Secundario,
    //        Observaciones = "Docente para patch academico"
    //    };

    //    var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
    //    var createBody = await createResponse.Content.ReadAsStringAsync();

    //    createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

    //    var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
    //    createdDocente.Should().NotBeNull();

    //    var patchDto = new UpdateAcademicoDocenteDto
    //    {
    //        MaxNivelAcademico = Titulo.Universitario
    //    };


    //    // Act
    //    var patchResponse = await _client.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/academico", patchDto);
    //    var patchBody = await patchResponse.Content.ReadAsStringAsync();

    //    // Assert
    //    patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, patchBody);

    //    var getResponse = await _client.GetAsync($"/api/docentes/{createdDocente.Id}");
    //    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var updatedDocente = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
    //    updatedDocente.Should().NotBeNull();
    //    updatedDocente!.MaxNivelAcademico.Should().Be(Titulo.Universitario);
    //}

    [Fact]
    public async Task PatchObservaciones_WhenValid_ReturnsNoContent_AndUpdatesObservaciones()
    {
        // Arrange
        var random = Random.Shared.Next(80000000, 89999999);
        var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
        var legajo = Random.Shared.Next(95000, 99999);
        var email = $"observaciones-{Guid.NewGuid():N}@test.com";

        var createDto = new CreateDocenteDto
        {
            Nombre = "Observaciones",
            Apellido = "Test",
            Dni = dni,
            Legajo = legajo,
            Email = email,
            MaxNivelAcademico = Titulo.Universitario,
            Observaciones = "Observación inicial"
        };

        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
        var createResponse = await admin.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        createdDocente.Should().NotBeNull();

        var patchDto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Observación actualizada desde integration test"
        };

        // Act
        //var patchResponse = await _client.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/observaciones", patchDto);
        var patchResponse = await admin.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/observaciones", patchDto);
        var patchBody = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, patchBody);

        //var getResponse = await _client.GetAsync($"/api/docentes/{createdDocente.Id}");
        // [Diana desde v4.0 OBSOLETO] las lecturas ahora requieren un usuario autenticado (Admin o User)
        //var getResponse = await Client.GetAsync($"/api/docentes/{createdDocente.Id}");
        var getResponse = await admin.GetAsync($"/api/docentes/{createdDocente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedDocente = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
        updatedDocente.Should().NotBeNull();
        updatedDocente!.Observaciones.Should().Be("Observación actualizada desde integration test");
    }

    [Fact]
    public async Task PatchContacto_WhenDocenteDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var patchDto = new UpdateContactoDocenteDto
        {
            Email = "nuevo@test.com"
        };

        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var response = await _client.PatchAsJsonAsync("/api/docentes/999/contacto", patchDto);
        var response = await admin.PatchAsJsonAsync("/api/docentes/999/contacto", patchDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task PatchAcademico_WhenBodyIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var patchDto = new UpdateAcademicoDocenteDto
        {
            // [Diana desde v4.0 OBSOLETO] Universitario es un valor válido (devolvía 204 una vez autenticado);
            // el máximo nivel académico es opcional, así que el body inválido es un valor fuera del enum
            //MaxNivelAcademico = Titulo.Universitario
            MaxNivelAcademico = (Titulo)99
        };

        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var response = await _client.PatchAsJsonAsync("/api/docentes/1/academico", patchDto);
        var response = await admin.PatchAsJsonAsync("/api/docentes/1/academico", patchDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }
    [Fact]
    public async Task PatchObservaciones_WhenBodyIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var patchDto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "   "
        };

        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var response = await _client.PatchAsJsonAsync("/api/docentes/1/observaciones", patchDto);
        var response = await admin.PatchAsJsonAsync("/api/docentes/1/observaciones", patchDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }

    [Fact]
    public async Task PatchContacto_WhenBodyIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var patchDto = new UpdateContactoDocenteDto
        {
            Email = "   "
        };

        // Act
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        var admin = await Auth.CreateAdminClientAsync();
        //var response = await _client.PatchAsJsonAsync("/api/docentes/1/contacto", patchDto);
        var response = await admin.PatchAsJsonAsync("/api/docentes/1/contacto", patchDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }

    #endregion

    #region Fix docentes (FechaNacimiento, designaciones del docente, orden)

    [Fact]
    public async Task FechaNacimiento_SeGuardaEnPostYPut_YLaDevuelveElGet()
    {
        // Arrange: alta con fecha
        var admin = await Auth.CreateAdminClientAsync();
        var unique = Guid.NewGuid().ToString("N")[..6];
        var createDto = new CreateDocenteDto
        {
            Nombre = "Fecha",
            Apellido = "Nacimiento",
            Dni = $"66{unique}",
            FechaNacimiento = "1985-03-20",
            MaxNivelAcademico = Titulo.Universitario
        };

        var createResponse = await admin.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);
        var created = System.Text.Json.JsonSerializer.Deserialize<DocenteDto>(
            createBody, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

        var get1 = await admin.GetFromJsonAsync<DocenteDto>($"/api/docentes/{created!.Id}");
        get1!.FechaNacimiento.Should().Be(new DateOnly(1985, 3, 20));

        // Act: edición (PUT) cambiando la fecha, como hace FormDocentes
        var updateDto = new UpdateDocenteDto
        {
            Nombre = createDto.Nombre,
            Apellido = createDto.Apellido,
            Dni = createDto.Dni,
            FechaNacimiento = "1986-07-09",
            Email = "fecha@test.com",
            MaxNivelAcademico = Titulo.Universitario
        };
        var putResponse = await admin.PutAsJsonAsync($"/api/docentes/{created.Id}", updateDto);
        var putBody = await putResponse.Content.ReadAsStringAsync();

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, putBody);

        var get2 = await admin.GetFromJsonAsync<DocenteDto>($"/api/docentes/{created.Id}");
        get2!.FechaNacimiento.Should().Be(new DateOnly(1986, 7, 9));
        get2.Email.Should().Be("fecha@test.com");
    }

    [Fact]
    public async Task GetDesignacionesByDocente_DevuelveCabeceraConSusCargos()
    {
        // Arrange
        var admin = await Auth.CreateAdminClientAsync();
        var unique = Guid.NewGuid().ToString("N")[..6];
        var createResponse = await admin.PostAsJsonAsync("/api/docentes", new CreateDocenteDto
        {
            Nombre = "Con",
            Apellido = "Designaciones",
            Dni = $"77{unique}",
            MaxNivelAcademico = Titulo.Universitario
        });
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);
        var docente = System.Text.Json.JsonSerializer.Deserialize<DocenteDto>(
            createBody, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

        await SeedDesignacionForDocenteAsync(docente!.Id);

        // Act: lo que llama DetalleDocente
        var user = await Auth.CreateUserClientAsync();
        var response = await user.GetAsync($"/api/designaciones/docente/{docente.Id}");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        var designaciones = System.Text.Json.JsonSerializer.Deserialize<List<DocentesApp.Shared.DTOs.Designaciones.DesignacionDto>>(
            body, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

        designaciones.Should().ContainSingle();
        var des = designaciones![0];
        des.DocenteId.Should().Be(docente.Id);
        des.NombreCompletoDocente.Should().Be("Designaciones, Con");
        des.EsVigente.Should().BeTrue();
        des.TotalPuntos.Should().Be(119);

        des.Cargos.Should().ContainSingle();
        var cargo = des.Cargos[0];
        cargo.Cargo.Should().Be(CargoDocente.JefeTrabajosPracticos);
        cargo.CargoTexto.Should().NotBeNullOrWhiteSpace();
        cargo.Condicion.Should().Be(Condicion.Interino);
        cargo.DedicacionTexto.Should().NotBeNullOrWhiteSpace();
        cargo.NombreUdb.Should().StartWith("UDB Test");
        cargo.Puntos.Should().Be(119);
        cargo.EnLicencia.Should().BeFalse();
    }

    [Fact]
    public async Task GetDesignacionesByDocente_WhenDocenteDoesNotExist_ReturnsNotFound()
    {
        var user = await Auth.CreateUserClientAsync();

        var response = await user.GetAsync("/api/designaciones/docente/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDocentes_DevuelveOrdenadoPorApellidoYNombre()
    {
        // Arrange: se crean en desorden; el apellido con prefijo propio los separa del resto de los datos del test
        var admin = await Auth.CreateAdminClientAsync();
        var prefijo = $"Orden{Guid.NewGuid():N}"[..13];
        var altas = new[] { ("Beta", "Zeta"), ("Alfa", "Zeta"), ("Unico", "Alfa") };
        foreach (var (nombre, sufijo) in altas)
        {
            var r = await admin.PostAsJsonAsync("/api/docentes", new CreateDocenteDto
            {
                Nombre = nombre,
                Apellido = $"{prefijo}{sufijo}",
                Dni = $"88{Guid.NewGuid().ToString("N")[..6]}",
                MaxNivelAcademico = Titulo.Universitario
            });
            r.StatusCode.Should().Be(HttpStatusCode.Created, await r.Content.ReadAsStringAsync());
        }

        // Act
        var docentes = await admin.GetFromJsonAsync<List<ListDocenteDto>>("/api/docentes");

        // Assert: el último creado ("Alfa") queda primero y los "Zeta" por nombre
        docentes!
            .Where(d => d.Apellido.StartsWith(prefijo))
            .Select(d => d.NombreCompleto)
            .Should().Equal(
                $"{prefijo}Alfa, Unico",
                $"{prefijo}Zeta, Alfa",
                $"{prefijo}Zeta, Beta");
    }

    #endregion

    //Diana arreglar seed cuando esten definidos los cargos, cursos y demas en la bd
    private async Task SeedDesignacionForDocenteAsync(int docenteId)
    {
        // [Diana desde v4.0 OBSOLETO] _client/_factory propios nunca se asignaban (null): se usan Client/Factory de IntegrationTestBase y un cliente admin para los endpoints [Authorize(Roles = "Admin")]
        //using var scope = _factory.Services.CreateScope();
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();

        // [Diana desde v4.0 OBSOLETO] Cargo y Dedicacion eliminados como entidades independientes
        // var cargo = new Cargo { ... };
        // var dedicacion = new Dedicacion { ... };
        // db.Cargos.Add(cargo);
        // db.Dedicaciones.Add(dedicacion);
        // await db.SaveChangesAsync();

        // v5: el cargo necesita una UDB (FK obligatoria)
        var udb = new Udb { Nombre = $"UDB Test {Guid.NewGuid():N}" };
        db.Udbs.Add(udb);
        await db.SaveChangesAsync();

        var designacion = new Designacion
        {
            DocenteId = docenteId,
            // [Diana desde v4.0 OBSOLETO] CargoId y DedicacionId eliminados
            // CargoId = cargo.Id,
            // DedicacionId = dedicacion.Id,
            FechaInicio = DateTime.UtcNow.AddDays(-10),
            FechaFin = null,
            EstadoDesignacion = Estado.Activa,
            Observaciones = "Designación para test",
            // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos -> Actividades (v5)
        //    Detalles = new List<DetalleDesignacion>
        //{
        //    new DetalleDesignacion
        //    {
        //        Denominacion = DenominacionCargo.JefeDeTrabajosPracticos,
        //        TipoCargo = TipoCargo.Adjunto,
        //        Condicion = Condicion.Interino,
        //        TipoDedicacion = TipoDedicacion.Simple,
        //        CantidadDedicacion = 1,
        //        Especificacion = EspecificacionCargo.Docencia,
        //        PuntosAsignados = 119
        //    }
        //}
            Cargos = new List<CargoDesignacion>
            {
                new CargoDesignacion
                {
                    Cargo = CargoDocente.JefeTrabajosPracticos,
                    Condicion = Condicion.Interino,
                    TipoDedicacion = TipoDedicacion.Simple,
                    CantidadDedicacion = 1,
                    UdbId = udb.Id,
                    Puntos = 119,
                    Actividades = new List<Actividad>
                    {
                        new Actividad { Especificacion = EspecificacionCargo.Docencia }
                    }
                }
            }
        };

        db.Designaciones.Add(designacion);
        await db.SaveChangesAsync();
    }
}


