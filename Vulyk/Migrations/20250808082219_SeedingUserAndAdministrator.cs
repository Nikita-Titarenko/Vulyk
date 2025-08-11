using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vulyk.Migrations
{
    /// <inheritdoc />
    public partial class SeedingUserAndAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
