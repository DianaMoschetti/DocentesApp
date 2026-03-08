using DocentesApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocentesApp.Data.Configurations
{
    public class DocenteUdbConfig : IEntityTypeConfiguration<DocenteUdb>
    {
        public void Configure(EntityTypeBuilder<DocenteUdb> builder)
        {
            builder.ToTable("DocentesUdb");
            //clave para no duplicar docente/udb
            builder.HasKey(du => new {du.DocenteId, du.UdbId});

            builder.HasOne(builder => builder.Docente)
                .WithMany(d => d.DocenteUdbs)
                .HasForeignKey(du => du.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(du => du.Udb)
                .WithMany(u => u.DocenteUdbs)
                .HasForeignKey(du => du.UdbId)
                .OnDelete(DeleteBehavior.Cascade);

            // Para listar docentes por udb
            builder.HasIndex(du => du.UdbId);
            
        }
    }
}
