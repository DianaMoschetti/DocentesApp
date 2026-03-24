using AutoMapper;
using DocentesApp.Application.DTOs.Designaciones;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//diana => src.Cargo.Denominacion.ToDisplayName() CUANDO TENGA EL EXTENSION METHOD REEMPLAZAR TODOS LOS ENUMS POR ESE

namespace DocentesApp.Application.Mappings
    {
        public class DesignacionProfile : Profile
        {
        public DesignacionProfile()
        {
            // DTO -> ENTITY 
            CreateMap<CreateDesignacionDto, Designacion>();
            CreateMap<UpdateDesignacionDto, Designacion>();

            // ENTITY -> DTO LIST   
            CreateMap<Designacion, ListDesignacionDto>()
                .ForMember(dest => dest.NombreCompletoDocente,
                    opt => opt.MapFrom(src =>
                        src.Docente != null
                            ? $"{src.Docente.Apellido}, {src.Docente.Nombre}"
                            : string.Empty))
                .ForMember(dest => dest.DescripcionCargo,
                    opt => opt.MapFrom(src =>
                        src.Cargo != null
                            ? src.Cargo.Denominacion.ToString()
                            : string.Empty))
                .ForMember(dest => dest.DescripcionDedicacion,
                    opt => opt.MapFrom(src =>
                        src.Dedicacion != null
                            ? src.Dedicacion.DescTipo.ToString()
                            : string.Empty))
                .ForMember(dest => dest.NombreAsignatura,
                    opt => opt.MapFrom(src =>
                        src.Asignatura != null
                            ? src.Asignatura.NombreAsignatura.ToString()
                            : null))
                .ForMember(dest => dest.DescripcionCurso,
                    opt => opt.MapFrom(src =>
                        src.Curso != null
                            ? $"{src.Curso.Año} - {src.Curso.Carrera} - Comisión {src.Curso.NroComision} - {src.Curso.Turno}"
                            : null));

            // ENTITY -> DTO DETALLE (el include base me mete los campos de list para no repetir tanto
            CreateMap<Designacion, DesignacionDto>()
                .IncludeBase<Designacion, ListDesignacionDto>();
        }       
    }
}
