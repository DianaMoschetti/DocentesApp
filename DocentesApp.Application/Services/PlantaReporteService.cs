using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
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

        public async Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByUdbAsync(int udbId)
        {
            var udb = await _udbRepository.GetByIdAsync(udbId);
            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {udbId}.");

            var resultados = await _reporteRepository.GetPlantaByUdbAsync(udbId);

            // Los enums vienen como int desde ADO.NET — los traducimos a texto legible
            return resultados.Select(TraducirEnums);
        }

        public async Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByDocenteAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {docenteId}.");

            var resultados = await _reporteRepository.GetPlantaByDocenteAsync(docenteId);

            return resultados.Select(TraducirEnums);
        }

        // Traduce los valores int de los enums a texto legible en castellano.
        // Necesario porque ADO.NET devuelve los enums como int (así están en la BD)
        // y el reporte necesita mostrar texto, no números.
        private static PlantaDocenteReporteDto TraducirEnums(PlantaDocenteReporteDto dto)
        {
            if (int.TryParse(dto.Denominacion, out var den))
                dto.Denominacion = ((DenominacionCargo)den) switch
                {
                    DenominacionCargo.Profesor => "Profesor",
                    DenominacionCargo.JefeDeTrabajosPracticos => "J.T.P.",
                    DenominacionCargo.AyudanteDePrimera => "Ayudante 1°",
                    DenominacionCargo.AyudanteDeSegunda => "Ayudante 2°",
                    DenominacionCargo.Becario => "Becario",
                    DenominacionCargo.Adscripto => "Adscripto",
                    DenominacionCargo.Administrativo => "Administrativo",
                    _ => dto.Denominacion
                };

            if (int.TryParse(dto.TipoCargo, out var tipo))
                dto.TipoCargo = ((TipoCargo)tipo) switch
                {
                    TipoCargo.Adjunto => "Adjunto",
                    TipoCargo.Asociado => "Asociado",
                    TipoCargo.Titular => "Titular",
                    _ => dto.TipoCargo
                };

            if (int.TryParse(dto.Condicion, out var cond))
                dto.Condicion = ((Condicion)cond) switch
                {
                    Condicion.Regular => "Regular",
                    Condicion.Interino => "Interino",
                    Condicion.Suplente => "Suplente",
                    Condicion.LicenciaConHaberes => "Lic. c/ Haberes",
                    Condicion.LicenciaSinHaberes => "Lic. s/ Haberes",
                    Condicion.Otros => "Otros",
                    _ => dto.Condicion
                };

            if (int.TryParse(dto.TipoDedicacion, out var ded))
                dto.TipoDedicacion = ((TipoDedicacion)ded) switch
                {
                    TipoDedicacion.Simple => "DS",
                    TipoDedicacion.SemiExclusiva => "DSE",
                    TipoDedicacion.Exclusiva => "DE",
                    _ => dto.TipoDedicacion
                };

            if (int.TryParse(dto.Especificacion, out var esp))
                dto.Especificacion = ((EspecificacionCargo)esp) switch
                {
                    EspecificacionCargo.Docencia => "Docencia",
                    EspecificacionCargo.Gestion => "Gestión",
                    EspecificacionCargo.Investigacion => "Investigación",
                    _ => dto.Especificacion
                };

            return dto;
        }
    }
}