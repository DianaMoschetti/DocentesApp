using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class HistorialCambioConfig : IEntityTypeConfiguration<HistorialCambio>
    {
        public void Configure(EntityTypeBuilder<HistorialCambio> builder)
        {
            builder.ToTable("HistorialCambios");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.FechaUtc).IsRequired().HasColumnType("datetime2");
            builder.Property(h => h.OperacionId).IsRequired();
            builder.Property(h => h.UsuarioId).HasMaxLength(450); // mismo largo que AspNetUsers.Id (sin FK)
            builder.Property(h => h.UsuarioNombre).HasMaxLength(256);
            builder.Property(h => h.UsuarioEmail).HasMaxLength(256);
            builder.Property(h => h.Entidad).IsRequired().HasMaxLength(100);
            builder.Property(h => h.EntidadId).IsRequired().HasMaxLength(200);
            builder.Property(h => h.EntidadDescripcion).HasMaxLength(500);
            builder.Property(h => h.Accion).IsRequired();

            // Sin FKs a las tablas de negocio: DocenteId / DesignacionId son solo vínculos para filtrar

            // Si se borra la cabecera se borran sus detalles
            builder.HasMany(h => h.Detalles)
                .WithOne(d => d.HistorialCambio)
                .HasForeignKey(d => d.HistorialCambioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para las consultas de la pantalla de auditoría y de las pestañas Historial
            builder.HasIndex(h => h.FechaUtc).IsDescending();
            builder.HasIndex(h => new { h.DocenteId, h.FechaUtc });
            builder.HasIndex(h => new { h.DesignacionId, h.FechaUtc });
            builder.HasIndex(h => new { h.Entidad, h.EntidadId });
            builder.HasIndex(h => new { h.UsuarioId, h.FechaUtc });
        }
    }
}
