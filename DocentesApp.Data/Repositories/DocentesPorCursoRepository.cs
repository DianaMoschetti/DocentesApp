using System.Linq.Expressions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Reportes;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    // Vista por curso con EF (a diferencia de PlantaReporteRepository, que usa ADO.NET): una fila por Actividad.
    public class DocentesPorCursoRepository : IDocentesPorCursoRepository
    {
        private readonly DocentesDbContext _context;

        public DocentesPorCursoRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocenteCursoRegistro>> GetConCursoAsync(Especialidad? carrera, Nivel? año, int? cursoId, int? asignaturaId)
        {
            var query = ActividadesVigentesConAsignatura().Where(a => a.CursoId != null);

            if (carrera.HasValue)
                query = query.Where(a => a.Curso!.Carrera == carrera.Value);
            if (año.HasValue)
                query = query.Where(a => a.Curso!.Año == año.Value);
            if (cursoId.HasValue)
                query = query.Where(a => a.CursoId == cursoId.Value);
            if (asignaturaId.HasValue)
                query = query.Where(a => a.AsignaturaId == asignaturaId.Value);

            return await query.Select(Proyeccion).ToListAsync();
        }

        public async Task<IEnumerable<DocenteCursoRegistro>> GetCatedraAsync(IEnumerable<int> asignaturaIds)
        {
            var ids = asignaturaIds.ToList();
            if (ids.Count == 0)
                return new List<DocenteCursoRegistro>();

            return await ActividadesVigentesConAsignatura()
                .Where(a => a.CursoId == null
                    && (a.Especificacion == EspecificacionCargo.JefeDeCatedra || a.Especificacion == EspecificacionCargo.Laboratorio)
                    && ids.Contains(a.AsignaturaId!.Value))
                .Select(Proyeccion)
                .ToListAsync();
        }

        private IQueryable<Actividad> ActividadesVigentesConAsignatura()
        {
            var hoy = DateTime.Now;
            return _context.Actividades
                .AsNoTracking()
                // vigente = Activa y sin FechaFin o con FechaFin futura
                .Where(a => a.AsignaturaId != null
                    && a.CargoDesignacion.Designacion.EstadoDesignacion == Estado.Activa
                    && (a.CargoDesignacion.Designacion.FechaFin == null || a.CargoDesignacion.Designacion.FechaFin > hoy));
        }

        private static readonly Expression<Func<Actividad, DocenteCursoRegistro>> Proyeccion = a => new DocenteCursoRegistro
        {
            CargoDesignacionId = a.CargoDesignacionId,
            DocenteId = a.CargoDesignacion.Designacion.DocenteId,
            DocenteApellido = a.CargoDesignacion.Designacion.Docente.Apellido,
            DocenteNombre = a.CargoDesignacion.Designacion.Docente.Nombre,
            Cargo = a.CargoDesignacion.Cargo,
            Condicion = a.CargoDesignacion.Condicion,
            EnLicencia = a.CargoDesignacion.EnLicencia,
            ReemplazaApellido = a.CargoDesignacion.ReemplazaACargo != null ? a.CargoDesignacion.ReemplazaACargo.Designacion.Docente.Apellido : null,
            ReemplazaNombre = a.CargoDesignacion.ReemplazaACargo != null ? a.CargoDesignacion.ReemplazaACargo.Designacion.Docente.Nombre : null,
            ReemplazaCargo = a.CargoDesignacion.ReemplazaACargo != null ? a.CargoDesignacion.ReemplazaACargo.Cargo : null,
            ReemplazaCantidadDedicacion = a.CargoDesignacion.ReemplazaACargo != null ? a.CargoDesignacion.ReemplazaACargo.CantidadDedicacion : null,
            ReemplazaTipoDedicacion = a.CargoDesignacion.ReemplazaACargo != null ? a.CargoDesignacion.ReemplazaACargo.TipoDedicacion : null,
            Especificacion = a.Especificacion,
            Horario = a.Horario,
            AsignaturaId = a.AsignaturaId!.Value,
            AsignaturaNombre = a.Asignatura!.Nombre,
            CursoId = a.CursoId,
            CursoAño = a.Curso != null ? a.Curso.Año : null,
            CursoCarrera = a.Curso != null ? a.Curso.Carrera : null,
            CursoNroComision = a.Curso != null ? a.Curso.NroComision : null,
            CursoTurno = a.Curso != null ? a.Curso.Turno : null
        };
    }
}
