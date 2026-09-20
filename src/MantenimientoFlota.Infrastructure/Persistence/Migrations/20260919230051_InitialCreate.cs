using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasActivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IntervaloMantenimientoKm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasActivo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CondicionesGenerales = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repuestos", x => x.Id);
                    table.CheckConstraint("CK_Repuestos_StockActual_NoNegativo", "[StockActual] >= 0");
                    table.CheckConstraint("CK_Repuestos_StockMinimo_NoNegativo", "[StockMinimo] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    TarifaPorHora = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaAdquisicion = table.Column<DateOnly>(type: "date", nullable: false),
                    ValorReferencia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KilometrajeActual = table.Column<int>(type: "int", nullable: false),
                    CategoriaActivoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activos_CategoriasActivo_CategoriaActivoId",
                        column: x => x.CategoriaActivoId,
                        principalTable: "CategoriasActivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepuestoProveedores",
                columns: table => new
                {
                    RepuestoId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    PrecioOfrecido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TiempoEntregaDias = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepuestoProveedores", x => new { x.RepuestoId, x.ProveedorId });
                    table.CheckConstraint("CK_RepuestoProveedores_PrecioOfrecido_NoNegativo", "[PrecioOfrecido] >= 0");
                    table.CheckConstraint("CK_RepuestoProveedores_TiempoEntregaDias_NoNegativo", "[TiempoEntregaDias] >= 0");
                    table.ForeignKey(
                        name: "FK_RepuestoProveedores_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepuestoProveedores_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TecnicoEspecialidades",
                columns: table => new
                {
                    TecnicoId = table.Column<int>(type: "int", nullable: false),
                    EspecialidadId = table.Column<int>(type: "int", nullable: false),
                    NivelCertificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaObtencion = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TecnicoEspecialidades", x => new { x.TecnicoId, x.EspecialidadId });
                    table.ForeignKey(
                        name: "FK_TecnicoEspecialidades_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TecnicoEspecialidades_Tecnicos_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Tecnicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activos_CategoriaActivoId",
                table: "Activos",
                column: "CategoriaActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Activos_Codigo",
                table: "Activos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasActivo_Nombre",
                table: "CategoriasActivo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepuestoProveedores_ProveedorId",
                table: "RepuestoProveedores",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_TecnicoEspecialidades_EspecialidadId",
                table: "TecnicoEspecialidades",
                column: "EspecialidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activos");

            migrationBuilder.DropTable(
                name: "RepuestoProveedores");

            migrationBuilder.DropTable(
                name: "TecnicoEspecialidades");

            migrationBuilder.DropTable(
                name: "CategoriasActivo");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Repuestos");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "Tecnicos");
        }
    }
}
