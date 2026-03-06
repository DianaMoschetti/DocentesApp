using DocentesApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Data.Configurations
{
    public class CursoConfig : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.ToTable("Cursos");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.NroComision).IsRequired();

            // (Turno + Año + Carrera + NroComision) identifica un curso “real”
            builder.HasIndex(c => new { c.Turno, c.Año, c.Carrera, c.NroComision })
                .IsUnique();

            builder.HasMany(c => c.AsignaturaModulos)
                .WithOne(am => am.Curso)
                .HasForeignKey(am => am.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
