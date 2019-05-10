using Microsoft.EntityFrameworkCore.Migrations;

namespace PisMirShow.Migrations
{
    public partial class UpdateTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToUser",
                table: "Tasks",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "FromUser",
                table: "Tasks",
                newName: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_FromUserId",
                table: "Tasks",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ToUserId",
                table: "Tasks",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_FromUserId",
                table: "Tasks",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_ToUserId",
                table: "Tasks",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_FromUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_ToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_FromUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ToUserId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Tasks",
                newName: "ToUser");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "Tasks",
                newName: "FromUser");
        }
    }
}
