using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MantenimientoFlota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogosYAdministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosActivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosActivo", x => x.Id);
                    table.UniqueConstraint("AK_EstadosActivo_Codigo", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "EstadosOrdenMantenimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosOrdenMantenimiento", x => x.Id);
                    table.UniqueConstraint("AK_EstadosOrdenMantenimiento_Codigo", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoAccesoUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EstadosActivo",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, 0, "Operativo" },
                    { 2, 1, "En Mantenimiento" },
                    { 3, 2, "Fuera de Servicio" },
                    { 4, 3, "Dado de Baja" }
                });

            migrationBuilder.InsertData(
                table: "EstadosOrdenMantenimiento",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, 0, "Solicitada" },
                    { 2, 1, "Diagnosticada" },
                    { 3, 2, "Aprobada" },
                    { 4, 3, "Rechazada" },
                    { 5, 4, "En Ejecución" },
                    { 6, 5, "En Espera de Repuesto" },
                    { 7, 6, "Completada" },
                    { 8, 7, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Coordinador/Técnico" },
                    { 3, "Solicitante/Consulta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMantenimiento_EstadoActual",
                table: "OrdenesMantenimiento",
                column: "EstadoActual");

            migrationBuilder.CreateIndex(
                name: "IX_Activos_Estado",
                table: "Activos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosActivo_Nombre",
                table: "EstadosActivo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosOrdenMantenimiento_Nombre",
                table: "EstadosOrdenMantenimiento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activos_EstadosActivo_Estado",
                table: "Activos",
                column: "Estado",
                principalTable: "EstadosActivo",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesMantenimiento_EstadosOrdenMantenimiento_EstadoActual",
                table: "OrdenesMantenimiento",
                column: "EstadoActual",
                principalTable: "EstadosOrdenMantenimiento",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activos_EstadosActivo_Estado",
                table: "Activos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesMantenimiento_EstadosOrdenMantenimiento_EstadoActual",
                table: "OrdenesMantenimiento");

            migrationBuilder.DropTable(
                name: "EstadosActivo");

            migrationBuilder.DropTable(
                name: "EstadosOrdenMantenimiento");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesMantenimiento_EstadoActual",
                table: "OrdenesMantenimiento");

            migrationBuilder.DropIndex(
                name: "IX_Activos_Estado",
                table: "Activos");
        }
    }
}
