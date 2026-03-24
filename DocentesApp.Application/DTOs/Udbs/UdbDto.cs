using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Udbs
{
    public class UdbDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? DirectorDocenteId { get; set; }
        public string? DirectorNombreCompleto { get; set; }
        public int? SecretarioDocenteId { get; set; }
        public string? SecretarioNombreCompleto { get; set; }
    }
}
