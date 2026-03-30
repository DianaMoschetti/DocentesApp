using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class DocenteConfig : IEntityTypeConfiguration<Docente>
    {
        public void Configure(EntityTypeBuilder<Docente> builder)
        {
            builder.ToTable("Docentes");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Legajo).IsRequired();
            builder.HasIndex(d => d.Legajo).IsUnique();

            // Campos de Persona
            builder.Property(d => d.Dni).IsRequired().HasMaxLength(20);
            builder.Property(d => d.Nombre).IsRequired().HasMaxLength(120);
            builder.Property(d => d.Apellido).IsRequired().HasMaxLength(120);
            builder.HasIndex(d => d.Dni).IsUnique();
            builder.Property(d => d.Email).HasMaxLength(200);
            builder.Property(d => d.EmailAlternativo).HasMaxLength(200);
            builder.Property(d => d.Celular).HasMaxLength(30);
            builder.Property(d => d.Direccion).HasMaxLength(250);
            builder.Property(d => d.FechaNacimiento).HasMaxLength(20);
            builder.Property(d => d.MaxNivelAcademico).HasMaxLength(120);
            builder.Property(d => d.Observaciones).HasMaxLength(500);

            // Relaciones
            builder.HasMany(d => d.Designaciones)
            .WithOne(x => x.Docente)
            .HasForeignKey(x => x.DocenteId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.DocenteUdbs)
             .WithOne(x => x.Docente)
             .HasForeignKey(x => x.DocenteId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
