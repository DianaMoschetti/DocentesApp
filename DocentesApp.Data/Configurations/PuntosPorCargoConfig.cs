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

            // [Diana desde v4.0 OBSOLETO] la clave pasa a ser Cargo (CargoDocente), una fila por cargo
            //builder.Property(p => p.Denominacion).IsRequired();
            //builder.Property(p => p.TipoCargo).IsRequired();
            builder.Property(p => p.Cargo).IsRequired();
            builder.Property(p => p.PuntosBase).IsRequired().HasPrecision(10, 2);

            // [Diana desde v4.0 OBSOLETO]
            //builder.HasIndex(p => new { p.Denominacion, p.TipoCargo })
            //.IsUnique();
            builder.HasIndex(p => p.Cargo)
                .IsUnique();

            // Seed inicial con valores institucionales estándar
            // [Diana desde v4.0 OBSOLETO] seed anterior por Denominacion + TipoCargo
            //builder.HasData(
            //// Profesores
            //new PuntosPorCargo { Id = 1, Denominacion = DenominacionCargo.Profesor, TipoCargo = TipoCargo.Adjunto, PuntosBase = 138 },
            //new PuntosPorCargo { Id = 2, Denominacion = DenominacionCargo.Profesor, TipoCargo = TipoCargo.Asociado, PuntosBase = 157 },
            //new PuntosPorCargo { Id = 3, Denominacion = DenominacionCargo.Profesor, TipoCargo = TipoCargo.Titular, PuntosBase = 176 },
            //// JTP — sin subdivisión de tipo
            //new PuntosPorCargo { Id = 4, Denominacion = DenominacionCargo.JefeDeTrabajosPracticos, TipoCargo = TipoCargo.Adjunto, PuntosBase = 119 },
            //// Ayudantes — sin subdivisión de tipo
            //new PuntosPorCargo { Id = 5, Denominacion = DenominacionCargo.AyudanteDePrimera, TipoCargo = TipoCargo.Adjunto, PuntosBase = 100 },
            //new PuntosPorCargo { Id = 6, Denominacion = DenominacionCargo.AyudanteDeSegunda, TipoCargo = TipoCargo.Adjunto, PuntosBase = 80 }
            //);
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
