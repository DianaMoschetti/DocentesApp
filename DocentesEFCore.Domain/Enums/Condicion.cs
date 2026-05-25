
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Condicion
    {
        Regular = 1,
        Interino = 2,
        Ordinario = 3,
        [Description("Licencia con haberes")]
        LicenciaConHaberes = 4,
        [Description("Licencia sinn haberes")]
        LicenciaSinHaberes = 5,
        Suplente = 6,
        Becario = 7,
        Otros = 8
        
    }
}
