using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulyk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingNewEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PendingNewEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingNewEmail",
                table: "AspNetUsers");
        }
    }
}
