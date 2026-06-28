using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Shared.DTOs.Udbs;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class UdbService : IUdbService
    {
        private readonly IUdbRepository _udbRepository;
        private readonly IMapper _mapper;

        public UdbService(IUdbRepository udbRepository, IMapper mapper)
        {
            _udbRepository = udbRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListUdbDto>> GetAllAsync()
        {
            var udbs = await _udbRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListUdbDto>>(udbs);
        }

        public async Task<UdbDto> GetByIdAsync(int id)
        {
            var udb = await _udbRepository.GetByIdAsync(id);

            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {id}.");

            return _mapper.Map<UdbDto>(udb);
        }

        public async Task<UdbDto> CreateAsync(CreateUdbDto dto)
        {
            var existe = await _udbRepository.ExistsAsync(dto.Nombre);

            if (existe)
                throw new BadRequestException("Ya existe una UDB con ese nombre.");

            var udb = _mapper.Map<Udb>(dto);

            await _udbRepository.AddAsync(udb);
            await _udbRepository.SaveChangesAsync();

            var udbConNavegaciones = await _udbRepository.GetByIdAsync(udb.Id);
            return _mapper.Map<UdbDto>(udbConNavegaciones!);
        }

        public async Task UpdateAsync(int id, UpdateUdbDto dto)
        {
            var udb = await _udbRepository.GetByIdAsync(id);

            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {id}.");

            var existe = await _udbRepository.ExistsAsync(dto.Nombre, excludeId: id);

            if (existe)
                throw new BadRequestException("Ya existe una UDB con ese nombre.");

            _mapper.Map(dto, udb);

            _udbRepository.Update(udb);
            await _udbRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var udb = await _udbRepository.GetByIdAsync(id);

            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {id}.");

            var tieneAsignaturas = await _udbRepository.HasAsignaturasAsync(id);

            if (tieneAsignaturas)
                throw new ConflictException("No se puede eliminar la UDB porque tiene asignaturas asociadas.");

            _udbRepository.Delete(udb);
            try
            {
                await _udbRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar la UDB porque tiene asignaturas asociadas.");
            }
        }
    }
}
