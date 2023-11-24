using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hotel_Listing.api.Migrations
{
    /// <inheritdoc />
    public partial class ChangingDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "67babb0d-245b-42a1-a961-67e065e3df4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8fea0292-ebc3-4cb8-b12d-95548928fd83");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21d13c49-9f7a-4c18-85c0-ca8872bdf8eb", "06D5E8BC-16AF-43AF-AB98-E41B372B6C00", "Administrator", "ADMINISTRATOR" },
                    { "2d72a847-be27-4ff1-8ce8-eaad638a2018", "19222D9D-C436-41DC-B1F4-8C6825D05781", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "67babb0d-245b-42a1-a961-67e065e3df4d", null, "User", "USER" },
                    { "8fea0292-ebc3-4cb8-b12d-95548928fd83", null, "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
