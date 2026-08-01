using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Asignaturas;
using MapsterMapper;
namespace DocentesApp.Application.Services
{
    public class AsignaturaService : IAsignaturaService
    {
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly IMapper _mapper;
        public AsignaturaService(IAsignaturaRepository asignaturaRepository, IMapper mapper)
        {
            _asignaturaRepository = asignaturaRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ListAsignaturaDto>> GetAllAsync()
        {
            var asignaturas = await _asignaturaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListAsignaturaDto>>(asignaturas);
        }
        public async Task<AsignaturaDto> GetByIdAsync(int id)
        {
            var asignatura = await _asignaturaRepository.GetByIdAsync(id);
            if (asignatura == null)
                throw new NotFoundException($"No se encontró la asignatura con ID {id}.");
            return _mapper.Map<AsignaturaDto>(asignatura);
        }
        public async Task<AsignaturaDto> CreateAsync(CreateAsignaturaDto dto)
        {
            // [Diana desde v4.0 OBSOLETO] ExistsAsync usaba (Materia)dto.NombreAsignatura
            // ahora usa dto.Nombre (string) directamente
            var existe = await _asignaturaRepository.ExistsAsync(
                dto.Nombre, (Nivel)dto.Nivel, dto.UdbId);
            if (existe)
                throw new BadRequestException("Ya existe una asignatura con esa combinación de nombre, nivel y UDB.");
            var asignatura = _mapper.Map<Asignatura>(dto);
            await _asignaturaRepository.AddAsync(asignatura);
            await _asignaturaRepository.SaveChangesAsync();
            var asignaturaConNavegaciones = await _asignaturaRepository.GetByIdAsync(asignatura.Id);
            return _mapper.Map<AsignaturaDto>(asignaturaConNavegaciones!);
        }
        public async Task UpdateAsync(int id, UpdateAsignaturaDto dto)
        {
            var asignatura = await _asignaturaRepository.GetByIdAsync(id);
            if (asignatura == null)
                throw new NotFoundException($"No se encontró la asignatura con ID {id}.");
            // [Diana desde v4.0 OBSOLETO] ExistsAsync usaba (Materia)dto.NombreAsignatura
            // ahora usa dto.Nombre (string) directamente
            var existe = await _asignaturaRepository.ExistsAsync(
                dto.Nombre, (Nivel)dto.Nivel, dto.UdbId, excludeId: id);
            if (existe)
                throw new BadRequestException("Ya existe una asignatura con esa combinación de nombre, nivel y UDB.");
            _mapper.Map(dto, asignatura);
            _asignaturaRepository.Update(asignatura);
            await _asignaturaRepository.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var asignatura = await _asignaturaRepository.GetByIdAsync(id);
            if (asignatura == null)
                throw new NotFoundException($"No se encontró la asignatura con ID {id}.");
            var tieneDetalles = await _asignaturaRepository.HasDetallesDesignacionAsync(id);
            if (tieneDetalles)
                throw new ConflictException("No se puede eliminar la asignatura porque tiene designaciones asociadas.");
            _asignaturaRepository.Delete(asignatura);
            try
            {
                await _asignaturaRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar la asignatura porque tiene designaciones asociadas.");
            }
        }
    }
}