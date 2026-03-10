using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ipz_marketplace.Migrations
{
    /// <inheritdoc />
    public partial class inininnn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Photos_SellerAdId",
                table: "Photos",
                column: "SellerAdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_SellerAds_SellerAdId",
                table: "Photos",
                column: "SellerAdId",
                principalTable: "SellerAds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_SellerAds_SellerAdId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_SellerAdId",
                table: "Photos");
        }
    }
}
