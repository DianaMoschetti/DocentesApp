using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class PuntosPorCargoConfig : IEntityTypeConfiguration<PuntosPorCargo>
    {
        public void Configure(EntityTypeBuilder<PuntosPorCargo> builder)
        {
            builder.ToTable("PuntosPorCargo");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Cargo).IsRequired();
            builder.Property(p => p.PuntosBase).IsRequired().HasPrecision(10, 2);

            builder.HasIndex(p => p.Cargo)
                .IsUnique();

            // Seed inicial con valores institucionales estándar
            // Valores oficiales por 1 dedicación simple. Id = (int)Cargo para que los ids sean estables.
            builder.HasData(
                new PuntosPorCargo { Id = (int)CargoDocente.ProfesorAdjunto, Cargo = CargoDocente.ProfesorAdjunto, PuntosBase = 138 },
                new PuntosPorCargo { Id = (int)CargoDocente.ProfesorAsociado, Cargo = CargoDocente.ProfesorAsociado, PuntosBase = 157 },
                new PuntosPorCargo { Id = (int)CargoDocente.ProfesorTitular, Cargo = CargoDocente.ProfesorTitular, PuntosBase = 176 },
                new PuntosPorCargo { Id = (int)CargoDocente.JefeTrabajosPracticos, Cargo = CargoDocente.JefeTrabajosPracticos, PuntosBase = 119 },
                new PuntosPorCargo { Id = (int)CargoDocente.AyudantePrimera, Cargo = CargoDocente.AyudantePrimera, PuntosBase = 100 },
                new PuntosPorCargo { Id = (int)CargoDocente.AyudanteSegunda, Cargo = CargoDocente.AyudanteSegunda, PuntosBase = 80 }
            );
        }
    }
}
