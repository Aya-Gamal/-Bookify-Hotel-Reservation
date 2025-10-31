using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookify.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "PricePerNight",
                value: 100m);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "PricePerNight",
                value: 130m);

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Description", "Name", "PricePerNight" },
                values: new object[,]
                {
                    { 4, "A spacious room featuring a king - size bed, elegant decor, and a stunning city view — perfect for couples or business travelers.", "Premium King Room", 159m },
                    { 5, "A large and comfortable room designed for families, featuring multiple beds and modern amenities to ensure a pleasant stay for everyone.", "Family Room", 299m },
                    { 6, "A spacious and elegant room featuring premium furnishings, modern amenities, and a beautiful view — perfect for guests seeking extra comfort and style.", "Deluxe Room", 198m }
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "RoomNumber", "RoomTypeId" },
                values: new object[] { "401", 4 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsAvailable", "RoomNumber", "RoomTypeId" },
                values: new object[] { false, "402", 4 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "IsAvailable", "RoomNumber", "RoomTypeId" },
                values: new object[] { true, "501", 5 });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "IsAvailable", "RoomNumber", "RoomTypeId" },
                values: new object[,]
                {
                    { 1, true, "101", 1 },
                    { 2, false, "102", 1 },
                    { 3, true, "201", 2 },
                    { 4, false, "202", 2 },
                    { 5, true, "301", 3 },
                    { 6, false, "302", 3 },
                    { 10, false, "502", 5 },
                    { 11, true, "601", 6 },
                    { 12, false, "602", 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "PricePerNight",
                value: 800m);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "PricePerNight",
                value: 1200m);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "RoomNumber", "RoomTypeId" },
                values: new object[] { "102", 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsAvailable", "RoomNumber", "RoomTypeId" },
                values: new object[] { true, "201", 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "IsAvailable", "RoomNumber", "RoomTypeId" },
                values: new object[] { false, "302", 3 });
        }
    }
}
