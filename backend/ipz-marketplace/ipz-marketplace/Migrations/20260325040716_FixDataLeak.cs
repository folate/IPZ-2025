using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ipz_marketplace.Migrations
{
    /// <inheritdoc />
    public partial class FixDataLeak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "SellerAds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "SellerAds");
        }
    }
}
