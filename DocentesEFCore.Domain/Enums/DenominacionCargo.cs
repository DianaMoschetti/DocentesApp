using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    // [Diana desde v4.0 OBSOLETO] reemplazado por CargoDocente (Denominacion + TipoCargo en un solo enum).
    // Becario, Adscripto y Administrativo no forman parte de CargoDocente.
    [Obsolete("Usar CargoDocente.")]
    public enum DenominacionCargo
    {
        Profesor = 1,
        [Description("Jefe de Trabajos Practicos")]
        JefeDeTrabajosPracticos = 2,
        [Description("Ayudante de Primera")]
        AyudanteDePrimera = 3,
        [Description("Ayudante de Segunda")]
        AyudanteDeSegunda = 4,
        Becario = 5,
        Adscripto = 6,
        Administrativo = 7 // ver si va aca o en tipo
    }
}
