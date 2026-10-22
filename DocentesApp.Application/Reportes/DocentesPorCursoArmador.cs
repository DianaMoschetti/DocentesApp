using DocentesApp.Application.Common.Formatos;
using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;
using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Reportes
{
    // Arma la vista por curso y la vista por asignatura a partir de las filas crudas (una por actividad).
    // Funciones puras (sin BD) para poder testearlas.
    // Cátedra (Jefe de Cátedra, Laboratorio): no tiene curso, se muestra en cada asignatura.
    // Docentes: por jerarquía del cargo (PT, PAs, PAd, JTP, Ay 1º, Ay 2º), después apellido y nombre.
    public static class DocentesPorCursoArmador
    {
        /// <summary>
        /// Cursos ordenados por carrera, año, comisión y turno; sus asignaturas por nombre.
        /// </summary>
        public static List<CursoDocentesDto> PorCurso(IEnumerable<DocenteCursoRegistro> conCurso, IEnumerable<DocenteCursoRegistro> catedra)
        {
            var catedraPorAsignatura = CatedraPorAsignatura(catedra);

            return conCurso
                .GroupBy(r => r.CursoId!.Value)
                .Select(curso =>
                {
                    var r = curso.First();
                    return new CursoDocentesDto
                    {
                        CursoId = curso.Key,
                        Descripcion = DescripcionCurso(r),
                        Carrera = r.CursoCarrera!.Value,
                        Año = r.CursoAño!.Value,
                        NroComision = r.CursoNroComision!.Value,
                        Turno = r.CursoTurno!.Value,
                        Asignaturas = curso
                            .GroupBy(a => a.AsignaturaId)
                            .Select(asignatura => new AsignaturaEnCursoDto
                            {
                                AsignaturaId = asignatura.Key,
                                Nombre = asignatura.First().AsignaturaNombre,
                                Docentes = Docentes(asignatura),
                                Catedra = catedraPorAsignatura.GetValueOrDefault(asignatura.Key) ?? new()
                            })
                            .OrderBy(a => a.Nombre)
                            .ToList()
                    };
                })
                .OrderBy(c => c.Carrera)
                .ThenBy(c => c.Año)
                .ThenBy(c => c.NroComision)
                .ThenBy(c => c.Turno)
                .ToList();
        }

        /// <summary>
        /// Una asignatura con todos sus cursos (mismo orden que la vista por curso) y su cátedra.
        /// </summary>
        public static AsignaturaDocentesDto PorAsignatura(
            int asignaturaId, string nombre, string nombreUdb,
            IEnumerable<DocenteCursoRegistro> conCurso, IEnumerable<DocenteCursoRegistro> catedra)
        {
            return new AsignaturaDocentesDto
            {
                AsignaturaId = asignaturaId,
                Nombre = nombre,
                NombreUdb = nombreUdb,
                Catedra = Docentes(catedra),
                Cursos = conCurso
                    .GroupBy(r => r.CursoId!.Value)
                    .OrderBy(c => c.First().CursoCarrera)
                    .ThenBy(c => c.First().CursoAño)
                    .ThenBy(c => c.First().CursoNroComision)
                    .ThenBy(c => c.First().CursoTurno)
                    .Select(curso => new CursoConDocentesDto
                    {
                        CursoId = curso.Key,
                        Descripcion = DescripcionCurso(curso.First()),
                        Docentes = Docentes(curso)
                    })
                    .ToList()
            };
        }

        private static Dictionary<int, List<DocenteActividadDto>> CatedraPorAsignatura(IEnumerable<DocenteCursoRegistro> catedra) =>
            catedra
                .GroupBy(r => r.AsignaturaId)
                .ToDictionary(g => g.Key, g => Docentes(g));

        private static List<DocenteActividadDto> Docentes(IEnumerable<DocenteCursoRegistro> registros) =>
            registros
                .OrderBy(r => r.Cargo.OrdenJerarquia())
                .ThenBy(r => r.DocenteApellido)
                .ThenBy(r => r.DocenteNombre)
                .ThenBy(r => r.CargoDesignacionId)
                .Select(CrearDocente)
                .ToList();

        private static DocenteActividadDto CrearDocente(DocenteCursoRegistro r) => new()
        {
            CargoDesignacionId = r.CargoDesignacionId,
            DocenteId = r.DocenteId,
            Apellido = r.DocenteApellido,
            Nombre = r.DocenteNombre,
            Cargo = r.Cargo,
            CargoTexto = r.Cargo.Descripcion(),
            Condicion = r.Condicion,
            CondicionTexto = r.Condicion.Descripcion(),
            Especificacion = r.Especificacion,
            EspecificacionTexto = r.Especificacion.Descripcion(),
            Horario = r.Horario,
            EnLicencia = r.EnLicencia,
            EsSuplente = r.Condicion == Condicion.Suplente,
            ReemplazaA = r.ReemplazaApellido != null && r.ReemplazaCargo.HasValue
                ? TextosDesignacion.CargoConDocente(r.ReemplazaApellido, r.ReemplazaNombre ?? string.Empty,
                    r.ReemplazaCargo.Value, r.ReemplazaCantidadDedicacion ?? 0, r.ReemplazaTipoDedicacion ?? TipoDedicacion.Simple)
                : null
        };

        private static string DescripcionCurso(DocenteCursoRegistro r) =>
            TextosDesignacion.Curso(r.CursoAño!.Value, r.CursoCarrera!.Value, r.CursoNroComision!.Value, r.CursoTurno!.Value);
    }
}
