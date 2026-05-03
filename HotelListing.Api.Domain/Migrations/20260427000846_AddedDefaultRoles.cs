using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelListing.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a4d3988c-5b82-40ce-8aca-d90d590b1387", "e2c15861-43b7-4a44-9e90-1279b8f99589", "Admin", "ADMIN" },
                    { "a740b281-d7d6-458f-9c25-8294e7de3fa6", "80e165a1-4518-4e08-b5c5-c95e9208ef77", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4d3988c-5b82-40ce-8aca-d90d590b1387");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a740b281-d7d6-458f-9c25-8294e7de3fa6");
        }
    }
}
