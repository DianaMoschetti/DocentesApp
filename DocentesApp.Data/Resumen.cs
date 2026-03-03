using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Data
{
    internal class Resumen
    {
        /* Referencias a otros proyectos:
         *  ✅ Referencia a DocentesEFCore.Domain

            ✅ Referencia a DocentesApp.Application solo si acá implementás interfaces definidas en Application (ej. repositorios).


         * Paquetes a instalar:
            Microsoft.EntityFrameworkCore

            Microsoft.EntityFrameworkCore.SqlServer

            Microsoft.EntityFrameworkCore.Design (para migraciones, normalmente en Data o en API)

            Microsoft.EntityFrameworkCore.Tools
         * 
         * CONTENIDO:
         * DocentesDbContext

            Configurations/ (clases IEntityTypeConfiguration<T>)

            Migrations/

            Repositories/ (implementaciones EF de interfaces)

            Seed/ (datos iniciales: cargos base, dedicaciones base, etc.)
         * 
         * 
         * 
         * */
    }
}
