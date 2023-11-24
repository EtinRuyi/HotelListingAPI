using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hotel_Listing.api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21d13c49-9f7a-4c18-85c0-ca8872bdf8eb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d72a847-be27-4ff1-8ce8-eaad638a2018");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c0cc439e-b695-4f71-b3e2-d888b3551be3", "06D5E8BC-16AF-43AF-AB98-E41B372B6C00", "Administrator", "ADMINISTRATOR" },
                    { "d59541a9-7485-4846-a4e6-e1590f6a4038", "19222D9D-C436-41DC-B1F4-8C6825D05781", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0cc439e-b695-4f71-b3e2-d888b3551be3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d59541a9-7485-4846-a4e6-e1590f6a4038");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21d13c49-9f7a-4c18-85c0-ca8872bdf8eb", null, "Administrator", "ADMINISTRATOR" },
                    { "2d72a847-be27-4ff1-8ce8-eaad638a2018", null, "User", "USER" }
                });
        }
    }
}
