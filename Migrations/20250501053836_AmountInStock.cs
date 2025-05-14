using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrewItBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AmountInStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountInStock",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmountInCart",
                table: "Carts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountInStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AmountInCart",
                table: "Carts");
        }
    }
}
