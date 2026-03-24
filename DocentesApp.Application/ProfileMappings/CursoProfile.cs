using AutoMapper;
using DocentesApp.Application.DTOs.Cursos;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.ProfileMappings
{
    public class CursoProfile : Profile
    {
        public CursoProfile() 
        {

            CreateMap<CreateCursoDto, Curso>();
            CreateMap<UpdateCursoDto, Curso>();

            CreateMap<Curso, CursoDto>()
                .ForMember(dest => dest.TurnoTexto,
                    opt => opt.MapFrom(src => src.Turno.ToString()))
                .ForMember(dest => dest.CarreraTexto,
                    opt => opt.MapFrom(src => src.Carrera.ToString()))
                .ForMember(dest => dest.Descripcion,
                    opt => opt.MapFrom(src => $"{src.Año} - {src.Carrera} - Comisión {src.NroComision} - {src.Turno}"));

            CreateMap<Curso, ListCursoDto>()
                .ForMember(dest => dest.Descripcion,
                    opt => opt.MapFrom(src => $"{src.Año} - {src.Carrera} - Comisión {src.NroComision} - {src.Turno}"));

        }
    }

}
