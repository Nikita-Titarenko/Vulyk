using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulyk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndChatRelationsToUserChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChat_AspNetUsers_ApplicationUserId",
                table: "UserChat");

            migrationBuilder.DropIndex(
                name: "IX_UserChat_ApplicationUserId",
                table: "UserChat");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserChat");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChat_AspNetUsers_UserId",
                table: "UserChat",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChat_AspNetUsers_UserId",
                table: "UserChat");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserChat",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChat_ApplicationUserId",
                table: "UserChat",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChat_AspNetUsers_ApplicationUserId",
                table: "UserChat",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
