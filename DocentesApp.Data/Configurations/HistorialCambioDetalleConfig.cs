using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class HistorialCambioDetalleConfig : IEntityTypeConfiguration<HistorialCambioDetalle>
    {
        public void Configure(EntityTypeBuilder<HistorialCambioDetalle> builder)
        {
            builder.ToTable("HistorialCambioDetalles");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Propiedad).IsRequired().HasMaxLength(100);
            builder.Property(d => d.ValorAnterior); // nvarchar(max) null
            builder.Property(d => d.ValorNuevo); // nvarchar(max) null

            // FK a la cabecera (cascade) configurada en HistorialCambioConfig; índice de la FK
            builder.HasIndex(d => d.HistorialCambioId);
        }
    }
}
