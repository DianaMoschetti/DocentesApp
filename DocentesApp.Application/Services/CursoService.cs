using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Cursos;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IMapper _mapper;

        public CursoService(ICursoRepository cursoRepository, IMapper mapper)
        {
            _cursoRepository = cursoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListCursoDto>> GetAllAsync()
        {
            var cursos = await _cursoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListCursoDto>>(cursos);
        }

        public async Task<CursoDto> GetByIdAsync(int id)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);

            if (curso == null)
                throw new NotFoundException($"No se encontró el curso con ID {id}.");

            return _mapper.Map<CursoDto>(curso);
        }

        public async Task<CursoDto> CreateAsync(CreateCursoDto dto)
        {
            var existe = await _cursoRepository.ExistsAsync(
                (Turno)dto.Turno, (Nivel)dto.Año, (Especialidad)dto.Carrera, dto.NroComision);

            if (existe)
                throw new BadRequestException("Ya existe un curso con esa combinación de turno, año, carrera y comisión.");

            var curso = _mapper.Map<Curso>(dto);

            await _cursoRepository.AddAsync(curso);
            await _cursoRepository.SaveChangesAsync();

            return _mapper.Map<CursoDto>(curso);
        }

        public async Task UpdateAsync(int id, UpdateCursoDto dto)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);

            if (curso == null)
                throw new NotFoundException($"No se encontró el curso con ID {id}.");

            var existe = await _cursoRepository.ExistsAsync(
                (Turno)dto.Turno, (Nivel)dto.Año, (Especialidad)dto.Carrera, dto.NroComision, excludeId: id);

            if (existe)
                throw new BadRequestException("Ya existe un curso con esa combinación de turno, año, carrera y comisión.");

            _mapper.Map(dto, curso);

            _cursoRepository.Update(curso);
            await _cursoRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);

            if (curso == null)
                throw new NotFoundException($"No se encontró el curso con ID {id}.");

            var tieneDependencias = await _cursoRepository.HasDependenciasAsync(id);

            if (tieneDependencias)
                throw new ConflictException("No se puede eliminar el curso porque tiene asignaturas o designaciones asociadas.");

            _cursoRepository.Delete(curso);
            try
            {
                await _cursoRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar el curso porque tiene asignaturas o designaciones asociadas.");
            }
        }
    }
}
