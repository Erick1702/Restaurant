using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class CampoUs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_AspNetUsers_UsuarioId1",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_UsuarioId1",
                table: "Comandas");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Comandas");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Comandas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_UsuarioId",
                table: "Comandas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_AspNetUsers_UsuarioId",
                table: "Comandas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_AspNetUsers_UsuarioId",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_UsuarioId",
                table: "Comandas");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Comandas",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId1",
                table: "Comandas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_UsuarioId1",
                table: "Comandas",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_AspNetUsers_UsuarioId1",
                table: "Comandas",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
