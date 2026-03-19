using AutoMapper;
using DocentesApp.Application.DTOs;
using DocentesApp.Model;

namespace DocentesApp.API.Configuraciones
{
    public class MapperConfig : Profile
    {
        public MapperConfig() 
        {
            CreateMap<CreateDocenteDto, Docente>().ReverseMap();
        }

    }
}
