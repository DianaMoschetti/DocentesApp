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

            builder.HasOne(d => d.Docente)
                .WithMany(doc => doc.Designaciones)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Cargo)
                .WithMany(c => c.Designaciones)
                .HasForeignKey(d => d.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Dedicacion)
               .WithMany()
               .HasForeignKey(d => d.DedicacionId)
               .OnDelete(DeleteBehavior.Restrict);

            // asi se hace la relacion con asignatura, no es necesario agregar una propiedad de navegacion
            // en Asignatura porque no es necesario navegar desde Asignatura a Designacion
            builder.HasOne(d => d.Asignatura)
               .WithMany()
               .HasForeignKey(d => d.AsignaturaId)
               .OnDelete(DeleteBehavior.Restrict);

            // curso es opcional
            builder.HasIndex(d => d.CursoId);

            // Indices para consultar planta actual o planta historica
            builder.HasIndex(d => new { d.DocenteId, d.FechaFin });
            builder.HasIndex(d => new { d.DocenteId, d.FechaInicio }); // ver
            builder.HasIndex(d => new { d.AsignaturaId, d.FechaInicio }); //ver
            builder.HasIndex(d => new { d.EstadoDesignacion, d.FechaInicio });

            // Evita “dos activas iguales” (heurístico): mismas claves con FechaFin null
            // (SQL Server soporta índice filtrado; EF lo genera)
            builder.HasIndex(d => new { d.DocenteId, d.CargoId, d.AsignaturaId, d.CursoId })
             .HasFilter("[FechaFin] IS NULL");

        }
    }
}
