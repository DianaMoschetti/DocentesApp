namespace DocentesApp.Shared.DTOs.Designaciones
{
    // Opción del selector "Reemplaza a": un cargo en licencia, sin suplente vigente, de la UDB elegida
    public class CargoEnLicenciaDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty; // "Perez, Juan – PAd 1 DE"
        public int UdbId { get; set; }
    }
}
