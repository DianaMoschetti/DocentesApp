namespace DocentesApp.API.Services
{
    // Etiquetas en castellano para mostrar el historial. Lo que no está acá se muestra con su nombre crudo.
    public static class EtiquetasAuditoria
    {
        // Clave guardada en HistorialCambio.Entidad -> etiqueta
        public static readonly IReadOnlyDictionary<string, string> Entidades = new Dictionary<string, string>
        {
            ["Docente"] = "Docente",
            ["Designacion"] = "Designación",
            ["CargoDesignacion"] = "Cargo de designación",
            ["Actividad"] = "Actividad",
            ["DocenteUdb"] = "Docente en UDB",
            ["Udb"] = "UDB",
            ["Asignatura"] = "Asignatura",
            ["Curso"] = "Curso",
            ["AsignaturaModulo"] = "Módulo de asignatura",
            ["PuntosPorCargo"] = "Puntos por cargo",
            ["Laboratorio"] = "Laboratorio",
            ["Cargo"] = "Cargo (obsoleto)",
            ["Dedicacion"] = "Dedicación (obsoleta)",
            ["Usuario"] = "Usuario",
        };

        // Nombre de propiedad -> etiqueta (iguales para todas las entidades)
        public static readonly IReadOnlyDictionary<string, string> Propiedades = new Dictionary<string, string>
        {
            // Personas y usuarios
            ["Nombre"] = "Nombre",
            ["Apellido"] = "Apellido",
            ["Dni"] = "DNI",
            ["Legajo"] = "Legajo",
            ["Email"] = "Email",
            ["EmailAlternativo"] = "Email alternativo",
            ["Celular"] = "Celular",
            ["Direccion"] = "Dirección",
            ["FechaNacimiento"] = "Fecha de nacimiento",
            ["MaxNivelAcademico"] = "Máximo nivel académico",
            ["Observaciones"] = "Observaciones",
            ["UserName"] = "Nombre de usuario",
            ["Activo"] = "Activo",
            ["EmailConfirmed"] = "Email confirmado",
            ["PhoneNumber"] = "Teléfono",
            ["PhoneNumberConfirmed"] = "Teléfono confirmado",
            ["TwoFactorEnabled"] = "Doble factor",
            ["Rol"] = "Rol",
            ["Contraseña"] = "Contraseña",
            // UDB y membresías
            ["UdbId"] = "UDB",
            ["DocenteId"] = "Docente",
            ["DirectorDocenteId"] = "Director",
            ["SecretarioDocenteId"] = "Secretario",
            // Designaciones, cargos y actividades
            ["NroResolucion"] = "Nro. de resolución",
            ["NroNota"] = "Nro. de nota",
            ["FechaInicio"] = "Fecha de inicio",
            ["FechaFin"] = "Fecha de fin",
            ["EstadoDesignacion"] = "Estado",
            ["DesignacionId"] = "Designación",
            ["Cargo"] = "Cargo",
            ["Condicion"] = "Condición",
            ["TipoDedicacion"] = "Tipo de dedicación",
            ["CantidadDedicacion"] = "Cantidad de dedicación",
            ["Puntos"] = "Puntos",
            ["EnLicencia"] = "En licencia",
            ["ReemplazaACargoId"] = "Reemplaza a",
            ["CargoDesignacionId"] = "Cargo",
            ["Especificacion"] = "Especificación",
            ["AsignaturaId"] = "Asignatura",
            ["CursoId"] = "Curso",
            ["Horario"] = "Horario",
            // Catálogos
            ["EsVigente"] = "Vigente",
            ["Frecuencia"] = "Frecuencia",
            ["Nivel"] = "Nivel",
            ["Turno"] = "Turno",
            ["NroComision"] = "Comisión",
            ["Año"] = "Año",
            ["Carrera"] = "Carrera",
            ["Modulo"] = "Módulo",
            ["PuntosBase"] = "Puntos base",
            ["Lugar"] = "Lugar",
            // Columnas de auditoría (no van al detalle; por si aparecen en una fila vieja o manual)
            ["CreatedDate"] = "Fecha de alta",
            ["CreatedBy"] = "Alta por",
            ["ModifiedDate"] = "Fecha de modificación",
            ["ModifiedBy"] = "Modificado por",
        };

        public static string Entidad(string clave) => Entidades.GetValueOrDefault(clave) ?? clave;

        public static string Propiedad(string nombre) => Propiedades.GetValueOrDefault(nombre) ?? nombre;
    }
}
