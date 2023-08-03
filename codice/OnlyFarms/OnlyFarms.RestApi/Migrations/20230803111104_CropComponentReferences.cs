using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class CropComponentReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FarmingCompanyId",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FarmingCompanyId",
                table: "Actuators",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FarmingCompanyId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "FarmingCompanyId",
                table: "Actuators");
        }
    }
}
