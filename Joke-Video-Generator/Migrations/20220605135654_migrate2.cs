using Microsoft.EntityFrameworkCore.Migrations;

namespace Joke_Video_Generator.Migrations
{
    public partial class migrate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "no_of_charachter",
                table: "Joke",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "no_of_charachter",
                table: "Joke");
        }
    }
}
