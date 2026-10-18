using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auditoria;
using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Shared.DTOs.Usuarios;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocentesApp.Tests.Integration
{
    // API de lectura de la auditoría (etapa 5). Filas de prueba:
    //   - cargadas a mano (FechaUtc controlada) con una Entidad propia por test, para que los filtros no se mezclen
    //   - o generadas por el interceptor al guardar con el DbContext del host (autor "Sistema")
    public class AuditoriaApiIntegrationTests : IntegrationTestBase
    {
        public AuditoriaApiIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region Helpers

        private void AgregarFilas(params HistorialCambio[] filas)
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            db.HistorialCambios.AddRange(filas);
            db.SaveChanges(); // el interceptor ignora las filas de historial
        }

        private static HistorialCambio Fila(string entidad, DateTime fechaUtc, AccionAuditoria accion = AccionAuditoria.Modificacion,
            string? usuarioId = "u-prueba", int? docenteId = null) => new()
        {
            Entidad = entidad,
            EntidadId = "1",
            FechaUtc = DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc),
            OperacionId = Guid.NewGuid(),
            Accion = accion,
            UsuarioId = usuarioId,
            UsuarioNombre = usuarioId == null ? null : "Prueba, Usuario",
            DocenteId = docenteId
        };

        private async Task<PaginaDto<HistorialCambioDto>> GetAsync(HttpClient cliente, string url)
        {
            var response = await cliente.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, body);
            return (await response.Content.ReadFromJsonAsync<PaginaDto<HistorialCambioDto>>())!;
        }

        #endregion

        #region Autorización

        public static TheoryData<string> Endpoints => new()
        {
            "/api/auditoria",
            "/api/auditoria/historial-docente/1",
            "/api/auditoria/historial-designacion/1",
            "/api/auditoria/entidades"
        };

        // Rutas renombradas en la etapa 6 (NSwag nombra los métodos por el último segmento: "docente" chocaba con api/designaciones/docente)
        [Theory]
        [InlineData("/api/auditoria/docente/1")]
        [InlineData("/api/auditoria/designacion/1")]
        public async Task RutasViejas_YaNoExisten(string url)
        {
            var admin = await Auth.CreateAdminClientAsync();

            (await admin.GetAsync(url)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [MemberData(nameof(Endpoints))]
        public async Task SinToken_Devuelve401(string url)
        {
            var response = await Client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [MemberData(nameof(Endpoints))]
        public async Task EditorYConsultante_Reciben403(string url)
        {
            var editor = await Auth.CreateEditorClientAsync();
            var consultante = await Auth.CreateConsultanteClientAsync();

            (await editor.GetAsync(url)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
            (await consultante.GetAsync(url)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region Filtros

        [Fact]
        public async Task FiltroPorUsuario_DevuelveSoloLasFilasDeEseUsuario()
        {
            var login = await Auth.LoginAsync("admin@docentesapp.com", "Admin123!");
            var adminId = new JwtSecurityTokenHandler().ReadJwtToken(login.Token).Claims.Single(c => c.Type == "userId").Value;
            var admin = Auth.CreateAuthenticatedClient(login.Token);
            var creado = await admin.PostAsJsonAsync("/api/docentes",
                new CreateDocenteDto { Nombre = "Filtro", Apellido = "Usuario", Dni = "77.777.777", Legajo = 77777 });
            creado.StatusCode.Should().Be(HttpStatusCode.Created, await creado.Content.ReadAsStringAsync());

            var pagina = await GetAsync(admin, $"/api/auditoria?usuarioId={adminId}&entidad=Docente&tamanioPagina=200");

            pagina.Items.Should().NotBeEmpty().And.OnlyContain(h => h.UsuarioId == adminId);
            pagina.Items.Should().Contain(h => h.EntidadDescripcion == "Usuario, Filtro" && h.AccionNombre == "Alta");
        }

        [Fact]
        public async Task FiltroSistema_DevuelveLasFilasSinUsuario_ComoSistema()
        {
            var admin = await Auth.CreateAdminClientAsync();

            var pagina = await GetAsync(admin, "/api/auditoria?usuarioId=sistema&entidad=Docente&tamanioPagina=200");

            // Los docentes del seed del factory se guardaron sin request
            pagina.Items.Should().Contain(h => h.EntidadDescripcion == "Perez, Juan");
            pagina.Items.Should().OnlyContain(h => h.UsuarioId == null && h.UsuarioNombre == "Sistema");
        }

        [Fact]
        public async Task FiltroDesdeHasta_UsaDiasDeArgentina_HastaInclusive()
        {
            // 02:30 UTC del 10/03 = 23:30 del 09/03 en Argentina; 03:30 UTC del 10/03 = 00:30 del 10/03
            var nocheDel9 = Fila("PruebaFechas", new DateTime(2026, 3, 10, 2, 30, 0));
            var madrugadaDel10 = Fila("PruebaFechas", new DateTime(2026, 3, 10, 3, 30, 0));
            AgregarFilas(nocheDel9, madrugadaDel10);
            var admin = await Auth.CreateAdminClientAsync();

            var del9 = await GetAsync(admin, "/api/auditoria?entidad=PruebaFechas&desde=2026-03-09&hasta=2026-03-09");
            var del10 = await GetAsync(admin, "/api/auditoria?entidad=PruebaFechas&desde=2026-03-10&hasta=2026-03-10");
            var soloDesde10 = await GetAsync(admin, "/api/auditoria?entidad=PruebaFechas&desde=2026-03-10");
            var soloHasta9 = await GetAsync(admin, "/api/auditoria?entidad=PruebaFechas&hasta=2026-03-09");

            del9.Items.Should().ContainSingle().Which.Id.Should().Be(nocheDel9.Id);
            del9.Items[0].FechaArgentina.Should().Be(new DateTime(2026, 3, 9, 23, 30, 0));
            del10.Items.Should().ContainSingle().Which.Id.Should().Be(madrugadaDel10.Id);
            soloDesde10.Items.Select(h => h.Id).Should().Equal(madrugadaDel10.Id);
            soloHasta9.Items.Select(h => h.Id).Should().Equal(nocheDel9.Id);
        }

        // FechaArgentina viaja sin offset ni "Z": Newtonsoft (cliente de Blazor) la lee tal cual, sin pasarla a la hora local
        [Fact]
        public async Task FechaArgentina_ViajaSinOffset_YNewtonsoftLaLeeSinConvertir()
        {
            var fila = Fila("PruebaJson", new DateTime(2026, 3, 10, 2, 30, 0));
            AgregarFilas(fila);
            var admin = await Auth.CreateAdminClientAsync();

            var json = await admin.GetStringAsync("/api/auditoria?entidad=PruebaJson");

            json.Should().Contain("\"fechaArgentina\":\"2026-03-09T23:30:00\"");
            json.Should().Contain("\"fechaUtc\":\"2026-03-10T02:30:00Z\"");

            // Mismos settings por defecto que el ServiceClient generado por NSwag (Newtonsoft)
            var pagina = Newtonsoft.Json.JsonConvert.DeserializeObject<PaginaDto<HistorialCambioDto>>(json, new Newtonsoft.Json.JsonSerializerSettings())!;
            var leida = pagina.Items.Single();
            leida.FechaArgentina.Should().Be(new DateTime(2026, 3, 9, 23, 30, 0));
            leida.FechaArgentina.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public async Task FiltroPorEntidadYAccion()
        {
            var ahora = DateTime.UtcNow;
            var alta = Fila("PruebaAccion", ahora.AddMinutes(-3), AccionAuditoria.Alta);
            var modificacion = Fila("PruebaAccion", ahora.AddMinutes(-2), AccionAuditoria.Modificacion);
            var baja = Fila("PruebaAccion", ahora.AddMinutes(-1), AccionAuditoria.Baja);
            AgregarFilas(alta, modificacion, baja, Fila("OtraEntidad", ahora, AccionAuditoria.Baja));
            var admin = await Auth.CreateAdminClientAsync();

            var todas = await GetAsync(admin, "/api/auditoria?entidad=PruebaAccion");
            var bajas = await GetAsync(admin, "/api/auditoria?entidad=PruebaAccion&accion=Baja");

            todas.Items.Select(h => h.AccionNombre).Should().Equal("Baja", "Modificación", "Alta"); // más nuevo primero
            todas.Items.Should().OnlyContain(h => h.Entidad == "PruebaAccion" && h.EntidadNombre == "PruebaAccion"); // sin etiqueta: nombre crudo
            bajas.Items.Should().ContainSingle().Which.Id.Should().Be(baja.Id);
        }

        [Fact]
        public async Task FiltroDesdePosteriorAHasta_Devuelve400()
        {
            var admin = await Auth.CreateAdminClientAsync();

            var response = await admin.GetAsync("/api/auditoria?desde=2026-03-10&hasta=2026-03-09");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Entidades_DevuelveClaveYEtiqueta()
        {
            var admin = await Auth.CreateAdminClientAsync();

            var entidades = await admin.GetFromJsonAsync<List<EntidadAuditoriaDto>>("/api/auditoria/entidades");

            entidades.Should().ContainEquivalentOf(new EntidadAuditoriaDto { Entidad = "CargoDesignacion", Nombre = "Cargo de designación" });
            entidades.Should().ContainEquivalentOf(new EntidadAuditoriaDto { Entidad = "Usuario", Nombre = "Usuario" });
            entidades.Should().ContainEquivalentOf(new EntidadAuditoriaDto { Entidad = "Designacion", Nombre = "Designación" });
        }

        #endregion

        #region Paginado

        [Fact]
        public async Task Paginado_MasNuevoPrimero_ConTotal()
        {
            var inicio = new DateTime(2026, 5, 1, 12, 0, 0);
            var filas = Enumerable.Range(0, 5).Select(i => Fila("PruebaPaginado", inicio.AddMinutes(i))).ToArray();
            AgregarFilas(filas);
            var admin = await Auth.CreateAdminClientAsync();

            var pagina2 = await GetAsync(admin, "/api/auditoria?entidad=PruebaPaginado&pagina=2&tamanioPagina=2");

            pagina2.Total.Should().Be(5);
            pagina2.Pagina.Should().Be(2);
            pagina2.TamanioPagina.Should().Be(2);
            pagina2.TotalPaginas.Should().Be(3);
            pagina2.Items.Select(h => h.Id).Should().Equal(filas[2].Id, filas[1].Id);
        }

        [Fact]
        public async Task Paginado_TamanioMayorA200_SeRecortaA200()
        {
            var admin = await Auth.CreateAdminClientAsync();

            var pagina = await GetAsync(admin, "/api/auditoria?tamanioPagina=500");
            var docente = await GetAsync(admin, "/api/auditoria/historial-docente/1?tamanioPagina=500");

            pagina.TamanioPagina.Should().Be(200);
            docente.TamanioPagina.Should().Be(200);
        }

        [Theory]
        [InlineData("/api/auditoria?pagina=0")]
        [InlineData("/api/auditoria?tamanioPagina=0")]
        [InlineData("/api/auditoria/historial-docente/1?pagina=0")]
        [InlineData("/api/auditoria/historial-designacion/1?tamanioPagina=0")]
        public async Task Paginado_PaginaOTamanioMenorA1_Devuelve400(string url)
        {
            var admin = await Auth.CreateAdminClientAsync();

            var response = await admin.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Historial de docente y de designación

        // Docente con una designación (cargo + actividad), una actividad editada y una membresía a una UDB que después se borra.
        // Se guarda con el DbContext del host: lo audita el interceptor, como "Sistema".
        // "n" distingue los datos de cada test (la base del factory se comparte entre los tests de la clase)
        private (int DocenteId, int DesignacionId, int UdbBorradaId) CrearHistorialDeDocente(int n)
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();

            var udb = new Udb { Nombre = n == 1 ? "UDB Auditoría API" : $"UDB Auditoría API {n}" };
            var temporal = new Udb { Nombre = $"UDB Temporal {n}" };
            var docente = new Docente { Nombre = "Docente", Apellido = "Historial", Dni = $"88.888.88{n}", Legajo = 88880 + n };
            db.AddRange(udb, temporal, docente);
            db.SaveChanges();

            var asignatura = new Asignatura { Nombre = "Química I", Nivel = Nivel.Primero, Frecuencia = Frecuencia.Anual, UdbId = udb.Id };
            db.Asignaturas.Add(asignatura);
            db.SaveChanges();

            var designacion = new Designacion
            {
                DocenteId = docente.Id,
                FechaInicio = new DateTime(2025, 3, 1),
                EstadoDesignacion = Estado.Activa,
                Cargos =
                {
                    new CargoDesignacion
                    {
                        UdbId = udb.Id, Cargo = CargoDocente.ProfesorAdjunto, Condicion = Condicion.Regular,
                        TipoDedicacion = TipoDedicacion.Simple, CantidadDedicacion = 1.5m, Puntos = 207,
                        Actividades = { new Actividad { Especificacion = EspecificacionCargo.JefeDeCatedra, AsignaturaId = asignatura.Id } }
                    }
                }
            };
            db.Designaciones.Add(designacion);
            db.DocenteUdbs.Add(new DocenteUdb { DocenteId = docente.Id, UdbId = temporal.Id, Rol = RolUdb.Docente });
            db.SaveChanges();

            designacion.Cargos.Single().Actividades.Single().Horario = "MI 18 a 20";
            db.SaveChanges();

            // Borrar la UDB temporal (con su membresía cargada): la FK de las filas viejas queda como "#id"
            db.Entry(temporal).Collection(u => u.DocenteUdbs).Load();
            db.Udbs.Remove(temporal);
            db.SaveChanges();

            return (docente.Id, designacion.Id, temporal.Id);
        }

        [Fact]
        public async Task HistorialDeDocente_IncluyeDesignacionCargoYActividad_ConFksYEnumsPorNombre()
        {
            var (docenteId, designacionId, udbBorradaId) = CrearHistorialDeDocente(1);
            var admin = await Auth.CreateAdminClientAsync();

            var pagina = await GetAsync(admin, $"/api/auditoria/historial-docente/{docenteId}?tamanioPagina=200");
            var items = pagina.Items;

            items.Select(h => h.Entidad).Distinct().Should().BeEquivalentTo(new[] { "Docente", "Designacion", "CargoDesignacion", "Actividad", "DocenteUdb" });
            items.Should().OnlyContain(h => h.DocenteId == docenteId && h.UsuarioNombre == "Sistema");
            items.Select(h => h.Id).Should().BeInDescendingOrder(); // más nuevo primero (mismas fechas: desempata el Id)

            var designacion = items.Single(h => h.Entidad == "Designacion");
            designacion.EntidadNombre.Should().Be("Designación");
            designacion.DesignacionId.Should().Be(designacionId);
            designacion.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Docente", Anterior = null, Nuevo = "Historial, Docente" });
            designacion.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Fecha de inicio", Anterior = null, Nuevo = "01/03/2025" });
            designacion.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Estado", Anterior = null, Nuevo = Estado.Activa.Descripcion() });

            var cargo = items.Single(h => h.Entidad == "CargoDesignacion");
            cargo.EntidadNombre.Should().Be("Cargo de designación");
            cargo.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "UDB", Nuevo = "UDB Auditoría API" });
            cargo.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Cargo", Nuevo = CargoDocente.ProfesorAdjunto.Descripcion() });
            cargo.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Cantidad de dedicación", Nuevo = "1,5" });
            cargo.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "En licencia", Nuevo = "No" });

            var altaActividad = items.Single(h => h.Entidad == "Actividad" && h.Accion == AccionAuditoria.Alta);
            altaActividad.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Especificación", Nuevo = "Jefe de Cátedra" });
            altaActividad.Detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Asignatura", Nuevo = "Química I" });
            var edicion = items.Single(h => h.Entidad == "Actividad" && h.Accion == AccionAuditoria.Modificacion);
            edicion.AccionNombre.Should().Be("Modificación");
            edicion.Detalles.Should().ContainSingle().Which.Should().BeEquivalentTo(new HistorialCambioDetalleDto { Propiedad = "Horario", Anterior = null, Nuevo = "MI 18 a 20" });

            // La UDB de la membresía se borró: su FK se muestra como "#id"
            var membresias = items.Where(h => h.Entidad == "DocenteUdb").ToList();
            membresias.Select(h => h.Accion).Should().BeEquivalentTo(new[] { AccionAuditoria.Alta, AccionAuditoria.Baja });
            membresias.Single(h => h.Accion == AccionAuditoria.Alta).Detalles
                .Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "UDB", Anterior = null, Nuevo = $"#{udbBorradaId}" });
            membresias.Single(h => h.Accion == AccionAuditoria.Baja).Detalles
                .Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Docente", Anterior = "Historial, Docente", Nuevo = null });
        }

        [Fact]
        public async Task HistorialDeDesignacion_IncluyeSusCargosYActividades_YNadaMasDelDocente()
        {
            var (_, designacionId, _) = CrearHistorialDeDocente(2);
            var admin = await Auth.CreateAdminClientAsync();

            var pagina = await GetAsync(admin, $"/api/auditoria/historial-designacion/{designacionId}");

            pagina.Items.Select(h => h.Entidad).Distinct().Should().BeEquivalentTo(new[] { "Designacion", "CargoDesignacion", "Actividad" });
            pagina.Items.Should().OnlyContain(h => h.DesignacionId == designacionId);
            pagina.Total.Should().Be(4); // designación, cargo, alta y edición de la actividad
        }

        #endregion

        #region Valores especiales

        [Fact]
        public async Task FechaVacia0001_NuncaSeMuestraComoFecha_YLoDesconocidoSeMuestraCrudo()
        {
            var fila = Fila("Docente", new DateTime(2026, 3, 10, 2, 30, 0), docenteId: 900001);
            fila.Detalles.Add(new HistorialCambioDetalle
            {
                Propiedad = "CreatedDate",
                ValorAnterior = "0001-01-01T00:00:00.0000000+00:00",
                ValorNuevo = "2026-03-10T02:30:00.0000000+00:00"
            });
            fila.Detalles.Add(new HistorialCambioDetalle { Propiedad = "PropiedadRara", ValorAnterior = "x", ValorNuevo = "y" });
            AgregarFilas(fila);
            var admin = await Auth.CreateAdminClientAsync();

            var detalles = (await GetAsync(admin, "/api/auditoria/historial-docente/900001")).Items.Single().Detalles;

            detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "Fecha de alta", Anterior = null, Nuevo = "09/03/2026 23:30" });
            detalles.Should().ContainEquivalentOf(new HistorialCambioDetalleDto { Propiedad = "PropiedadRara", Anterior = "x", Nuevo = "y" });
        }

        [Fact]
        public async Task Usuarios_LaRespuestaNuncaTraeDatosDeContrasenia()
        {
            var admin = await Auth.CreateAdminClientAsync();
            var usuario = await Auth.RegisterUserAsync(password: "Secreta123", rol: Roles.Editor);
            var reset = await admin.PostAsJsonAsync($"/api/usuarios/{usuario.Id}/reset-password", new ResetPasswordDto { PasswordNueva = "Reseteada456" });
            reset.IsSuccessStatusCode.Should().BeTrue();

            var response = await admin.GetAsync("/api/auditoria?entidad=Usuario&tamanioPagina=200");
            var json = await response.Content.ReadAsStringAsync();
            var pagina = (await response.Content.ReadFromJsonAsync<PaginaDto<HistorialCambioDto>>())!;

            foreach (var prohibido in new[] { "Secreta123", "Reseteada456", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "AQAAAA" })
                json.Should().NotContain(prohibido);

            var delUsuario = pagina.Items.Where(h => h.EntidadId == usuario.Id).ToList();
            delUsuario.Should().Contain(h => h.Detalles.Any(d => d.Propiedad == "Contraseña" && d.Anterior == null && d.Nuevo == null));
            delUsuario.Should().Contain(h => h.Detalles.Any(d => d.Propiedad == "Rol" && d.Anterior == null && d.Nuevo == Roles.Editor));
            delUsuario.Single(h => h.Accion == AccionAuditoria.Alta).Detalles
                .Should().Contain(d => d.Propiedad == "Nombre de usuario" && d.Nuevo == usuario.UserName);
        }

        #endregion
    }
}
