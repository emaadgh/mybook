using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyBook.API.Migrations
{
    /// <inheritdoc />
    public partial class updateseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("0d35c02d-11f2-4fad-9b76-e2562538c6fa"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("1a177495-2212-4a00-94cf-8d1e91086f41"));

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Category", "Description", "ISBN", "PublicationDate", "Publisher", "Title" },
                values: new object[,]
                {
                    { new Guid("4599eef6-d41f-45a7-8c75-c9c9172ea62b"), new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"), null, null, "043965548", new DateTimeOffset(new DateTime(2004, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "Harry Potter and the Prisoner of Azkaban (Harry Potter  #3)" },
                    { new Guid("9a747b83-82d3-4968-a34f-dab4b5b9ee2b"), new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"), null, null, "439358078", new DateTimeOffset(new DateTime(2004, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "Harry Potter and the Order of the Phoenix (Harry Potter  #5)" },
                    { new Guid("9e1d17e0-9fbd-4242-888a-ea225218ffb3"), new Guid("d3b05403-79b9-460a-9d5e-a3641fd5a1b2"), null, null, "043965548", new DateTimeOffset(new DateTime(2004, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "The Hitchhiker's Guide to the Galaxy (Hitchhiker's Guide to the Galaxy  #1)" },
                    { new Guid("d7d654b5-97e9-4b91-a3c3-580ce3fdc73d"), new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"), null, null, "439554896", new DateTimeOffset(new DateTime(2003, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "Harry Potter and the Chamber of Secrets (Harry Potter  #2)" },
                    { new Guid("ffba8a54-c990-4862-931e-927b35b3b003"), new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"), null, null, "439785960", new DateTimeOffset(new DateTime(2006, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "Harry Potter and the Half-Blood Prince (Harry Potter  #6)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("4599eef6-d41f-45a7-8c75-c9c9172ea62b"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("9a747b83-82d3-4968-a34f-dab4b5b9ee2b"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("9e1d17e0-9fbd-4242-888a-ea225218ffb3"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("d7d654b5-97e9-4b91-a3c3-580ce3fdc73d"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("ffba8a54-c990-4862-931e-927b35b3b003"));

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Category", "Description", "ISBN", "PublicationDate", "Publisher", "Title" },
                values: new object[,]
                {
                    { new Guid("0d35c02d-11f2-4fad-9b76-e2562538c6fa"), new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"), null, null, "439785960", new DateTimeOffset(new DateTime(2006, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "Harry Potter and the Half-Blood Prince (Harry Potter  #6)" },
                    { new Guid("1a177495-2212-4a00-94cf-8d1e91086f41"), new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"), null, null, "439358078", new DateTimeOffset(new DateTime(2004, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)), "Scholastic Inc.", "Harry Potter and the Order of the Phoenix (Harry Potter  #5)" }
                });
        }
    }
}
