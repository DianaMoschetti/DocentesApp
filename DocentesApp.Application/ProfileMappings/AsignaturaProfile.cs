using AutoMapper;
using DocentesApp.Application.DTOs.Asignaturas;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.ProfileMappings
{
    public class AsignaturaProfile : Profile
    {
        public AsignaturaProfile()
        {
            // DTO -> ENTITY 
            CreateMap<CreateAsignaturaDto, Asignatura>();
            CreateMap<UpdateAsignaturaDto, Asignatura>();
            
            CreateMap<Asignatura, AsignaturaDto>()
                .ForMember(dest => dest.NombreAsignaturaTexto,
                    opt => opt.MapFrom(src => src.NombreAsignatura.ToString()))
                .ForMember(dest => dest.FrecuenciaTexto,
                    opt => opt.MapFrom(src => src.Frecuencia.ToString()))
                .ForMember(dest => dest.NivelTexto,
                    opt => opt.MapFrom(src => src.Nivel.ToString()))
                .ForMember(dest => dest.NombreUdb,
                    opt => opt.MapFrom(src => src.Udb != null ? src.Udb.Nombre : null));


            // ENTITY->DTO DETALLE(el include base me mete los campos de list para no repetir tanto
            //CreateMap<Asignatura, AsignaturaDto>()
            //    .IncludeBase<Asignatura, ListAsignaturaDto>();
            // ENTITY->DTO LIST
            //CreateMap<Asignatura, ListAsignaturaDto>();
            CreateMap<Asignatura, ListAsignaturaDto>()
                .ForMember(dest => dest.NombreAsignaturaTexto,
                    opt => opt.MapFrom(src => src.NombreAsignatura.ToString()))
                .ForMember(dest => dest.FrecuenciaTexto,
                    opt => opt.MapFrom(src => src.Frecuencia.ToString()))
                .ForMember(dest => dest.NivelTexto,
                    opt => opt.MapFrom(src => src.Nivel.ToString()))
                .ForMember(dest => dest.NombreUdb,
                    opt => opt.MapFrom(src => src.Udb != null ? src.Udb.Nombre : null));

        }    
    }
}
