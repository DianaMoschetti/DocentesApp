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
    // [OBSOLETO v4.0] Esta configuración corresponde a la entidad Cargo que fue
    // reemplazada en el refactor v4.0. Los atributos de cargo pasan a DetalleDesignacion.
    // Mantener hasta completar el refactor completo.
    [Obsolete("CargoConfig obsoleta desde v4.0. Los atributos de cargo pasan a DetalleDesignacion. No usar en código nuevo.")]
    public class CargoConfig : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> builder)
        {
            // v5: Designacion ya no tiene CargoId. Sin esto EF crea por convención la columna sombra
            // Designaciones.CargoId (FK a Cargos) a partir de Cargo.Designaciones.
            builder.Ignore(c => c.Designaciones);
        }

    }
}
