using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Shared.DTOs.Cursos
{
    public class ListCursoDto
    {
        public int Id { get; set; }        
        public string Descripcion { get; set; } = string.Empty;
    }
}
