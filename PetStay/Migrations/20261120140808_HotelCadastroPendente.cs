using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStay.Migrations
{
    /// <inheritdoc />
    public partial class HotelCadastroPendente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aprovado",
                table: "Hotels",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "FaixasEtariasAceitas",
                table: "Hotels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GestorId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioAbertura",
                table: "Hotels",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioFechamento",
                table: "Hotels",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "PortesAceitos",
                table: "Hotels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_GestorId",
                table: "Hotels",
                column: "GestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_AspNetUsers_GestorId",
                table: "Hotels",
                column: "GestorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_AspNetUsers_GestorId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_GestorId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Aprovado",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "FaixasEtariasAceitas",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "GestorId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HorarioAbertura",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HorarioFechamento",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "PortesAceitos",
                table: "Hotels");
        }
    }
}
