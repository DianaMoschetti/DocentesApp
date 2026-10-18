namespace DocentesApp.Shared
{
    // Hora de Argentina para la auditoría: la base guarda UTC; la API filtra por días de Argentina y Blazor muestra hora de Argentina.
    // Offset fijo UTC-3: el sistema siempre corre en Argentina, que no tiene horario de verano desde 2009.
    // Si eso cambiara algún día, este es el único lugar a modificar.
    public static class ZonaHorariaArgentina
    {
        public static readonly TimeSpan Offset = TimeSpan.FromHours(-3);

        // UTC -> hora de Argentina, con Kind Unspecified: viaja en el JSON sin offset ni "Z" ("2026-03-09T23:30:00"),
        // así Newtonsoft en Blazor no la convierte a la hora local del servidor. Blazor la muestra tal cual.
        public static DateTime DesdeUtc(DateTime fechaUtc) =>
            DateTime.SpecifyKind(fechaUtc + Offset, DateTimeKind.Unspecified);

        // 00:00 del día de Argentina, en UTC (ej. 2026-03-10 -> 2026-03-10 03:00 UTC)
        public static DateTime InicioDelDiaUtc(DateOnly dia) =>
            DateTime.SpecifyKind(dia.ToDateTime(TimeOnly.MinValue) - Offset, DateTimeKind.Utc);
    }
}
