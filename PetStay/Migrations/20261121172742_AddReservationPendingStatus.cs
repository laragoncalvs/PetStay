using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStay.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationPendingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoRejeicao",
                table: "Reservations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondidaEm",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SolicitadaEm",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoRejeicao",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RespondidaEm",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SolicitadaEm",
                table: "Reservations");
        }
    }
}
