using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeHelper.Server.Migrations
{
    public partial class soldfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Sold",
                table: "AssetSells",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Bought",
                table: "AssetBuys",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sold",
                table: "AssetSells");

            migrationBuilder.DropColumn(
                name: "Bought",
                table: "AssetBuys");
        }
    }
}
