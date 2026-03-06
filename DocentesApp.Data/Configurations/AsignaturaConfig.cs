using DocentesApp.Model;
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

            // Los enums son ints por defecto, no hace falta convertirlos
            builder.Property(a => a.NombreAsignatura).IsRequired();
            builder.Property(a => a.Frecuencia).IsRequired();
            builder.Property(a => a.Nivel).IsRequired();

            // Relaciones
            // Asignatura pertenece a Udb 
            builder.HasOne(x => x.Udb)
             .WithMany(u => u.Asignaturas)
             .HasForeignKey(x => x.UdbId)
             .OnDelete(DeleteBehavior.SetNull);

            // Unicidad lógica: una asignatura no se puede repetir en una misma udb con el mismo nombre y nivel
            builder.HasIndex(x => new { x.UdbId, x.NombreAsignatura, x.Nivel })
             .IsUnique();

            // Una asignatura tiene muchas designaciones y una designación pertenece a una asignatura
            builder.HasMany(x => x.Designaciones)
            .WithOne(d => d.Asignatura)
            .HasForeignKey(d => d.AsignaturaId)
            .OnDelete(DeleteBehavior.Restrict);

            // Una asignatura tiene muchos asignatura-modulos y un asignatura-modulo pertenece a una asignatura VER
            builder.HasMany(x => x.AsignaturaModulos)
             .WithOne(am => am.Asignatura)
             .HasForeignKey(am => am.AsignaturaId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
