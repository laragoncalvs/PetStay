using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStay.Migrations
{
    /// <inheritdoc />
    public partial class HotelAprovacaoAdministrativa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_AspNetUsers_GestorId",
                table: "Hotels");

            migrationBuilder.AddColumn<int>(
                name: "AdministradorId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AprovadoEm",
                table: "Hotels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JustificativaAprovacao",
                table: "Hotels",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusAprovacao",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_AdministradorId",
                table: "Hotels",
                column: "AdministradorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_AspNetUsers_AdministradorId",
                table: "Hotels",
                column: "AdministradorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_AspNetUsers_GestorId",
                table: "Hotels",
                column: "GestorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_AspNetUsers_AdministradorId",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_AspNetUsers_GestorId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_AdministradorId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "AdministradorId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "AprovadoEm",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "JustificativaAprovacao",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "StatusAprovacao",
                table: "Hotels");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_AspNetUsers_GestorId",
                table: "Hotels",
                column: "GestorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
