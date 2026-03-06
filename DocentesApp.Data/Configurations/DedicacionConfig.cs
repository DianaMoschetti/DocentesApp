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
    public class DedicacionConfig : IEntityTypeConfiguration<Dedicacion>
    {
        public void Configure(EntityTypeBuilder<Dedicacion> builder) 
        { 
            builder.ToTable("Dedicaciones");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.CantidadHoras).IsRequired();
            builder.Property(d => d.CantidadDedicacion).IsRequired();

            //builder.HasIndex(d => new { d.DescTipo, d.CantidadHoras, d.CantidadDedicacion })
            // .IsUnique(); LA COMBINACION DE ESTOS CAMPOS SI SE PUEDE REPETIR.
        }
    }
}
