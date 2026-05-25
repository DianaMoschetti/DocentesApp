
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum EspecificacionCargo
    {
        Docencia = 1,
        Gestion = 2,
        Investigación = 3,
        [Description("Docencia y Gestión")]
        DocenciaYGestion = 4,
        [Description("Jefe de Cátedra")]
        JefeDeCatedra = 5
    }
}
