using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DocentesApp.Data.Configurations
{
    // [OBSOLETO v4.0] Esta configuración corresponde a la entidad Dedicacion que fue
    // reemplazada en el refactor v4.0. Los atributos de dedicación pasan a DetalleDesignacion.
    // Mantener hasta completar el refactor completo.
    [Obsolete("DedicacionConfig obsoleta desde v4.0. Los atributos de dedicación pasan a DetalleDesignacion. No usar en código nuevo.")]
    public class DedicacionConfig : IEntityTypeConfiguration<Dedicacion>
    {
        public void Configure(EntityTypeBuilder<Dedicacion> builder)
        {
            //builder.ToTable("Dedicaciones");
            //builder.HasKey(d => d.Id);

            //builder.Property(d => d.CantidadHoras).IsRequired();
            //builder.Property(d => d.CantidadDedicacion).IsRequired();

            ////builder.HasIndex(d => new { d.DescTipo, d.CantidadHoras, d.CantidadDedicacion })
            //// .IsUnique(); LA COMBINACION DE ESTOS CAMPOS SI SE PUEDE REPETIR.
        }
    }
}
