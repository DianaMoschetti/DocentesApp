using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorV4_ModeloCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designaciones_Cargos_CargoId",
                table: "Designaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Designaciones_Dedicaciones_DedicacionId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_DedicacionId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_DocenteId_CargoId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Denominacion_TipoCargo_Condicion",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Asignaturas_UdbId_NombreAsignatura_Nivel",
                table: "Asignaturas");

            migrationBuilder.DropColumn(
                name: "PuntosUtilizados",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "DedicacionId",
                table: "Designaciones");

            migrationBuilder.DropColumn(
                name: "NombreAsignatura",
                table: "Asignaturas");

            migrationBuilder.AddColumn<float>(
                name: "CantidadDedicacion",
                table: "DetallesDesignacion",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Condicion",
                table: "DetallesDesignacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Denominacion",
                table: "DetallesDesignacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "DetallesDesignacion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PuntosAsignados",
                table: "DetallesDesignacion",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TipoCargo",
                table: "DetallesDesignacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoDedicacion",
                table: "DetallesDesignacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CargoId",
                table: "Designaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "Cargos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsVigente",
                table: "Asignaturas",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Asignaturas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PuntosPorCargo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denominacion = table.Column<int>(type: "int", nullable: false),
                    TipoCargo = table.Column<int>(type: "int", nullable: false),
                    PuntosBase = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntosPorCargo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asignaturas_UdbId_Nombre_Nivel",
                table: "Asignaturas",
                columns: new[] { "UdbId", "Nombre", "Nivel" },
                unique: true,
                filter: "[UdbId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PuntosPorCargo_Denominacion_TipoCargo",
                table: "PuntosPorCargo",
                columns: new[] { "Denominacion", "TipoCargo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Designaciones_Cargos_CargoId",
                table: "Designaciones",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designaciones_Cargos_CargoId",
                table: "Designaciones");

            migrationBuilder.DropTable(
                name: "PuntosPorCargo");

            migrationBuilder.DropIndex(
                name: "IX_Asignaturas_UdbId_Nombre_Nivel",
                table: "Asignaturas");

            migrationBuilder.DropColumn(
                name: "CantidadDedicacion",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "Condicion",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "Denominacion",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "PuntosAsignados",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "TipoCargo",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "TipoDedicacion",
                table: "DetallesDesignacion");

            migrationBuilder.DropColumn(
                name: "EsVigente",
                table: "Asignaturas");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Asignaturas");

            migrationBuilder.AddColumn<decimal>(
                name: "PuntosUtilizados",
                table: "DetallesDesignacion",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CargoId",
                table: "Designaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DedicacionId",
                table: "Designaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "Cargos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NombreAsignatura",
                table: "Asignaturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DedicacionId",
                table: "Designaciones",
                column: "DedicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DocenteId_CargoId",
                table: "Designaciones",
                columns: new[] { "DocenteId", "CargoId" },
                unique: true,
                filter: "[FechaFin] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Denominacion_TipoCargo_Condicion",
                table: "Cargos",
                columns: new[] { "Denominacion", "TipoCargo", "Condicion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asignaturas_UdbId_NombreAsignatura_Nivel",
                table: "Asignaturas",
                columns: new[] { "UdbId", "NombreAsignatura", "Nivel" },
                unique: true,
                filter: "[UdbId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Designaciones_Cargos_CargoId",
                table: "Designaciones",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Designaciones_Dedicaciones_DedicacionId",
                table: "Designaciones",
                column: "DedicacionId",
                principalTable: "Dedicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
