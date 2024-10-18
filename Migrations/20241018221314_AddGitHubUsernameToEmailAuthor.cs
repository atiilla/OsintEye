using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitRecon.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubUsernameToEmailAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubUsername",
                table: "EmailAuthors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubUsername",
                table: "EmailAuthors");
        }
    }
}
