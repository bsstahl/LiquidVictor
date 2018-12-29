using Microsoft.EntityFrameworkCore.Migrations;

namespace LiquidVictor.Data.Postgres.Migrations
{
    public partial class AddEncodedContentItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "encodedcontent",
                table: "contentitems",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "encodedcontent",
                table: "contentitems");
        }
    }
}
