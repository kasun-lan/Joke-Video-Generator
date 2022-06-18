using Microsoft.EntityFrameworkCore.Migrations;

namespace Joke_Animation_Video_Generator.Migrations
{
    public partial class migration1000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "used",
                table: "Joke",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "used",
                table: "Joke");
        }
    }
}
