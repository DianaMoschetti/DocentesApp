using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    internal class UDB : BaseDomainModel 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Navigation Properties
        public int IdDirector { get; set; }
        public int IdSecretario { get; set; }

    }
}
