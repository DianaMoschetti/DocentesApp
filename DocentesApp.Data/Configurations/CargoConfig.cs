using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Data.Configurations
{
    public class CargoConfig : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> builder)
        {
            builder.ToTable("Cargos");
            builder.HasKey(c => c.Id);

            // Guardo los enums como ints -> no hace falta pq se guardan como int por defecto
            //builder.Property(c => c.TipoCargo).HasConversion<int>();
            builder.Property(c => c.Observaciones).HasMaxLength(500);

            // para evitar cargos repetidos, no se pueden repetir la combinación de estos campos
            builder.HasIndex(x => new { x.Denominacion, x.TipoCargo, x.DetalleCargo, x.Condicion })
             .IsUnique();

            // Relaciones
            builder.HasMany(x => x.Designaciones)
             .WithOne(d => d.Cargo)
             .HasForeignKey(d => d.CargoId)
             .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
