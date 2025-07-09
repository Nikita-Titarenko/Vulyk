using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulyk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderUserIdtoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderUserId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderUserId",
                table: "User");
        }
    }
}
