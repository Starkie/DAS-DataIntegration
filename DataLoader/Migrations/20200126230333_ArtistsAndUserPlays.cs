using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLoader.Migrations
{
    public partial class ArtistsAndUserPlays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserArtistPlays",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: true),
                    ArtistId = table.Column<string>(nullable: true),
                    PlaysNumber = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArtistPlays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArtistPlays_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserArtistPlays_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserArtistPlays_ArtistId",
                table: "UserArtistPlays",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArtistPlays_UserId",
                table: "UserArtistPlays",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserArtistPlays");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
