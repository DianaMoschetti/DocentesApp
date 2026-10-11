using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Common.Formatos;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Domain.Services;
using DocentesApp.Shared.DTOs.Designaciones;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class DesignacionService : IDesignacionService
    {
        private readonly IDesignacionRepository _designacionRepository;
        private readonly IDocenteRepository _docenteRepository;
        // [Diana desde v4.0 OBSOLETO] ICargoRepository eliminado — cargo pasa a DetalleDesignacion
        // private readonly ICargoRepository _cargoRepository;
        // v5: validaciones de cargos y actividades
        private readonly IUdbRepository _udbRepository;
        private readonly IPuntosPorCargoRepository _puntosPorCargoRepository;
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IMapper _mapper;

        public DesignacionService(
            IDesignacionRepository designacionRepository,
            IDocenteRepository docenteRepository,
            // [Diana desde v4.0 OBSOLETO] ICargoRepository eliminado
            // ICargoRepository cargoRepository,
            IUdbRepository udbRepository,
            IPuntosPorCargoRepository puntosPorCargoRepository,
            IAsignaturaRepository asignaturaRepository,
            ICursoRepository cursoRepository,
            IMapper mapper)
        {
            _designacionRepository = designacionRepository;
            _docenteRepository = docenteRepository;
            // [Diana desde v4.0 OBSOLETO]
            // _cargoRepository = cargoRepository;
            _udbRepository = udbRepository;
            _puntosPorCargoRepository = puntosPorCargoRepository;
            _asignaturaRepository = asignaturaRepository;
            _cursoRepository = cursoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListDesignacionDto>> GetAllAsync()
        {
            var designaciones = await _designacionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListDesignacionDto>>(designaciones);
        }

        public async Task<IEnumerable<ListDesignacionDto>> GetVigentesAsync()
        {
            var designaciones = await _designacionRepository.GetVigentesAsync();
            return _mapper.Map<IEnumerable<ListDesignacionDto>>(designaciones);
        }

        public async Task<IEnumerable<ListDesignacionDto>> GetByDocenteAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {docenteId}.");

            var designaciones = await _designacionRepository.GetByDocenteAsync(docenteId);
            return _mapper.Map<IEnumerable<ListDesignacionDto>>(designaciones);
        }

        public async Task<DesignacionDto> GetByIdAsync(int id)
        {
            var designacion = await _designacionRepository.GetByIdAsync(id);
            if (designacion == null)
                throw new NotFoundException($"No se encontró la designación con ID {id}.");

            return _mapper.Map<DesignacionDto>(designacion);
        }

        // Opciones del selector "Reemplaza a": cargos en licencia de la UDB, en designaciones vigentes y sin suplente vigente
        public async Task<IEnumerable<CargoEnLicenciaDto>> GetCargosEnLicenciaAsync(int udbId)
        {
            var udb = await _udbRepository.GetByIdAsync(udbId);
            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {udbId}.");

            var cargos = await _designacionRepository.GetCargosEnLicenciaAsync(udbId);

            return cargos.Select(c => new CargoEnLicenciaDto
            {
                Id = c.Id,
                UdbId = c.UdbId,
                Descripcion = TextosDesignacion.CargoConDocente(
                    c.Designacion.Docente.Apellido, c.Designacion.Docente.Nombre,
                    c.Cargo, c.CantidadDedicacion, c.TipoDedicacion)
            });
        }

        // [Diana desde v4.0 OBSOLETO] creación v4 (cabecera + detalles con Mapster); reemplazada por la versión v5 de abajo
        //public async Task<DesignacionDto> CreateAsync(CreateDesignacionDto dto)
        //{
        //    // Validar que el docente existe
        //    var docente = await _docenteRepository.GetByIdAsync(dto.DocenteId);
        //    if (docente == null)
        //        throw new NotFoundException($"No se encontró el docente con ID {dto.DocenteId}.");
        //
        //    // [Diana desde v4.0 OBSOLETO] validación de cargo eliminada — cargo pasa a DetalleDesignacion
        //    // var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);
        //    // if (cargo == null)
        //    //     throw new NotFoundException($"No se encontró el cargo con ID {dto.CargoId}.");
        //
        //    // [Diana desde v4.0 OBSOLETO] Regla 33 eliminada — Regla 41: un docente puede tener
        //    // múltiples designaciones activas con la misma categoría de cargo
        //    // var existeActiva = await _designacionRepository
        //    //     .ExisteDesignacionActivaAsync(dto.DocenteId, dto.CargoId);
        //    // if (existeActiva)
        //    //     throw new BadRequestException(
        //    //         "Ya existe una designación activa para ese docente con ese cargo.");
        //
        //    // DIANA VER Validar que tenga al menos un detalle (regla 25)
        //    // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos (v5); la creación completa se reescribe en la etapa Application
        //    //if (dto.Detalles == null || !dto.Detalles.Any())
        //    //    throw new BadRequestException(
        //    //        "La designación debe tener al menos un detalle de actividad.");
        //    if (dto.Cargos == null || !dto.Cargos.Any())
        //        throw new BadRequestException(
        //            "La designación debe tener al menos un cargo.");
        //
        //    // Mapster crea la cabecera y los detalles juntos
        //    var designacion = _mapper.Map<Designacion>(dto);
        //
        //    await _designacionRepository.AddAsync(designacion);
        //    await _designacionRepository.SaveChangesAsync();
        //
        //    // Recargo con todas las navegaciones para devolver el dto completo
        //    var designacionCompleta = await _designacionRepository.GetByIdAsync(designacion.Id);
        //    return _mapper.Map<DesignacionDto>(designacionCompleta!);
        //}

        public async Task<DesignacionDto> CreateAsync(CreateDesignacionDto dto)
        {
            // Validar que el docente existe
            var docente = await _docenteRepository.GetByIdAsync(dto.DocenteId);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {dto.DocenteId}.");

            // Regla 41: un docente puede tener múltiples designaciones activas con la misma categoría de cargo

            if (dto.Cargos == null || !dto.Cargos.Any())
                throw new BadRequestException(
                    "La designación debe tener al menos un cargo.");

            // Validaciones que necesitan la BD (UDB, catálogo de puntos, asignaturas, cursos, suplencias)
            var puntosBase = await ValidarCargosAsync(dto.DocenteId, dto.Cargos);

            // Mapster arma solo la cabecera; los cargos se arman acá porque los puntos los calcula el backend
            var designacion = _mapper.Map<Designacion>(dto);
            foreach (var cargoDto in dto.Cargos)
                designacion.Cargos.Add(CrearCargo(cargoDto, puntosBase[cargoDto.Cargo]));

            // Si el docente todavía no pertenece a la UDB de algún cargo, se lo agrega (mismo SaveChanges)
            await AgregarMembresiasAsync(dto.DocenteId, dto.Cargos.Select(c => c.UdbId));

            await _designacionRepository.AddAsync(designacion);
            await _designacionRepository.SaveChangesAsync();

            // Recargo con todas las navegaciones para devolver el dto completo
            var designacionCompleta = await _designacionRepository.GetByIdAsync(designacion.Id);
            return _mapper.Map<DesignacionDto>(designacionCompleta!);
        }

        // [Diana desde v4.0 OBSOLETO] edición v4 (Mapster sobre cabecera + detalles); reemplazada por la versión v5 de abajo
        //public async Task UpdateAsync(int id, UpdateDesignacionDto dto)
        //{
        //    var designacion = await _designacionRepository.GetByIdAsync(id);
        //    if (designacion == null)
        //        throw new NotFoundException($"No se encontró la designación con ID {id}.");
        //
        //    // [Diana desde v4.0 OBSOLETO] validación de CargoId eliminada — cargo pasa a DetalleDesignacion
        //    // if (dto.CargoId != designacion.CargoId)
        //    // {
        //    //     var existeActiva = await _designacionRepository
        //    //         .ExisteDesignacionActivaAsync(designacion.DocenteId, dto.CargoId, excludeId: id);
        //    //     if (existeActiva)
        //    //         throw new BadRequestException(
        //    //             "Ya existe una designación activa para ese docente con ese cargo.");
        //    // }
        //
        //    // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos (v5); la edición completa se reescribe en la etapa Application
        //    //if (dto.Detalles == null || !dto.Detalles.Any())
        //    //    throw new BadRequestException(
        //    //        "La designación debe tener al menos un detalle de actividad.");
        //    if (dto.Cargos == null || !dto.Cargos.Any())
        //        throw new BadRequestException(
        //            "La designación debe tener al menos un cargo.");
        //
        //    _mapper.Map(dto, designacion);
        //    _designacionRepository.Update(designacion);
        //    await _designacionRepository.SaveChangesAsync();
        //}

        // Edición v5: sincroniza los cargos por Id.
        //   - cargo sin Id: se agrega y se calculan sus puntos
        //   - cargo con Id: se actualiza; los puntos se recalculan SOLO si cambió Cargo, dedicación, cantidad o EnLicencia
        //     (un cambio posterior en PuntosPorCargo no altera designaciones existentes)
        //   - cargo que no viene: se quita (409 si otro cargo lo reemplaza)
        //   - las actividades de cada cargo se reemplazan por las que vienen
        public async Task UpdateAsync(int id, UpdateDesignacionDto dto)
        {
            var designacion = await _designacionRepository.GetByIdAsync(id);
            if (designacion == null)
                throw new NotFoundException($"No se encontró la designación con ID {id}.");

            if (dto.Cargos == null || !dto.Cargos.Any())
                throw new BadRequestException(
                    "La designación debe tener al menos un cargo.");

            var existentes = designacion.Cargos.ToDictionary(c => c.Id);

            var idAjeno = dto.Cargos
                .Where(c => c.Id.HasValue && !existentes.ContainsKey(c.Id.Value))
                .Select(c => c.Id)
                .FirstOrDefault();
            if (idAjeno != null)
                throw new BadRequestException($"El cargo con ID {idAjeno} no pertenece a esta designación.");

            var puntosBase = await ValidarCargosAsync(designacion.DocenteId, dto.Cargos);

            // Cargos quitados
            var idsEnviados = dto.Cargos.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToHashSet();
            foreach (var quitado in existentes.Values.Where(c => !idsEnviados.Contains(c.Id)).ToList())
            {
                if (await _designacionRepository.TieneReemplazosAsync(quitado.Id))
                    throw new ConflictException(
                        $"No se puede quitar el cargo {TextosDesignacion.CargoCorto(quitado.Cargo, quitado.CantidadDedicacion, quitado.TipoDedicacion)} porque otro cargo lo reemplaza.");

                designacion.Cargos.Remove(quitado); // EF borra el cargo huérfano y sus actividades
            }

            // Cabecera (Mapster ignora Id, DocenteId y Cargos)
            _mapper.Map(dto, designacion);

            foreach (var cargoDto in dto.Cargos)
            {
                if (!cargoDto.Id.HasValue)
                {
                    designacion.Cargos.Add(CrearCargo(cargoDto, puntosBase[cargoDto.Cargo]));
                    continue;
                }

                var cargo = existentes[cargoDto.Id.Value];

                var recalcular = cargo.Cargo != cargoDto.Cargo
                    || cargo.TipoDedicacion != cargoDto.TipoDedicacion
                    || cargo.CantidadDedicacion != cargoDto.CantidadDedicacion
                    || cargo.EnLicencia != cargoDto.EnLicencia;

                cargo.Cargo = cargoDto.Cargo;
                cargo.Condicion = cargoDto.Condicion;
                cargo.TipoDedicacion = cargoDto.TipoDedicacion;
                cargo.CantidadDedicacion = cargoDto.CantidadDedicacion;
                cargo.UdbId = cargoDto.UdbId;
                cargo.EnLicencia = cargoDto.EnLicencia;
                cargo.ReemplazaACargoId = cargoDto.ReemplazaACargoId;
                cargo.Observaciones = Normalizar(cargoDto.Observaciones);

                if (recalcular)
                    cargo.Puntos = CalculadoraPuntos.Calcular(
                        puntosBase[cargoDto.Cargo], cargoDto.TipoDedicacion, cargoDto.CantidadDedicacion, cargoDto.EnLicencia);

                cargo.Actividades.Clear(); // EF borra las actividades huérfanas
                foreach (var actividadDto in cargoDto.Actividades)
                    cargo.Actividades.Add(CrearActividad(actividadDto));
            }

            await AgregarMembresiasAsync(designacion.DocenteId, dto.Cargos.Select(c => c.UdbId));

            // Sin _designacionRepository.Update(): la entidad ya está trackeada y Update() marcaría como modificado
            // todo el grafo cargado (incluidas las designaciones de los cargos reemplazados).
            await _designacionRepository.SaveChangesAsync();
        }

        public async Task CerrarAsync(int id)
        {
            var designacion = await _designacionRepository.GetByIdAsync(id);
            if (designacion == null)
                throw new NotFoundException($"No se encontró la designación con ID {id}.");

            if (designacion.FechaFin != null && designacion.FechaFin <= DateTime.Now)
                throw new BadRequestException("La designación ya está cerrada.");

            designacion.FechaFin = DateTime.Now;
            designacion.EstadoDesignacion = Estado.Finalizada;

            // [Diana desde v4.0 OBSOLETO] la entidad ya está trackeada; Update() marcaría como modificado todo el grafo
            // cargado por GetByIdAsync (cargos, actividades y designaciones de los cargos reemplazados)
            //_designacionRepository.Update(designacion);
            await _designacionRepository.SaveChangesAsync();
        }

        // Validaciones de los cargos que necesitan la BD. Devuelve los PuntosBase vigentes del catálogo por cargo.
        // (Las reglas sin BD — actividades por especificación, Ayudante de 2° solo Simple, etc. — están en los validadores de Shared.)
        private async Task<Dictionary<CargoDocente, decimal>> ValidarCargosAsync(int docenteId, IEnumerable<UpsertCargoDesignacionDto> cargos)
        {
            var puntosBase = new Dictionary<CargoDocente, decimal>();
            var udbsValidadas = new HashSet<int>();
            var asignaturas = new Dictionary<int, Asignatura>();
            var cursosValidados = new HashSet<int>();
            var reemplazados = new HashSet<int>();

            foreach (var cargo in cargos)
            {
                if (udbsValidadas.Add(cargo.UdbId) && await _udbRepository.GetByIdAsync(cargo.UdbId) == null)
                    throw new NotFoundException($"No se encontró la UDB con ID {cargo.UdbId}.");

                if (!puntosBase.ContainsKey(cargo.Cargo))
                {
                    var fila = await _puntosPorCargoRepository.GetByCargoAsync(cargo.Cargo);
                    if (fila == null)
                        throw new BadRequestException(
                            $"No hay puntos cargados para el cargo {cargo.Cargo.Descripcion()} en Puntos por cargo.");
                    puntosBase[cargo.Cargo] = fila.PuntosBase;
                }

                foreach (var actividad in cargo.Actividades)
                {
                    if (actividad.AsignaturaId is int asignaturaId)
                    {
                        if (!asignaturas.TryGetValue(asignaturaId, out var asignatura))
                        {
                            asignatura = await _asignaturaRepository.GetByIdAsync(asignaturaId)
                                ?? throw new NotFoundException($"No se encontró la asignatura con ID {asignaturaId}.");
                            asignaturas[asignaturaId] = asignatura;
                        }

                        // La asignatura de cada actividad debe ser de la misma UDB que el cargo
                        if (asignatura.UdbId != cargo.UdbId)
                            throw new BadRequestException(
                                $"La asignatura {asignatura.Nombre} no pertenece a la UDB del cargo.");
                    }

                    if (actividad.CursoId is int cursoId
                        && cursosValidados.Add(cursoId)
                        && await _cursoRepository.GetByIdAsync(cursoId) == null)
                        throw new NotFoundException($"No se encontró el curso con ID {cursoId}.");
                }

                if (cargo.ReemplazaACargoId is int reemplazadoId)
                {
                    if (!reemplazados.Add(reemplazadoId))
                        throw new BadRequestException("Dos cargos de la designación no pueden reemplazar al mismo cargo.");

                    await ValidarReemplazoAsync(docenteId, cargo, reemplazadoId);
                }
            }

            return puntosBase;
        }

        // Suplencias: (a) no a sí mismo y (e) condición Suplente se validan en UpsertCargoDesignacionDtoValidator.
        private async Task ValidarReemplazoAsync(int docenteId, UpsertCargoDesignacionDto cargo, int reemplazadoId)
        {
            // (b) el cargo reemplazado existe y está en una designación vigente
            var reemplazado = await _designacionRepository.GetCargoByIdAsync(reemplazadoId);
            if (reemplazado == null)
                throw new NotFoundException($"No se encontró el cargo a reemplazar (ID {reemplazadoId}).");

            if (!TextosDesignacion.EsVigente(reemplazado.Designacion, DateTime.Now))
                throw new BadRequestException("El cargo a reemplazar no pertenece a una designación vigente.");

            // (c) está en licencia
            if (!reemplazado.EnLicencia)
                throw new BadRequestException("Solo se puede reemplazar un cargo que está en licencia.");

            // (d) misma UDB
            if (reemplazado.UdbId != cargo.UdbId)
                throw new BadRequestException("El cargo suplente debe ser de la misma UDB que el cargo que reemplaza.");

            // (g) de otro docente
            if (reemplazado.Designacion.DocenteId == docenteId)
                throw new BadRequestException("Un docente no puede ser suplente de su propio cargo.");

            // (f) a lo sumo un suplente vigente por cargo en licencia
            if (await _designacionRepository.ExisteSuplenteActivoAsync(reemplazadoId, cargo.Id))
                throw new ConflictException("El cargo a reemplazar ya tiene un suplente vigente.");
        }

        private async Task AgregarMembresiasAsync(int docenteId, IEnumerable<int> udbIds)
        {
            foreach (var udbId in udbIds.Distinct())
            {
                if (!await _udbRepository.EsMiembroAsync(docenteId, udbId))
                    await _udbRepository.AddMiembroAsync(new DocenteUdb
                    {
                        DocenteId = docenteId,
                        UdbId = udbId,
                        Rol = RolUdb.Docente
                    });
            }
        }

        private static CargoDesignacion CrearCargo(UpsertCargoDesignacionDto dto, decimal puntosBase)
        {
            return new CargoDesignacion
            {
                Cargo = dto.Cargo,
                Condicion = dto.Condicion,
                TipoDedicacion = dto.TipoDedicacion,
                CantidadDedicacion = dto.CantidadDedicacion,
                UdbId = dto.UdbId,
                EnLicencia = dto.EnLicencia,
                ReemplazaACargoId = dto.ReemplazaACargoId,
                Observaciones = Normalizar(dto.Observaciones),
                Puntos = CalculadoraPuntos.Calcular(puntosBase, dto.TipoDedicacion, dto.CantidadDedicacion, dto.EnLicencia),
                Actividades = dto.Actividades.Select(CrearActividad).ToList()
            };
        }

        private static Actividad CrearActividad(UpsertActividadDto dto)
        {
            return new Actividad
            {
                Especificacion = dto.Especificacion,
                AsignaturaId = dto.AsignaturaId,
                CursoId = dto.CursoId,
                Horario = Normalizar(dto.Horario),
                Observaciones = Normalizar(dto.Observaciones)
            };
        }

        // Texto vacío -> null
        private static string? Normalizar(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
