using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unite.Migrations
{
    /// <inheritdoc />
    public partial class AddingStateEnumToUserEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "UserEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "UserEvents");
        }
    }
}
