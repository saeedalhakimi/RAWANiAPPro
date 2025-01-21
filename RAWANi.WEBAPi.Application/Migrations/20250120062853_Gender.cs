using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAWANi.WEBAPi.Application.Migrations
{
    /// <inheritdoc />
    public partial class Gender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicInfo_CurrentCity",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "BasicInfo_Phone",
                table: "UserProfiles",
                newName: "BasicInfo_Gender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BasicInfo_Gender",
                table: "UserProfiles",
                newName: "BasicInfo_Phone");

            migrationBuilder.AddColumn<string>(
                name: "BasicInfo_CurrentCity",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
