using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulyk.Data.Migrations
{
    /// <inheritdoc />
    public partial class Changekeyinmessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_UserChat_UserChatUserId_UserChatChatId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_UserChatUserId_UserChatChatId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "UserChatChatId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "UserChatUserId",
                table: "Message");

            migrationBuilder.CreateIndex(
                name: "IX_Message_UserId_ChatId",
                table: "Message",
                columns: new[] { "UserId", "ChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserChat_UserId_ChatId",
                table: "Message",
                columns: new[] { "UserId", "ChatId" },
                principalTable: "UserChat",
                principalColumns: new[] { "UserId", "ChatId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_UserChat_UserId_ChatId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_UserId_ChatId",
                table: "Message");

            migrationBuilder.AddColumn<int>(
                name: "UserChatChatId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserChatUserId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Message_UserChatUserId_UserChatChatId",
                table: "Message",
                columns: new[] { "UserChatUserId", "UserChatChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserChat_UserChatUserId_UserChatChatId",
                table: "Message",
                columns: new[] { "UserChatUserId", "UserChatChatId" },
                principalTable: "UserChat",
                principalColumns: new[] { "UserId", "ChatId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
