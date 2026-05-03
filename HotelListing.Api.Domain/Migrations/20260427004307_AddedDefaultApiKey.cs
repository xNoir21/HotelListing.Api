using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApiKeys",
                columns: new[] { "Id", "AppName", "CreatedOnUtc", "ExpiresOnUtc", "KeyHash", "KeyId" },
                values: new object[] { new Guid("a4d3988c-5b82-40ce-8aca-d90d590b1387"), "app", new DateTimeOffset(new DateTime(2026, 4, 27, 0, 43, 7, 40, DateTimeKind.Unspecified).AddTicks(6670), new TimeSpan(0, 0, 0, 0, 0)), null, "36F27D144462317E37C5F364A9657A667A76C56C38896EF2E70031391A69B2B2", "ba0bd8fd6a88" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4d3988c-5b82-40ce-8aca-d90d590b1387",
                column: "ConcurrencyStamp",
                value: "2b8c8d5a-d201-43bd-9817-0843d9fc8f0f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a740b281-d7d6-458f-9c25-8294e7de3fa6",
                column: "ConcurrencyStamp",
                value: "1bf8771f-2d57-40da-a3e9-d41149de51ff");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApiKeys",
                keyColumn: "Id",
                keyValue: new Guid("a4d3988c-5b82-40ce-8aca-d90d590b1387"));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4d3988c-5b82-40ce-8aca-d90d590b1387",
                column: "ConcurrencyStamp",
                value: "e2c15861-43b7-4a44-9e90-1279b8f99589");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a740b281-d7d6-458f-9c25-8294e7de3fa6",
                column: "ConcurrencyStamp",
                value: "80e165a1-4518-4e08-b5c5-c95e9208ef77");
        }
    }
}
