using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Data.Configurations
{
    public class LaboratorioConfig : IEntityTypeConfiguration<Laboratorio>
    {
        public void Configure(EntityTypeBuilder<Laboratorio> builder)
        {
            builder.ToTable("Laboratorios");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Nombre).IsRequired().HasMaxLength(300);
            builder.Property(l => l.Lugar).IsRequired().HasMaxLength(100);

            // many to many con docentes ef crea una tabla puente automaticamente
            // ver mas adelante si quiero algun dato extra tendria que crear la tabla/objeto explicitamente
            builder.HasMany(l => l.Docentes)
                .WithMany()
                .UsingEntity(j => j.ToTable("DocentesLaboratorios")); //tabla puente

            builder.HasIndex(l => l.Nombre).IsUnique(); //no puede haber dos laboratorios con el mismo nombre
        }
    }
}
