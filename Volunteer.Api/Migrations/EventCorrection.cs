using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteer.Api.Migrations
{
    /// <inheritdoc />
    public partial class EventCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Events",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Events",
                newName: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CityId",
                table: "Events",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TypeId",
                table: "Events",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Cities_CityId",
                table: "Events",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventTypes_TypeId",
                table: "Events",
                column: "TypeId",
                principalTable: "EventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Cities_CityId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventTypes_TypeId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CityId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_TypeId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Events",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Events",
                newName: "City");
        }
    }
}