using DocentesApp.Application.DTOs.Asignaturas;
using DocentesApp.Application.DTOs.Cursos;
using DocentesApp.Application.DTOs.Designaciones;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.DTOs.Snapshots;
using DocentesApp.Application.DTOs.Udbs;
using DocentesApp.Domain.Entities;
using Mapster;
using System.Globalization;

namespace DocentesApp.Application.Mappings
{
    public static class MapsterConfig
    {
        // https://medium.com/codenx/mapster-high-performance-mapper-for-net-767a3f361043
        public static void Register(TypeAdapterConfig config)
        {
            config.NewConfig<string?, DateOnly?>()
                .MapWith(src => ParseDateOnly(src));

            config.NewConfig<CreateAsignaturaDto, Asignatura>();
            config.NewConfig<UpdateAsignaturaDto, Asignatura>();

            config.NewConfig<Asignatura, AsignaturaDto>()
                .Map(dest => dest.NombreAsignaturaTexto, src => src.NombreAsignatura.ToString())
                .Map(dest => dest.FrecuenciaTexto, src => src.Frecuencia.ToString())
                .Map(dest => dest.NivelTexto, src => src.Nivel.ToString())
                .Map(dest => dest.NombreUdb, src => src.Udb != null ? src.Udb.Nombre : null);

            config.NewConfig<Asignatura, ListAsignaturaDto>()
                .Map(dest => dest.NombreAsignaturaTexto, src => src.NombreAsignatura.ToString())
                .Map(dest => dest.FrecuenciaTexto, src => src.Frecuencia.ToString())
                .Map(dest => dest.NivelTexto, src => src.Nivel.ToString())
                .Map(dest => dest.NombreUdb, src => src.Udb != null ? src.Udb.Nombre : null);

            config.NewConfig<CreateCursoDto, Curso>();
            config.NewConfig<UpdateCursoDto, Curso>();

            config.NewConfig<Curso, CursoDto>()
                .Map(dest => dest.TurnoTexto, src => src.Turno.ToString())
                .Map(dest => dest.CarreraTexto, src => src.Carrera.ToString())
                .Map(dest => dest.Descripcion, src => $"{src.Año} - {src.Carrera} - Comisión {src.NroComision} - {src.Turno}");

            config.NewConfig<Curso, ListCursoDto>()
                .Map(dest => dest.Descripcion, src => $"{src.Año} - {src.Carrera} - Comisión {src.NroComision} - {src.Turno}");

            config.NewConfig<CreateDesignacionDto, Designacion>();
            config.NewConfig<UpdateDesignacionDto, Designacion>();

            config.NewConfig<Designacion, ListDesignacionDto>()
                .Map(dest => dest.NombreCompletoDocente, src => src.Docente != null ? $"{src.Docente.Apellido}, {src.Docente.Nombre}" : string.Empty)
                .Map(dest => dest.DescripcionCargo, src => src.Cargo != null ? src.Cargo.Denominacion.ToString() : string.Empty)
                .Map(dest => dest.DescripcionDedicacion, src => src.Dedicacion != null ? src.Dedicacion.DescTipo.ToString() : string.Empty)
                .Map(dest => dest.NombreAsignatura, src => src.Asignatura != null ? src.Asignatura.NombreAsignatura.ToString() : null)
                .Map(dest => dest.DescripcionCurso, src => src.Curso != null ? $"{src.Curso.Año} - {src.Curso.Carrera} - Comisión {src.Curso.NroComision} - {src.Curso.Turno}" : null);

            config.NewConfig<Designacion, DesignacionDto>()
                .Map(dest => dest.NombreCompletoDocente, src => src.Docente != null ? $"{src.Docente.Apellido}, {src.Docente.Nombre}" : string.Empty)
                .Map(dest => dest.DescripcionCargo, src => src.Cargo != null ? src.Cargo.Denominacion.ToString() : string.Empty)
                .Map(dest => dest.DescripcionDedicacion, src => src.Dedicacion != null ? src.Dedicacion.DescTipo.ToString() : string.Empty)
                .Map(dest => dest.NombreAsignatura, src => src.Asignatura != null ? src.Asignatura.NombreAsignatura.ToString() : null)
                .Map(dest => dest.DescripcionCurso, src => src.Curso != null ? $"{src.Curso.Año} - {src.Curso.Carrera} - Comisión {src.Curso.NroComision} - {src.Curso.Turno}" : null);

            config.NewConfig<CreateDocenteDto, Docente>();
            config.NewConfig<UpdateDocenteDto, Docente>();

            config.NewConfig<UpdateAcademicoDocenteDto, Docente>()
                .IgnoreNullValues(true);

            config.NewConfig<UpdateContactoDocenteDto, Docente>()
                .IgnoreNullValues(true);

            config.NewConfig<UpdateObservacionesDocenteDto, Docente>()
                .IgnoreNullValues(true);

            config.NewConfig<Docente, DocenteConDesignacionesDto>()
                .Map(dest => dest.NombreCompleto, src => $"{src.Apellido}, {src.Nombre}");

            config.NewConfig<Docente, DocenteDto>()
                 .Map(dest => dest.NombreCompleto, src => $"{src.Apellido}, {src.Nombre}")
                 .Map(dest => dest.Email, src => src.Email)
                 .Map(dest => dest.EmailAlternativo, src => src.EmailAlternativo)
                 .Map(dest => dest.Celular, src => src.Celular)
                 .Map(dest => dest.Direccion, src => src.Direccion)
                 .Map(dest => dest.MaxNivelAcademico, src => src.MaxNivelAcademico)
                 .Map(dest => dest.Observaciones, src => src.Observaciones);

            config.NewConfig<Docente, ListDocenteDto>()
                .Map(dest => dest.NombreCompleto, src => $"{src.Apellido}, {src.Nombre}");

            config.NewConfig<CreatePlantaSnapshotDto, PlantaSnapshot>();
            config.NewConfig<CreatePlantaDocenteSnapshotDto, PlantaDocenteSnapshot>();
            config.NewConfig<PlantaSnapshot, PlantaSnapshotDto>()
                .Ignore(dest => dest.Filas);
            config.NewConfig<PlantaDocenteSnapshot, PlantaDocenteSnapshotDto>();

            config.NewConfig<CreateUdbDto, Udb>();
            config.NewConfig<UpdateUdbDto, Udb>();

            config.NewConfig<Udb, UdbDto>()
                .Map(dest => dest.DirectorNombreCompleto, src => src.Director != null ? $"{src.Director.Apellido}, {src.Director.Nombre}" : null)
                .Map(dest => dest.SecretarioNombreCompleto, src => src.Secretario != null ? $"{src.Secretario.Apellido}, {src.Secretario.Nombre}" : null);

            config.NewConfig<Udb, ListUdbDto>();
        }

        private static DateOnly? ParseDateOnly(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            return null;
        }
    }
}
