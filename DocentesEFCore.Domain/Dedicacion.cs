using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Dedicacion 
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } // Simples (10hs) Exclusiva (40)        
        public float CantidadHoras { get; set; }

        public float CantidadDedicacion { get; set; } //una dedicacion, media dedicacion, una y media
    }
}
