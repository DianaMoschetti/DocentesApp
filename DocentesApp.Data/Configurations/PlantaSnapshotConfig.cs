using DocentesApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class PlantaSnapshotConfig : IEntityTypeConfiguration<PlantaSnapshot>
    {
        public void Configure(EntityTypeBuilder<PlantaSnapshot> builder)
        {
            builder.ToTable("PlantaSnapshots");
            builder.HasKey(x => x.Id);

            builder.Property(ps =>ps.SnapshotDate).IsRequired();
            builder.Property(ps => ps.Fuente).IsRequired().HasMaxLength(200);

            builder.HasIndex(ps => ps.SnapshotDate);
           
            builder.HasIndex(ps => ps.IsCurrent)
                .IsUnique()
                .HasFilter("[IsCurrent] = 1"); // Asegura que solo un registro pueda tener IsCurrent = true

            builder.HasMany(ps => ps.Filas)
                .WithOne(f => f.PlantaSnapshot)
                .HasForeignKey(f => f.PlantaSnapshotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
