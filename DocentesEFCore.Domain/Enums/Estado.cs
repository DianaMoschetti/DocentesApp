
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Estado
    {
        Activa = 1,
        Finalizada = 2,
        [Description("Licencia sin haberes")]
        LicenciaSinHaberes = 3,
        [Description("Licencia con haberes")]
        LicenciaConHaberes = 4,
        Otros = 5
    }
}
