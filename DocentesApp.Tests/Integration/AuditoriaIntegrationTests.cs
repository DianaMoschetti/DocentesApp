using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DocentesApp.API.Services;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Docentes;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocentesApp.Tests.Integration
{
    // Interceptor de auditoría registrado en la API (Program.cs + CustomWebApplicationFactory)
    public class AuditoriaIntegrationTests : IntegrationTestBase
    {
        public AuditoriaIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_EmiteElClaimNombre()
        {
            var login = await Auth.LoginAsync("admin@docentesapp.com", "Admin123!");

            var token = new JwtSecurityTokenHandler().ReadJwtToken(login.Token);

            token.Claims.Should().Contain(c => c.Type == CurrentUserService.ClaimNombre && c.Value != "");
        }

        [Fact]
        public async Task CrearDocentePorApi_RegistraHistorialConElUsuarioDelToken()
        {
            var login = await Auth.LoginAsync("admin@docentesapp.com", "Admin123!");
            var claims = new JwtSecurityTokenHandler().ReadJwtToken(login.Token).Claims.ToList();
            var userId = claims.Single(c => c.Type == "userId").Value;
            var nombre = claims.Single(c => c.Type == CurrentUserService.ClaimNombre).Value;
            var admin = Auth.CreateAuthenticatedClient(login.Token);

            var response = await admin.PostAsJsonAsync("/api/docentes", new CreateDocenteDto
            {
                Nombre = "Auditado",
                Apellido = "Por Api",
                Dni = "55.555.555",
                Legajo = 55555
            });
            var body = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.Created, body);
            var id = JsonDocument.Parse(body).RootElement.GetProperty("id").GetInt32();

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();

            var docente = await db.Docentes.AsNoTracking().SingleAsync(d => d.Id == id);
            docente.CreatedBy.Should().Be(userId);

            var fila = await db.HistorialCambios.AsNoTracking()
                .SingleAsync(h => h.Entidad == "Docente" && h.EntidadId == id.ToString());
            fila.Accion.Should().Be(AccionAuditoria.Alta);
            fila.UsuarioId.Should().Be(userId);
            fila.UsuarioNombre.Should().Be(nombre);
            fila.UsuarioEmail.Should().Be("admin@docentesapp.com");
            fila.DocenteId.Should().Be(id);
        }

        [Fact]
        public async Task EditorEditaUnDocente_HistorialConElIdYNombreDelEditor_YSoloLaColumnaQueCambio()
        {
            var login = await Auth.LoginAsync("editor@docentesapp.com", "Editor123!");
            var claims = new JwtSecurityTokenHandler().ReadJwtToken(login.Token).Claims.ToList();
            var editorId = claims.Single(c => c.Type == "userId").Value;
            var nombre = claims.Single(c => c.Type == CurrentUserService.ClaimNombre).Value;
            var editor = Auth.CreateAuthenticatedClient(login.Token);

            int docenteId;
            using (var scope = Factory.Services.CreateScope())
                docenteId = (await scope.ServiceProvider.GetRequiredService<DocentesDbContext>()
                    .Docentes.AsNoTracking().SingleAsync(d => d.Dni == "22.222.222")).Id; // Ana Gomez, del seed del factory

            var response = await editor.PatchAsJsonAsync($"/api/docentes/{docenteId}/contacto",
                new UpdateContactoDocenteDto { Celular = "11-5555-0000" });
            var body = await response.Content.ReadAsStringAsync();
            response.IsSuccessStatusCode.Should().BeTrue(body);

            using var lectura = Factory.Services.CreateScope();
            var db = lectura.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var fila = await db.HistorialCambios.AsNoTracking().Include(h => h.Detalles)
                .SingleAsync(h => h.Entidad == "Docente" && h.EntidadId == docenteId.ToString() && h.Accion == AccionAuditoria.Modificacion);
            fila.UsuarioId.Should().Be(editorId);
            fila.UsuarioNombre.Should().Be(nombre);
            fila.UsuarioEmail.Should().Be("editor@docentesapp.com");
            fila.DocenteId.Should().Be(docenteId);
            fila.Detalles.Should().ContainSingle(d => d.Propiedad == "Celular" && d.ValorNuevo == "11-5555-0000");

            (await db.Docentes.AsNoTracking().SingleAsync(d => d.Id == docenteId)).ModifiedBy.Should().Be(editorId);
        }

        [Fact]
        public async Task SeedAlArrancar_SeRegistraComoSistema()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();

            // Docentes que carga CustomWebApplicationFactory.SeedData, sin request (usuario null)
            var filas = await db.HistorialCambios.AsNoTracking()
                .Where(h => h.Entidad == "Docente" && h.EntidadDescripcion == "Perez, Juan")
                .ToListAsync();

            filas.Should().NotBeEmpty();
            filas.Should().OnlyContain(h => h.UsuarioId == null && h.UsuarioNombre == null && h.UsuarioEmail == null);
            (await db.Docentes.AsNoTracking().SingleAsync(d => d.Dni == "11.111.111")).CreatedBy.Should().Be("Sistema");
        }
    }
}
