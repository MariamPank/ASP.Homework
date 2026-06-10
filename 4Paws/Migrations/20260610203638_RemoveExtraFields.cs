using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4Paws.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "CareGiverName",
                table: "CareGivers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Owners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CareGiverName",
                table: "CareGivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
