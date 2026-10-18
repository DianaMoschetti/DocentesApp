using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Reportes;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Reportes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DocentesApp.Data.Repositories
{
    // Este repositorio usa ADO.NET.
    // ADO.NET es la capa de acceso a datos de bajo nivel de .NET.
    // EF Core internamente usa ADO.NET 
    public class PlantaReporteRepository : IPlantaReporteRepository
    {
        private readonly string _connectionString;

        public PlantaReporteRepository(IConfiguration configuration)
        {
            // desde appsettings.json
            _connectionString = configuration
                .GetConnectionString("DocentesAppDbConnection")!;
        }

        // Consulta v5: una fila por Actividad de cada CargoDesignacion vigente.
        // Vigente = designación Activa y sin FechaFin o con FechaFin futura.
        // El filtro de UDB es por el CARGO (c.UdbId), no por la asignatura: así aparecen también
        // las actividades sin asignatura (gestión, investigación, otros).
        // LEFT JOIN Actividades: un cargo sin actividades igual aparece (como "Otro") para no perder sus puntos.
        private const string SqlRegistrosV5 = @"
                SELECT
                    c.Id AS CargoDesignacionId,
                    d.Apellido,
                    d.Nombre,
                    d.Legajo,
                    c.UdbId,
                    u.Nombre AS NombreUdb,
                    c.Cargo,
                    c.Condicion,
                    c.TipoDedicacion,
                    c.CantidadDedicacion,
                    c.Puntos,
                    c.EnLicencia,
                    c.Observaciones AS CargoObservaciones,
                    ISNULL(act.Especificacion, @EspecificacionOtro) AS Especificacion,
                    a.Nombre AS Asignatura,
                    a.Nivel AS AsignaturaNivel,
                    a.Frecuencia AS AsignaturaFrecuencia,
                    cu.[Año] AS CursoAnio,
                    cu.Carrera AS CursoCarrera,
                    cu.NroComision AS CursoNroComision,
                    cu.Turno AS CursoTurno,
                    act.Horario,
                    act.Observaciones AS ActividadObservaciones
                FROM Designaciones des
                JOIN Docentes d ON des.DocenteId = d.Id
                JOIN CargosDesignacion c ON c.DesignacionId = des.Id
                JOIN Udbs u ON c.UdbId = u.Id
                LEFT JOIN Actividades act ON act.CargoDesignacionId = c.Id
                LEFT JOIN Asignaturas a ON act.AsignaturaId = a.Id
                LEFT JOIN Cursos cu ON act.CursoId = cu.Id
                WHERE des.EstadoDesignacion = @EstadoActiva
                AND (des.FechaFin IS NULL OR des.FechaFin > @Hoy)";

        public async Task<IEnumerable<PlantaUdbRegistro>> GetRegistrosByUdbAsync(int udbId)
        {
            // El orden lo define PlantaUdbArmador según la vista (por asignatura / por docente)
            return await LeerRegistrosAsync(SqlRegistrosV5 + " AND c.UdbId = @UdbId", "@UdbId", udbId);
        }

        public async Task<IEnumerable<PlantaUdbRegistro>> GetRegistrosByDocenteAsync(int docenteId)
        {
            return await LeerRegistrosAsync(SqlRegistrosV5 + " AND d.Id = @DocenteId", "@DocenteId", docenteId);
        }

        private async Task<List<PlantaUdbRegistro>> LeerRegistrosAsync(string sql, string nombreParametro, int valorParametro)
        {
            var resultados = new List<PlantaUdbRegistro>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue(nombreParametro, valorParametro);
            command.Parameters.AddWithValue("@EstadoActiva", (int)Estado.Activa);
            command.Parameters.AddWithValue("@EspecificacionOtro", (int)EspecificacionCargo.Otro);
            command.Parameters.AddWithValue("@Hoy", DateTime.Now);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultados.Add(new PlantaUdbRegistro
                {
                    CargoDesignacionId = reader.GetInt32(reader.GetOrdinal("CargoDesignacionId")),
                    DocenteApellido = reader.GetString(reader.GetOrdinal("Apellido")),
                    DocenteNombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Legajo = LeerIntNullable(reader, "Legajo"),
                    UdbId = reader.GetInt32(reader.GetOrdinal("UdbId")),
                    NombreUdb = reader.GetString(reader.GetOrdinal("NombreUdb")),
                    // Los enums están guardados como int en la BD
                    Cargo = (CargoDocente)reader.GetInt32(reader.GetOrdinal("Cargo")),
                    Condicion = (Condicion)reader.GetInt32(reader.GetOrdinal("Condicion")),
                    TipoDedicacion = (TipoDedicacion)reader.GetInt32(reader.GetOrdinal("TipoDedicacion")),
                    CantidadDedicacion = reader.GetDecimal(reader.GetOrdinal("CantidadDedicacion")),
                    Puntos = reader.GetDecimal(reader.GetOrdinal("Puntos")),
                    EnLicencia = reader.GetBoolean(reader.GetOrdinal("EnLicencia")),
                    CargoObservaciones = LeerStringNullable(reader, "CargoObservaciones"),
                    Especificacion = (EspecificacionCargo)reader.GetInt32(reader.GetOrdinal("Especificacion")),
                    Asignatura = LeerStringNullable(reader, "Asignatura"),
                    AsignaturaNivel = (Nivel?)LeerIntNullable(reader, "AsignaturaNivel"),
                    AsignaturaFrecuencia = (Frecuencia?)LeerIntNullable(reader, "AsignaturaFrecuencia"),
                    CursoAño = (Nivel?)LeerIntNullable(reader, "CursoAnio"),
                    CursoCarrera = (Especialidad?)LeerIntNullable(reader, "CursoCarrera"),
                    CursoNroComision = LeerIntNullable(reader, "CursoNroComision"),
                    CursoTurno = (Turno?)LeerIntNullable(reader, "CursoTurno"),
                    Horario = LeerStringNullable(reader, "Horario"),
                    ActividadObservaciones = LeerStringNullable(reader, "ActividadObservaciones")
                });
            }

            return resultados;
        }

        private static int? LeerIntNullable(SqlDataReader reader, string columna)
        {
            var ordinal = reader.GetOrdinal(columna);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private static string? LeerStringNullable(SqlDataReader reader, string columna)
        {
            var ordinal = reader.GetOrdinal(columna);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}