using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrewItBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureUrlToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""); // Set default value if needed (can be empty string)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Products");
        }
    }
}
