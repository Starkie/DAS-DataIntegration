using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLoader.Migrations
{
    public partial class RequiredAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserArtistPlays_Artists_ArtistId",
                table: "UserArtistPlays");

            migrationBuilder.DropForeignKey(
                name: "FK_UserArtistPlays_Users_UserId",
                table: "UserArtistPlays");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserArtistPlays",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArtistId",
                table: "UserArtistPlays",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserArtistPlays_Artists_ArtistId",
                table: "UserArtistPlays",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserArtistPlays_Users_UserId",
                table: "UserArtistPlays",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserArtistPlays_Artists_ArtistId",
                table: "UserArtistPlays");

            migrationBuilder.DropForeignKey(
                name: "FK_UserArtistPlays_Users_UserId",
                table: "UserArtistPlays");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserArtistPlays",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ArtistId",
                table: "UserArtistPlays",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_UserArtistPlays_Artists_ArtistId",
                table: "UserArtistPlays",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserArtistPlays_Users_UserId",
                table: "UserArtistPlays",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
