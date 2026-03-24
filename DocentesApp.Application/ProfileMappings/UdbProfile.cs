using AutoMapper;
using DocentesApp.Application.DTOs.Udbs;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.ProfileMappings
{
    public class UdbProfile : Profile
    {
        public UdbProfile()

        {
            CreateMap<CreateUdbDto, Udb>();
            CreateMap<UpdateUdbDto, Udb>();

            CreateMap<Udb, UdbDto>()
                .ForMember(dest => dest.DirectorNombreCompleto,
                    opt => opt.MapFrom(src => src.Director != null ? $"{src.Director.Apellido}, {src.Director.Nombre}" : null))
                .ForMember(dest => dest.SecretarioNombreCompleto,
                    opt => opt.MapFrom(src => src.Secretario != null ? $"{src.Secretario.Apellido}, {src.Secretario.Nombre}" : null));

            CreateMap<Udb, ListUdbDto>();

        }
    }
}
