using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStay.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarGestorDono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GestorDonoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GestorDonoId",
                table: "AspNetUsers",
                column: "GestorDonoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_GestorDonoId",
                table: "AspNetUsers",
                column: "GestorDonoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_GestorDonoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GestorDonoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GestorDonoId",
                table: "AspNetUsers");
        }
    }
}
