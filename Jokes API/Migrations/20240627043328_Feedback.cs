using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jokes_API.Migrations
{
    /// <inheritdoc />
    public partial class Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Jokes",
                table: "Jokes");

            migrationBuilder.RenameTable(
                name: "Jokes",
                newName: "Joke");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Joke",
                table: "Joke",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JokeId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Joke",
                table: "Joke");

            migrationBuilder.RenameTable(
                name: "Joke",
                newName: "Jokes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jokes",
                table: "Jokes",
                column: "Id");
        }
    }
}
