using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Application.Reportes;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Services
{
    public class DocentesPorCursoService : IDocentesPorCursoService
    {
        private readonly IDocentesPorCursoRepository _repository;
        private readonly IAsignaturaRepository _asignaturaRepository;

        public DocentesPorCursoService(IDocentesPorCursoRepository repository, IAsignaturaRepository asignaturaRepository)
        {
            _repository = repository;
            _asignaturaRepository = asignaturaRepository;
        }

        public async Task<IEnumerable<CursoDocentesDto>> GetPorCursoAsync(Especialidad? carrera, Nivel? año, int? cursoId)
        {
            var conCurso = (await _repository.GetConCursoAsync(carrera, año, cursoId, null)).ToList();
            var catedra = await _repository.GetCatedraAsync(conCurso.Select(r => r.AsignaturaId).Distinct());

            return DocentesPorCursoArmador.PorCurso(conCurso, catedra);
        }

        public async Task<AsignaturaDocentesDto> GetPorAsignaturaAsync(int asignaturaId)
        {
            var asignatura = await _asignaturaRepository.GetByIdAsync(asignaturaId);
            if (asignatura == null)
                throw new NotFoundException($"No se encontró la asignatura con ID {asignaturaId}.");

            var conCurso = await _repository.GetConCursoAsync(null, null, null, asignaturaId);
            var catedra = await _repository.GetCatedraAsync(new[] { asignaturaId });

            return DocentesPorCursoArmador.PorAsignatura(asignatura.Id, asignatura.Nombre, asignatura.Udb.Nombre, conCurso, catedra);
        }
    }
}
