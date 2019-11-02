using Microsoft.EntityFrameworkCore.Migrations;

namespace PisMirShow.Migrations
{
    public partial class AddDocumentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocType",
                table: "Files",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocType",
                table: "Files");
        }
    }
}
