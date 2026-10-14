using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Services;
using DocentesApp.Data.Context;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Designaciones;
using DocentesApp.Shared.DTOs.PuntosPorCargo;
using DocentesApp.Tests.Helpers;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Tests.Services
{
    // DesignacionService v5 contra SQLite en memoria (repositorios reales).
    // Cada operación usa un contexto nuevo, como un request de la API.
    public class DesignacionServiceTests : IDisposable
    {
        private readonly SqliteTestDatabase _db = new();
        private readonly MapsterMapper.IMapper _mapper = MapperFactory.Create();

        private readonly int _udbFisica;
        private readonly int _udbQuimica;
        private readonly int _docenteA;
        private readonly int _docenteB;
        private readonly int _asignaturaFisica;
        private readonly int _curso;

        public DesignacionServiceTests()
        {
            using var context = _db.CrearContexto();

            var fisica = new Udb { Nombre = "UDB Física" };
            var quimica = new Udb { Nombre = "UDB Química" };
            var docenteA = new Docente { Nombre = "Juan", Apellido = "Perez", Legajo = 1, Dni = "1" };
            var docenteB = new Docente { Nombre = "Ana", Apellido = "Gomez", Legajo = 2, Dni = "2" };
            context.AddRange(fisica, quimica, docenteA, docenteB);
            context.SaveChanges();

            var asignatura = new Asignatura { Nombre = "Física I", Nivel = Nivel.Primero, Frecuencia = Frecuencia.Anual, UdbId = fisica.Id };
            var curso = new Curso { Año = Nivel.Primero, Carrera = default, NroComision = 1, Turno = default };
            context.AddRange(asignatura, curso);
            context.SaveChanges();

            _udbFisica = fisica.Id;
            _udbQuimica = quimica.Id;
            _docenteA = docenteA.Id;
            _docenteB = docenteB.Id;
            _asignaturaFisica = asignatura.Id;
            _curso = curso.Id;
        }

        public void Dispose() => _db.Dispose();

        #region Puntos

        [Fact]
        public async Task CreateAsync_CalculaPuntosDelCargo_UnaSolaVezAunqueTengaVariasActividades()
        {
            // Profesor Asociado DTP 1 = 157 x 2 x 1 = 314, con 3 actividades
            var cargo = Cargo(_udbFisica);
            cargo.Actividades = new List<UpsertActividadDto>
            {
                Docencia(), Docencia(), new UpsertActividadDto { Especificacion = EspecificacionCargo.Gestion }
            };

            var result = await CrearAsync(Designacion(_docenteA, cargo));

            result.Cargos.Should().ContainSingle().Which.Puntos.Should().Be(314);
            result.Cargos[0].Actividades.Should().HaveCount(3);
            result.TotalPuntos.Should().Be(314);
        }

        [Fact]
        public async Task CreateAsync_CargoEnLicencia_TienePuntosCero()
        {
            var result = await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica, enLicencia: true)));

            result.Cargos[0].Puntos.Should().Be(0);
            result.TotalPuntos.Should().Be(0);
        }

        [Fact]
        public async Task CreateAsync_AsignaturaDeOtraUdb_LanzaBadRequest()
        {
            var cargo = Cargo(_udbQuimica);
            cargo.Actividades = new List<UpsertActividadDto> { Docencia() }; // Física I es de la UDB Física

            var act = () => CrearAsync(Designacion(_docenteA, cargo));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("La asignatura Física I no pertenece a la UDB del cargo.");
        }

        [Fact]
        public async Task CreateAsync_DocenteSinMembresia_CreaDocenteUdbComoDocente()
        {
            await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica), Cargo(_udbFisica, cargo: CargoDocente.AyudantePrimera)));

            using var context = _db.CrearContexto();
            var membresias = await context.DocenteUdbs.Where(m => m.DocenteId == _docenteA).ToListAsync();
            membresias.Should().ContainSingle();
            membresias[0].UdbId.Should().Be(_udbFisica);
            membresias[0].Rol.Should().Be(RolUdb.Docente);
        }

        [Fact]
        public async Task UpdateAsync_SinCambiosRelevantes_NoRecalculaPuntos_AunqueCambieElCatalogo()
        {
            var creada = await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica))); // 314
            await CambiarPuntosBaseAsync(CargoDocente.ProfesorAsociado, 200);

            var dto = ParaEditar(creada);
            dto.Cargos[0].Observaciones = "Solo cambia la observación";
            dto.Cargos[0].Condicion = Condicion.Interino;
            await ActualizarAsync(creada.Id, dto);

            var editada = await ObtenerAsync(creada.Id);
            editada.Cargos[0].Puntos.Should().Be(314);
        }

        [Theory]
        [InlineData("dedicacion")]
        [InlineData("cantidad")]
        [InlineData("cargo")]
        public async Task UpdateAsync_CambioRelevante_RecalculaConElCatalogoVigente(string cambio)
        {
            var creada = await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica))); // 157 x 2 x 1 = 314
            await CambiarPuntosBaseAsync(CargoDocente.ProfesorAsociado, 200);

            var dto = ParaEditar(creada);
            var esperado = cambio switch
            {
                "dedicacion" => SetYDevolver(() => dto.Cargos[0].TipoDedicacion = TipoDedicacion.Simple, 200m),   // 200 x 1 x 1
                "cantidad" => SetYDevolver(() => dto.Cargos[0].CantidadDedicacion = 1.5m, 600m),                  // 200 x 2 x 1,5
                _ => SetYDevolver(() => dto.Cargos[0].Cargo = CargoDocente.ProfesorTitular, 352m)                 // 176 x 2 x 1
            };
            await ActualizarAsync(creada.Id, dto);

            var editada = await ObtenerAsync(creada.Id);
            editada.Cargos[0].Puntos.Should().Be(esperado);
        }

        [Fact]
        public async Task UpdateAsync_PasaAEnLicencia_PuntosCero()
        {
            var creada = await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica)));

            var dto = ParaEditar(creada);
            dto.Cargos[0].EnLicencia = true;
            await ActualizarAsync(creada.Id, dto);

            (await ObtenerAsync(creada.Id)).Cargos[0].Puntos.Should().Be(0);
        }

        [Fact]
        public async Task UpdateAsync_CargoDeOtraDesignacion_LanzaBadRequest()
        {
            var otra = await CrearAsync(Designacion(_docenteB, Cargo(_udbFisica)));
            var creada = await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica)));

            var dto = ParaEditar(creada);
            dto.Cargos[0].Id = otra.Cargos[0].Id;

            var act = () => ActualizarAsync(creada.Id, dto);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage($"El cargo con ID {otra.Cargos[0].Id} no pertenece a esta designación.");
        }

        #endregion

        #region Suplencias (b) a (g); (a) y (e) están en UpsertCargoDesignacionDtoValidatorTests

        [Fact]
        public async Task CreateAsync_SuplenteValido_SeGuarda()
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);

            var result = await CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, enLicencia)));

            result.Cargos[0].ReemplazaACargoId.Should().Be(enLicencia);
        }

        [Fact]
        public async Task CreateAsync_ReemplazaCargoInexistente_LanzaNotFound()          // (b)
        {
            var act = () => CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, 999)));

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No se encontró el cargo a reemplazar (ID 999).");
        }

        [Fact]
        public async Task CreateAsync_ReemplazaCargoDeDesignacionNoVigente_LanzaBadRequest()   // (b)
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica, Estado.Finalizada);

            var act = () => CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, enLicencia)));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El cargo a reemplazar no pertenece a una designación vigente.");
        }

        [Fact]
        public async Task CreateAsync_ReemplazaCargoSinLicencia_LanzaBadRequest()        // (c)
        {
            var sinLicencia = (await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica)))).Cargos[0].Id;

            var act = () => CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, sinLicencia)));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Solo se puede reemplazar un cargo que está en licencia.");
        }

        [Fact]
        public async Task CreateAsync_ReemplazaCargoDeOtraUdb_LanzaBadRequest()          // (d)
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);

            var act = () => CrearAsync(Designacion(_docenteB, Suplente(_udbQuimica, enLicencia)));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El cargo suplente debe ser de la misma UDB que el cargo que reemplaza.");
        }

        [Fact]
        public async Task CreateAsync_CargoYaTieneSuplenteVigente_LanzaConflict()        // (f)
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);
            await CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, enLicencia)));

            using (var context = _db.CrearContexto())
            {
                context.Docentes.Add(new Docente { Nombre = "Otro", Apellido = "Suplente", Legajo = 3, Dni = "3" });
                await context.SaveChangesAsync();
            }
            var docenteC = await IdDocenteAsync(3);

            var act = () => CrearAsync(Designacion(docenteC, Suplente(_udbFisica, enLicencia)));

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("El cargo a reemplazar ya tiene un suplente vigente.");
        }

        [Fact]
        public async Task CreateAsync_ReemplazaCargoDelMismoDocente_LanzaBadRequest()    // (g)
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);

            var act = () => CrearAsync(Designacion(_docenteA, Suplente(_udbFisica, enLicencia)));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Un docente no puede ser suplente de su propio cargo.");
        }

        // Ajuste de la etapa 4: las reglas de suplencia se validan solo si ReemplazaACargoId es nuevo o cambió
        [Fact]
        public async Task UpdateAsync_SuplenteConMismoReemplazo_SeEditaAunqueElReemplazadoYaNoEsteEnLicencia()
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);
            var suplente = await CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, enLicencia)));

            // El titular vuelve de la licencia
            var titular = await ObtenerAsync(await IdDesignacionDelCargoAsync(enLicencia));
            var dtoTitular = ParaEditar(titular);
            dtoTitular.Cargos[0].EnLicencia = false;
            await ActualizarAsync(titular.Id, dtoTitular);

            // Editar el suplente sin cambiar ReemplazaACargoId sigue funcionando
            var dto = ParaEditar(suplente);
            dto.Cargos[0].Observaciones = "Fin de la suplencia pendiente";
            var act = () => ActualizarAsync(suplente.Id, dto);

            await act.Should().NotThrowAsync();
            (await ObtenerAsync(suplente.Id)).Cargos[0].Observaciones.Should().Be("Fin de la suplencia pendiente");
        }

        [Fact]
        public async Task UpdateAsync_SuplenteCambiaAUnCargoSinLicencia_LanzaBadRequest()
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);
            var suplente = await CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, enLicencia)));
            var sinLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica, enLicencia: false);

            var dto = ParaEditar(suplente);
            dto.Cargos[0].ReemplazaACargoId = sinLicencia;
            var act = () => ActualizarAsync(suplente.Id, dto);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Solo se puede reemplazar un cargo que está en licencia.");
        }

        [Fact]
        public async Task UpdateAsync_QuitarCargoQueTieneSuplente_LanzaConflict()
        {
            var enLicencia = await CrearCargoEnLicenciaAsync(_docenteA, _udbFisica);
            await CrearAsync(Designacion(_docenteB, Suplente(_udbFisica, enLicencia)));

            var titular = await ObtenerAsync(await IdDesignacionDelCargoAsync(enLicencia));
            var dto = ParaEditar(titular);
            dto.Cargos[0] = Cargo(_udbFisica, cargo: CargoDocente.AyudantePrimera); // reemplaza el cargo por uno nuevo

            var act = () => ActualizarAsync(titular.Id, dto);

            await act.Should().ThrowAsync<ConflictException>();
        }

        #endregion

        #region PuntosPorCargo (catálogo)

        [Fact]
        public async Task PuntosPorCargo_DeleteEnUso_LanzaConflict()
        {
            await CrearAsync(Designacion(_docenteA, Cargo(_udbFisica)));

            using var context = _db.CrearContexto();
            var service = new PuntosPorCargoService(new PuntosPorCargoRepository(context), _mapper);
            var id = (await context.PuntosPorCargo.SingleAsync(p => p.Cargo == CargoDocente.ProfesorAsociado)).Id;

            var act = () => service.DeleteAsync(id);

            await act.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task PuntosPorCargo_CreateDuplicado_LanzaConflict()
        {
            using var context = _db.CrearContexto();
            var service = new PuntosPorCargoService(new PuntosPorCargoRepository(context), _mapper);

            var act = () => service.CreateAsync(new CreatePuntosPorCargoDto { Cargo = CargoDocente.ProfesorAdjunto, PuntosBase = 1 });

            await act.Should().ThrowAsync<ConflictException>();
        }

        #endregion

        #region Helpers

        private DesignacionService CrearServicio(DocentesDbContext context) => new(
            new DesignacionRepository(context),
            new DocenteRepository(context),
            new UdbRepository(context),
            new PuntosPorCargoRepository(context),
            new AsignaturaRepository(context),
            new CursoRepository(context),
            _mapper);

        private async Task<DesignacionDto> CrearAsync(CreateDesignacionDto dto)
        {
            using var context = _db.CrearContexto();
            return await CrearServicio(context).CreateAsync(dto);
        }

        private async Task ActualizarAsync(int id, UpdateDesignacionDto dto)
        {
            using var context = _db.CrearContexto();
            await CrearServicio(context).UpdateAsync(id, dto);
        }

        private async Task<DesignacionDto> ObtenerAsync(int id)
        {
            using var context = _db.CrearContexto();
            return await CrearServicio(context).GetByIdAsync(id);
        }

        private async Task CambiarPuntosBaseAsync(CargoDocente cargo, decimal puntosBase)
        {
            using var context = _db.CrearContexto();
            var service = new PuntosPorCargoService(new PuntosPorCargoRepository(context), _mapper);
            var id = (await context.PuntosPorCargo.AsNoTracking().SingleAsync(p => p.Cargo == cargo)).Id;
            await service.UpdateAsync(id, new UpdatePuntosPorCargoDto { PuntosBase = puntosBase });
        }

        // Crea una designación con un cargo (por defecto en licencia) y devuelve el Id del cargo
        private async Task<int> CrearCargoEnLicenciaAsync(int docenteId, int udbId, Estado estado = Estado.Activa, bool enLicencia = true)
        {
            var dto = Designacion(docenteId, Cargo(udbId, enLicencia: enLicencia));
            dto.EstadoDesignacion = estado;
            return (await CrearAsync(dto)).Cargos[0].Id;
        }

        private async Task<int> IdDesignacionDelCargoAsync(int cargoId)
        {
            using var context = _db.CrearContexto();
            return (await context.CargosDesignacion.SingleAsync(c => c.Id == cargoId)).DesignacionId;
        }

        private async Task<int> IdDocenteAsync(int legajo)
        {
            using var context = _db.CrearContexto();
            return (await context.Docentes.SingleAsync(d => d.Legajo == legajo)).Id;
        }

        private static CreateDesignacionDto Designacion(int docenteId, params UpsertCargoDesignacionDto[] cargos) => new()
        {
            DocenteId = docenteId,
            FechaInicio = DateTime.Today.AddMonths(-1),
            EstadoDesignacion = Estado.Activa,
            Cargos = cargos.ToList()
        };

        // Por defecto: Profesor Asociado DTP 1 (314 puntos) con una actividad de gestión
        private static UpsertCargoDesignacionDto Cargo(
            int udbId,
            CargoDocente cargo = CargoDocente.ProfesorAsociado,
            bool enLicencia = false) => new()
        {
            Cargo = cargo,
            Condicion = Condicion.Regular,
            TipoDedicacion = TipoDedicacion.SemiExclusiva,
            CantidadDedicacion = 1,
            UdbId = udbId,
            EnLicencia = enLicencia,
            Actividades = new List<UpsertActividadDto> { new() { Especificacion = EspecificacionCargo.Gestion } }
        };

        private static UpsertCargoDesignacionDto Suplente(int udbId, int reemplazaACargoId)
        {
            var cargo = Cargo(udbId);
            cargo.Condicion = Condicion.Suplente;
            cargo.ReemplazaACargoId = reemplazaACargoId;
            return cargo;
        }

        private UpsertActividadDto Docencia() => new()
        {
            Especificacion = EspecificacionCargo.Docencia,
            AsignaturaId = _asignaturaFisica,
            CursoId = _curso
        };

        // Arma el dto de edición a partir de lo guardado (como hace el formulario)
        private static UpdateDesignacionDto ParaEditar(DesignacionDto d) => new()
        {
            FechaInicio = d.FechaInicio,
            FechaFin = d.FechaFin,
            NroResolucion = d.NroResolucion,
            NroNota = d.NroNota,
            EstadoDesignacion = d.EstadoDesignacion,
            Observaciones = d.Observaciones,
            Cargos = d.Cargos.Select(c => new UpsertCargoDesignacionDto
            {
                Id = c.Id,
                Cargo = c.Cargo,
                Condicion = c.Condicion,
                TipoDedicacion = c.TipoDedicacion,
                CantidadDedicacion = c.CantidadDedicacion,
                UdbId = c.UdbId,
                EnLicencia = c.EnLicencia,
                ReemplazaACargoId = c.ReemplazaACargoId,
                Observaciones = c.Observaciones,
                Actividades = c.Actividades.Select(a => new UpsertActividadDto
                {
                    Id = a.Id,
                    Especificacion = a.Especificacion,
                    AsignaturaId = a.AsignaturaId,
                    CursoId = a.CursoId,
                    Horario = a.Horario,
                    Observaciones = a.Observaciones
                }).ToList()
            }).ToList()
        };

        private static decimal SetYDevolver(Action cambio, decimal esperado)
        {
            cambio();
            return esperado;
        }

        #endregion
    }
}
