using DocentesApp.Domain.Entities;
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
        }
    }
}
