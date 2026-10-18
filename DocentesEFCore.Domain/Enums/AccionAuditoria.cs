using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    // Acción registrada en el historial de cambios (auditoría)
    public enum AccionAuditoria
    {
        Alta = 1,
        [Description("Modificación")]
        Modificacion = 2,
        Baja = 3
    }
}
