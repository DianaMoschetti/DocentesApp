using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Auditoria
{
    // Filtros de GET api/auditoria (query string)
    public class FiltroAuditoriaDto
    {
        public const string UsuarioSistema = "sistema"; // usuarioId=sistema: filas sin usuario
        public const int TamanioPaginaPorDefecto = 50;
        public const int TamanioPaginaMaximo = 200;

        public string? UsuarioId { get; set; }
        public DateOnly? Desde { get; set; } // día de Argentina
        public DateOnly? Hasta { get; set; } // día de Argentina, inclusive
        public string? Entidad { get; set; } // clave (ver GET api/auditoria/entidades)
        public AccionAuditoria? Accion { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanioPagina { get; set; } = TamanioPaginaPorDefecto; // más de 200 se recorta a 200
    }
}
