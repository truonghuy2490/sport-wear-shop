using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportWearShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductStatusEnums : Migration
    {

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Product
                SET Status =
                    CASE
                        WHEN Status = 'DRAFT' THEN '0'
                        WHEN Status = 'ACTIVE' THEN '1'
                        WHEN Status = 'DELETED' THEN '2'
                        ELSE '0'
                    END
            ");

            migrationBuilder.Sql(@"
                UPDATE ProductVariant
                SET Status =
                    CASE
                        WHEN Status = 'DRAFT' THEN '0'
                        WHEN Status = 'ACTIVE' THEN '1'
                        WHEN Status = 'INACTIVE' THEN '2'
                        WHEN Status = 'DELETED' THEN '3'
                        ELSE '0'
                    END
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProductVariant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldDefaultValue: "ACTIVE")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_ProductVariant_Status");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldDefaultValue: "ACTIVE")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_Product_Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductVariant",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "ACTIVE",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0)
                .Annotation("Relational:DefaultConstraintName", "DF_ProductVariant_Status");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Product",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "ACTIVE",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0)
                .Annotation("Relational:DefaultConstraintName", "DF_Product_Status");
        }
    }
}
