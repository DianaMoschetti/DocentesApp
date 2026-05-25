using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum TipoDedicacion
    {
        Simple = 1,
        [Description("Semi exclusiva")]
        SemiExclusiva = 2,
        Exclusiva = 3
    }
}
