using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToyEcommerceASPNET.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "PaymentMethod", "Status", "Timestamp", "Type" },
                values: new object[,]
                {
                    { 1, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(300), "Test" },
                    { 2, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(311), "Test" },
                    { 3, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(312), "Test" },
                    { 4, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(312), "Test" },
                    { 5, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(313), "Test" },
                    { 6, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(314), "Test" },
                    { 7, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(314), "Test" },
                    { 8, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(315), "Test" },
                    { 9, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(315), "Test" },
                    { 10, 9.9900000000000002, 1, 1, new DateTime(2023, 10, 6, 23, 5, 52, 504, DateTimeKind.Local).AddTicks(316), "Test" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
