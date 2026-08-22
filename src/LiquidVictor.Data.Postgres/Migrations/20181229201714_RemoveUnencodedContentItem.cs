using Microsoft.EntityFrameworkCore.Migrations;

namespace LiquidVictor.Data.Postgres.Migrations;

public partial class RemoveUnencodedContentItem : Migration
{
    private static byte[] default_content_value => [];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropColumn(
            name: "content",
            table: "contentitems");

        migrationBuilder.AlterColumn<string>(
            name: "contenttype",
            table: "contentitems",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AlterColumn<string>(
            name: "contenttype",
            table: "contentitems",
            nullable: false,
            oldClrType: typeof(string),
            oldMaxLength: 100);

        migrationBuilder.AddColumn<byte[]>(
            name: "content",
            table: "contentitems",
            nullable: false,
            defaultValue: RemoveUnencodedContentItem.default_content_value);
    }
}
