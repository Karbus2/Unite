using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unite.Migrations
{
    /// <inheritdoc />
    public partial class ClientCascade_on_Event_EventRating_rel_NoAction_on_EventRating_Admin_User_rel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRatings_Events_EventId",
                table: "EventRatings");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRatings_Events_EventId",
                table: "EventRatings",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRatings_Events_EventId",
                table: "EventRatings");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRatings_Events_EventId",
                table: "EventRatings",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
