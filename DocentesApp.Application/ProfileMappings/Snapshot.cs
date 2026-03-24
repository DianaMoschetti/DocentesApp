using AutoMapper;
using DocentesApp.Application.DTOs.Snapshots;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.ProfileMappings
{
    public class Snapshot : Profile
    {
        public Snapshot() 
        {
            CreateMap<CreatePlantaSnapshotDto, PlantaSnapshot>();
            CreateMap<CreatePlantaDocenteSnapshotDto, PlantaDocenteSnapshot>();
            CreateMap<PlantaSnapshot, PlantaSnapshotDto>();
            CreateMap<PlantaDocenteSnapshot, PlantaDocenteSnapshotDto>();
        }
    }
}
