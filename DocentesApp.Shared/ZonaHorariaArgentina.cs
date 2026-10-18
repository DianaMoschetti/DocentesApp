namespace DocentesApp.Shared
{
    // Hora de Argentina para la auditoría: la base guarda UTC; la API filtra por días de Argentina y Blazor muestra hora de Argentina.
    // Usa el id IANA (en Windows .NET lo mapea con ICU); si el sistema no lo tiene, UTC-3 fijo (Argentina no tiene horario de verano).
    public static class ZonaHorariaArgentina
    {
        public const string IdIana = "America/Argentina/Buenos_Aires";

        public static TimeZoneInfo Zona { get; } = Crear(IdIana);

        // Público para poder probar el fallback con un id inexistente
        public static TimeZoneInfo Crear(string idIana)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(idIana);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                return TimeZoneInfo.CreateCustomTimeZone("Argentina UTC-3", TimeSpan.FromHours(-3), "Argentina (UTC-3)", "Argentina (UTC-3)");
            }
        }

        // UTC -> hora de Argentina
        public static DateTime DesdeUtc(DateTime fechaUtc) =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc), Zona);

        // 00:00 del día de Argentina, en UTC (ej. 2026-03-10 -> 2026-03-10 03:00 UTC)
        public static DateTime InicioDelDiaUtc(DateOnly dia) =>
            TimeZoneInfo.ConvertTimeToUtc(dia.ToDateTime(TimeOnly.MinValue), Zona);
    }
}
