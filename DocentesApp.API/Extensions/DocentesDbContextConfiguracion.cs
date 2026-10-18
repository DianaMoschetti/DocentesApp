using DocentesApp.Data.Auditoria;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.API.Extensions
{
    // Configuración del DbContext de la API (Program.cs). Separada para que un test verifique la estrategia de ejecución.
    public static class DocentesDbContextConfiguracion
    {
        public static void ConfigurarSqlServer(DbContextOptionsBuilder options, string? connString, AuditoriaInterceptor auditoria)
        {
            // Sin EnableRetryOnFailure: el interceptor de auditoría abre su propia transacción en SaveChanges,
            // lo que no se puede con una estrategia que reintenta (ver docs/auditoria-plan.md §1)
            options.UseSqlServer(connString)
                .AddInterceptors(auditoria);
        }
    }
}
