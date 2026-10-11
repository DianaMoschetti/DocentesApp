
namespace DocentesApp.Domain.Enums
{
    // [Diana desde v4.0 OBSOLETO] reemplazado por CargoDocente (Denominacion + TipoCargo en un solo enum)
    [Obsolete("Usar CargoDocente.")]
    public enum TipoCargo
    {
        Adjunto = 1,
        Asociado = 2,
        Titular = 3
    }
}
