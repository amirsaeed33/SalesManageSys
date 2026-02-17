using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleManagementSys.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Sales",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Sales");
        }
    }
}
