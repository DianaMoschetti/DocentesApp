namespace DocentesApp.Domain.Base
{
    // Entidad con columnas de auditoría. La implementan BaseDomainModel y ApplicationUser (que no puede heredar de BaseDomainModel).
    // CreatedBy/ModifiedBy = userId o "Sistema"; fechas en UTC.
    public interface IAuditable
    {
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset ModifiedDate { get; set; }
        string? CreatedBy { get; set; }
        string? ModifiedBy { get; set; }
    }
}
