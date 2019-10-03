using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PisMirShow.Migrations
{
    public partial class UpdateFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Tasks_TaskId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "Сonfirmed",
                table: "Files",
                newName: "Confirmed");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Files",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedDateTime",
                table: "Files",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ConfirmedUserId",
                table: "Files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Files",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_UserId",
                table: "Files",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Tasks_TaskId",
                table: "Files",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_UserId",
                table: "Files",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Tasks_TaskId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_UserId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_UserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ConfirmedDateTime",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ConfirmedUserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "Confirmed",
                table: "Files",
                newName: "Сonfirmed");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Files",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Tasks_TaskId",
                table: "Files",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
