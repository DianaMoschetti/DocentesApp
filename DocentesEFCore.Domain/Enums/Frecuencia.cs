

using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Frecuencia
    {
        Anual = 1,
        Cuatrimestral = 2,
        [Description("Electiva anual")]
        ElectivaAnual = 3,
        [Description("Electiva cuatrimestral")]
        ElectivaCuatrimestral = 4,
        Taller = 5,
        Otro = 6
    }
}
