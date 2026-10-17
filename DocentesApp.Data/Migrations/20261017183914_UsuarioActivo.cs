using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                // [Diana desde v4.0 OBSOLETO] generado por EF: dejaba inactivos a todos los usuarios existentes (admin incluido)
                //defaultValue: false);
                // Los usuarios que ya existen quedan activos. El default solo va acá (no en el modelo): con
                // HasDefaultValue(true) EF no mandaría el false en un INSERT y la base guardaría true.
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AspNetUsers");
        }
    }
}
