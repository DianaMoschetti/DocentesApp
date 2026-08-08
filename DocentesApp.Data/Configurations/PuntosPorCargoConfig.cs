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

            builder.Property(p => p.Denominacion).IsRequired();
            builder.Property(p => p.TipoCargo).IsRequired();
            builder.Property(p => p.PuntosBase).IsRequired().HasPrecision(10, 2);

            builder.HasIndex(p => new { p.Denominacion, p.TipoCargo })
                .IsUnique();

            // Seed inicial con valores institucionales estándar
            builder.HasData(
                // Profesores
                new PuntosPorCargo { Id = 1, Denominacion = DenominacionCargo.Profesor, TipoCargo = TipoCargo.Adjunto, PuntosBase = 138 },
                new PuntosPorCargo { Id = 2, Denominacion = DenominacionCargo.Profesor, TipoCargo = TipoCargo.Asociado, PuntosBase = 157 },
                new PuntosPorCargo { Id = 3, Denominacion = DenominacionCargo.Profesor, TipoCargo = TipoCargo.Titular, PuntosBase = 176 },
                // JTP — sin subdivisión de tipo
                new PuntosPorCargo { Id = 4, Denominacion = DenominacionCargo.JefeDeTrabajosPracticos, TipoCargo = TipoCargo.Adjunto, PuntosBase = 119 },
                // Ayudantes — sin subdivisión de tipo
                new PuntosPorCargo { Id = 5, Denominacion = DenominacionCargo.AyudanteDePrimera, TipoCargo = TipoCargo.Adjunto, PuntosBase = 100 },
                new PuntosPorCargo { Id = 6, Denominacion = DenominacionCargo.AyudanteDeSegunda, TipoCargo = TipoCargo.Adjunto, PuntosBase = 80 }
            );
        }
    }
}
