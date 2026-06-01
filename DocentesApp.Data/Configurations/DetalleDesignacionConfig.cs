using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class DetalleDesignacionConfig : IEntityTypeConfiguration<DetalleDesignacion>
    {
        public void Configure(EntityTypeBuilder<DetalleDesignacion> builder)
        {
            builder.ToTable("DetallesDesignacion");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Especificacion).IsRequired();
            builder.Property(d => d.PuntosUtilizados).HasPrecision(10, 2);

            // Relación con Designacion (cabecera)
            builder.HasOne(d => d.Designacion)
                .WithMany(des => des.Detalles)
                .HasForeignKey(d => d.DesignacionId)
                .OnDelete(DeleteBehavior.Cascade); // si se borra la designacion, se borran los detalles
            // habria que poner un warning? diana todo

            // Relación con Asignatura (opcional)
            builder.HasOne(d => d.Asignatura)
                .WithMany(a => a.DetalleDesignacion)
                .HasForeignKey(d => d.AsignaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con Curso (opcional)
            builder.HasOne(d => d.Curso)
                .WithMany()
                .HasForeignKey(d => d.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices útiles para consultas de planta
            builder.HasIndex(d => new { d.DesignacionId, d.Especificacion });
            builder.HasIndex(d => new { d.AsignaturaId });
        }
    }
}