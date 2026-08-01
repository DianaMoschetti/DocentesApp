using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class DesignacionConfig : IEntityTypeConfiguration<Designacion>
    {
        public void Configure(EntityTypeBuilder<Designacion> builder)
        {
            builder.ToTable("Designaciones");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.FechaInicio).IsRequired();
            builder.Property(d => d.NroResolucion).HasMaxLength(50);
            builder.Property(d => d.NroNota).HasMaxLength(50);
            builder.Property(d => d.Observaciones).HasMaxLength(500);
            builder.Property(d => d.PuntosLibres).HasPrecision(10, 2);

            builder.HasOne(d => d.Docente)
                .WithMany(doc => doc.Designaciones)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indices para consultar planta actual o planta historica
            builder.HasIndex(d => new { d.DocenteId, d.FechaFin });
            builder.HasIndex(d => new { d.DocenteId, d.FechaInicio }); // ver
            // Regla 41: no existe restricción de unicidad a nivel de cabecera.
            // Un docente puede tener múltiples designaciones activas con la misma categoría de cargo.
            builder.HasIndex(d => new { d.EstadoDesignacion, d.FechaInicio });


            // Diana refactor v4 p2.3
            //builder.HasOne(d => d.Cargo)
            //    .WithMany(c => c.Designaciones)
            //    .HasForeignKey(d => d.CargoId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(d => d.Dedicacion)
            //   .WithMany()
            //   .HasForeignKey(d => d.DedicacionId)
            //   .OnDelete(DeleteBehavior.Restrict);
                        
            // Evita “dos activas iguales” (heurístico): mismas claves con FechaFin null
            // (SQL Server soporta índice filtrado; EF lo genera)
            // Una sola designación activa por combinación docente-cargo
            //builder.HasIndex(d => new { d.DocenteId, d.CargoId })
            //    .IsUnique()
            //    .HasFilter("[FechaFin] IS NULL");           


        }
    }
}
