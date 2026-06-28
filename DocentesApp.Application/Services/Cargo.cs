using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Cargos;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class CargoService : ICargoService
    {
        private readonly ICargoRepository _cargoRepository;
        private readonly IMapper _mapper;

        public CargoService(ICargoRepository cargoRepository, IMapper mapper)
        {
            _cargoRepository = cargoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListCargoDto>> GetAllAsync()
        {
            var cargos = await _cargoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListCargoDto>>(cargos);
        }

        public async Task<CargoDto> GetByIdAsync(int id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);

            if (cargo == null)
                throw new NotFoundException($"No se encontró el cargo con ID {id}.");

            return _mapper.Map<CargoDto>(cargo);
        }

        public async Task<CargoDto> CreateAsync(CreateCargoDto dto)
        {
            var existe = await _cargoRepository.ExistsAsync(dto.Denominacion, dto.TipoCargo, dto.Condicion);

            if (existe)
                throw new BadRequestException("Ya existe un cargo con esa combinación de denominación, tipo y condición.");

            var cargo = _mapper.Map<Cargo>(dto);

            await _cargoRepository.AddAsync(cargo);
            try
            {
                await _cargoRepository.SaveChangesAsync();
            }
            catch (Exception ex) when (TryMapUniqueConstraint(ex, out var mappedException))
            {
                throw mappedException;
            }

            return _mapper.Map<CargoDto>(cargo);
        }

        public async Task DeleteAsync(int id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);

            if (cargo == null)
                throw new NotFoundException($"No se encontró el cargo con ID {id}.");

            var tieneDesignaciones = await _cargoRepository.HasDesignacionesAsync(id);

            if (tieneDesignaciones)
                throw new ConflictException("No se puede eliminar el cargo porque tiene designaciones asociadas.");

            _cargoRepository.Delete(cargo);
            try
            {
                await _cargoRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar el cargo porque tiene designaciones asociadas.");
            }
        }

        // This method attempts to map a database exception to a more user-friendly application exception.
        // ver si es necesario sino borrarlo - diana  espara justo la situacion en la q dos personas van a editar al mismo
        // mismo tiempo y una de ellas genera un error de constraint unico, entonces se mapea a un BadRequestException
        private static bool TryMapUniqueConstraint(Exception ex, out AppException mappedException)
        {
            if (ex.GetType().Name != "DbUpdateException")
            {
                mappedException = null!;
                return false;
            }

            var message = ex.InnerException?.Message ?? ex.Message;
            var normalized = message.ToLowerInvariant();

            if (normalized.Contains("ix_cargos_denominacion_tipocargo_condicion") && normalized.Contains("unique"))
            {
                mappedException = new BadRequestException("Ya existe un cargo con esa combinación de denominación, tipo y condición.");
                return true;
            }

            mappedException = null!;
            return false;
        }
    }
}