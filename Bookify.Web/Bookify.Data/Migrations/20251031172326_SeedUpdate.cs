using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 12);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "IsAvailable", "RoomNumber", "RoomTypeId" },
                values: new object[] { 12, false, "602", 6 });
        }
    }
}
