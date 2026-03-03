using DocentesApp.Model;
using DocentesApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class Dedicacion 
    {
        public int Id { get; set; }
        public TipoDedicacion DescTipo { get; set; } // Simples (10hs) Exclusiva (40)        
        public float CantidadHoras { get; set; }
        public float CantidadDedicacion { get; set; } //una dedicacion, media dedicacion, una y media
    }
}
