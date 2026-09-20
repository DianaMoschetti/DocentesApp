using DocentesApp.Application.Interfaces.Repositories;
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

        public async Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByUdbAsync(int udbId)
        {
            var resultados = new List<PlantaDocenteReporteDto>();

            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    d.Apellido + ', ' + d.Nombre AS NombreCompleto,
                    d.Legajo,
                    det.Denominacion,
                    det.TipoCargo,
                    det.Condicion,
                    det.TipoDedicacion,
                    det.CantidadDedicacion,
                    det.Especificacion,
                    a.Nombre AS Asignatura,
                    det.PuntosAsignados,
                    des.NroResolucion
                FROM Designaciones des
                JOIN Docentes d ON des.DocenteId = d.Id
                JOIN DetallesDesignacion det ON det.DesignacionId = des.Id
                LEFT JOIN Asignaturas a ON det.AsignaturaId = a.Id
                WHERE (des.FechaFin IS NULL OR des.FechaFin > GETDATE())
                AND a.UdbId = @UdbId
                ORDER BY d.Apellido, d.Nombre";

            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@UdbId", udbId);

            await connection.OpenAsync();

            // SqlDataReader lee los resultados fila por fila es más eficiente que cargar todo en memoria de una
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultados.Add(new PlantaDocenteReporteDto
                {
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                    Legajo = reader.GetInt32(reader.GetOrdinal("Legajo")),
                    Denominacion = reader.GetInt32(reader.GetOrdinal("Denominacion")).ToString(),
                    TipoCargo = reader.GetInt32(reader.GetOrdinal("TipoCargo")).ToString(),
                    Condicion = reader.GetInt32(reader.GetOrdinal("Condicion")).ToString(),
                    TipoDedicacion = reader.GetInt32(reader.GetOrdinal("TipoDedicacion")).ToString(),
                    CantidadDedicacion = reader.GetFloat(reader.GetOrdinal("CantidadDedicacion")),
                    Especificacion = reader.GetInt32(reader.GetOrdinal("Especificacion")).ToString(),
                    Asignatura = reader.IsDBNull(reader.GetOrdinal("Asignatura"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Asignatura")),
                    PuntosAsignados = reader.GetDecimal(reader.GetOrdinal("PuntosAsignados")),
                    NroResolucion = reader.IsDBNull(reader.GetOrdinal("NroResolucion"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("NroResolucion"))
                });
            }

            return resultados;
        }

        public async Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByDocenteAsync(int docenteId)
        {
            var resultados = new List<PlantaDocenteReporteDto>();

            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    d.Apellido + ', ' + d.Nombre AS NombreCompleto,
                    d.Legajo,
                    det.Denominacion,
                    det.TipoCargo,
                    det.Condicion,
                    det.TipoDedicacion,
                    det.CantidadDedicacion,
                    det.Especificacion,
                    a.Nombre AS Asignatura,
                    det.PuntosAsignados,
                    des.NroResolucion
                FROM Designaciones des
                JOIN Docentes d ON des.DocenteId = d.Id
                JOIN DetallesDesignacion det ON det.DesignacionId = des.Id
                LEFT JOIN Asignaturas a ON det.AsignaturaId = a.Id
                WHERE (des.FechaFin IS NULL OR des.FechaFin > GETDATE())
                AND d.Id = @DocenteId
                ORDER BY des.FechaInicio DESC";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DocenteId", docenteId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultados.Add(new PlantaDocenteReporteDto
                {
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                    Legajo = reader.GetInt32(reader.GetOrdinal("Legajo")),
                    Denominacion = reader.GetInt32(reader.GetOrdinal("Denominacion")).ToString(),
                    TipoCargo = reader.GetInt32(reader.GetOrdinal("TipoCargo")).ToString(),
                    Condicion = reader.GetInt32(reader.GetOrdinal("Condicion")).ToString(),
                    TipoDedicacion = reader.GetInt32(reader.GetOrdinal("TipoDedicacion")).ToString(),
                    CantidadDedicacion = reader.GetFloat(reader.GetOrdinal("CantidadDedicacion")),
                    Especificacion = reader.GetInt32(reader.GetOrdinal("Especificacion")).ToString(),
                    Asignatura = reader.IsDBNull(reader.GetOrdinal("Asignatura"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Asignatura")),
                    PuntosAsignados = reader.GetDecimal(reader.GetOrdinal("PuntosAsignados")),
                    NroResolucion = reader.IsDBNull(reader.GetOrdinal("NroResolucion"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("NroResolucion"))
                });
            }

            return resultados;
        }
    }
}