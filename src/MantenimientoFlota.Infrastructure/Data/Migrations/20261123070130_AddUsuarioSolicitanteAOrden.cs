using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantenimientoFlota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioSolicitanteAOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioSolicitanteId",
                table: "OrdenesMantenimiento",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesMantenimiento_UsuarioSolicitanteId",
                table: "OrdenesMantenimiento",
                column: "UsuarioSolicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesMantenimiento_Usuarios_UsuarioSolicitanteId",
                table: "OrdenesMantenimiento",
                column: "UsuarioSolicitanteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesMantenimiento_Usuarios_UsuarioSolicitanteId",
                table: "OrdenesMantenimiento");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesMantenimiento_UsuarioSolicitanteId",
                table: "OrdenesMantenimiento");

            migrationBuilder.DropColumn(
                name: "UsuarioSolicitanteId",
                table: "OrdenesMantenimiento");
        }
    }
}
