using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Condicion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } // Regular (Ordinario) / Interino / Licencia
        public bool? TieneSuplente { get; set; } = false; // Solo para Licencia

        public string? TipoLicencia { get; set; } // // Licencia por enfermedad (con)
    }
}
