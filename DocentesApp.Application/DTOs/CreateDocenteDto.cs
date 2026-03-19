using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs
{
    public class CreateDocenteDto
    {
        [Required]
        [MaxLength(10)]
        public string Dni { get; set; }
        [Required]
        public int Legajo { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }
    }
}
