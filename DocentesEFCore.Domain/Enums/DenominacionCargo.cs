using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
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
        Administrativo = 6 // ver si va aca o en tipo
    }
}
