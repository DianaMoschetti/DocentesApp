using DocentesApp.Application.Common.Formatos;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Extensions;
using DocentesApp.Shared.DTOs.Asignaturas;
using DocentesApp.Shared.DTOs.Cargos;
using DocentesApp.Shared.DTOs.Cursos;
using DocentesApp.Shared.DTOs.Dedicaciones;
using DocentesApp.Shared.DTOs.Designaciones;
using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Shared.DTOs.PuntosPorCargo;
using DocentesApp.Shared.DTOs.Snapshots;
using DocentesApp.Shared.DTOs.Udbs;
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

            #region Asignatura
            config.NewConfig<CreateAsignaturaDto, Asignatura>();
            config.NewConfig<UpdateAsignaturaDto, Asignatura>()
                .Ignore(dest => dest.Id);

            config.NewConfig<Asignatura, AsignaturaDto>()
                .Map(dest => dest.Nombre, src => src.Nombre)
                .Map(dest => dest.FrecuenciaTexto, src => src.Frecuencia.ToString())
                .Map(dest => dest.NivelTexto, src => src.Nivel.ToString())
                .Map(dest => dest.NombreUdb, src => src.Udb != null ? src.Udb.Nombre : null);

            config.NewConfig<Asignatura, ListAsignaturaDto>()
                .Map(dest => dest.UdbId, src => src.UdbId)
                .Map(dest => dest.Nombre, src => src.Nombre)
                .Map(dest => dest.FrecuenciaTexto, src => src.Frecuencia.ToString())
                .Map(dest => dest.NivelTexto, src => src.Nivel.ToString())
                .Map(dest => dest.NombreUdb, src => src.Udb != null ? src.Udb.Nombre : null);
            #endregion

            #region Curso
            config.NewConfig<CreateCursoDto, Curso>();
            config.NewConfig<UpdateCursoDto, Curso>()
                .Ignore(dest => dest.Id);

            config.NewConfig<Curso, CursoDto>()
                .Map(dest => dest.TurnoTexto, src => src.Turno.ToString())
                .Map(dest => dest.CarreraTexto, src => src.Carrera.ToString())
                .Map(dest => dest.Descripcion, src => $"{src.Año} - {src.Carrera} - Comisión {src.NroComision} - {src.Turno}");

            config.NewConfig<Curso, ListCursoDto>()
                .Map(dest => dest.Descripcion, src => $"{src.Año} - {src.Carrera} - Comisión {src.NroComision} - {src.Turno}");
            #endregion

            #region Designacion
            // Mapster arma solo la cabecera: los cargos los arma DesignacionService (calcula los puntos y sincroniza por Id)
            config.NewConfig<CreateDesignacionDto, Designacion>()
                .Ignore(dest => dest.Cargos);

            config.NewConfig<UpdateDesignacionDto, Designacion>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.DocenteId)
                .Ignore(dest => dest.Cargos);

            // Lectura v5: Designacion -> Cargos -> Actividades
            config.NewConfig<Actividad, ActividadDto>()
                .Map(dest => dest.NombreAsignatura, src => src.Asignatura != null
                    ? src.Asignatura.Nombre : null)
                .Map(dest => dest.DescripcionCurso, src => src.Curso != null
                    ? TextosDesignacion.Curso(src.Curso.Año, src.Curso.Carrera, src.Curso.NroComision, src.Curso.Turno) : null);

            config.NewConfig<CargoDesignacion, CargoDesignacionDto>()
                .Map(dest => dest.CargoTexto, src => src.Cargo.Descripcion())
                .Map(dest => dest.CargoCorto, src => src.Cargo.NombreCorto())
                .Map(dest => dest.CondicionCorta, src => src.Condicion.NombreCorto())
                .Map(dest => dest.DedicacionTexto, src => TextosDesignacion.Dedicacion(src.CantidadDedicacion, src.TipoDedicacion))
                .Map(dest => dest.NombreUdb, src => src.Udb != null ? src.Udb.Nombre : null)
                .Map(dest => dest.ReemplazaADescripcion, src => TextosDesignacion.Reemplazo(src))
                .Map(dest => dest.Actividades, src => src.Actividades);

            config.NewConfig<Designacion, ListDesignacionDto>()
                .Map(dest => dest.NombreCompletoDocente, src => src.Docente != null
                    ? $"{src.Docente.Apellido}, {src.Docente.Nombre}" : string.Empty)
                .Map(dest => dest.TotalPuntos, src => src.Cargos.Sum(c => c.Puntos));

            config.NewConfig<Designacion, DesignacionDto>()
                .Map(dest => dest.NombreCompletoDocente, src => src.Docente != null
                    ? $"{src.Docente.Apellido}, {src.Docente.Nombre}" : string.Empty)
                .Map(dest => dest.Cargos, src => src.Cargos)
                .Map(dest => dest.TotalPuntos, src => src.Cargos.Sum(c => c.Puntos));
            #endregion

            #region Docente
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
                 .Map(dest => dest.Nombre, src => src.Nombre)
                 .Map(dest => dest.Apellido, src => src.Apellido)
                 .Map(dest => dest.NombreCompleto, src => $"{src.Apellido}, {src.Nombre}")
                 .Map(dest => dest.FechaNacimiento, src => src.FechaNacimiento)
                 .Map(dest => dest.Email, src => src.Email)
                 .Map(dest => dest.EmailAlternativo, src => src.EmailAlternativo)
                 .Map(dest => dest.Celular, src => src.Celular)
                 .Map(dest => dest.Direccion, src => src.Direccion)
                 .Map(dest => dest.MaxNivelAcademico, src => src.MaxNivelAcademico)
                 .Map(dest => dest.Observaciones, src => src.Observaciones);

            config.NewConfig<Docente, ListDocenteDto>()
                .Map(dest => dest.Nombre, src => src.Nombre)
                .Map(dest => dest.Apellido, src => src.Apellido)
                .Map(dest => dest.NombreCompleto, src => $"{src.Apellido}, {src.Nombre}");
            #endregion

            #region Planta
            config.NewConfig<CreatePlantaSnapshotDto, PlantaSnapshot>();
            config.NewConfig<CreatePlantaDocenteSnapshotDto, PlantaDocenteSnapshot>();
            config.NewConfig<PlantaSnapshot, PlantaSnapshotDto>()
                .Ignore(dest => dest.Filas);
            config.NewConfig<PlantaDocenteSnapshot, PlantaDocenteSnapshotDto>();
            #endregion

            #region Udb
            config.NewConfig<CreateUdbDto, Udb>();
            config.NewConfig<UpdateUdbDto, Udb>();

            config.NewConfig<Udb, UdbDto>()
                .Map(dest => dest.DirectorNombreCompleto, src => src.Director != null ? $"{src.Director.Apellido}, {src.Director.Nombre}" : null)
                .Map(dest => dest.SecretarioNombreCompleto, src => src.Secretario != null ? $"{src.Secretario.Apellido}, {src.Secretario.Nombre}" : null);

            config.NewConfig<Udb, ListUdbDto>();
            #endregion

            #region Cargo
            #endregion

            #region Dedicacion
            #endregion

            #region PuntosPorCargo
            config.NewConfig<CreatePuntosPorCargoDto, PuntosPorCargo>();

            config.NewConfig<PuntosPorCargo, PuntosPorCargoDto>()
                .Map(dest => dest.Descripcion, src => src.Cargo.Descripcion()); // "Profesor Adjunto"
            #endregion
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