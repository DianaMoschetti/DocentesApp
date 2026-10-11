using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Services
{
    public enum Requisito
    {
        Obligatorio = 1,
        Opcional = 2,
        Vacio = 3 // el campo debe quedar en null
    }

    // Qué datos lleva una Actividad según su Especificacion. Lo usan los validadores y la UI (habilitar/limpiar campos).
    //   Docencia         -> Asignatura obligatoria, Curso obligatorio
    //   Laboratorio      -> Asignatura obligatoria, Curso vacío
    //   Jefe de Cátedra  -> Asignatura opcional,    Curso vacío
    //   Investigación    -> Asignatura opcional,    Curso vacío
    //   Gestión          -> Asignatura vacía,       Curso vacío
    //   Otro             -> Asignatura opcional,    Curso opcional
    // "Sin asignatura" es null, nunca un registro comodín.
    public static class ActividadReglas
    {
        public static Requisito Asignatura(EspecificacionCargo especificacion) => especificacion switch
        {
            EspecificacionCargo.Docencia => Requisito.Obligatorio,
            EspecificacionCargo.Laboratorio => Requisito.Obligatorio,
            EspecificacionCargo.JefeDeCatedra => Requisito.Opcional,
            EspecificacionCargo.Investigacion => Requisito.Opcional,
            EspecificacionCargo.Gestion => Requisito.Vacio,
            EspecificacionCargo.Otro => Requisito.Opcional,
            _ => Requisito.Opcional
        };

        public static Requisito Curso(EspecificacionCargo especificacion) => especificacion switch
        {
            EspecificacionCargo.Docencia => Requisito.Obligatorio,
            EspecificacionCargo.Laboratorio => Requisito.Vacio,
            EspecificacionCargo.JefeDeCatedra => Requisito.Vacio,
            EspecificacionCargo.Investigacion => Requisito.Vacio,
            EspecificacionCargo.Gestion => Requisito.Vacio,
            EspecificacionCargo.Otro => Requisito.Opcional,
            _ => Requisito.Opcional
        };

        /// <summary>
        /// true si el valor cumple el requisito (obligatorio: tiene valor; vacío: es null; opcional: siempre).
        /// </summary>
        public static bool Cumple(Requisito requisito, int? valor) => requisito switch
        {
            Requisito.Obligatorio => valor.HasValue,
            Requisito.Vacio => !valor.HasValue,
            _ => true
        };
    }
}
