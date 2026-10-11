using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class CargoDesignacionConfig : IEntityTypeConfiguration<CargoDesignacion>
    {
        public void Configure(EntityTypeBuilder<CargoDesignacion> builder)
        {
            builder.ToTable("CargosDesignacion");
            builder.HasKey(c => c.Id);

            // Los enums son ints por defecto
            builder.Property(c => c.Cargo).IsRequired();
            builder.Property(c => c.Condicion).IsRequired();
            builder.Property(c => c.TipoDedicacion).IsRequired();
            builder.Property(c => c.CantidadDedicacion).IsRequired().HasPrecision(4, 2); // 0,5 / 1 / 1,5 ...
            builder.Property(c => c.Puntos).IsRequired().HasPrecision(10, 2);
            builder.Property(c => c.EnLicencia).IsRequired().HasDefaultValue(false);
            builder.Property(c => c.Observaciones).HasMaxLength(500);

            // Cabecera: si se borra la designación se borran sus cargos
            builder.HasOne(c => c.Designacion)
                .WithMany(d => d.Cargos)
                .HasForeignKey(c => c.DesignacionId)
                .OnDelete(DeleteBehavior.Cascade);

            // UDB obligatoria; no se puede borrar una UDB con cargos
            builder.HasOne(c => c.Udb)
                .WithMany()
                .HasForeignKey(c => c.UdbId)
                .OnDelete(DeleteBehavior.Restrict);

            // Suplencia: auto-referencia al cargo en licencia que se reemplaza.
            // NoAction: SQL Server no admite cascadas múltiples sobre la misma tabla; el borrado de un cargo
            // reemplazado lo valida el service (409 si tiene suplentes).
            builder.HasOne(c => c.ReemplazaACargo)
                .WithMany(c => c.Reemplazos)
                .HasForeignKey(c => c.ReemplazaACargoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(c => c.DesignacionId);
            builder.HasIndex(c => c.UdbId); // planta por UDB
            builder.HasIndex(c => c.ReemplazaACargoId);
        }
    }
}
