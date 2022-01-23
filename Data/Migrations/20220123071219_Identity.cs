using Microsoft.EntityFrameworkCore.Migrations;

namespace MastersOfCinema.Migrations
{
    public partial class Identity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MovieRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MovieRatings_MovieId",
                table: "MovieRatings",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieRatings_Movies_MovieId",
                table: "MovieRatings",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieRatings_Movies_MovieId",
                table: "MovieRatings");

            migrationBuilder.DropIndex(
                name: "IX_MovieRatings_MovieId",
                table: "MovieRatings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MovieRatings");
        }
    }
}
