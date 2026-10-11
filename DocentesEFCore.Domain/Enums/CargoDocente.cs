using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    // Reemplaza a DenominacionCargo + TipoCargo (v5): un único valor identifica el cargo y sus puntos.
    // "Auxiliar" no es un cargo: es la categoría de JTP y Ayudantes (ver CargoDocenteExtensions.EsAuxiliar).
    // Se persiste como int: los valores nuevos van siempre al final, nunca renumerar.
    public enum CargoDocente
    {
        [Description("Profesor Adjunto")]
        ProfesorAdjunto = 1,
        [Description("Profesor Asociado")]
        ProfesorAsociado = 2,
        [Description("Profesor Titular")]
        ProfesorTitular = 3,
        [Description("Jefe de Trabajos Prácticos (Auxiliar)")]
        JefeTrabajosPracticos = 4,
        [Description("Ayudante de 1° (Auxiliar)")]
        AyudantePrimera = 5,
        [Description("Ayudante de 2° (Auxiliar)")]
        AyudanteSegunda = 6
    }
}
