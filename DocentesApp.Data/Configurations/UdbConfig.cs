using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DocentesApp.Data.Configurations
{
    public class UdbConfig : IEntityTypeConfiguration<Udb>
    {
        public void Configure(EntityTypeBuilder<Udb> builder)
        {
            builder.ToTable("Udbs");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nombre).IsRequired().HasMaxLength(300);
            builder.HasIndex(u => u.Nombre).IsUnique();

            // FK a docente por director y secretario
            builder.HasOne(u => u.Director)
                .WithMany()
                .HasForeignKey(u => u.DirectorDocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Secretario)  
                .WithMany()
                .HasForeignKey(u => u.SecretarioDocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.DocenteUdbs)
            .WithOne(x => x.Udb)
            .HasForeignKey(x => x.UdbId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
