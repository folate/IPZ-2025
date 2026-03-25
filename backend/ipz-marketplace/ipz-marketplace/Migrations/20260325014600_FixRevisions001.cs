using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ipz_marketplace.Migrations
{
    /// <inheritdoc />
    public partial class FixRevisions001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Revisions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Revisions");
        }
    }
}
