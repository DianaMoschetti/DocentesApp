using DocentesApp.Application.Common.Formatos;
using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Reportes
{
    // Arma las dos vistas del reporte "Planta docente por UDB" a partir de las filas crudas (una por actividad).
    // Funciones puras (sin BD) para poder testearlas.
    //
    // Puntos: pertenecen al CARGO. Se muestran solo en la primera fila de cada cargo y 0 en las demás
    // (así lo hace el reporte institucional para no contar dos veces); TotalPuntos suma cada cargo una vez.
    public static class PlantaUdbArmador
    {
        public const string SeccionGestion = "Gestión de la UDB";
        public const string SeccionLaboratoriosInvestigacion = "Laboratorios e Investigación";
        public const string SeccionOtros = "Otros";

        /// <summary>
        /// Vista por asignatura: (1) gestión (Gestión y Jefe de Cátedra), (2) laboratorios e investigación,
        /// (3) un bloque por asignatura con las filas ordenadas por curso, (4) "Otros" (sin asignatura).
        /// Los cargos en licencia quedan en su sección, marcados y con sus observaciones.
        /// </summary>
        public static PlantaUdbReporteDto PorAsignatura(int udbId, string nombreUdb, IEnumerable<PlantaUdbRegistro> registros)
        {
            var ordenados = registros
                .OrderBy(r => OrdenSeccion(r))
                .ThenBy(r => OrdenSeccion(r) == 2 ? (int?)r.AsignaturaNivel ?? int.MaxValue : 0)
                .ThenBy(r => OrdenSeccion(r) == 2 ? r.Asignatura : null)
                .ThenBy(r => (int?)r.CursoAño ?? int.MaxValue)
                .ThenBy(r => r.CursoCarrera?.ToString())
                .ThenBy(r => r.CursoNroComision ?? int.MaxValue)
                .ThenBy(r => r.DocenteApellido)
                .ThenBy(r => r.DocenteNombre)
                .ThenBy(r => r.CargoDesignacionId)
                .ToList();

            return Armar(udbId, nombreUdb, ordenados);
        }

        /// <summary>
        /// Vista por docente: una fila por actividad, ordenadas por docente y por cargo.
        /// </summary>
        public static PlantaUdbReporteDto PorDocente(int udbId, string nombreUdb, IEnumerable<PlantaUdbRegistro> registros)
        {
            var ordenados = registros
                .OrderBy(r => r.DocenteApellido)
                .ThenBy(r => r.DocenteNombre)
                .ThenBy(r => r.CargoDesignacionId)
                .ThenBy(r => OrdenSeccion(r))
                .ThenBy(r => r.Asignatura)
                .ThenBy(r => (int?)r.CursoAño ?? int.MaxValue)
                .ThenBy(r => r.CursoNroComision ?? int.MaxValue)
                .ToList();

            return Armar(udbId, nombreUdb, ordenados);
        }

        private static PlantaUdbReporteDto Armar(int udbId, string nombreUdb, List<PlantaUdbRegistro> ordenados)
        {
            var cargosConPuntos = new HashSet<int>();
            var filas = new List<PlantaUdbFilaDto>();

            foreach (var r in ordenados)
            {
                // Primera fila del cargo (en el orden de esta vista): lleva los puntos; las demás 0
                var esPrimeraDelCargo = cargosConPuntos.Add(r.CargoDesignacionId);
                filas.Add(CrearFila(r, esPrimeraDelCargo ? r.Puntos : 0));
            }

            var totalPuntos = ordenados
                .GroupBy(r => r.CargoDesignacionId)
                .Sum(g => g.First().Puntos);

            return new PlantaUdbReporteDto
            {
                UdbId = udbId,
                NombreUdb = nombreUdb,
                Filas = filas,
                TotalPuntos = totalPuntos
            };
        }

        private static PlantaUdbFilaDto CrearFila(PlantaUdbRegistro r, decimal puntos)
        {
            return new PlantaUdbFilaDto
            {
                Seccion = Seccion(r),
                Asignatura = r.Asignatura,
                AC = r.AsignaturaFrecuencia.HasValue ? TextosDesignacion.AnualCuatrimestral(r.AsignaturaFrecuencia.Value) : null,
                Curso = TieneCurso(r)
                    ? TextosDesignacion.Curso(r.CursoAño!.Value, r.CursoCarrera!.Value, r.CursoNroComision!.Value, r.CursoTurno!.Value)
                    : null,
                Carrera = r.CursoCarrera?.ToString(),
                Docente = $"{r.DocenteApellido}, {r.DocenteNombre}",
                Legajo = r.Legajo,
                CargoCorto = r.Cargo.NombreCorto(),
                CondicionCorta = r.Condicion.NombreCorto(),
                Dedicacion = TextosDesignacion.Dedicacion(r.CantidadDedicacion, r.TipoDedicacion),
                Especificacion = r.Especificacion.Descripcion(),
                Horario = r.Horario,
                Observaciones = UnirObservaciones(r.CargoObservaciones, r.ActividadObservaciones),
                EnLicencia = r.EnLicencia,
                CargoDesignacionId = r.CargoDesignacionId,
                Puntos = puntos
            };
        }

        // 0 = gestión, 1 = laboratorios e investigación, 2 = bloque por asignatura, 3 = otros (sin asignatura)
        private static int OrdenSeccion(PlantaUdbRegistro r) => r.Especificacion switch
        {
            EspecificacionCargo.Gestion => 0,
            EspecificacionCargo.JefeDeCatedra => 0,
            EspecificacionCargo.Laboratorio => 1,
            EspecificacionCargo.Investigacion => 1,
            _ => string.IsNullOrWhiteSpace(r.Asignatura) ? 3 : 2
        };

        private static string Seccion(PlantaUdbRegistro r) => OrdenSeccion(r) switch
        {
            0 => SeccionGestion,
            1 => SeccionLaboratoriosInvestigacion,
            2 => r.Asignatura!,
            _ => SeccionOtros
        };

        private static bool TieneCurso(PlantaUdbRegistro r) =>
            r.CursoAño.HasValue && r.CursoCarrera.HasValue && r.CursoNroComision.HasValue && r.CursoTurno.HasValue;

        private static string? UnirObservaciones(string? cargo, string? actividad)
        {
            var partes = new[] { cargo, actividad }.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim()).ToList();
            return partes.Count == 0 ? null : string.Join(" / ", partes);
        }
    }
}
