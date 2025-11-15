using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationItemRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReservationItems_RoomId",
                table: "ReservationItems",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationItems_Rooms_RoomId",
                table: "ReservationItems",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationItems_Rooms_RoomId",
                table: "ReservationItems");

            migrationBuilder.DropIndex(
                name: "IX_ReservationItems_RoomId",
                table: "ReservationItems");
        }
    }
}
