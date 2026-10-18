using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Domain.Base
{
    // Columnas de auditoría: las completa el interceptor de auditoría (CreatedBy/ModifiedBy = userId o "Sistema", fechas UTC).
    // Filas anteriores a la auditoría: CreatedDate 0001-01-01 y CreatedBy NULL (desconocido).
    public abstract class BaseDomainModel : IAuditable
    {
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
