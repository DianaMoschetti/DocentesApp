using DocentesApp.Data.Identity;
using DocentesApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Context
{
    public class DocentesDbContext : IdentityDbContext<ApplicationUser>
    {
        public DocentesDbContext(DbContextOptions<DocentesDbContext> options) : base(options)
        {
        }

        // This line defines a read-only property Docentes that retrieves the DbSet<Docente> from the
        // DbContext using the Set<Docente>() method. It allows you to interact with the Docente table in your database.
        public DbSet<Docente> Docentes => Set<Docente>(); // It’s a shorthand for writing a property with only a getter.

        // [Diana desde v4.0] Cargo reemplazado por atributos en DetalleDesignacion
        [Obsolete("Cargo obsoleto desde v4.0. No usar en código nuevo.")]
        public DbSet<Cargo> Cargos => Set<Cargo>();
        // [Diana desde v4.0] Dedicacion reemplazada por atributos en DetalleDesignacion
        [Obsolete("Dedicacion obsoleta desde v4.0. No usar en código nuevo.")]
        public DbSet<Dedicacion> Dedicaciones => Set<Dedicacion>();
        public DbSet<Designacion> Designaciones => Set<Designacion>();
        public DbSet<Udb> Udbs => Set<Udb>();
        public DbSet<Asignatura> Asignaturas => Set<Asignatura>();
        public DbSet<Curso> Cursos => Set<Curso>();
        public DbSet<AsignaturaModulo> AsignaturaModulos => Set<AsignaturaModulo>();
        public DbSet<DocenteUdb> DocenteUdbs => Set<DocenteUdb>();
        public DbSet<Laboratorio> Laboratorios => Set<Laboratorio>();
        public DbSet<PlantaSnapshot> PlantaSnapshots => Set<PlantaSnapshot>();
        public DbSet<PlantaDocenteSnapshot> PlantaDocenteSnapshots => Set<PlantaDocenteSnapshot>();
        public DbSet<DetalleDesignacion> DetalleDesignaciones => Set<DetalleDesignacion>();
        public DbSet<PuntosPorCargo> PuntosPorCargo => Set<PuntosPorCargo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas las IEntityTypeConfiguration<T> del assembly DocentesApp.Data
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocentesDbContext).Assembly);
        }

    }
}
