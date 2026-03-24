using AutoMapper;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.Mappings
{
    public class DocenteProfile : Profile
    {
        public DocenteProfile()
        {
            // CREATE
            CreateMap<CreateDocenteDto, Docente>();

            // UPDATE COMPLETO - PUT (si no mando un dato lo pisa con null)
            CreateMap<UpdateDocenteDto, Docente>();

            // UPDATE PARCIAL - PATCH - GRADO ACADEMICO (ignora nulls, deja como estaba)
            CreateMap<UpdateAcademicoDocenteDto, Docente>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            // UPDATE PARCIAL - CONTACTO (ignora nulls, deja como estaba)
            CreateMap<UpdateContactoDocenteDto, Docente>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            // UPDATE PARCIAL - OBSERVACIONES (ignora nulls, deja como estaba)
            CreateMap<UpdateContactoDocenteDto, Docente>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            // NOMBRE COMPLETO
            CreateMap<Docente, DetalleDocenteDto>()
                .ForMember(destino => destino.NombreCompleto,
                    opt => opt.MapFrom(src => $"{src.Apellido}, {src.Nombre}"));

            // ENTITY → DTO
            CreateMap<Docente, DocenteDto>()
                .ForMember(dest => dest.NombreCompleto,
                    opt => opt.MapFrom(src => $"{src.Apellido}, {src.Nombre}"));

            CreateMap<Docente, ListDocenteDto>()
                .ForMember(destino => destino.NombreCompleto,
                    opt => opt.MapFrom(src => $"{src.Apellido}, {src.Nombre}"));
        }
    }
}
