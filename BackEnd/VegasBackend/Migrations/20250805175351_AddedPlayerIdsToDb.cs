using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VegasBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerIdsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Player1Id",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Player2Id",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Player2Id",
                table: "Games");
        }
    }
}
