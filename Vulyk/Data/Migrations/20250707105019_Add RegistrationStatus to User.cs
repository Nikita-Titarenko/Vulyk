using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulyk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationStatustoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegisterStatus",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterStatus",
                table: "User");
        }
    }
}
