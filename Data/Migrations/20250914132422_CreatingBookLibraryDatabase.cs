using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatingBookLibraryDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    PublishedYear = table.Column<int>(type: "int", nullable: false),
                    TotalCopies = table.Column<int>(type: "int", nullable: false),
                    AvailableCopies = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BorrowRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReaderId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowRecords_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowRecords_Readers_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "AvailableCopies", "ISBN", "PublishedYear", "Title", "TotalCopies" },
                values: new object[,]
                {
                    { 1, "John Doe", 5, "29192-33-221211", 2020, "C# Basics", 5 },
                    { 2, "Jane Smith", 6, "3232-4442-19191", 2021, "ASP.NET Core", 6 },
                    { 3, "Andrew Hunt", 10, "978-0201616224", 1999, "The Pragmatic Programmer", 10 },
                    { 4, "Robert C. Martin", 3, "978-0132350884", 2021, "Clean Code", 3 }
                });

            migrationBuilder.InsertData(
                table: "Readers",
                columns: new[] { "Id", "Email", "FullName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "ani@mail.com", "Ani Hakobyan", "43993555" },
                    { 2, "david@mail.com", "David Sargsyan", "98323322" },
                    { 3, "edgargasparyan10.12.2006@gmail.com", "Edgar Gasparyan", "99132004" }
                });

            migrationBuilder.InsertData(
                table: "BorrowRecords",
                columns: new[] { "Id", "BookId", "BorrowDate", "ReaderId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2025, 9, 14, 18, 19, 30, 0, DateTimeKind.Unspecified), 1, null },
                    { 2, 3, new DateTime(2025, 9, 14, 18, 19, 30, 0, DateTimeKind.Unspecified), 3, null },
                    { 3, 1, new DateTime(2025, 9, 14, 18, 19, 30, 0, DateTimeKind.Unspecified), 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_BookId",
                table: "BorrowRecords",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_ReaderId",
                table: "BorrowRecords",
                column: "ReaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowRecords");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Readers");
        }
    }
}
