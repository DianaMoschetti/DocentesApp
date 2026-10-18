using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum TipoDedicacion
    {
        Simple = 1,
        [Description("Semiexclusiva (DTP)")]
        SemiExclusiva = 2,
        Exclusiva = 3,
        // [Diana desde v4.0 OBSOLETO] las medias dedicaciones se cargan con CantidadDedicacion = 0,5
        [Obsolete("Usar la dedicación completa con CantidadDedicacion = 0,5.")]
        [Description("Media simple")]
        MediaSimple = 4,
        [Obsolete("Usar la dedicación completa con CantidadDedicacion = 0,5.")]
        [Description("Media semi exclusiva")]
        MediaSemiExclusiva = 5,
        [Obsolete("Usar la dedicación completa con CantidadDedicacion = 0,5.")]
        [Description("Media exclusiva")]
        MediaExclusiva = 6
    }
}
