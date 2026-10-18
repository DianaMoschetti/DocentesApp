using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DocentesApp.Data.Configurations
{
    public class AsignaturaConfig : IEntityTypeConfiguration<Asignatura>
    {
        public void Configure(EntityTypeBuilder<Asignatura> builder)
        {
            builder.ToTable("Asignaturas");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Nombre).IsRequired().HasMaxLength(200);
            builder.Property(a => a.EsVigente).IsRequired().HasDefaultValue(true);
            // Los enums son ints por defecto, no hace falta convertirlos
            builder.Property(a => a.Frecuencia).IsRequired();
            builder.Property(a => a.Nivel).IsRequired();
            // Relaciones
            // Asignatura pertenece a Udb 
            builder.HasOne(x => x.Udb)
             .WithMany(u => u.Asignaturas)
             .HasForeignKey(x => x.UdbId)
             .IsRequired() // v5: toda asignatura pertenece a una UDB
             .OnDelete(DeleteBehavior.Restrict);
            // Unicidad lógica: una asignatura no se puede repetir en una misma udb con el mismo nombre y nivel
            builder.HasIndex(x => new { x.UdbId, x.Nombre, x.Nivel })
             .IsUnique();
            // Una asignatura tiene muchos asignatura-modulos y un asignatura-modulo pertenece a una asignatura VER
            builder.HasMany(x => x.AsignaturaModulos)
             .WithOne(am => am.Asignatura)
             .HasForeignKey(am => am.AsignaturaId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}