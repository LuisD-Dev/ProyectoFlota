using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueRepuestoCodigo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_Codigo",
                table: "Repuestos",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Repuestos_Codigo",
                table: "Repuestos");
        }
    }
}
