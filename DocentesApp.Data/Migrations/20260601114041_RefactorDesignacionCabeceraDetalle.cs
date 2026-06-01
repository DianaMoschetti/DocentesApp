using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDesignacionCabeceraDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designaciones_Asignaturas_AsignaturaId",
                table: "Designaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Designaciones_Cursos_CursoId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_AsignaturaId_FechaInicio",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_CursoId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_DocenteId_CargoId_AsignaturaId_CursoId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Denominacion_TipoCargo_DetalleCargo_Condicion",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "AsignaturaId",
                table: "Designaciones");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "Designaciones");

            migrationBuilder.DropColumn(
                name: "PuntosUtilizados",
                table: "Designaciones");

            migrationBuilder.DropColumn(
                name: "DetalleCargo",
                table: "Cargos");

            migrationBuilder.CreateTable(
                name: "DetallesDesignacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignacionId = table.Column<int>(type: "int", nullable: false),
                    Especificacion = table.Column<int>(type: "int", nullable: false),
                    AsignaturaId = table.Column<int>(type: "int", nullable: true),
                    CursoId = table.Column<int>(type: "int", nullable: true),
                    PuntosUtilizados = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesDesignacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesDesignacion_Asignaturas_AsignaturaId",
                        column: x => x.AsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesDesignacion_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesDesignacion_Designaciones_DesignacionId",
                        column: x => x.DesignacionId,
                        principalTable: "Designaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_DetallesDesignacion_AsignaturaId",
                table: "DetallesDesignacion",
                column: "AsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDesignacion_CursoId",
                table: "DetallesDesignacion",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDesignacion_DesignacionId_Especificacion",
                table: "DetallesDesignacion",
                columns: new[] { "DesignacionId", "Especificacion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesDesignacion");

            migrationBuilder.DropIndex(
                name: "IX_Designaciones_DocenteId_CargoId",
                table: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Denominacion_TipoCargo_Condicion",
                table: "Cargos");

            migrationBuilder.AddColumn<int>(
                name: "AsignaturaId",
                table: "Designaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CursoId",
                table: "Designaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PuntosUtilizados",
                table: "Designaciones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DetalleCargo",
                table: "Cargos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_AsignaturaId_FechaInicio",
                table: "Designaciones",
                columns: new[] { "AsignaturaId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_CursoId",
                table: "Designaciones",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DocenteId_CargoId_AsignaturaId_CursoId",
                table: "Designaciones",
                columns: new[] { "DocenteId", "CargoId", "AsignaturaId", "CursoId" },
                unique: true,
                filter: "[FechaFin] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Denominacion_TipoCargo_DetalleCargo_Condicion",
                table: "Cargos",
                columns: new[] { "Denominacion", "TipoCargo", "DetalleCargo", "Condicion" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Designaciones_Asignaturas_AsignaturaId",
                table: "Designaciones",
                column: "AsignaturaId",
                principalTable: "Asignaturas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Designaciones_Cursos_CursoId",
                table: "Designaciones",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
