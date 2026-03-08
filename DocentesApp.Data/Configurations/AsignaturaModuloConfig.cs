using DocentesApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class AsignaturaModuloConfig : IEntityTypeConfiguration<AsignaturaModulo>
    {
        public void Configure(EntityTypeBuilder<AsignaturaModulo> builder)
        {
            builder.ToTable("AsignaturaModulos");

            // Clave compuesta: evita duplicar Asignatura+Curso+Modulo
            builder.HasKey(am => new { am.AsignaturaId, am.CursoId, am.Modulo });

            builder.HasOne(am => am.Asignatura)
                .WithMany(a => a.AsignaturaModulos)
                .HasForeignKey(am => am.AsignaturaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(am => am.Curso)
                .WithMany(c => c.AsignaturaModulos)
                .HasForeignKey(am => am.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indices que capaz son utiles VER
            builder.HasIndex(am => am.CursoId);
            builder.HasIndex(am => am.AsignaturaId);

        }
    }
}
