using Microsoft.EntityFrameworkCore.Migrations;

namespace Joke_Animation_Video_Generator.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "used",
                table: "Joke");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "used",
                table: "Joke",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
