using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStay.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInCheckOutFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCheckInEfetiva",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCheckOutEfetiva",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraCheckInEfetiva",
                table: "Reservations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraCheckOutEfetiva",
                table: "Reservations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacoeGestor",
                table: "Reservations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCheckInEfetiva",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DataCheckOutEfetiva",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "HoraCheckInEfetiva",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "HoraCheckOutEfetiva",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ObservacoeGestor",
                table: "Reservations");
        }
    }
}
