using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAWANi.WEBAPi.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddressPhoneCurrentCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasicInfo_Address",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BasicInfo_CurrentCity",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicInfo_Address",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BasicInfo_CurrentCity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BasicInfo_Phone",
                table: "UserProfiles");
        }
    }
}
