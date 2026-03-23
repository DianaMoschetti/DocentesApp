using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Udbs
{
    public class CreateUdbDto
    {       
        public string Nombre { get; set; } = string.Empty;
        public int? DirectorDocenteId { get; set; }        
        public int? SecretarioDocenteId { get; set; }
    }
}
