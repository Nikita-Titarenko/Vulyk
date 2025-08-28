using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulyk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtPropertyToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ad2c0fd6-b551-4c68-a619-4ba4d182bdc6",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastOnline", "PasswordHash", "SecurityStamp" },
                values: new object[] { "28c34f24-dec4-4212-8014-41abb8f5e7bc", new DateTime(2025, 8, 23, 11, 7, 9, 918, DateTimeKind.Utc).AddTicks(1418), null, "AQAAAAIAAYagAAAAEPSaieR9NDSI1qGmv6pUJ8ZihQEEN9eqjNN8sgdvkhGL87GD2PLM5fBKOTkn/dB64g==", "233c3edf-2a54-4497-9f37-1a45005d605e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ad2c0fd6-b551-4c68-a619-4ba4d182bdc6",
                columns: new[] { "ConcurrencyStamp", "LastOnline", "PasswordHash", "SecurityStamp" },
                values: new object[] { "83d9b43a-e832-4811-a1b8-97c54f73c12d", new DateTime(2025, 8, 22, 13, 4, 22, 500, DateTimeKind.Local).AddTicks(7483), "AQAAAAIAAYagAAAAEEmXS7+fw00ANiSlEqvkTV6erCIzJ67uZV0FgwShmK/TSXm6GiTuLG179JG1w7FZcg==", "f28a325c-a867-45a1-a017-f4c0f1c70517" });
        }
    }
}
