using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Designaciones;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class DesignacionService : IDesignacionService
    {
        private readonly IDesignacionRepository _designacionRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IMapper _mapper;

        public DesignacionService(
            IDesignacionRepository designacionRepository,
            IDocenteRepository docenteRepository,
            ICargoRepository cargoRepository,
            IMapper mapper)
        {
            _designacionRepository = designacionRepository;
            _docenteRepository = docenteRepository;
            _cargoRepository = cargoRepository;
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

        public async Task<DesignacionDto> CreateAsync(CreateDesignacionDto dto)
        {
            // Validar que el docente existe
            var docente = await _docenteRepository.GetByIdAsync(dto.DocenteId);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {dto.DocenteId}.");

            // Validar que el cargo existe
            var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);
            if (cargo == null)
                throw new NotFoundException($"No se encontró el cargo con ID {dto.CargoId}.");

            // DIANA VER Validar regla 33: no pueden existir dos designaciones activas con la misma combinación docente-cargo
            var existeActiva = await _designacionRepository
                .ExisteDesignacionActivaAsync(dto.DocenteId, dto.CargoId);
            if (existeActiva)
                throw new BadRequestException(
                    "Ya existe una designación activa para ese docente con ese cargo.");

            // DIANA VER Validar que tenga al menos un detalle (regla 25)
            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new BadRequestException(
                    "La designación debe tener al menos un detalle de actividad.");

            // Mapster crea la cabecera y los detalles juntos
            var designacion = _mapper.Map<Designacion>(dto);

            await _designacionRepository.AddAsync(designacion);
            await _designacionRepository.SaveChangesAsync();

            // Recargo con todas las navegaciones para devolver el dto completo
            var designacionCompleta = await _designacionRepository.GetByIdAsync(designacion.Id);
            return _mapper.Map<DesignacionDto>(designacionCompleta!);
        }

        public async Task UpdateAsync(int id, UpdateDesignacionDto dto)
        {
            var designacion = await _designacionRepository.GetByIdAsync(id);
            if (designacion == null)
                throw new NotFoundException($"No se encontró la designación con ID {id}.");

            // Si cambia el cargo, validar que no haya otra designación activa
            // con la nueva combinación docente-cargo
            if (dto.CargoId != designacion.CargoId)
            {
                var existeActiva = await _designacionRepository
                    .ExisteDesignacionActivaAsync(designacion.DocenteId, dto.CargoId, excludeId: id);
                if (existeActiva)
                    throw new BadRequestException(
                        "Ya existe una designación activa para ese docente con ese cargo.");
            }

            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new BadRequestException(
                    "La designación debe tener al menos un detalle de actividad.");

            _mapper.Map(dto, designacion);
            _designacionRepository.Update(designacion);
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

            _designacionRepository.Update(designacion);
            await _designacionRepository.SaveChangesAsync();
        }
    }
}