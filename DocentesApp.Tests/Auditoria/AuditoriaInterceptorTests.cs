using DocentesApp.API.Extensions;
using DocentesApp.Data.Auditoria;
using DocentesApp.Data.Context;
using DocentesApp.Data.Identity;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Tests.Helpers;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DocentesApp.Tests.Auditoria
{
    // AuditoriaInterceptor contra SQLite en memoria con un usuario actual falso.
    // Los datos previos se cargan con un contexto sin auditoría; cada operación auditada usa un contexto nuevo (como un request).
    public class AuditoriaInterceptorTests : IDisposable
    {
        private static readonly string[] CamposTecnicosUsuario =
        {
            "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "AccessFailedCount",
            "LockoutEnd", "LockoutEnabled", "NormalizedEmail", "NormalizedUserName"
        };

        private static readonly string[] ColumnasAuditoria = { "CreatedDate", "CreatedBy", "ModifiedDate", "ModifiedBy" };

        private readonly UsuarioActualFalso _usuario = UsuarioActualFalso.Editor();
        private readonly SqliteTestDatabase _db;

        public AuditoriaInterceptorTests()
        {
            _db = new SqliteTestDatabase(_usuario);
        }

        public void Dispose() => _db.Dispose();

        #region Helpers

        // Contexto sin auditoría, para preparar datos y leer resultados
        private DocentesDbContext ContextoSinAuditoria() => new(_db.Options);

        private List<HistorialCambio> Historial()
        {
            using var context = ContextoSinAuditoria();
            return context.HistorialCambios.Include(h => h.Detalles).AsNoTracking().OrderBy(h => h.Id).ToList();
        }

        private static readonly DateTimeOffset FechaPrevia = new(2025, 3, 1, 12, 0, 0, TimeSpan.Zero);

        private int CrearDocenteSinAuditoria(string dni = "1", string apellido = "Perez")
        {
            using var context = ContextoSinAuditoria();
            var docente = new Docente
            {
                Nombre = "Juan", Apellido = apellido, Dni = dni, Legajo = int.Parse(dni),
                CreatedBy = "creador", CreatedDate = FechaPrevia, ModifiedBy = "creador", ModifiedDate = FechaPrevia
            };
            context.Docentes.Add(docente);
            context.SaveChanges();
            return docente.Id;
        }

        private int CrearUdbSinAuditoria(string nombre = "UDB Física")
        {
            using var context = ContextoSinAuditoria();
            var udb = new Udb { Nombre = nombre };
            context.Udbs.Add(udb);
            context.SaveChanges();
            return udb.Id;
        }

        private static Designacion NuevaDesignacion(int docenteId, int udbId) => new()
        {
            DocenteId = docenteId,
            FechaInicio = new DateTime(2025, 3, 1),
            EstadoDesignacion = Estado.Activa,
            Cargos =
            {
                new CargoDesignacion
                {
                    UdbId = udbId,
                    Cargo = CargoDocente.ProfesorAdjunto,
                    Condicion = Condicion.Regular,
                    TipoDedicacion = TipoDedicacion.Simple,
                    CantidadDedicacion = 1,
                    Puntos = 138,
                    Actividades = { new Actividad { Especificacion = EspecificacionCargo.Docencia, Horario = "LU 8 a 10" } }
                }
            }
        };

        // Designación con un cargo y una actividad, cargada sin auditoría
        private (int DocenteId, int DesignacionId, int CargoId, int ActividadId) CrearDesignacionSinAuditoria()
        {
            var docenteId = CrearDocenteSinAuditoria();
            var udbId = CrearUdbSinAuditoria();
            using var context = ContextoSinAuditoria();
            var designacion = NuevaDesignacion(docenteId, udbId);
            context.Designaciones.Add(designacion);
            context.SaveChanges();
            var cargo = designacion.Cargos.Single();
            return (docenteId, designacion.Id, cargo.Id, cargo.Actividades.Single().Id);
        }

        // Interceptor que rompe el guardado del historial (Entidad null viola el NOT NULL de la base)
        private sealed class InterceptorQueFallaAlGuardarHistorial : AuditoriaInterceptor
        {
            public InterceptorQueFallaAlGuardarHistorial(UsuarioActualFalso usuario) : base(usuario, new ContextoOperacionAuditoria()) { }

            protected override Task GuardarHistorialAsync(DbContext context, IReadOnlyList<HistorialCambio> filas, bool async, CancellationToken cancellationToken)
            {
                filas[0].Entidad = null!;
                return base.GuardarHistorialAsync(context, filas, async, cancellationToken);
            }
        }

        // Estrategia de ejecución que reintenta (como EnableRetryOnFailure)
        private sealed class EstrategiaConReintentos : ExecutionStrategy
        {
            public EstrategiaConReintentos(ExecutionStrategyDependencies dependencies) : base(dependencies, 3, TimeSpan.FromSeconds(1)) { }
            protected override bool ShouldRetryOn(Exception exception) => false;
        }

        #endregion

        #region Alta / Modificación / Baja

        [Fact]
        public async Task Alta_CompletaColumnasDeAuditoria_YRegistraFilaConDetalles()
        {
            int id;
            await using (var context = _db.CrearContexto())
            {
                var docente = new Docente { Nombre = "Juan", Apellido = "Perez", Dni = "1", Legajo = 1 };
                context.Docentes.Add(docente);
                await context.SaveChangesAsync();
                id = docente.Id;
            }

            using (var context = ContextoSinAuditoria())
            {
                var docente = context.Docentes.Single(d => d.Id == id);
                docente.CreatedBy.Should().Be("user-editor");
                docente.ModifiedBy.Should().Be("user-editor");
                docente.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
                docente.ModifiedDate.Should().Be(docente.CreatedDate);
            }

            var fila = Historial().Should().ContainSingle().Subject; // el historial no se audita a sí mismo
            fila.Accion.Should().Be(AccionAuditoria.Alta);
            fila.Entidad.Should().Be("Docente");
            fila.EntidadId.Should().Be(id.ToString());
            fila.EntidadDescripcion.Should().Be("Perez, Juan");
            fila.DocenteId.Should().Be(id);
            fila.DesignacionId.Should().BeNull();
            fila.UsuarioId.Should().Be("user-editor");
            fila.UsuarioNombre.Should().Be("Editora, Eva");
            fila.UsuarioEmail.Should().Be("editor@test.com");
            fila.OperacionId.Should().NotBe(Guid.Empty);
            fila.FechaUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            fila.Detalles.Select(d => d.Propiedad).Should().Contain(new[] { "Nombre", "Apellido", "Dni", "Legajo" })
                .And.NotContain(ColumnasAuditoria).And.NotContain("Id");
            var nombre = fila.Detalles.Single(d => d.Propiedad == "Nombre");
            nombre.ValorAnterior.Should().BeNull();
            nombre.ValorNuevo.Should().Be("Juan");
        }

        [Fact]
        public async Task Modificacion_RegistraSoloLoQueCambio_YActualizaModifiedSinTocarCreated()
        {
            var id = CrearDocenteSinAuditoria();

            await using (var context = _db.CrearContexto())
            {
                var docente = await context.Docentes.SingleAsync(d => d.Id == id);
                docente.Apellido = "Pérez";
                await context.SaveChangesAsync();
            }

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Accion.Should().Be(AccionAuditoria.Modificacion);
            fila.DocenteId.Should().Be(id);
            var detalle = fila.Detalles.Should().ContainSingle().Subject;
            detalle.Propiedad.Should().Be("Apellido");
            detalle.ValorAnterior.Should().Be("Perez");
            detalle.ValorNuevo.Should().Be("Pérez");

            using var lectura = ContextoSinAuditoria();
            var guardado = lectura.Docentes.Single(d => d.Id == id);
            guardado.CreatedBy.Should().Be("creador");
            guardado.CreatedDate.Should().Be(FechaPrevia);
            guardado.ModifiedBy.Should().Be("user-editor");
            guardado.ModifiedDate.Should().BeAfter(FechaPrevia);
        }

        [Fact]
        public async Task Modificacion_ConUpdateSinCambiosReales_NoRegistraFila_NiTocaModified()
        {
            var id = CrearDocenteSinAuditoria();

            await using (var context = _db.CrearContexto())
            {
                var docente = await context.Docentes.SingleAsync(d => d.Id == id);
                context.Docentes.Update(docente); // marca todas las columnas como modificadas, sin cambiar valores
                await context.SaveChangesAsync();
            }

            Historial().Should().BeEmpty();
            using var lectura = ContextoSinAuditoria();
            var guardado = lectura.Docentes.Single(d => d.Id == id);
            guardado.ModifiedBy.Should().Be("creador");
            guardado.ModifiedDate.Should().Be(FechaPrevia);
        }

        [Fact]
        public async Task Baja_RegistraLosValoresAnteriores()
        {
            var id = CrearDocenteSinAuditoria();

            await using (var context = _db.CrearContexto())
            {
                context.Docentes.Remove(await context.Docentes.SingleAsync(d => d.Id == id));
                await context.SaveChangesAsync();
            }

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Accion.Should().Be(AccionAuditoria.Baja);
            fila.EntidadId.Should().Be(id.ToString());
            fila.EntidadDescripcion.Should().Be("Perez, Juan");
            fila.DocenteId.Should().Be(id);
            var nombre = fila.Detalles.Single(d => d.Propiedad == "Nombre");
            nombre.ValorAnterior.Should().Be("Juan");
            nombre.ValorNuevo.Should().BeNull();
            fila.Detalles.Select(d => d.Propiedad).Should().NotContain(ColumnasAuditoria);
        }

        [Fact]
        public void CaminoSincronico_SaveChanges_RegistraAltaModificacionYBaja()
        {
            var aBorrar = CrearDocenteSinAuditoria(dni: "1");
            var aModificar = CrearDocenteSinAuditoria(dni: "2", apellido: "Gomez");

            using (var context = _db.CrearContexto())
            {
                context.Udbs.Add(new Udb { Nombre = "UDB Química" });
                context.Docentes.Single(d => d.Id == aModificar).Apellido = "Gómez";
                context.Docentes.Remove(context.Docentes.Single(d => d.Id == aBorrar));
                context.SaveChanges();
            }

            var filas = Historial();
            filas.Select(f => (f.Entidad, f.Accion)).Should().BeEquivalentTo(new[]
            {
                ("Udb", AccionAuditoria.Alta),
                ("Docente", AccionAuditoria.Modificacion),
                ("Docente", AccionAuditoria.Baja)
            });
            filas.Select(f => f.OperacionId).Distinct().Should().ContainSingle();

            using var lectura = ContextoSinAuditoria();
            lectura.Udbs.Single().CreatedBy.Should().Be("user-editor");
        }

        [Fact]
        public async Task DocenteUdb_ClaveCompuesta_SeGuardaComoTexto_YSeVinculaAlDocente()
        {
            var docenteId = CrearDocenteSinAuditoria();
            var udbId = CrearUdbSinAuditoria();

            await using (var context = _db.CrearContexto())
            {
                context.DocenteUdbs.Add(new DocenteUdb { DocenteId = docenteId, UdbId = udbId, Rol = RolUdb.Docente });
                await context.SaveChangesAsync();
            }

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Entidad.Should().Be("DocenteUdb");
            fila.EntidadId.Should().Be($"DocenteId={docenteId};UdbId={udbId}");
            fila.DocenteId.Should().Be(docenteId);
            fila.Detalles.Select(d => d.Propiedad).Should().Contain(new[] { "DocenteId", "UdbId", "Rol" });
            fila.Detalles.Single(d => d.Propiedad == "Rol").ValorNuevo.Should().Be(((int)RolUdb.Docente).ToString());
        }

        #endregion

        #region Usuario actual

        [Fact]
        public async Task SinUsuario_RegistraSistema()
        {
            using var db = new SqliteTestDatabase(UsuarioActualFalso.Sistema());

            await using (var context = db.CrearContexto())
            {
                context.Udbs.Add(new Udb { Nombre = "UDB Física" });
                await context.SaveChangesAsync();
            }

            using var lectura = new DocentesDbContext(db.Options);
            var udb = lectura.Udbs.Single();
            udb.CreatedBy.Should().Be(AuditoriaInterceptor.UsuarioSistema);
            udb.ModifiedBy.Should().Be(AuditoriaInterceptor.UsuarioSistema);

            var fila = lectura.HistorialCambios.Should().ContainSingle().Subject;
            fila.UsuarioId.Should().BeNull();
            fila.UsuarioNombre.Should().BeNull();
            fila.UsuarioEmail.Should().BeNull();
        }

        #endregion

        #region ApplicationUser

        [Fact]
        public async Task Usuario_Alta_NoGuardaCamposTecnicos()
        {
            var user = new ApplicationUser
            {
                UserName = "ana", NormalizedUserName = "ANA", Email = "ana@test.com", NormalizedEmail = "ANA@TEST.COM",
                PasswordHash = "hash-secreto", SecurityStamp = "stamp-secreto", ConcurrencyStamp = "concurrency-secreto",
                LockoutEnabled = true, Nombre = "Ana", Apellido = "Gomez"
            };

            await using (var context = _db.CrearContexto())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Entidad.Should().Be("Usuario");
            fila.EntidadId.Should().Be(user.Id);
            fila.EntidadDescripcion.Should().Be("Gomez, Ana (ana)");
            fila.Detalles.Select(d => d.Propiedad).Should().Contain(new[] { "UserName", "Email", "Nombre", "Apellido", "Activo" })
                .And.NotContain(CamposTecnicosUsuario).And.NotContain(ColumnasAuditoria);
            fila.Detalles.Select(d => d.ValorNuevo).Should().NotContain(new[] { "hash-secreto", "stamp-secreto", "concurrency-secreto" });

            using var lectura = ContextoSinAuditoria();
            lectura.Users.Single(u => u.Id == user.Id).CreatedBy.Should().Be("user-editor");
        }

        [Fact]
        public async Task Usuario_Modificacion_IgnoraCamposTecnicos()
        {
            var user = new ApplicationUser { UserName = "ana", Email = "ana@test.com", PasswordHash = "hash-1", SecurityStamp = "stamp-1", Nombre = "Ana" };
            using (var context = ContextoSinAuditoria())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            // Solo campos técnicos (login, cambio de contraseña): no es una modificación
            await using (var context = _db.CrearContexto())
            {
                var u = await context.Users.SingleAsync(x => x.Id == user.Id);
                u.PasswordHash = "hash-2";
                u.SecurityStamp = "stamp-2";
                u.ConcurrencyStamp = Guid.NewGuid().ToString();
                u.AccessFailedCount = 1;
                u.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(5);
                await context.SaveChangesAsync();
            }
            Historial().Should().BeEmpty();

            // Nombre + campos técnicos: solo se registra el nombre
            await using (var context = _db.CrearContexto())
            {
                var u = await context.Users.SingleAsync(x => x.Id == user.Id);
                u.Nombre = "Anita";
                u.PasswordHash = "hash-3";
                u.SecurityStamp = "stamp-3";
                context.Users.Update(u); // como UserStore.UpdateAsync
                await context.SaveChangesAsync();
            }

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Accion.Should().Be(AccionAuditoria.Modificacion);
            var detalle = fila.Detalles.Should().ContainSingle().Subject;
            detalle.Propiedad.Should().Be("Nombre");
            detalle.ValorAnterior.Should().Be("Ana");
            detalle.ValorNuevo.Should().Be("Anita");

            using var lectura = ContextoSinAuditoria();
            lectura.HistorialCambioDetalles.Select(d => d.Propiedad).ToList().Should().NotContain(CamposTecnicosUsuario);
        }

        #endregion

        #region Transacción y fallas

        [Fact]
        public async Task FallaElGuardadoDeNegocio_NoQuedaHistorial_YLaTransaccionSeLibera()
        {
            CrearDocenteSinAuditoria(dni: "1");

            await using var context = _db.CrearContexto();
            context.Docentes.Add(new Docente { Nombre = "Otro", Apellido = "Docente", Dni = "1", Legajo = 2 }); // DNI repetido

            var act = () => context.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateException>();
            context.Database.CurrentTransaction.Should().BeNull();
            Historial().Should().BeEmpty();

            // El mismo contexto puede seguir guardando
            context.ChangeTracker.Clear();
            context.Udbs.Add(new Udb { Nombre = "UDB Química" });
            await context.SaveChangesAsync();
            Historial().Should().ContainSingle().Which.Entidad.Should().Be("Udb");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FallaElGuardadoDelHistorial_DeshaceTambienElCambioDeNegocio(bool async)
        {
            await using var context = _db.CrearContextoAuditado(new InterceptorQueFallaAlGuardarHistorial(_usuario));
            context.Docentes.Add(new Docente { Nombre = "Juan", Apellido = "Perez", Dni = "9", Legajo = 9 });

            var act = async ? (Func<Task>)(() => context.SaveChangesAsync()) : () => Task.FromResult(context.SaveChanges());

            await act.Should().ThrowAsync<DbUpdateException>();
            context.Database.CurrentTransaction.Should().BeNull();

            using var lectura = ContextoSinAuditoria();
            lectura.Docentes.Any(d => d.Dni == "9").Should().BeFalse();
            lectura.HistorialCambios.Should().BeEmpty();
        }

        [Fact]
        public async Task ConTransaccionDelLlamador_TodosLosGuardadosYEventosCompartenOperacionId()
        {
            var operacion = new ContextoOperacionAuditoria();
            await using var context = _db.CrearContextoAuditado(new AuditoriaInterceptor(_usuario, operacion));
            var eventos = new AuditoriaEventos(context, _usuario, operacion);

            await using (var transaccion = await context.Database.BeginTransactionAsync())
            {
                context.Udbs.Add(new Udb { Nombre = "UDB Física" });
                await context.SaveChangesAsync();
                context.Docentes.Add(new Docente { Nombre = "Juan", Apellido = "Perez", Dni = "1", Legajo = 1 });
                await context.SaveChangesAsync();
                await eventos.RegistrarAsync("Usuario", "u-1", "Gomez, Ana (ana)", AccionAuditoria.Modificacion,
                    new[] { new DetalleEventoAuditoria("Rol", "Editor", "Administrador") });

                // la transacción es del llamador: el interceptor no la confirma ni la cierra
                context.Database.CurrentTransaction.Should().BeSameAs(transaccion);
                await transaccion.CommitAsync();
            }

            var filas = Historial();
            filas.Should().HaveCount(3);
            filas.Select(f => f.OperacionId).Distinct().Should().ContainSingle();
            var evento = filas.Single(f => f.Entidad == "Usuario");
            evento.UsuarioId.Should().Be("user-editor");
            evento.Detalles.Should().ContainSingle(d => d.Propiedad == "Rol" && d.ValorAnterior == "Editor" && d.ValorNuevo == "Administrador");
        }

        [Fact]
        public async Task ConTransaccionDelLlamador_SiSeDeshace_NoQuedaNiNegocioNiHistorial()
        {
            await using (var context = _db.CrearContexto())
            await using (var transaccion = await context.Database.BeginTransactionAsync())
            {
                context.Udbs.Add(new Udb { Nombre = "UDB Física" });
                await context.SaveChangesAsync();
                await transaccion.RollbackAsync();
            }

            using var lectura = ContextoSinAuditoria();
            lectura.Udbs.Should().BeEmpty();
            lectura.HistorialCambios.Should().BeEmpty();
        }

        [Fact]
        public async Task SinTransaccion_CadaSaveChangesEsUnaOperacionDistinta()
        {
            await using (var context = _db.CrearContexto())
            {
                context.Udbs.Add(new Udb { Nombre = "UDB Física" });
                await context.SaveChangesAsync();
                context.Udbs.Add(new Udb { Nombre = "UDB Química" });
                await context.SaveChangesAsync();
                context.Database.CurrentTransaction.Should().BeNull(); // la transacción propia se cerró
            }

            Historial().Select(f => f.OperacionId).Distinct().Should().HaveCount(2);
        }

        #endregion

        #region Vínculo con docente y designación

        [Fact]
        public async Task AltaDeDesignacionConCargoYActividad_TodasLasFilasLlevanDocenteYDesignacion()
        {
            var docenteId = CrearDocenteSinAuditoria();
            var udbId = CrearUdbSinAuditoria();

            int designacionId;
            await using (var context = _db.CrearContexto())
            {
                var designacion = NuevaDesignacion(docenteId, udbId);
                context.Designaciones.Add(designacion);
                await context.SaveChangesAsync();
                designacionId = designacion.Id;
            }

            var filas = Historial();
            filas.Select(f => f.Entidad).Should().BeEquivalentTo(new[] { "Designacion", "CargoDesignacion", "Actividad" });
            filas.Should().OnlyContain(f => f.Accion == AccionAuditoria.Alta && f.DocenteId == docenteId && f.DesignacionId == designacionId);
            filas.Select(f => f.OperacionId).Distinct().Should().ContainSingle();
            filas.Single(f => f.Entidad == "CargoDesignacion").EntidadDescripcion.Should().Be("PAd 1 DS");
        }

        [Fact]
        public async Task BajaDeCargoConSusActividades_LasFilasLlevanDocenteYDesignacion()
        {
            var (docenteId, designacionId, cargoId, actividadId) = CrearDesignacionSinAuditoria();

            await using (var context = _db.CrearContexto())
            {
                // Como DesignacionService.UpdateAsync: el cargo quitado y sus actividades quedan trackeados como Deleted
                var designacion = await context.Designaciones
                    .Include(d => d.Cargos).ThenInclude(c => c.Actividades)
                    .SingleAsync(d => d.Id == designacionId);
                designacion.Cargos.Remove(designacion.Cargos.Single());
                await context.SaveChangesAsync();
            }

            var filas = Historial();
            filas.Select(f => (f.Entidad, f.EntidadId)).Should().BeEquivalentTo(new[]
            {
                ("CargoDesignacion", cargoId.ToString()),
                ("Actividad", actividadId.ToString())
            });
            filas.Should().OnlyContain(f => f.Accion == AccionAuditoria.Baja && f.DocenteId == docenteId && f.DesignacionId == designacionId);
        }

        [Fact]
        public async Task ModificacionDeActividadSinElCargoCargado_ResuelveElVinculoDesdeLaBase()
        {
            var (docenteId, designacionId, _, actividadId) = CrearDesignacionSinAuditoria();

            await using (var context = _db.CrearContexto())
            {
                var actividad = await context.Actividades.SingleAsync(a => a.Id == actividadId);
                actividad.Horario = "MA 10 a 12";
                await context.SaveChangesAsync();
            }

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Entidad.Should().Be("Actividad");
            fila.DocenteId.Should().Be(docenteId);
            fila.DesignacionId.Should().Be(designacionId);
            fila.Detalles.Should().ContainSingle(d => d.Propiedad == "Horario" && d.ValorAnterior == "LU 8 a 10" && d.ValorNuevo == "MA 10 a 12");
        }

        [Fact]
        public async Task ResolverVinculosDesdeLaBase_NoAgregaEntidadesAlChangeTracker()
        {
            var (_, _, _, actividadId) = CrearDesignacionSinAuditoria();

            await using var context = _db.CrearContexto();
            var actividad = await context.Actividades.SingleAsync(a => a.Id == actividadId);
            actividad.Horario = "MA 10 a 12";
            await context.SaveChangesAsync();

            // El vínculo (cargo -> designación -> docente) se consultó en la base durante el guardado, sin trackear nada;
            // las filas de historial tampoco quedan trackeadas
            context.ChangeTracker.Entries().Select(e => e.Entity).Should().ContainSingle().Which.Should().BeSameAs(actividad);
            Historial().Should().ContainSingle().Which.DesignacionId.Should().NotBeNull();
        }

        #endregion

        #region Estrategia de ejecución

        [Fact]
        public void ConfiguracionDeLaApi_NoUsaUnaEstrategiaQueReintente()
        {
            var options = new DbContextOptionsBuilder<DocentesDbContext>();
            DocentesDbContextConfiguracion.ConfigurarSqlServer(options, "Server=.;Database=NoSeConecta;Trusted_Connection=True",
                new AuditoriaInterceptor(_usuario, new ContextoOperacionAuditoria()));

            using var context = new DocentesDbContext(options.Options);

            context.Database.CreateExecutionStrategy().RetriesOnFailure.Should().BeFalse();
        }

        [Fact]
        public async Task ConUnaEstrategiaQueReintenta_ElInterceptorFallaExplicandoQueHacer()
        {
            var options = new DbContextOptionsBuilder<DocentesDbContext>()
                .UseSqlite(_db.Connection, o => o.ExecutionStrategy(d => new EstrategiaConReintentos(d)))
                .AddInterceptors(new AuditoriaInterceptor(_usuario, new ContextoOperacionAuditoria()))
                .Options;
            await using var context = new DocentesDbContext(options);
            context.Udbs.Add(new Udb { Nombre = "UDB Física" });

            var act = () => context.SaveChangesAsync();

            (await act.Should().ThrowAsync<InvalidOperationException>()).WithMessage("*ExecuteAsync*");
            Historial().Should().BeEmpty();
        }

        #endregion
    }
}
