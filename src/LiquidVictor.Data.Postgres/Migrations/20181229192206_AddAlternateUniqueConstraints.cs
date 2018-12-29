using Microsoft.EntityFrameworkCore.Migrations;

namespace LiquidVictor.Data.Postgres.Migrations
{
    public partial class AddAlternateUniqueConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_slidedeckslides_slidedeckid",
                table: "slidedeckslides");

            migrationBuilder.DropIndex(
                name: "IX_slidecontentitems_slideid",
                table: "slidecontentitems");

            migrationBuilder.AddUniqueConstraint(
                name: "UX_slidedeckid_slideid_sortorder",
                table: "slidedeckslides",
                columns: new[] { "slidedeckid", "slideid", "sortorder" });

            migrationBuilder.AddUniqueConstraint(
                name: "UX_slideid_slidecontentitd_sortorder",
                table: "slidecontentitems",
                columns: new[] { "slideid", "contentitemid", "sortorder" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "UX_slidedeckid_slideid_sortorder",
                table: "slidedeckslides");

            migrationBuilder.DropUniqueConstraint(
                name: "UX_slideid_slidecontentitd_sortorder",
                table: "slidecontentitems");

            migrationBuilder.CreateIndex(
                name: "IX_slidedeckslides_slidedeckid",
                table: "slidedeckslides",
                column: "slidedeckid");

            migrationBuilder.CreateIndex(
                name: "IX_slidecontentitems_slideid",
                table: "slidecontentitems",
                column: "slideid");
        }
    }
}
