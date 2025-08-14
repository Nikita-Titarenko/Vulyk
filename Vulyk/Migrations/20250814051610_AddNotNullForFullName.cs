using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vulyk.Migrations
{
    /// <inheritdoc />
    public partial class AddNotNullForFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "77c959e7-2790-4d53-9119-ea55b36d699d");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5f7fb4a4-0579-4bc3-a052-f07bcc5144a7", "dd56a5c2-e5a1-410d-b328-06d2595af6b5" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f7fb4a4-0579-4bc3-a052-f07bcc5144a7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dd56a5c2-e5a1-410d-b328-06d2595af6b5");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5f7fb4a4-0579-4bc3-a052-f07bcc5144a7", null, "Administrator", "ADMINISTRATOR" },
                    { "77c959e7-2790-4d53-9119-ea55b36d699d", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FullName", "LastOnline", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PendingNewEmail", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "dd56a5c2-e5a1-410d-b328-06d2595af6b5", 0, "1b29d10e-005a-4116-bb48-38ce9923e910", "ApplicationUser", "vulyk.messenger@gmail.com", true, "Mykyta Titarenko", new DateTime(2025, 8, 8, 11, 22, 17, 71, DateTimeKind.Local).AddTicks(7428), false, null, "VULYK.MESSENGER@GMAIL.COM", "VULYK.MESSENGER@GMAIL.COM", "AQAAAAIAAYagAAAAEAaL88i4ZW2TSRcUszE/Y/UVeQTZktlondKRkKd0VvqykpNyGSkWa3mLHIDEXCc6SA==", null, "+380953589545", false, "ec236aa4-9e7d-46b2-ad44-0028977e5427", false, "vulyk.messenger@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5f7fb4a4-0579-4bc3-a052-f07bcc5144a7", "dd56a5c2-e5a1-410d-b328-06d2595af6b5" });
        }
    }
}
