using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Tecnicos_TarifaPorHora_NoNegativa",
                table: "Tecnicos",
                sql: "[TarifaPorHora] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CategoriasActivo_IntervaloMantenimientoKm_NoNegativo",
                table: "CategoriasActivo",
                sql: "[IntervaloMantenimientoKm] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Tecnicos_TarifaPorHora_NoNegativa",
                table: "Tecnicos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CategoriasActivo_IntervaloMantenimientoKm_NoNegativo",
                table: "CategoriasActivo");
        }
    }
}
