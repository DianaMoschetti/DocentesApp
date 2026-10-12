
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Estado
    {
        Activa = 1,
        Finalizada = 2,
        // [Diana desde v4.0 OBSOLETO] la licencia ahora es por cargo: CargoDesignacion.EnLicencia
        [Obsolete("La licencia se registra en CargoDesignacion.EnLicencia.")]
        [Description("Licencia sin haberes")]
        LicenciaSinHaberes = 3,
        [Obsolete("La licencia se registra en CargoDesignacion.EnLicencia.")]
        [Description("Licencia con haberes")]
        LicenciaConHaberes = 4,
        Otros = 5
    }
}
