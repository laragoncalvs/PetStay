using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStay.Migrations
{
    /// <inheritdoc />
    public partial class PetsCadastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Raca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tamanho = table.Column<int>(type: "int", nullable: false),
                    FaixaEtaria = table.Column<int>(type: "int", nullable: false),
                    CuidadosEspecificos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeclaracaoVacinacao = table.Column<bool>(type: "bit", nullable: false),
                    TutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pets_AspNetUsers_TutorId",
                        column: x => x.TutorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_TutorId",
                table: "Pets",
                column: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pets");
        }
    }
}
