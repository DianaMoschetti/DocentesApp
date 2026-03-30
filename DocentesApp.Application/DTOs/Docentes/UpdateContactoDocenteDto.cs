using DocentesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class UpdateContactoDocenteDto
    {
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
    }
}
//Solo actualizo lo que mando, si mando null no lo actualizo

//CreateMap<UpdateDocenteContactoDto, Docente>()
//    .ForAllMembers(opts =>
//        opts.Condition((src, dest, srcMember) => srcMember != null));