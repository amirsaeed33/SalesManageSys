using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleManagementSys.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDefaultSalePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSalePrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultSalePrice",
                table: "Products");
        }
    }
}
