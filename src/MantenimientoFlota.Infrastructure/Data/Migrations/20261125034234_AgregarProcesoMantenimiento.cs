using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProcesoMantenimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EspecialidadRequeridaId",
                table: "OrdenesMantenimiento",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistorialEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenMantenimientoId = table.Column<int>(type: "int", nullable: true),
                    ActivoId = table.Column<int>(type: "int", nullable: true),
                    EstadoAnterior = table.Column<int>(type: "int", nullable: true),
                    EstadoNuevo = table.Column<int>(type: "int", nullable: false),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstados", x => x.Id);
                    table.CheckConstraint("CK_HistorialEstados_Recurso", "([OrdenMantenimientoId] IS NOT NULL AND [ActivoId] IS NULL) OR ([OrdenMantenimientoId] IS NULL AND [ActivoId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Activos_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "Activos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_OrdenesMantenimiento_OrdenMantenimientoId",
                        column: x => x.OrdenMantenimientoId,
                        principalTable: "OrdenesMantenimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepuestosOrden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenMantenimientoId = table.Column<int>(type: "int", nullable: false),
                    RepuestoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Pendiente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepuestosOrden", x => x.Id);
                    table.CheckConstraint("CK_RepuestosOrden_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_RepuestosOrden_Costo", "[CostoUnitario] >= 0");
                    table.ForeignKey(
                        name: "FK_RepuestosOrden_OrdenesMantenimiento_OrdenMantenimientoId",
                        column: x => x.OrdenMantenimientoId,
                        principalTable: "OrdenesMantenimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepuestosOrden_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TecnicosOrden",
                columns: table => new
                {
                    OrdenMantenimientoId = table.Column<int>(type: "int", nullable: false),
                    TecnicoId = table.Column<int>(type: "int", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TarifaPorHora = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaAsignacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TecnicosOrden", x => new { x.OrdenMantenimientoId, x.TecnicoId });
                    table.CheckConstraint("CK_TecnicosOrden_Horas", "[HorasTrabajadas] > 0");
                    table.CheckConstraint("CK_TecnicosOrden_Tarifa", "[TarifaPorHora] >= 0");
                    table.ForeignKey(
                        name: "FK_TecnicosOrden_OrdenesMantenimiento_OrdenMantenimientoId",
                        column: x => x.OrdenMantenimientoId,
                        principalTable: "OrdenesMantenimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TecnicosOrden_Tecnicos_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Tecnicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMantenimiento_EspecialidadRequeridaId",
                table: "OrdenesMantenimiento",
                column: "EspecialidadRequeridaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_ActivoId",
                table: "HistorialEstados",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_OrdenMantenimientoId",
                table: "HistorialEstados",
                column: "OrdenMantenimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_UsuarioId",
                table: "HistorialEstados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RepuestosOrden_OrdenMantenimientoId",
                table: "RepuestosOrden",
                column: "OrdenMantenimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_RepuestosOrden_RepuestoId",
                table: "RepuestosOrden",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_TecnicosOrden_TecnicoId",
                table: "TecnicosOrden",
                column: "TecnicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesMantenimiento_Especialidades_EspecialidadRequeridaId",
                table: "OrdenesMantenimiento",
                column: "EspecialidadRequeridaId",
                principalTable: "Especialidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesMantenimiento_Especialidades_EspecialidadRequeridaId",
                table: "OrdenesMantenimiento");

            migrationBuilder.DropTable(
                name: "HistorialEstados");

            migrationBuilder.DropTable(
                name: "RepuestosOrden");

            migrationBuilder.DropTable(
                name: "TecnicosOrden");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesMantenimiento_EspecialidadRequeridaId",
                table: "OrdenesMantenimiento");

            migrationBuilder.DropColumn(
                name: "EspecialidadRequeridaId",
                table: "OrdenesMantenimiento");
        }
    }
}
