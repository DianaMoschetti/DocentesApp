using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Shared.DTOs.PuntosPorCargo;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class PuntosPorCargoService : IPuntosPorCargoService
    {
        private readonly IPuntosPorCargoRepository _puntosPorCargoRepository;
        private readonly IMapper _mapper;

        public PuntosPorCargoService(IPuntosPorCargoRepository puntosPorCargoRepository, IMapper mapper)
        {
            _puntosPorCargoRepository = puntosPorCargoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PuntosPorCargoDto>> GetAllAsync()
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PuntosPorCargoDto>>(puntosPorCargo);
        }

        public async Task<PuntosPorCargoDto> GetByIdAsync(int id)
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetByIdAsync(id);

            if (puntosPorCargo == null)
                throw new NotFoundException($"No se encontraron puntos por cargo con ID {id}.");

            return _mapper.Map<PuntosPorCargoDto>(puntosPorCargo);
        }

        public async Task<PuntosPorCargoDto> CreateAsync(CreatePuntosPorCargoDto dto)
        {
            // Una sola fila por cargo (índice único en Cargo)
            var existente = await _puntosPorCargoRepository.GetByCargoAsync(dto.Cargo);
            if (existente != null)
                throw new ConflictException($"Ya hay puntos cargados para el cargo {dto.Cargo.Descripcion()}.");

            var puntosPorCargo = _mapper.Map<PuntosPorCargo>(dto);

            await _puntosPorCargoRepository.AddAsync(puntosPorCargo);
            await _puntosPorCargoRepository.SaveChangesAsync();

            return _mapper.Map<PuntosPorCargoDto>(puntosPorCargo);
        }

        // Solo cambia los puntos base. Los puntos ya guardados en designaciones existentes NO se recalculan.
        public async Task UpdateAsync(int id, UpdatePuntosPorCargoDto dto)
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetByIdAsync(id);

            if (puntosPorCargo == null)
                throw new NotFoundException($"No se encontraron puntos por cargo con ID {id}.");

            puntosPorCargo.PuntosBase = dto.PuntosBase;
            await _puntosPorCargoRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetByIdAsync(id);

            if (puntosPorCargo == null)
                throw new NotFoundException($"No se encontraron puntos por cargo con ID {id}.");

            // No se puede borrar la fila de un cargo que ya se usa en designaciones
            if (await _puntosPorCargoRepository.EstaEnUsoAsync(puntosPorCargo.Cargo))
                throw new ConflictException(
                    $"No se pueden eliminar los puntos de {puntosPorCargo.Cargo.Descripcion()} porque hay designaciones con ese cargo.");

            _puntosPorCargoRepository.Delete(puntosPorCargo);
            await _puntosPorCargoRepository.SaveChangesAsync();
        }

        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser CargoDocente
        //public async Task<decimal> GetPuntosAsync(DenominacionCargo denominacion, TipoCargo tipoCargo)
        //{
        //    var puntosPorCargo = await _puntosPorCargoRepository.GetByCargoAsync(denominacion, tipoCargo);
        //    return puntosPorCargo?.PuntosBase ?? 0;
        //}
        public async Task<decimal> GetPuntosAsync(CargoDocente cargo)
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetByCargoAsync(cargo);
            return puntosPorCargo?.PuntosBase ?? 0;
        }
    }
}
