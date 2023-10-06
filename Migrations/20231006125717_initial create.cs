using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToyEcommerceASPNET.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Ratings = table.Column<double>(type: "float", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "Name", "Price", "Quantity", "Ratings" },
                values: new object[,]
                {
                    { 1, "", "No Content", "Diable 2: Resurrected", 9.9900000000000002, 4, 3.0 },
                    { 2, "", "No Content", "The Matrix", 9.9900000000000002, 4, 3.0 },
                    { 3, "", "No Content", "Super Nintendo", 9.9900000000000002, 4, 3.0 },
                    { 4, "", "No Content", "Back to the Future", 9.9900000000000002, 4, 3.0 },
                    { 5, "", "No Content", "Ready or Not", 9.9900000000000002, 4, 3.0 },
                    { 6, "", "No Content", "1984", 9.9900000000000002, 4, 3.0 },
                    { 7, "", "No Content", "Brave New World", 9.9900000000000002, 4, 3.0 },
                    { 8, "", "No Content", "A Nightmare on Elm Street", 9.9900000000000002, 4, 3.0 },
                    { 9, "", "No Content", "Awakenings ", 9.9900000000000002, 4, 3.0 },
                    { 10, "", "No Content", "A League of Their Own", 9.9900000000000002, 4, 3.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductId",
                table: "Images",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
