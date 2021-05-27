using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentEditing.Migrations
{
    public partial class ChangeUserFileModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_AspNetUsers_FileOwnerId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_FileOwnerId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FileOwnerId",
                table: "Files");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileOwnerId",
                table: "Files",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileOwnerId",
                table: "Files",
                column: "FileOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_AspNetUsers_FileOwnerId",
                table: "Files",
                column: "FileOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
