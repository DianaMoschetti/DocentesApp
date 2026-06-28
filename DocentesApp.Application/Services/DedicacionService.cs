using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Shared.DTOs.Dedicaciones;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class DedicacionService : IDedicacionService
    {
        private readonly IDedicacionRepository _dedicacionRepository;
        private readonly IMapper _mapper;

        public DedicacionService(IDedicacionRepository dedicacionRepository, IMapper mapper)
        {
            _dedicacionRepository = dedicacionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListDedicacionDto>> GetAllAsync()
        {
            var dedicaciones = await _dedicacionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListDedicacionDto>>(dedicaciones);
        }

        public async Task<DedicacionDto> GetByIdAsync(int id)
        {
            var dedicacion = await _dedicacionRepository.GetByIdAsync(id);

            if (dedicacion == null)
                throw new NotFoundException($"No se encontró la dedicación con ID {id}.");

            return _mapper.Map<DedicacionDto>(dedicacion);
        }

        public async Task<DedicacionDto> CreateAsync(CreateDedicacionDto dto)
        {
            var dedicacion = _mapper.Map<Dedicacion>(dto);

            await _dedicacionRepository.AddAsync(dedicacion);
            await _dedicacionRepository.SaveChangesAsync();

            return _mapper.Map<DedicacionDto>(dedicacion);
        }

        public async Task DeleteAsync(int id)
        {
            var dedicacion = await _dedicacionRepository.GetByIdAsync(id);

            if (dedicacion == null)
                throw new NotFoundException($"No se encontró la dedicación con ID {id}.");

            var tieneDesignaciones = await _dedicacionRepository.HasDesignacionesAsync(id);

            if (tieneDesignaciones)
                throw new ConflictException("No se puede eliminar la dedicación porque tiene designaciones asociadas.");

            _dedicacionRepository.Delete(dedicacion);
            try
            {
                await _dedicacionRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar la dedicación porque tiene designaciones asociadas.");
            }
        }
    }
}