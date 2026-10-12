using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocentesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denominacion = table.Column<int>(type: "int", nullable: false),
                    TipoCargo = table.Column<int>(type: "int", nullable: false),
                    Condicion = table.Column<int>(type: "int", nullable: false),
                    PuntosBase = table.Column<float>(type: "real", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    Legajo = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<int>(type: "int", nullable: false),
                    MaxNivelAcademico = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dni = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
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
                name: "PuntosPorCargo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cargo = table.Column<int>(type: "int", nullable: false),
                    PuntosBase = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntosPorCargo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
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
                    NroResolucion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NroNota = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstadoDesignacion = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Designaciones_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laboratorios_Docentes_DirectorDocenteId",
                        column: x => x.DirectorDocenteId,
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
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EsVigente = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Frecuencia = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    UdbId = table.Column<int>(type: "int", nullable: false)
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
                name: "CargosDesignacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignacionId = table.Column<int>(type: "int", nullable: false),
                    Cargo = table.Column<int>(type: "int", nullable: false),
                    Condicion = table.Column<int>(type: "int", nullable: false),
                    TipoDedicacion = table.Column<int>(type: "int", nullable: false),
                    CantidadDedicacion = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    UdbId = table.Column<int>(type: "int", nullable: false),
                    Puntos = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EnLicencia = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReemplazaACargoId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargosDesignacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargosDesignacion_CargosDesignacion_ReemplazaACargoId",
                        column: x => x.ReemplazaACargoId,
                        principalTable: "CargosDesignacion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CargosDesignacion_Designaciones_DesignacionId",
                        column: x => x.DesignacionId,
                        principalTable: "Designaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CargosDesignacion_Udbs_UdbId",
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

            migrationBuilder.CreateTable(
                name: "Actividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoDesignacionId = table.Column<int>(type: "int", nullable: false),
                    Especificacion = table.Column<int>(type: "int", nullable: false),
                    AsignaturaId = table.Column<int>(type: "int", nullable: true),
                    CursoId = table.Column<int>(type: "int", nullable: true),
                    Horario = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actividades_Asignaturas_AsignaturaId",
                        column: x => x.AsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Actividades_CargosDesignacion_CargoDesignacionId",
                        column: x => x.CargoDesignacionId,
                        principalTable: "CargosDesignacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actividades_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PuntosPorCargo",
                columns: new[] { "Id", "Cargo", "PuntosBase" },
                values: new object[,]
                {
                    { 1, 1, 138m },
                    { 2, 2, 157m },
                    { 3, 3, 176m },
                    { 4, 4, 119m },
                    { 5, 5, 100m },
                    { 6, 6, 80m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_AsignaturaId",
                table: "Actividades",
                column: "AsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_CargoDesignacionId",
                table: "Actividades",
                column: "CargoDesignacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_CursoId",
                table: "Actividades",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturaModulos_AsignaturaId",
                table: "AsignaturaModulos",
                column: "AsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturaModulos_CursoId",
                table: "AsignaturaModulos",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaturas_UdbId_Nombre_Nivel",
                table: "Asignaturas",
                columns: new[] { "UdbId", "Nombre", "Nivel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CargosDesignacion_DesignacionId",
                table: "CargosDesignacion",
                column: "DesignacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CargosDesignacion_ReemplazaACargoId",
                table: "CargosDesignacion",
                column: "ReemplazaACargoId");

            migrationBuilder.CreateIndex(
                name: "IX_CargosDesignacion_UdbId",
                table: "CargosDesignacion",
                column: "UdbId");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Turno_Año_Carrera_NroComision",
                table: "Cursos",
                columns: new[] { "Turno", "Año", "Carrera", "NroComision" },
                unique: true);

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
                unique: true,
                filter: "[Dni] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Legajo",
                table: "Docentes",
                column: "Legajo",
                unique: true,
                filter: "[Legajo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DocentesLaboratorios_LaboratorioId",
                table: "DocentesLaboratorios",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_DocentesUdb_UdbId",
                table: "DocentesUdb",
                column: "UdbId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratorios_DirectorDocenteId",
                table: "Laboratorios",
                column: "DirectorDocenteId");

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
                name: "IX_PuntosPorCargo_Cargo",
                table: "PuntosPorCargo",
                column: "Cargo",
                unique: true);

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
                name: "Actividades");

            migrationBuilder.DropTable(
                name: "AsignaturaModulos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Dedicaciones");

            migrationBuilder.DropTable(
                name: "DocentesLaboratorios");

            migrationBuilder.DropTable(
                name: "DocentesUdb");

            migrationBuilder.DropTable(
                name: "PlantaDocenteSnapshots");

            migrationBuilder.DropTable(
                name: "PuntosPorCargo");

            migrationBuilder.DropTable(
                name: "CargosDesignacion");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Laboratorios");

            migrationBuilder.DropTable(
                name: "Asignaturas");

            migrationBuilder.DropTable(
                name: "PlantaSnapshots");

            migrationBuilder.DropTable(
                name: "Designaciones");

            migrationBuilder.DropTable(
                name: "Udbs");

            migrationBuilder.DropTable(
                name: "Docentes");
        }
    }
}
