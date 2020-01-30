using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLoader.Migrations
{
    public partial class ArtistsAndUserPlaysCsv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPlaysCsvs",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: true),
                    ArtistId = table.Column<string>(nullable: true),
                    ArtistName = table.Column<string>(nullable: true),
                    Plays = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlaysCsvs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlaysCsvs");
        }
    }
}
