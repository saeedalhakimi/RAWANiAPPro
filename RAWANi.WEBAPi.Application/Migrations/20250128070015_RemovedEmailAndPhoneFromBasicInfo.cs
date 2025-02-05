using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAWANi.WEBAPi.Application.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEmailAndPhoneFromBasicInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicInfo_Email",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BasicInfo_Phone",
                table: "UserProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasicInfo_Email",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BasicInfo_Phone",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
