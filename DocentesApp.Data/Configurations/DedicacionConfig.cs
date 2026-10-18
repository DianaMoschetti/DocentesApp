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
        }
    }
}
