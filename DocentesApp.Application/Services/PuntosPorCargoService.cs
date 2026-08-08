using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
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
            var puntosPorCargo = _mapper.Map<PuntosPorCargo>(dto);

            await _puntosPorCargoRepository.AddAsync(puntosPorCargo);
            await _puntosPorCargoRepository.SaveChangesAsync();

            return _mapper.Map<PuntosPorCargoDto>(puntosPorCargo);
        }

        public async Task DeleteAsync(int id)
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetByIdAsync(id);

            if (puntosPorCargo == null)
                throw new NotFoundException($"No se encontraron puntos por cargo con ID {id}.");

            _puntosPorCargoRepository.Delete(puntosPorCargo);
            await _puntosPorCargoRepository.SaveChangesAsync();
        }

        public async Task<decimal> GetPuntosAsync(DenominacionCargo denominacion, TipoCargo tipoCargo)
        {
            var puntosPorCargo = await _puntosPorCargoRepository.GetByCargoAsync(denominacion, tipoCargo);
            return puntosPorCargo?.PuntosBase ?? 0;
        }
    }
}
