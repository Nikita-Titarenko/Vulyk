using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vulyk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeApplicationUserIdRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec24450d-3cb6-4005-a52f-70f0ea6471c6");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3ba83f79-0a53-482e-bfca-5bcc338adde5", "2181b1a4-f10e-4e08-a82c-7d08da0b83c4" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ba83f79-0a53-482e-bfca-5bcc338adde5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2181b1a4-f10e-4e08-a82c-7d08da0b83c4");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserChat",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "25085c5c-508d-4603-81c6-59f695b75f94", null, "Administrator", "ADMINISTRATOR" },
                    { "b1dac97e-e351-4610-a71c-9ba836b1834f", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LastOnline", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PendingNewEmail", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ad2c0fd6-b551-4c68-a619-4ba4d182bdc6", 0, "83d9b43a-e832-4811-a1b8-97c54f73c12d", "vulyk.messenger@gmail.com", true, "Mykyta Titarenko", new DateTime(2025, 8, 22, 13, 4, 22, 500, DateTimeKind.Local).AddTicks(7483), false, null, "VULYK.MESSENGER@GMAIL.COM", "VULYK.MESSENGER@GMAIL.COM", "AQAAAAIAAYagAAAAEEmXS7+fw00ANiSlEqvkTV6erCIzJ67uZV0FgwShmK/TSXm6GiTuLG179JG1w7FZcg==", null, "+380953589545", false, "f28a325c-a867-45a1-a017-f4c0f1c70517", false, "vulyk.messenger@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "25085c5c-508d-4603-81c6-59f695b75f94", "ad2c0fd6-b551-4c68-a619-4ba4d182bdc6" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChat_AspNetUsers_ApplicationUserId",
                table: "UserChat");

            migrationBuilder.DropIndex(
                name: "IX_UserChat_ApplicationUserId",
                table: "UserChat");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1dac97e-e351-4610-a71c-9ba836b1834f");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "25085c5c-508d-4603-81c6-59f695b75f94", "ad2c0fd6-b551-4c68-a619-4ba4d182bdc6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25085c5c-508d-4603-81c6-59f695b75f94");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ad2c0fd6-b551-4c68-a619-4ba4d182bdc6");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserChat");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3ba83f79-0a53-482e-bfca-5bcc338adde5", null, "Administrator", "ADMINISTRATOR" },
                    { "ec24450d-3cb6-4005-a52f-70f0ea6471c6", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LastOnline", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PendingNewEmail", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2181b1a4-f10e-4e08-a82c-7d08da0b83c4", 0, "4d52446d-40c0-4c45-943e-6a054550e13d", "vulyk.messenger@gmail.com", true, "Mykyta Titarenko", new DateTime(2025, 8, 14, 8, 16, 7, 776, DateTimeKind.Local).AddTicks(2400), false, null, "VULYK.MESSENGER@GMAIL.COM", "VULYK.MESSENGER@GMAIL.COM", "AQAAAAIAAYagAAAAEFV1sm2onnpgqBKE6DdKZjynQaDt2Y4W7A0MI/EFKqzhinPLV4x+Gdr1I/2c3kXniA==", null, "+380953589545", false, "2ac7525d-188f-4130-b26a-5b658018b955", false, "vulyk.messenger@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "3ba83f79-0a53-482e-bfca-5bcc338adde5", "2181b1a4-f10e-4e08-a82c-7d08da0b83c4" });
        }
    }
}
