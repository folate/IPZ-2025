using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ipz_marketplace.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuyerAdId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BuyerAds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Budget = table.Column<int>(type: "integer", nullable: false),
                    BuyerId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerAds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyerAds_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BuyerAdId",
                table: "AspNetUsers",
                column: "BuyerAdId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerAds_BuyerId",
                table: "BuyerAds",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BuyerAds_BuyerAdId",
                table: "AspNetUsers",
                column: "BuyerAdId",
                principalTable: "BuyerAds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BuyerAds_BuyerAdId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BuyerAds");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BuyerAdId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BuyerAdId",
                table: "AspNetUsers");
        }
    }
}
