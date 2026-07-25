using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum TipoDedicacion
    {
        Simple = 1,
        [Description("Semi exclusiva")]
        SemiExclusiva = 2,
        Exclusiva = 3,
        [Description("Media simple")]
        MediaSimple = 4,
        [Description("Media semi exclusiva")]
        MediaSemiExclusiva = 5,
        [Description("Media exclusiva")]
        MediaExclusiva = 6
    }
}
