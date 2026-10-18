namespace DocentesApp.Domain.Entities
{
    // Detalle de un HistorialCambio: una fila por propiedad cambiada (valor anterior -> nuevo).
    // Valores serializados con cultura invariante; FKs y enums se guardan crudos y se resuelven a nombres al leer.
    // Eventos de contraseña: Propiedad "Contraseña" con ambos valores en null (nunca se guarda la contraseña ni el hash).
    public class HistorialCambioDetalle
    {
        public long Id { get; set; }
        public long HistorialCambioId { get; set; }
        public HistorialCambio HistorialCambio { get; set; } = null!;

        public string Propiedad { get; set; } = null!;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
    }
}
