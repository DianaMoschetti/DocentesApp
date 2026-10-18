using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Application.Reportes;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Services
{
    public class PlantaReporteService : IPlantaReporteService
    {
        private readonly IPlantaReporteRepository _reporteRepository;
        private readonly IUdbRepository _udbRepository;
        private readonly IDocenteRepository _docenteRepository;

        public PlantaReporteService(
            IPlantaReporteRepository reporteRepository,
            IUdbRepository udbRepository,
            IDocenteRepository docenteRepository)
        {
            _reporteRepository = reporteRepository;
            _udbRepository = udbRepository;
            _docenteRepository = docenteRepository;
        }

        public async Task<PlantaUdbReporteDto> GetPlantaUdbPorAsignaturaAsync(int udbId)
        {
            var udb = await _udbRepository.GetByIdAsync(udbId);
            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {udbId}.");

            var registros = await _reporteRepository.GetRegistrosByUdbAsync(udbId);

            return PlantaUdbArmador.PorAsignatura(udb.Id, udb.Nombre, registros);
        }

        public async Task<PlantaUdbReporteDto> GetPlantaUdbPorDocenteAsync(int udbId)
        {
            var udb = await _udbRepository.GetByIdAsync(udbId);
            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {udbId}.");

            var registros = await _reporteRepository.GetRegistrosByUdbAsync(udbId);

            return PlantaUdbArmador.PorDocente(udb.Id, udb.Nombre, registros);
        }

        // Planta de un docente: un reporte (vista por docente) por cada UDB donde tiene cargos vigentes
        public async Task<IEnumerable<PlantaUdbReporteDto>> GetPlantaByDocenteAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {docenteId}.");

            var registros = await _reporteRepository.GetRegistrosByDocenteAsync(docenteId);

            return registros
                .GroupBy(r => new { r.UdbId, r.NombreUdb })
                .OrderBy(g => g.Key.NombreUdb)
                .Select(g => PlantaUdbArmador.PorDocente(g.Key.UdbId, g.Key.NombreUdb, g))
                .ToList();
        }
    }
}