using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBook.API.Migrations
{
    /// <inheritdoc />
    public partial class FixesISBNnumbersinseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("4599eef6-d41f-45a7-8c75-c9c9172ea62b"),
                column: "ISBN",
                value: "0747542155");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("9a747b83-82d3-4968-a34f-dab4b5b9ee2b"),
                column: "ISBN",
                value: "0747551006");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("9e1d17e0-9fbd-4242-888a-ea225218ffb3"),
                column: "ISBN",
                value: "0330258648");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("d7d654b5-97e9-4b91-a3c3-580ce3fdc73d"),
                column: "ISBN",
                value: "0747538492");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("ffba8a54-c990-4862-931e-927b35b3b003"),
                column: "ISBN",
                value: "0747581088");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("4599eef6-d41f-45a7-8c75-c9c9172ea62b"),
                column: "ISBN",
                value: "043965548");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("9a747b83-82d3-4968-a34f-dab4b5b9ee2b"),
                column: "ISBN",
                value: "439358078");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("9e1d17e0-9fbd-4242-888a-ea225218ffb3"),
                column: "ISBN",
                value: "043965548");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("d7d654b5-97e9-4b91-a3c3-580ce3fdc73d"),
                column: "ISBN",
                value: "439554896");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("ffba8a54-c990-4862-931e-927b35b3b003"),
                column: "ISBN",
                value: "439785960");
        }
    }
}
