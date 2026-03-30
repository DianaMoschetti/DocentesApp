using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class PlantaDocenteSnapshotConfig : IEntityTypeConfiguration<PlantaDocenteSnapshot>
    {
        public void Configure(EntityTypeBuilder<PlantaDocenteSnapshot> builder)
        {
            builder.ToTable("PlantaDocenteSnapshots");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Ano).HasMaxLength(10);
            builder.Property(p => p.Division).HasMaxLength(30);
            builder.Property(p => p.EspecialidadTextoOriginal).HasMaxLength(60);
            builder.Property(p => p.MateriaTextoOriginal).HasMaxLength(200);
            builder.Property(p => p.CategoriaDocente).HasMaxLength(150);
            builder.Property(p => p.DedicacionTexto).HasMaxLength(50);
            builder.Property(p => p.Observaciones).HasMaxLength(500);

            builder.Property(p => p.RowKey)
                .IsRequired()
                .HasMaxLength(250)
                .HasDefaultValue("");
            
            builder.HasIndex(p => p.PlantaSnapshotId);
            builder.HasIndex(p => new { p.PlantaSnapshotId, p.RowKey }).IsUnique();
            builder.HasIndex(p => p.DocenteId);
            builder.HasIndex(p => p.AsignaturaId);
            builder.HasIndex(p => p.UdbId);

            // opcionales ver
            builder.HasOne<Docente>()
                .WithMany()
                .HasForeignKey(p => p.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Asignatura>()
                .WithMany()
                .HasForeignKey(p => p.AsignaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Udb>()
                .WithMany()
                .HasForeignKey(p => p.UdbId)
                .OnDelete(DeleteBehavior.Restrict);
               
        }
    }
}
