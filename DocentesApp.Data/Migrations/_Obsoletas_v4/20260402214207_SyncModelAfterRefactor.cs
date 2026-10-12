using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelAfterRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CursoId",
                table: "Designaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_CursoId",
                table: "Designaciones",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designaciones_Cursos_CursoId",
                table: "Designaciones",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designaciones_Cursos_CursoId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_CursoId",
                table: "Designaciones");

            migrationBuilder.AlterColumn<int>(
                name: "CursoId",
                table: "Designaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
