using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdenMantenimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenesMantenimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActivoId = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TipoMantenimiento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Prioridad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CostoTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstadoActual = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesMantenimiento", x => x.Id);
                    table.CheckConstraint("CK_OrdenesMantenimiento_CostoTotal_NoNegativo", "[CostoTotal] >= 0");
                    table.ForeignKey(
                        name: "FK_OrdenesMantenimiento_Activos_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "Activos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMantenimiento_ActivoId",
                table: "OrdenesMantenimiento",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMantenimiento_Codigo",
                table: "OrdenesMantenimiento",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenesMantenimiento");
        }
    }
}
