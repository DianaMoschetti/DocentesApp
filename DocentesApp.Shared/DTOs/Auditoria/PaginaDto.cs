namespace DocentesApp.Shared.DTOs.Auditoria
{
    // Una página de resultados. Pagina empieza en 1.
    public class PaginaDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanioPagina { get; set; }
        public int TotalPaginas => TamanioPagina > 0 ? (Total + TamanioPagina - 1) / TamanioPagina : 0;
    }
}
