using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedPuntosPorCargo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PuntosPorCargo",
                columns: new[] { "Id", "Denominacion", "PuntosBase", "TipoCargo" },
                values: new object[,]
                {
                    { 1, 1, 138m, 1 },
                    { 2, 1, 157m, 2 },
                    { 3, 1, 176m, 3 },
                    { 4, 2, 119m, 1 },
                    { 5, 3, 100m, 1 },
                    { 6, 4, 80m, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PuntosPorCargo",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PuntosPorCargo",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PuntosPorCargo",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PuntosPorCargo",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PuntosPorCargo",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PuntosPorCargo",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
