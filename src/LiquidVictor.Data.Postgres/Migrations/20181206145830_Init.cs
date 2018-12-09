using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LiquidVictor.Data.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contentitems",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodifieddate = table.Column<DateTime>(nullable: false),
                    content = table.Column<byte[]>(nullable: false),
                    contenttype = table.Column<string>(nullable: false),
                    title = table.Column<string>(maxLength: 200, nullable: true),
                    filename = table.Column<string>(maxLength: 260, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contentitems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "slidedeck",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodifieddate = table.Column<DateTime>(nullable: false),
                    title = table.Column<string>(maxLength: 200, nullable: true),
                    subtitle = table.Column<string>(maxLength: 500, nullable: true),
                    presenter = table.Column<string>(maxLength: 200, nullable: true),
                    themename = table.Column<string>(maxLength: 50, nullable: true),
                    aspectratio = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slidedeck", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "slides",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodifieddate = table.Column<DateTime>(nullable: false),
                    title = table.Column<string>(nullable: false),
                    layout = table.Column<int>(nullable: false),
                    transitionin = table.Column<int>(nullable: false),
                    transitionout = table.Column<int>(nullable: false),
                    notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slides", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "slidecontentitems",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodifieddate = table.Column<DateTime>(nullable: false),
                    sortorder = table.Column<int>(nullable: false),
                    slideid = table.Column<Guid>(nullable: false),
                    contentitemid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slidecontentitems", x => x.id);
                    table.ForeignKey(
                        name: "FK_slidecontentitems_contentitems_contentitemid",
                        column: x => x.contentitemid,
                        principalTable: "contentitems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_slidecontentitems_slides_slideid",
                        column: x => x.slideid,
                        principalTable: "slides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "slidedeckslides",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodifieddate = table.Column<DateTime>(nullable: false),
                    sortorder = table.Column<int>(nullable: false),
                    slidedeckid = table.Column<Guid>(nullable: false),
                    slideid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slidedeckslides", x => x.id);
                    table.ForeignKey(
                        name: "FK_slidedeckslides_slidedeck_slidedeckid",
                        column: x => x.slidedeckid,
                        principalTable: "slidedeck",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_slidedeckslides_slides_slideid",
                        column: x => x.slideid,
                        principalTable: "slides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_slidecontentitems_contentitemid",
                table: "slidecontentitems",
                column: "contentitemid");

            migrationBuilder.CreateIndex(
                name: "IX_slidecontentitems_slideid",
                table: "slidecontentitems",
                column: "slideid");

            migrationBuilder.CreateIndex(
                name: "IX_slidedeckslides_slidedeckid",
                table: "slidedeckslides",
                column: "slidedeckid");

            migrationBuilder.CreateIndex(
                name: "IX_slidedeckslides_slideid",
                table: "slidedeckslides",
                column: "slideid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "slidecontentitems");

            migrationBuilder.DropTable(
                name: "slidedeckslides");

            migrationBuilder.DropTable(
                name: "contentitems");

            migrationBuilder.DropTable(
                name: "slidedeck");

            migrationBuilder.DropTable(
                name: "slides");
        }
    }
}
