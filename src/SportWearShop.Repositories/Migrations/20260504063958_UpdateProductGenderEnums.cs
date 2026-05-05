using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportWearShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductGenderEnums : Migration
    {

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Product
                SET Gender =
                    CASE
                        WHEN Gender = 'MEN' THEN '0'
                        WHEN Gender = 'WOMEN' THEN '1'
                        WHEN Gender = 'UNISEX' THEN '2'
                        WHEN Gender = 'KIDS' THEN '3'
                        ELSE '2'
                    END
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Product",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
