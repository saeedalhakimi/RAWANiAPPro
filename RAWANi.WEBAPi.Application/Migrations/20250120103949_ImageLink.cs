using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAWANi.WEBAPi.Application.Migrations
{
    /// <inheritdoc />
    public partial class ImageLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "UserProfiles");
        }
    }
}
