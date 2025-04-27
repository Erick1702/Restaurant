using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class DetllComanPKComputes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleComandas_Platos_PlatoId",
                table: "DetalleComandas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleComandas",
                table: "DetalleComandas");

            migrationBuilder.DropIndex(
                name: "IX_DetalleComandas_ComandaId",
                table: "DetalleComandas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DetalleComandas");

            migrationBuilder.AlterColumn<int>(
                name: "PlatoId",
                table: "DetalleComandas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleComandas",
                table: "DetalleComandas",
                columns: new[] { "ComandaId", "PlatoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleComandas_Platos_PlatoId",
                table: "DetalleComandas",
                column: "PlatoId",
                principalTable: "Platos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleComandas_Platos_PlatoId",
                table: "DetalleComandas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleComandas",
                table: "DetalleComandas");

            migrationBuilder.AlterColumn<int>(
                name: "PlatoId",
                table: "DetalleComandas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DetalleComandas",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleComandas",
                table: "DetalleComandas",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleComandas_ComandaId",
                table: "DetalleComandas",
                column: "ComandaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleComandas_Platos_PlatoId",
                table: "DetalleComandas",
                column: "PlatoId",
                principalTable: "Platos",
                principalColumn: "Id");
        }
    }
}
