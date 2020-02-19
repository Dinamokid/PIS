using Microsoft.EntityFrameworkCore.Migrations;

namespace PisMirShow.Migrations
{
    public partial class UpdateDialogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Dialogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Dialogs");
        }
    }
}
