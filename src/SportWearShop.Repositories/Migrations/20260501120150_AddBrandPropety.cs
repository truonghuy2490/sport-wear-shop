using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportWearShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandPropety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandImage",
                table: "Brand",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandImage",
                table: "Brand");
        }
    }
}
