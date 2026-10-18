
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum EspecificacionCargo
    {
        Docencia = 1,
        Gestion = 2,
        Investigacion = 3,
        [Description("Jefe de Cátedra")]
        JefeDeCatedra = 5,
        Laboratorio = 6,
        Otro = 7
    }
}
