using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoG_LocaGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class RentedGame_Reserved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Reserved",
                table: "RentedGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reserved",
                table: "RentedGames");
        }
    }
}
