namespace DocentesApp.Shared
{
    // Hora de Argentina para la auditoría: la base guarda UTC; la API filtra por días de Argentina y Blazor muestra hora de Argentina.
    // [Diana desde v4.0 OBSOLETO] buscaba la zona por id IANA (con fallback a UTC-3); reemplazado por un offset fijo
    //// Usa el id IANA (en Windows .NET lo mapea con ICU); si el sistema no lo tiene, UTC-3 fijo (Argentina no tiene horario de verano).
    // Offset fijo UTC-3: el sistema siempre corre en Argentina, que no tiene horario de verano desde 2009.
    // Si eso cambiara algún día, este es el único lugar a modificar.
    public static class ZonaHorariaArgentina
    {
        // [Diana desde v4.0 OBSOLETO] búsqueda de la zona por id IANA, con fallback a UTC-3 si el sistema no la tenía
        //public const string IdIana = "America/Argentina/Buenos_Aires";
        //
        //public static TimeZoneInfo Zona { get; } = Crear(IdIana);
        //
        //// Público para poder probar el fallback con un id inexistente
        //public static TimeZoneInfo Crear(string idIana)
        //{
        //    try
        //    {
        //        return TimeZoneInfo.FindSystemTimeZoneById(idIana);
        //    }
        //    catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        //    {
        //        return TimeZoneInfo.CreateCustomTimeZone("Argentina UTC-3", TimeSpan.FromHours(-3), "Argentina (UTC-3)", "Argentina (UTC-3)");
        //    }
        //}
        public static readonly TimeSpan Offset = TimeSpan.FromHours(-3);

        // UTC -> hora de Argentina, con Kind Unspecified: viaja en el JSON sin offset ni "Z" ("2026-03-09T23:30:00"),
        // así Newtonsoft en Blazor no la convierte a la hora local del servidor. Blazor la muestra tal cual.
        // [Diana desde v4.0 OBSOLETO] con TimeZoneInfo (podía devolver Kind Local si el servidor estaba en hora de Argentina)
        //public static DateTime DesdeUtc(DateTime fechaUtc) =>
        //    DateTime.SpecifyKind(
        //        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc), Zona),
        //        DateTimeKind.Unspecified);
        public static DateTime DesdeUtc(DateTime fechaUtc) =>
            DateTime.SpecifyKind(fechaUtc + Offset, DateTimeKind.Unspecified);

        // 00:00 del día de Argentina, en UTC (ej. 2026-03-10 -> 2026-03-10 03:00 UTC)
        // [Diana desde v4.0 OBSOLETO]
        //public static DateTime InicioDelDiaUtc(DateOnly dia) =>
        //    TimeZoneInfo.ConvertTimeToUtc(dia.ToDateTime(TimeOnly.MinValue), Zona);
        public static DateTime InicioDelDiaUtc(DateOnly dia) =>
            DateTime.SpecifyKind(dia.ToDateTime(TimeOnly.MinValue) - Offset, DateTimeKind.Utc);
    }
}
