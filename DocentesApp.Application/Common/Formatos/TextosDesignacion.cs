using System.Globalization;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;

namespace DocentesApp.Application.Common.Formatos
{
    // Textos y reglas comunes de designaciones, para que DTOs, selector "Reemplaza a" y reportes muestren lo mismo.
    public static class TextosDesignacion
    {
        private static readonly CultureInfo EsAr = CultureInfo.GetCultureInfo("es-AR");

        /// <summary>
        /// "1 DS", "1,5 DS", "1 DTP", "0,5 DE".
        /// </summary>
        public static string Dedicacion(decimal cantidad, TipoDedicacion tipo) =>
            $"{cantidad.ToString("0.##", EsAr)} {tipo.NombreCorto()}";

        /// <summary>
        /// "PAd 1 DE".
        /// </summary>
        public static string CargoCorto(CargoDocente cargo, decimal cantidad, TipoDedicacion tipo) =>
            $"{cargo.NombreCorto()} {Dedicacion(cantidad, tipo)}";

        /// <summary>
        /// "Perez, Juan – PAd 1 DE" (para el selector y la descripción del cargo reemplazado).
        /// </summary>
        public static string CargoConDocente(string apellido, string nombre, CargoDocente cargo, decimal cantidad, TipoDedicacion tipo) =>
            $"{apellido}, {nombre} – {CargoCorto(cargo, cantidad, tipo)}";

        /// <summary>
        /// Descripción del cargo reemplazado, o null si no reemplaza a nadie o no se cargaron las navegaciones.
        /// </summary>
        public static string? Reemplazo(CargoDesignacion cargo)
        {
            var reemplazado = cargo.ReemplazaACargo;
            var docente = reemplazado?.Designacion?.Docente;
            if (reemplazado == null || docente == null)
                return null;

            return CargoConDocente(docente.Apellido, docente.Nombre, reemplazado.Cargo, reemplazado.CantidadDedicacion, reemplazado.TipoDedicacion);
        }

        /// <summary>
        /// Formato actual del curso (el mismo que usa Mapster para CursoDto.Descripcion).
        /// </summary>
        public static string Curso(Nivel año, Especialidad carrera, int nroComision, Turno turno) =>
            $"{año} - {carrera} - Comisión {nroComision} - {turno}";

        /// <summary>
        /// Columna A/C del reporte: A = anual, C = cuatrimestral; el resto con su nombre.
        /// </summary>
        public static string AnualCuatrimestral(Frecuencia frecuencia) => frecuencia switch
        {
            Frecuencia.Anual => "A",
            Frecuencia.ElectivaAnual => "A",
            Frecuencia.Cuatrimestral => "C",
            Frecuencia.ElectivaCuatrimestral => "C",
            _ => frecuencia.Descripcion()
        };

        /// <summary>
        /// Vigente = Activa y sin FechaFin o con FechaFin futura.
        /// </summary>
        public static bool EsVigente(Designacion designacion, DateTime hoy) =>
            designacion.EstadoDesignacion == Estado.Activa
            && (designacion.FechaFin == null || designacion.FechaFin > hoy);
    }
}
