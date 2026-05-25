using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Modulo
    {
        [Description("Pre pre hora")]
        PrePreHora = 1,
        [Description("Pre hora")]
        PreHora = 2,
        [Description("Primer hora 1°")]
        PrimerHora = 3,
        [Description("2°")]
        SegundaHora = 4,
        [Description("3°")]
        TercerHora = 5,
        [Description("4°")]
        CuartaHora = 6,
        [Description("5°")]
        QuintaHora = 7,
        [Description("6°")]
        SextaHora = 8,
    }
}
