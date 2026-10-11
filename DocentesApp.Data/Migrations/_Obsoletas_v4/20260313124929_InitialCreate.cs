using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denominacion = table.Column<int>(type: "int", nullable: false),
                    TipoCargo = table.Column<int>(type: "int", nullable: false),
                    DetalleCargo = table.Column<int>(type: "int", nullable: false),
                    Condicion = table.Column<int>(type: "int", nullable: false),
                    PuntosBase = table.Column<float>(type: "real", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    NroComision = table.Column<int>(type: "int", nullable: false),
                    Año = table.Column<int>(type: "int", nullable: false),
                    Carrera = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dedicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescTipo = table.Column<int>(type: "int", nullable: false),
                    CantidadHoras = table.Column<float>(type: "real", nullable: false),
                    CantidadDedicacion = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dedicaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Legajo = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<int>(type: "int", nullable: false),
                    MaxNivelAcademico = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dni = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmailAlternativo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Celular = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantaSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fuente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantaSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Laboratorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Lugar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DirectorDocenteId = table.Column<int>(type: "int", nullable: true),
                    DirectorId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laboratorios_Docentes_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Docentes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Udbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DirectorDocenteId = table.Column<int>(type: "int", nullable: true),
                    SecretarioDocenteId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Udbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Udbs_Docentes_DirectorDocenteId",
                        column: x => x.DirectorDocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Udbs_Docentes_SecretarioDocenteId",
                        column: x => x.SecretarioDocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocentesLaboratorios",
                columns: table => new
                {
                    DocentesId = table.Column<int>(type: "int", nullable: false),
                    LaboratorioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocentesLaboratorios", x => new { x.DocentesId, x.LaboratorioId });
                    table.ForeignKey(
                        name: "FK_DocentesLaboratorios_Docentes_DocentesId",
                        column: x => x.DocentesId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocentesLaboratorios_Laboratorios_LaboratorioId",
                        column: x => x.LaboratorioId,
                        principalTable: "Laboratorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asignaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreAsignatura = table.Column<int>(type: "int", nullable: false),
                    Frecuencia = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    UdbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asignaturas_Udbs_UdbId",
                        column: x => x.UdbId,
                        principalTable: "Udbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocentesUdb",
                columns: table => new
                {
                    DocenteId = table.Column<int>(type: "int", nullable: false),
                    UdbId = table.Column<int>(type: "int", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocentesUdb", x => new { x.DocenteId, x.UdbId });
                    table.ForeignKey(
                        name: "FK_DocentesUdb_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocentesUdb_Udbs_UdbId",
                        column: x => x.UdbId,
                        principalTable: "Udbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AsignaturaModulos",
                columns: table => new
                {
                    AsignaturaId = table.Column<int>(type: "int", nullable: false),
                    Modulo = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturaModulos", x => new { x.AsignaturaId, x.CursoId, x.Modulo });
                    table.ForeignKey(
                        name: "FK_AsignaturaModulos_Asignaturas_AsignaturaId",
                        column: x => x.AsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignaturaModulos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Designaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteId = table.Column<int>(type: "int", nullable: false),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    AsignaturaId = table.Column<int>(type: "int", nullable: true),
                    DedicacionId = table.Column<int>(type: "int", nullable: false),
                    NroResolucion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NroNota = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PuntosUtilizados = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PuntosLibres = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EstadoDesignacion = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Designaciones_Asignaturas_AsignaturaId",
                        column: x => x.AsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Designaciones_Cargos_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Designaciones_Dedicaciones_DedicacionId",
                        column: x => x.DedicacionId,
                        principalTable: "Dedicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Designaciones_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlantaDocenteSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantaSnapshotId = table.Column<int>(type: "int", nullable: false),
                    DocenteId = table.Column<int>(type: "int", nullable: true),
                    AsignaturaId = table.Column<int>(type: "int", nullable: true),
                    UdbId = table.Column<int>(type: "int", nullable: true),
                    Ano = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Division = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EspecialidadTextoOriginal = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Especialidad = table.Column<int>(type: "int", nullable: true),
                    MateriaEnum = table.Column<int>(type: "int", nullable: true),
                    MateriaTextoOriginal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoriaDocente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DedicacionTexto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowKey = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantaDocenteSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantaDocenteSnapshots_Asignaturas_AsignaturaId",
                        column: x => x.AsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantaDocenteSnapshots_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantaDocenteSnapshots_PlantaSnapshots_PlantaSnapshotId",
                        column: x => x.PlantaSnapshotId,
                        principalTable: "PlantaSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantaDocenteSnapshots_Udbs_UdbId",
                        column: x => x.UdbId,
                        principalTable: "Udbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturaModulos_AsignaturaId",
                table: "AsignaturaModulos",
                column: "AsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturaModulos_CursoId",
                table: "AsignaturaModulos",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaturas_UdbId_NombreAsignatura_Nivel",
                table: "Asignaturas",
                columns: new[] { "UdbId", "NombreAsignatura", "Nivel" },
                unique: true,
                filter: "[UdbId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Denominacion_TipoCargo_DetalleCargo_Condicion",
                table: "Cargos",
                columns: new[] { "Denominacion", "TipoCargo", "DetalleCargo", "Condicion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Turno_Año_Carrera_NroComision",
                table: "Cursos",
                columns: new[] { "Turno", "Año", "Carrera", "NroComision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_AsignaturaId_FechaInicio",
                table: "Designaciones",
                columns: new[] { "AsignaturaId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_CargoId",
                table: "Designaciones",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DedicacionId",
                table: "Designaciones",
                column: "DedicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DocenteId_CargoId_AsignaturaId_CursoId",
                table: "Designaciones",
                columns: new[] { "DocenteId", "CargoId", "AsignaturaId", "CursoId" },
                unique: true,
                filter: "[FechaFin] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DocenteId_FechaFin",
                table: "Designaciones",
                columns: new[] { "DocenteId", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_DocenteId_FechaInicio",
                table: "Designaciones",
                columns: new[] { "DocenteId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_Designaciones_EstadoDesignacion_FechaInicio",
                table: "Designaciones",
                columns: new[] { "EstadoDesignacion", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Dni",
                table: "Docentes",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Legajo",
                table: "Docentes",
                column: "Legajo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocentesLaboratorios_LaboratorioId",
                table: "DocentesLaboratorios",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_DocentesUdb_UdbId",
                table: "DocentesUdb",
                column: "UdbId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratorios_DirectorId",
                table: "Laboratorios",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratorios_Nombre",
                table: "Laboratorios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantaDocenteSnapshots_AsignaturaId",
                table: "PlantaDocenteSnapshots",
                column: "AsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantaDocenteSnapshots_DocenteId",
                table: "PlantaDocenteSnapshots",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantaDocenteSnapshots_PlantaSnapshotId",
                table: "PlantaDocenteSnapshots",
                column: "PlantaSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantaDocenteSnapshots_PlantaSnapshotId_RowKey",
                table: "PlantaDocenteSnapshots",
                columns: new[] { "PlantaSnapshotId", "RowKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantaDocenteSnapshots_UdbId",
                table: "PlantaDocenteSnapshots",
                column: "UdbId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantaSnapshots_IsCurrent",
                table: "PlantaSnapshots",
                column: "IsCurrent",
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PlantaSnapshots_SnapshotDate",
                table: "PlantaSnapshots",
                column: "SnapshotDate");

            migrationBuilder.CreateIndex(
                name: "IX_Udbs_DirectorDocenteId",
                table: "Udbs",
                column: "DirectorDocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Udbs_Nombre",
                table: "Udbs",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Udbs_SecretarioDocenteId",
                table: "Udbs",
                column: "SecretarioDocenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignaturaModulos");

            migrationBuilder.DropTable(
                name: "Designaciones");

            migrationBuilder.DropTable(
                name: "DocentesLaboratorios");

            migrationBuilder.DropTable(
                name: "DocentesUdb");

            migrationBuilder.DropTable(
                name: "PlantaDocenteSnapshots");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Dedicaciones");

            migrationBuilder.DropTable(
                name: "Laboratorios");

            migrationBuilder.DropTable(
                name: "Asignaturas");

            migrationBuilder.DropTable(
                name: "PlantaSnapshots");

            migrationBuilder.DropTable(
                name: "Udbs");

            migrationBuilder.DropTable(
                name: "Docentes");
        }
    }
}
