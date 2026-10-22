using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEspecialidadNombre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Especialidades_Nombre",
                table: "Especialidades",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Especialidades_Nombre",
                table: "Especialidades");
        }
    }
}
