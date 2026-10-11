using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class ActividadConfig : IEntityTypeConfiguration<Actividad>
    {
        public void Configure(EntityTypeBuilder<Actividad> builder)
        {
            builder.ToTable("Actividades");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Especificacion).IsRequired();
            builder.Property(a => a.Horario).HasMaxLength(200); // texto libre por ahora
            builder.Property(a => a.Observaciones).HasMaxLength(500);

            // Si se borra el cargo se borran sus actividades
            builder.HasOne(a => a.CargoDesignacion)
                .WithMany(c => c.Actividades)
                .HasForeignKey(a => a.CargoDesignacionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Asignatura opcional (null = sin asignatura); no se puede borrar una asignatura con actividades
            builder.HasOne(a => a.Asignatura)
                .WithMany(asig => asig.Actividades)
                .HasForeignKey(a => a.AsignaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Curso opcional
            builder.HasOne(a => a.Curso)
                .WithMany()
                .HasForeignKey(a => a.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.AsignaturaId);
            builder.HasIndex(a => a.CursoId);
        }
    }
}
