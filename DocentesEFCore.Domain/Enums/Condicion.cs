
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Condicion
    {
        Regular = 1,
        Interino = 2,
        Ordinario = 3,
        // [Diana desde v4.0 OBSOLETO] la licencia ahora es por cargo: CargoDesignacion.EnLicencia
        [Obsolete("La licencia se registra en CargoDesignacion.EnLicencia.")]
        [Description("Licencia con haberes")]
        LicenciaConHaberes = 4,
        // [Diana desde v4.0 OBSOLETO] typo corregido en la descripción
        //[Description("Licencia sinn haberes")]
        [Obsolete("La licencia se registra en CargoDesignacion.EnLicencia.")]
        [Description("Licencia sin haberes")]
        LicenciaSinHaberes = 5,
        Suplente = 6,
        // [Diana desde v4.0 OBSOLETO] Becario no es una condición del cargo
        [Obsolete("Becario no es una condición de cargo.")]
        Becario = 7,
        Otros = 8
        
    }
}
